using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Roma
{
    public class GUIManager : Singleton
    {
        public GUIManager()
            : base(true)
        {
        }

        public override void Init()
        {
            ms_gameRoot = GameObject.Find("game");
            GameObject root = GameObject.Find("ui_root");
            if (root != null)
            {
                ms_UIRoot = root.transform;
                ms_UIRoot.GetComponent<Canvas>().sortingOrder = 0;
                ms_uiCamera = ms_UIRoot.FindChild("camera").GetComponent<Camera>();
                m_PanelRoot = ms_UIRoot.FindChild("panel_root");
            }
            InitParam();
            InitEvent();

        }

        public override void Update(float fTime, float fDTime)
        {

        }

        public override void Destroy()
        {
            
            GameObject.DestroyObject(ms_UIRoot);
        }

        private static void InitParam()
        {
            m_screenWidth = Screen.width;
            m_screenHeight = Screen.height;
            m_screenHalf = new Vector2(m_screenWidth * 0.5f, m_screenHeight * 0.5f);

            if(ms_UIRoot != null)
            {
                m_uiWidthScale = ms_UIRoot.GetComponent<RectTransform>().sizeDelta.x / m_screenWidth;
                m_uiHeightScale = ms_UIRoot.GetComponent<RectTransform>().sizeDelta.y / m_screenHeight;
                m_uiScreenScale = new Vector2(m_uiWidthScale, m_uiHeightScale);
            }
        }

        /// <summary>
        /// UGUI的一个BUG，要在最开始重新激活才生效
        /// </summary>
        private static void InitEvent()
        {
            GameObject esObj = GameObject.Find("EventSystem");
            EventSystem es = esObj.GetComponent<EventSystem>();
            es.enabled = false;
            es.enabled = true;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                StandaloneInputModule stand = esObj.GetComponent<StandaloneInputModule>();
                stand.enabled = true;
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                StandaloneInputModule stand = esObj.GetComponent<StandaloneInputModule>();
                TouchInputModule touch = esObj.GetComponent<TouchInputModule>();
                stand.enabled = false;
                touch.enabled = true;
            }
        }


        public static float GetWidthScale()
        {
            //if (Application.isEditor)
            //{
            //    InitParam();
            //}
            return m_uiWidthScale;
        }

        public static float GetHeightScale()
        {
            if (Application.isEditor)
            {
                InitParam();
            }
            return m_uiHeightScale;
        }

        public static Vector2 GetScale()
        {
            return m_uiScreenScale;
        }

        /// <summary>
        /// 左下角0，0
        /// </summary>
        /// <param name="uiObj"></param>
        /// <returns></returns>
        public static Vector2 GetUIObjPos(Transform uiObj)
        {
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(ms_uiCamera, uiObj.transform.position);
            pos = new Vector2(
                       (pos.x - (m_screenHalf.x)) * GUIManager.GetWidthScale(),
                       (pos.y - (m_screenHalf.y)) * GUIManager.GetHeightScale());
            RectTransform rect = uiObj.GetComponent<RectTransform>();
            if (rect.anchorMin == Vector2.up && rect.anchorMax == Vector2.up && rect.pivot == Vector2.up)
            {
                pos = pos + new Vector2(rect.sizeDelta.x, -1 * rect.sizeDelta.y) * 0.5f;
            }
            return pos;
        }


        public static Vector2 GetSceneObjPos(Vector3 worldPos)
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(worldPos);
            pos = new Vector2(
                       (pos.x - m_screenHalf.x) * GUIManager.GetWidthScale(),
                       (pos.y - m_screenHalf.y) * GUIManager.GetHeightScale());
            return pos;
        }

        public static Vector2 GetUIPos(Vector3 wPos)
        {
            Vector2 pos = Vector2.zero;
            if (CameraMgr.Inst.m_cam != null)
            {
                pos = CameraMgr.Inst.m_cam.WorldToScreenPoint(wPos);
            }
            if (pos != Vector2.zero)
            {
                float x = (pos.x - GUIManager.m_screenHalf.x) * GUIManager.GetWidthScale();
                float y = (pos.y - GUIManager.m_screenHalf.y) * GUIManager.GetHeightScale();

                //if (x < GUIManager.m_screenRect.x ||
                //    x > GUIManager.m_screenRect.y ||
                //    y < GUIManager.m_screenRect.z ||
                //    y > GUIManager.m_screenRect.w
                //   )
                //{
                //    return Vector2.zero;
                //}
                return new Vector2(x, y);
            }
            return Vector2.zero;

        }

        public static GUIManager Inst;

        public static GameObject ms_gameRoot;
        public static Transform ms_UIRoot;
        public static Transform m_PanelRoot;
        public static Camera ms_uiCamera;

        public static float m_screenWidth;
        public static float m_screenHeight;
        public static Vector2 m_screenHalf;
        public static float m_uiWidthScale;
        public static float m_uiHeightScale;
        public static Vector2 m_uiScreenScale;
    }
}

