using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public class UIHUD : MonoBehaviour
    {
        public GameObject m_item;
        public Transform m_parent;
        public GameObject m_effect;

        private List<HUDInfo> m_tempList = new List<HUDInfo>();
        private struct HUDInfo
        {
            public string txt;
            public Vector2 pos;
        }
        private bool m_Start = false;
        private float m_curTime = 0;
        private float m_maxTime = 0.01f;   // 间隔时间

        private LinkedList<GameObject> m_cacheList = new LinkedList<GameObject>();

        public virtual void Start()
        {
            m_item = transform.FindChild("item").gameObject;
            m_parent = transform.FindChild("parent").transform;
            m_item.SetActiveNew(false);
        }

        /// <summary>
        /// 如果是伤害中带属性伤害，用|分割
        /// </summary>
        public void Add(string txt, Vector2 pos)
        {
            //if (m_tempList.Count > m_maxCount)
            //{
            //    return;
            //}
            HUDInfo info;
            info.txt = txt;
            info.pos = pos;
            m_tempList.Add(info);

            m_Start = true;
        }

        private void Update()
        {
            if (m_Start)
            {
                m_curTime += Time.deltaTime;
                if (m_curTime > m_maxTime)
                {
                    HUDInfo hudInfo = m_tempList[0];
                    //Debug.Log("hud:" + hudInfo.txt);
                    Transform item;
                    if (m_cacheList.First != null)
                    {
                        item = m_cacheList.First.Value.transform;
                        m_cacheList.RemoveFirst();
                    }
                    else
                    {
                        item = GameObject.Instantiate(m_item).transform;
                    }
                    item.SetParent(m_parent);
                    item.gameObject.SetActiveNew(true);
                    item.localPosition = hudInfo.pos;

                    item.SetParent(GUIManager.m_PanelRoot);

                    string text = hudInfo.txt;
                    GameObject txtObj = UIItem.SetText(item, UIItemTitle.txt, text);

                    UIAnimationHelp help = UIAnimationHelp.Get(txtObj);
                    help.m_param = item.gameObject;
                    help.Init();
                    help.Play(OnAnimaEnd);

                    m_curTime = 0;
                    m_tempList.RemoveAt(0);
                    if (m_tempList.Count == 0)
                    {
                        m_Start = false;
                    }
                }
            }
        }

        private void OnAnimaEnd(object obj)
        {
            GameObject item = (GameObject)obj;
            item.SetActiveNew(false);
            m_cacheList.AddLast(item);
        }

        public bool is_send = false;
       
    }
}
