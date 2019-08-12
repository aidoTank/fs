using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Roma
{
    public class Widget
    {
        protected LogicModuleIndex m_uiIndex;
        public GameObject m_uiObj;
        private UIBase m_ui;
        public bool m_bLoaded = false;
        public bool m_bLoading = false;
        public Action m_openEnd;

        public bool m_bNeedShow = true;

        public Widget(LogicModuleIndex uiOp)
        {
            m_uiIndex = uiOp;
        }

        public LogicModuleIndex GetMouduleIdx()
        {
            return m_uiIndex;
        }

        public T GetUI<T>() where T : Component
        {
            //if (null == m_ui)
            //{
            //    Debug.LogError("ui component is null:" + m_uiIndex);
            //}
            // 附加脚本
            m_ui = m_uiObj.AddComponent<T>() as UIBase;
            m_ui.Init();
            return (T)(System.Object)m_ui;
        }

        public virtual void SetVisible(bool bShow)
        {
            //Debug.Log(bShow+"界面"+ m_uiObj);
            m_bNeedShow = bShow;
            if (bShow)
            {
                if (!m_bLoading)
                {
                    m_bLoading = true;
                    // 加载
                    OnLoadPrefab();
                    return;
                }
                if (!m_bLoaded)
                    return;
            }

            if (m_ui == null)
                return;

            m_ui.OpenPanel(bShow);
            if (bShow)
            {
               
                InitData();
            }
            else
            {
                UnInit();
            }
        }

        public virtual bool IsShow()
        {
            if (m_ui != null)
            {
                return m_ui.IsShow();
            }
            return false;
        }

        public void OnLoadPrefab()
        {
            ResourceFactory.Inst.LoadResource((int)m_uiIndex + 1000, (res) =>
            {
                m_uiObj = res.InstantiateGameObject();
                m_uiObj.name = res.GetResInfo().strName;
                if (m_uiObj == null)
                {
                    Debug.LogError("UI资源加载失败:" + m_uiIndex);
                    return;
                }
                RectTransform rect = m_uiObj.GetComponent<RectTransform>();
                rect.SetParent(GUIManager.m_PanelRoot);
                rect.localPosition = Vector3.one;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector2.zero;
                rect.anchoredPosition = Vector3.zero;
                rect.sizeDelta = Vector2.zero;
                rect.localScale = Vector3.one;

                m_bLoaded = true;
                Init();

                SetVisible(m_bNeedShow);

            });
        }

        public virtual void Init()
        {

        }

        public virtual void InitData()
        {
            if (m_openEnd != null)
            {
                m_openEnd();
                m_openEnd = null;
            }
        }

        public virtual void UnInit()
        {

        }

        public virtual void UpdateUI(float fTime, float fDTime)
        {

        }
    }
}

