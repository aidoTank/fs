using UnityEngine;

namespace Roma
{
    public delegate void OnClickEvent(int index);
    /// <summary>
    /// 按钮父节点命名为list
    /// 焦点图片命名为focus
    /// 可自动查找
    /// </summary>
    public class UITab : MonoBehaviour
    {
        public Transform m_btnParent;
        public RectTransform m_focus;

        public Color m_focusTextColor = new Color(0.63f, 0.3f, 0.01f);
        public bool m_changeTextColor = false;

        private OnClickEvent m_onClickBtn;
        private int m_index;    //当前选项卡

        private bool m_init = false;

        //动画需求块
        public Transform pos;
        public GameObject[] m_tabList;

        public bool m_aniable=false;

        public void Init()
        {
            if (m_init) return;
            m_init = true;

            if (m_btnParent == null)
                m_btnParent = transform;
            if (m_focus == null)
            {
                m_focus = transform.FindChild("focus").GetComponent<RectTransform>();
            }

            if (m_aniable)
            {
                for (int i = 0; i < m_btnParent.childCount - 2; i++)
                {
                    UIEventListener listener = UIEventListener.Get(m_btnParent.FindChild(i.ToString()).gameObject);
                    listener.onClick = OnClickBtn;
                    listener.parameter = i;
                    //UIButton.Get(listener.gameObject);
                }
            }
            else
            {
                for (int i = 0; i < m_btnParent.childCount - 1; i++)
                {
                    UIEventListener listener = UIEventListener.Get(m_btnParent.FindChild(i.ToString()).gameObject);
                    listener.onClick = OnClickBtn;
                    listener.parameter = i;
                    //UIButton.Get(listener.gameObject);
                }
            }


            if (m_btnParent.childCount - 1 > 0 && m_changeTextColor)
            {
                //UIButton.Get(m_btnParent.FindChild("0").gameObject).SetTextColor(m_focusTextColor);
            }


            if (m_aniable)
            {
                pos = transform.FindChild("light/pos");
                m_tabList = new GameObject[m_btnParent.transform.childCount - 2];
                for (int i = 0; i < m_tabList.Length; i++)
                {
                    m_tabList[i] = UIItem.GetChild(m_btnParent.transform, i.ToString()).gameObject;
                }
            }
        }

        public void SetFocusColor(Color color)
        {
            m_focusTextColor = color;
        }

        /// <summary>
        /// tab可点击开关
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        public void SetTabEnable(bool  active)
        {
            //m_focus.SetActiveNew(active);
            for (int i = 0; i < m_btnParent.childCount - 1; i++)
            {
                UIItem.SetGray(transform.gameObject, !active, active);
            }
        }


        // 逻辑注册
        public void RegisterClickEvent(OnClickEvent click)
        {
            m_onClickBtn = click;
        }

        public void SetIndex(int index)
        {
            if (index > m_btnParent.childCount - 1 || index < 0)
            {
                return;
            }

            Transform trans = UIItem.GetChild(m_btnParent, index.ToString());
            if(null == trans || trans == m_focus)
            {
                return;
            }

            m_index = index;
            if (m_focus != null)
            {
                m_focus.gameObject.SetActiveNew(true);
                m_focus.SetParent(trans);
                m_focus.localPosition = Vector3.zero;
                m_focus.localScale = Vector3.one;
                m_focus.SetSiblingIndex(1);
            }
            if (m_aniable)
            {
                SetTabAnima(index);
            }
        }

        /// <summary>
        /// 设置未读
        /// </summary>
        /// <param name="index">0>leng-1</param>
        /// <param name="unread"></param>
        public void SetUnread(int index, bool unread)
        {
            if (index >= m_btnParent.childCount || index < 0)
            {
                return;
            }
            Transform item = m_btnParent.FindChild(index.ToString());
            if(item == null)
            {
                Debug.LogError("item:" + index);
            }
            else
            {
                GameObject obj = item.gameObject;
                UIItem.GetChild(obj.transform, "unread").gameObject.SetActiveNew(unread);
            }

        }


        public Transform GetIndexItem(int index)
        {
            return m_btnParent.FindChild(index.ToString());
        }

        public void SetIndexActive(int index, bool active)
        {
            if (m_btnParent == null)
                return;
            Transform item = m_btnParent.FindChild(index.ToString());
            if (item != null)
            {
                item.gameObject.SetActiveNew(active);
            }
        }

        public void SetTipsActive(int index, bool active)
        {
            Transform item = m_btnParent.FindChild(index.ToString());
            if (item != null)
            {
                Transform tips = item.FindChild("tips");
                if (tips != null)
                    tips.gameObject.SetActiveNew(active);
            }
        }

        public void HideFocus()
        {
            if (m_focus != null)
                m_focus.gameObject.SetActiveNew(false);
        }

        public int GetIndex()
        {
            return m_index;
        }
        public void SetTabTxtOnIndex(int index, string value)
        {
            UIItem.SetText(m_btnParent, index + "/txt", value);
        }

        private void OnClickBtn(GameObject go)
        {
            int index = (int)UIEventListener.Get(go).parameter;
            m_index = index;
            if (m_focus != null)
            {
                m_focus.gameObject.SetActiveNew(true);
                m_focus.SetParent(go.transform);
                m_focus.localPosition = Vector3.zero;
                m_focus.localScale = Vector3.one;
                m_focus.SetSiblingIndex(1);
            }
            for (int i = 0; i < m_btnParent.childCount - 1; i++)
            {
                GameObject obj = m_btnParent.FindChild(i.ToString()).gameObject;
            }
            //LogicSystem.Inst.PlaySound((int)eUISound.tab);
            

            if (m_onClickBtn != null)
                m_onClickBtn(index);

            if (m_aniable)
            {
                SetTabAnima(index);
            }
        }


        public void SetTabAnima(int index)
        {
            GameObject go = m_tabList[index];
            TweenPosition tw = TweenPosition.Get(pos.gameObject);
            tw.from = pos.transform.localPosition;
            tw.to = go.transform.localPosition;
            tw.duration = 0.2f;
            tw.Play(true);
            tw.Reset();
        }
        
        public static UITab Get(GameObject go)
        {
            UITab load = go.GetComponent<UITab>();
            if (load == null) load = go.AddComponent<UITab>();
            if (!load.m_init) load.Init();
            return load;
        }

        //设置选中动画播放
        public void SetToggleAni()
        {

        }
    }
}
