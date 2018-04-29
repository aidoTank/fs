using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public delegate void UpdateListItemEvent(Transform item, int index);
    public delegate void MoveListEnd(Transform item);

    public class UISuperList : MonoBehaviour
    {
        //基础设置
        public ScrollRect scrollRect;
        public RectTransform itemParent;
        public GameObject itemObj;
        public Transform spreadTrans;
        public Scrollbar scrollBar;
        public Toggle togSelectAll;
        // 外部基础参数设置  
        public int itemWidth;            //单元格宽
        public int itemHeight;           //单元格高
        public int rowCount;             // 显示行数
        public int columnCount;      // 显示列数
        public int offsetX = 0;
        public int offsetY = 0;
        private int rowSum;          //一共多少行
        private Vector2 m_maskSize;
        //根据参数来计算
        private int m_rectWidth;           //列表宽度
        private int m_rectHeigh;           //列表高度
        private int m_showCount;              //当前实际显示的数量(小于或等于createCount)
        private int m_createCount;            //创建的数量
        private int m_listCount;              //列表总的需要显示的数量，外部给
        private int m_startIndex = 0;     //显示开始序号
        private int m_endIndex = 0;           //显示结束序号
        private Dictionary<int, Transform> dic_itemIndex = new Dictionary<int, Transform>();      //item对应的序号
        private UpdateListItemEvent m_updateItem = null;
        //某个item展开功能
        public int spreadHeigh;         //展开的高度
        private int m_spreadIndex = -1;          //选中需要展开的序号
        //选中功能
        public enum eSelectType
        {
            Null = 0,
            Single = 1,         //单选
            Multi = 2,          //多选
        }
        public eSelectType selectType = eSelectType.Null;
        private int m_focusIndex = -1;                  //单选
        private List<int> m_focusList = new List<int>();        //多选
        //临时用
        private Transform m_item;
        private Transform m_choice;     //单选
        private Transform m_select;     //多选

        public Vector3 curItemParentPos = Vector3.zero;

        public void Init(int width, int heigh, int column, int row)
        {
            itemWidth = width;
            itemHeight = heigh;
            columnCount = column;
            rowCount = row + 2;
            m_createCount = columnCount * rowCount;
            if (m_createCount <= 0)
            {
                Debug.LogError("列表初始化有问题！");
                return;
            }
            scrollRect = transform.FindChild("scroll").GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.onValueChanged.AddListener(OnValueChange);
            }
            m_rectWidth = column * width;
            itemParent = transform.FindChild("scroll/top/item_parent").GetComponent<RectTransform>();
            itemParent.anchorMin = new Vector2(0, 1);
            itemParent.anchorMax = new Vector2(0, 1);
            itemParent.pivot = new Vector2(0, 1);
            itemObj = transform.FindChild("scroll/top/item").gameObject;
            itemObj.SetActive(false);
            spreadTrans = transform.FindChild("scroll/top/spread");
            RectTransform itemRec = itemObj.GetComponent<RectTransform>();
            itemRec.anchorMin = new Vector2(0, 1);
            itemRec.anchorMax = new Vector2(0, 1);
            itemRec.pivot = new Vector2(0, 1);
            m_maskSize = GetComponent<RectTransform>().sizeDelta;

            Transform sbTrans = transform.FindChild("sb");
            if (sbTrans != null)
            {
                scrollBar = sbTrans.GetComponent<Scrollbar>();
                scrollBar.onValueChanged.AddListener(
                    delegate(float val)
                    {
                        OnDragSB(sbTrans.gameObject, val);
                    });
            }
            Transform togTrans = transform.FindChild("select_all");
            if (togTrans != null)
                togSelectAll = transform.FindChild("select_all").GetComponent<Toggle>();

            topArrow = transform.FindChild("top");
            bottomArrow = transform.FindChild("bottom");
        }
        public void SetActive(bool bShow)
        {
            gameObject.SetActive(bShow);
        }

        public void SetOffset(int x, int y)
        {
            offsetX = x;
            offsetY = y;
            m_rectWidth = (columnCount - 1) * (itemWidth + offsetX);
        }

        //全选功能
        public bool selectAll
        {
            set
            {
                if (value)
                {
                    for (int i = 0; i < m_listCount; i++)
                    {
                        if (!m_focusList.Contains(i))
                        {
                            SetFocusNew(i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < m_listCount; i++)
                    {
                        if (m_focusList.Contains(i))
                        {
                            SetFocusNew(i);
                        }
                    }
                }
            }
        }

        public void Init(int count, UpdateListItemEvent updateItem)
        {
            m_listCount = count;                  //记录有多少个item
            m_updateItem = updateItem;
            itemParent.transform.localPosition = Vector2.zero;
            rowSum = count / columnCount + (count % columnCount > 0 ? 1 : 0);      //计算有多少行，用于计算出总高度
            m_rectHeigh = Mathf.Max(0, rowSum * itemHeight + (rowSum - 1) * offsetY);
            if (m_spreadIndex >= 0)
                m_rectHeigh += spreadHeigh;
            itemParent.sizeDelta = new Vector2(m_rectWidth, m_rectHeigh);
            m_showCount = Mathf.Min(count, m_createCount);     //显示item的数量
            m_startIndex = 0;
            dic_itemIndex.Clear();
            //多选要重新清除一下，然后再设置
            if (selectType == eSelectType.Multi)
                m_focusList.Clear();
            for (int i = 0; i < m_showCount; i++)
            {
                Transform item = GetItem(i);
                SetItem(item, i);
            }
            ShowListCount(itemParent, m_showCount);         //显示多少个
            if (scrollBar != null)
                scrollBar.size = m_maskSize.y / m_rectHeigh;
            if (togSelectAll != null)
            {
                if (selectType == eSelectType.Single)
                    togSelectAll.gameObject.SetActive(false);
                else togSelectAll.gameObject.SetActive(count > 1);
            }
            ToEdge();
        }

        public void Refresh(int count, UpdateListItemEvent updateItem)
        {
            m_updateItem = updateItem;
            rowSum = count / columnCount + (count % columnCount > 0 ? 1 : 0);
            m_rectHeigh = Mathf.Max(0, rowSum * itemHeight + (rowSum - 1) * offsetY);
            if (m_spreadIndex >= 0)
                m_rectHeigh += spreadHeigh;
            itemParent.sizeDelta = new Vector2(m_rectWidth, m_rectHeigh);
            m_listCount = count;
            m_showCount = Mathf.Min(count, m_createCount);     //显示item的数量
            if (count == 0)
            {
                ShowListCount(itemParent, m_showCount);
                return;
            }

            dic_itemIndex.Clear();
            //计算起始的终止序号
            //--如果数量小于遮罩正常状态下能显示的总量
            if (count <= m_createCount)
            {
                m_startIndex = 0;
                m_endIndex = count - 1;
            }
            else
            {
                m_startIndex = GetStartIndex(itemParent.localPosition.y);
                if (m_startIndex + m_createCount >= count)
                {

                    m_startIndex = count - m_createCount;
                    m_endIndex = count - 1;
                }
                else
                {
                    m_endIndex = m_startIndex + m_createCount - 1;
                }
            }
            lastStartIndex = m_startIndex;
            if (m_endIndex < m_startIndex)
            {
                Debug.LogError("列表有问题！");
                return;
            }
            for (int i = m_startIndex; i <= m_endIndex; i++)
            {
                m_item = GetItem(i - m_startIndex);
                SetItem(m_item, i);
            }
            ShowListCount(itemParent, m_showCount);
            if (scrollBar != null)
                scrollBar.size = m_maskSize.y / m_rectHeigh;
        }
        //有就拿来用，没有就创建
        private Transform GetItem(int index)
        {
            Transform item = null;
            if (index < itemParent.childCount)
                item = itemParent.GetChild(index);
            else
                item = ((GameObject)GameObject.Instantiate(itemObj.gameObject)).transform;
            item.name = index.ToString();
            item.SetParent(itemParent);
            item.localScale = Vector3.one;
            return item;
        }


        private int GetStartIndex(float y)
        {
            int _spreadHeigh = m_spreadIndex >= 0 ? spreadHeigh : 0;        //展开占用的高度
            if (y <= (itemHeight + _spreadHeigh))
                return 0;
            float scrollHeigh = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            if (y >= (itemParent.sizeDelta.y - scrollHeigh - _spreadHeigh))        //拉到底部了
            {
                if (m_listCount <= m_createCount)
                    return 0;
                else return m_listCount - m_createCount;
            }
            return ((int)((y - _spreadHeigh) / (itemHeight + offsetY)) + ((y - _spreadHeigh) % (itemHeight + offsetY) > 0 ? 1 : 0) - 1) * columnCount;
        }

        private Vector2 GetPos(int index)
        {
            int spread = 0;
            if (m_spreadIndex >= 0 && spreadHeigh > 0)
            {
                if (index > m_spreadIndex)
                    spread = spreadHeigh;
            }
            return new Vector2(index % columnCount * (itemWidth + offsetX), -index / columnCount * (itemHeight + offsetY) - spread);
        }

        //当列表全部生成 外部调用 刷新滚动值
        public void ResetChangeValue()
        {
            itemParent.localPosition = curItemParentPos;
            ToEdge();
        }

        int lastStartIndex = 0;     //记录上次的初始序号
        List<int> newIndexList = new List<int>();
        List<int> changeIndexList = new List<int>();
        private void OnValueChange(Vector2 pos)
        {
            curItemParentPos = itemParent.localPosition;
            ToEdge();
            //如果列表总数小于
            if (scrollBar != null)
                scrollBar.value = itemParent.localPosition.y / (m_rectHeigh - m_maskSize.y);
            if (m_listCount <= m_createCount)
                return;
            m_startIndex = GetStartIndex(itemParent.localPosition.y);
            if (m_startIndex + m_createCount >= m_listCount)
            {

                m_startIndex = m_listCount - m_createCount;
                m_endIndex = m_listCount - 1;
            }
            else
            {
                m_endIndex = m_startIndex + m_createCount - 1;
            }
            if (m_startIndex == lastStartIndex)
                return;
            lastStartIndex = m_startIndex;
            newIndexList.Clear();
            changeIndexList.Clear();
            for (int i = m_startIndex; i <= m_endIndex; i++)
            {
                newIndexList.Add(i);
            }

            var e = dic_itemIndex.GetEnumerator();
            while (e.MoveNext())
            {
                int index = e.Current.Key;
                if (index >= m_startIndex && index <= m_endIndex)
                {
                    if (newIndexList.Contains(index))
                        newIndexList.Remove(index);
                    continue;
                }
                else
                {
                    changeIndexList.Add(e.Current.Key);
                }
            }
            for (int i = 0; i < newIndexList.Count && i < changeIndexList.Count; i++)
            {
                int oldIndex = changeIndexList[i];
                int newIndex = newIndexList[i];
                if (newIndex >= 0 && newIndex < m_listCount)
                {
                    m_item = dic_itemIndex[oldIndex];
                    dic_itemIndex.Remove(oldIndex);
                    SetItem(m_item, newIndex);
                    if (spreadTrans != null && m_spreadIndex > 0)
                    {
                        if (oldIndex == m_spreadIndex)
                            spreadTrans.gameObject.SetActive(false);        //如果旧的是有展开的，就隐藏掉展开
                        if (newIndex == m_spreadIndex)
                            spreadTrans.gameObject.SetActive(true);        //如果新的是有展开的，就显示展开
                    }

                }
            }
        }

        private void SetItem(Transform item, int index)
        {
            dic_itemIndex[index] = item;
            item.localPosition = GetPos(index);
            item.name = index.ToString();
            if (m_updateItem != null)
                m_updateItem(item, index);
            if (selectType == eSelectType.Null)
                return;
            m_choice = item.FindChild("choice");
            if (m_choice != null)
                SetChoiceActive(index, m_focusIndex == index);
            m_select = item.FindChild("select");
            if (m_select != null)
                SetSelectActive(index, m_focusList.Contains(index));
        }

        public void SetFocusNew(int index)
        {
            if (selectType == eSelectType.Single)
                SetChoice(index);
            else if (selectType == eSelectType.Multi)
                SetSelect(index);
        }

        public void SetChoice(int index)
        {
            SetChoiceActive(m_focusIndex, false);
            SetChoiceActive(index, true);
            m_focusIndex = index;
        }

        public void SetSpread(int index)
        {
            if (spreadHeigh == 0 || spreadTrans == null)
                return;
            if (index < 0)
                spreadTrans.gameObject.SetActive(false);
            else spreadTrans.gameObject.SetActive(true);
            m_spreadIndex = index;
            //刷新位置
            var e = dic_itemIndex.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.localPosition = GetPos(e.Current.Key);
                if (e.Current.Key == index)
                    spreadTrans.SetParent(e.Current.Value);
            }
            spreadTrans.localPosition = new Vector2(0, -itemHeight);
            itemParent.sizeDelta = new Vector2(m_rectWidth, m_rectHeigh + spreadHeigh);
        }

        public void SetSelect(int index)
        {
            if (m_focusList.Contains(index))
            {
                SetSelectActive(index, false);
                m_focusList.Remove(index);
            }
            else
            {
                SetSelectActive(index, true);
                m_focusList.Add(index);
            }
            if (togSelectAll != null)
            {
                if (m_focusList.Count < m_listCount && togSelectAll.isOn)
                    togSelectAll.isOn = false;
                else if (m_focusList.Count == m_listCount)
                    togSelectAll.isOn = true;
            }
        }

        //多选
        public void SetSelectList(List<int> indexList)
        {
            for (int i = 0; i < m_focusList.Count; i++)
            {
                if (!indexList.Contains(m_focusList[i]))
                    SetSelectActive(m_focusList[i], false);
            }
            for (int i = 0; i < indexList.Count; i++)
            {
                if (!m_focusList.Contains(indexList[i]))
                    SetSelectActive(indexList[i], true);
            }
            if (togSelectAll != null)
            {
                if (indexList.Count < m_listCount && togSelectAll.isOn)
                    togSelectAll.isOn = false;
                else if (indexList.Count == m_listCount)
                    togSelectAll.isOn = true;
            }
            m_focusList.Clear();
            m_focusList.AddRange(indexList);
        }

        public GameObject GetItemByIndex(int index)
        {
            if (dic_itemIndex.ContainsKey(index))
                return dic_itemIndex[index].gameObject;
            else return null;
        }

        public int GetChoice()
        {
            return m_focusIndex;
        }

        public List<int> GetSelectList()
        {
            return m_focusList;
        }

        private void SetChoiceActive(int index, bool active)
        {
            m_item = itemParent.FindChild(index.ToString());
            if (m_item != null)
            {
                m_choice = m_item.FindChild("choice");
                if (m_choice != null)
                {
                    if (m_choice.gameObject.activeSelf != active)
                        m_choice.gameObject.SetActive(active);
                }
            }
        }

        private void SetSelectActive(int index, bool active)
        {
            m_item = itemParent.FindChild(index.ToString());
            if (m_item != null)
            {
                m_select = m_item.FindChild("select");
                if (m_select != null)
                {
                    if (m_select.gameObject.activeSelf != active)
                        m_select.gameObject.SetActive(active);
                }
            }
        }

        /// <summary>
        /// 判断是否被选中
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetIsSelected(int index)
        {
            m_item = itemParent.FindChild(index.ToString());
            if (m_item != null)
            {
                m_select = m_item.FindChild("select");
                if (m_select != null)
                {
                    return m_select.gameObject.activeSelf;
                }
            }
            return false;
        }

        private List<int> GetCurFocus()
        {
            if (selectType == eSelectType.Single)
                return new List<int>() { m_focusIndex };
            else if (selectType == eSelectType.Multi)
                return m_focusList;
            else return null;
        }

        private void OnDragSB(GameObject s, float val)
        {
            UpdateParentPos(val);
        }

        private void UpdateParentPos(float dropVal)
        {
            itemParent.localPosition = new Vector2(0, dropVal * (m_rectHeigh - m_maskSize.y));
        }

        public void MoveToItem(int index)
        {
            if (index < 0 || index > m_listCount)
                return;
            float y;
            if (m_rectHeigh <= m_maskSize.y)
                y = 0;
            else y = Mathf.Clamp(index / columnCount * itemHeight + (index / columnCount) * offsetY, 0, m_rectHeigh - m_maskSize.y);
            itemParent.localPosition = new Vector3(0, y);
            lastStartIndex = -1;        //此处是为了能够确保强行刷一遍列表
            OnValueChange(itemParent.localPosition);
        }


        public void MoveToEnd()
        {
            if (m_rectHeigh <= m_maskSize.y)
                return;
            itemParent.localPosition = new Vector3(0, m_rectHeigh);
            lastStartIndex = -1;        //此处是为了能够确保强行刷一遍列表
            OnValueChange(itemParent.localPosition);
        }


        //显示子物体的数量
        private void ShowListCount(Transform trans, int num)
        {
            if (trans.childCount < num)
                return;
            for (int i = 0; i < num; i++)
            {
                trans.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = num; i < trans.childCount; i++)
            {
                trans.GetChild(i).gameObject.SetActive(false);
            }
        }

        public Transform topArrow;     //顶部箭头
        public Transform bottomArrow;        //底部箭头
        public System.Action switchPage;    //翻页
        private bool flash = false;
        private float offset = 40f;
        //是否到边界 距离边界20以内为到边界
        private void ToEdge()
        {
            if (scrollRect == null)
                return;
            float higth = scrollRect.GetComponent<RectTransform>().sizeDelta.y;
            //Debug.Log("hight=" + higth);
            float y = itemParent.sizeDelta.y - (itemParent.localPosition.y + higth);
            if (y > offset)
            {
                //Debug.Log("可以显示向下");
                if (bottomArrow != null)
                {
                    if (!bottomArrow.gameObject.activeSelf)
                        bottomArrow.gameObject.SetActive(true);
                }
                flash = true;
            }
            else if (y <= offset)
            {
                // Debug.Log("关闭下箭头");
                if (bottomArrow != null)
                {
                    if (bottomArrow.gameObject.activeSelf)
                        bottomArrow.gameObject.SetActive(false);
                }
                if (flash && switchPage != null)
                {
                    flash = false;
                    switchPage();
                }
            }
            if (itemParent.localPosition.y < offset)
            {
                //Debug.Log("关闭向上箭头");
                if (topArrow != null)
                {
                    if (topArrow.gameObject.activeSelf)
                        topArrow.gameObject.SetActive(false);
                }
            }
            else if (itemParent.localPosition.y >= offset)
            {
                //Debug.Log("显示向上箭头");
                if (topArrow != null)
                {
                    if (!topArrow.gameObject.activeSelf)
                        topArrow.gameObject.SetActive(true);
                }
            }
        }
    }
}
