using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Roma
{
    public class UIBase : MonoBehaviour
    {
        public Transform m_root;
        public GameObject m_panel;
        private RectTransform m_rect;

        public Canvas m_canvasDynamic;    // 每个界面必须有
        public Canvas m_canvasStatic;
        public GraphicRaycaster m_grDynamic;
        public GraphicRaycaster m_grStatic;

        public Button m_btnClose;

        /// <summary>
        /// 当前界面的动态资源
        /// </summary>
        private List<Resource> m_dynamicResourceList;

        public virtual void Init()
        {
            m_root = transform;
            m_panel = m_root.FindChild("panel").gameObject;
            m_rect = m_panel.GetComponent<RectTransform>();

            Transform dynamicTrans = m_root.Find("panel/dynamic");
            if(dynamicTrans == null)
            {
                Debug.LogError("界面dynamic错误： " + m_root.name);
                return;
            }
            m_canvasDynamic = dynamicTrans.GetComponent<Canvas>();
            if(m_canvasDynamic == null)
                m_canvasDynamic = dynamicTrans.gameObject.AddComponent<Canvas>();
            m_grDynamic = dynamicTrans.GetComponent<GraphicRaycaster>();
            if (m_grDynamic == null)
                m_grDynamic = dynamicTrans.gameObject.AddComponent<GraphicRaycaster>();
            
            Transform staticTrans = m_root.Find("panel/static");
            if(staticTrans != null)
            {
                m_canvasStatic = staticTrans.GetComponent<Canvas>();
                if (m_canvasStatic == null)
                    m_canvasStatic = staticTrans.gameObject.AddComponent<Canvas>();
                m_grStatic = staticTrans.GetComponent<GraphicRaycaster>();
                if (m_grStatic == null)
                    m_grStatic = staticTrans.gameObject.AddComponent<GraphicRaycaster>();
            }

            //if (!m_panel.activeSelf)
            //    m_panel.SetActive(true);

            SetActive(false);

            var tempObj = dynamicTrans.FindChild("btn_close");
            if (tempObj != null)
                m_btnClose = tempObj.GetComponent<Button>();
        }

        public virtual void OpenPanel(bool bOpen)
        {
            //Debug.Log(bOpen + "界面" + transform.name);
            if (null == m_panel)
                return;
            if (bOpen)
            {
                SetActive(true);
            }
            else
            {
                SetActive(false);

                if(m_dynamicResourceList != null)
                {
                    for (int i = 0; i < m_dynamicResourceList.Count; i++)
                    {
                        ResourceFactory.Inst.UnLoadResource(m_dynamicResourceList[i]);
                    }
                    m_dynamicResourceList.Clear();
                }
            }
            return;
        }


        public virtual void SetActive(bool open)
        {
            if (m_canvasDynamic != null)
            {
                if (m_canvasDynamic.enabled == open)
                    return;

                m_canvasDynamic.enabled = open;
                m_grDynamic.enabled = open;
            }
 
            if (m_canvasStatic != null)
            {
                m_canvasStatic.enabled = open;
                m_grStatic.enabled = open;
            }
       
            if (open)
            {
                m_rect.anchoredPosition3D = Vector3.zero;
                m_rect.sizeDelta = Vector2.zero;
                // 避免一些预加载界面在开始时关闭无法设置层级，再次打开时需要设置开启层级
                SetOrder(m_canvasDynamic.sortingOrder);

                InitData();
            }
            else
            {
                m_rect.anchoredPosition3D = Vector3.up * 2000;
                m_rect.sizeDelta = Vector2.zero;
                UnInitData();
            }
        }

        public virtual bool IsShow()
        {
            if (null == m_panel)
                return false;
            return m_canvasDynamic.enabled;
        }

        public void SetOrder(int order)
        {
            if (m_canvasStatic != null)
            {
                if (!m_canvasStatic.overrideSorting)
                    m_canvasStatic.overrideSorting = true;
                if(order != m_canvasStatic.sortingOrder)
                    m_canvasStatic.sortingOrder = order;
            }

            if(m_canvasDynamic != null)
            {
                if (!m_canvasDynamic.overrideSorting)
                    m_canvasDynamic.overrideSorting = true;
                if (order + 1 != m_canvasDynamic.sortingOrder)
                    m_canvasDynamic.sortingOrder = order + 1;
            }
        }

        public int GetOrder()
        {
            if (m_canvasDynamic == null)
                return 0;
            return m_canvasDynamic.sortingOrder;
        }

        public virtual void InitData()
        {

        }


        public virtual void UnInitData()
        {

        }

        public void SetSubCanvas(GameObject parent)
        {
            Canvas canvas = parent.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = parent.AddComponent<Canvas>();
            }
            GraphicRaycaster gra = parent.GetComponent<GraphicRaycaster>();
            if (gra == null)
            {
                gra = parent.AddComponent<GraphicRaycaster>();
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        /// <param name="tra"></param>
        public virtual void CleanList(Transform tra)
        {
            if (tra.childCount != 0)
            {
                for (int i = 0; i < tra.childCount; i++)
                {
                    Transform child = tra.GetChild(i);
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// 删除单个
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tra"></param>
        public virtual void DeletList(int index, Transform tra)
        {
            if (tra.childCount != 0)
            {
                Transform child = tra.GetChild(index);
                Destroy(child.gameObject);
            }
        }
        public virtual void Update()
        {

        }

        /// <summary>
        /// 添加新的UI动态资源
        /// </summary>
        /// <param name="res"></param>
        public void OnAddDynamicResource(Resource res)
        {
            if (m_dynamicResourceList == null)
                m_dynamicResourceList = new List<Resource>();
            m_dynamicResourceList.Add(res);
        }
        /// <summary>
        /// 删除原有UI动态资源
        /// </summary>
        /// <param name="res"></param>
        public void OnDelDynamicResource(Resource res)
        {
            if (m_dynamicResourceList.Contains(res))
                m_dynamicResourceList.Remove(res);
        }
    }
}

