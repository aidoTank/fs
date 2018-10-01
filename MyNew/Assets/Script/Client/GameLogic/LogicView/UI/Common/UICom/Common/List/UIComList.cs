//using UnityEngine;
//using System.Collections;
//using System.ComponentModel;
//using UnityEngine.UI;

//// 使用这种委托逻辑层和UI层不用考虑item的数据列表存储和对接，方便快速开发
//public delegate void UpdateListItem(Transform item, int index);
//public class UIComList : MonoBehaviour
//{
//    public GridLayoutGroup m_itemParent;
//    public GameObject m_item;
//    public ScrollRect m_scrollRect;
//    public Transform m_itemFocus;

//    public void Init()
//    {
//        for (int i = 0; i < m_itemParent.transform.childCount; i++)
//        {
//            m_itemParent.transform.GetChild(i).gameObject.SetActive(false);
//        }
//        if (m_item != null)
//        {
//            m_item.SetActive(false);
//        }
//    }

//    public Transform CreateItem(int index)
//    {
//        Transform item = m_itemParent.transform.FindChild(index.ToString());
//        if (item == null)
//        {
//            item = ((GameObject)GameObject.Instantiate(m_item)).transform;
//            item.SetParent(m_itemParent.transform);
//            item.name = index.ToString();
//            item.localScale = Vector3.one;
//        }
//        item.gameObject.SetActive(true);
//        return item;
//    }

//    public void UnInit()
//    {
//        StartCoroutine(UpdateListHeight());
//    }

//    IEnumerator UpdateListHeight()
//    {
//        yield return new WaitForEndOfFrame();
//        // 这里只对垂直方向做了更新
//        m_itemParent.GetComponent<RectTransform>().sizeDelta =
//            new Vector2(0, m_itemParent.minHeight);
//    }
//}
