using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Roma
{

    public class UIGray : MonoBehaviour
    {
        public Image m_image;
        public RawImage m_rawimage;
        public Text m_text;

        private GameObject m_obj;
        private Material m_matGray;
        private List<MaskableGraphic> m_childList = new List<MaskableGraphic>();
        private bool m_gray = false; // 默认可用，变灰不可以再变色
        private bool m_init = false;

        public void Start()
        {
            if (m_init) return;
            m_init = true;
            //Button btn = GetComponent<Button>();
            //if (btn != null)
            //    GameObject.Destroy(btn);
            m_obj = gameObject;
            if (null == m_image)
                m_image = GetComponent<Image>();
            if (m_rawimage == null)
            {
                m_rawimage = GetComponent<RawImage>();
            }
            if (null == m_text)
            {
                if (transform.childCount > 0 && transform.FindChild("text") != null)
                    m_text = transform.FindChild("text").GetComponent<Text>();
            }
        }


        public void SetGray(bool isgray, bool isactive)
        {
            m_gray = isgray;
            // if (!isgray)
            //     SetShader(eBtnEffectTpye.None);
            // else
            //     SetShader(eBtnEffectTpye.Gray);
            CanvasGroup g = gameObject.GetComponent<CanvasGroup>();
            if (g == null) g = gameObject.AddComponent<CanvasGroup>();
            g.blocksRaycasts = isactive;
        }

        public void SetGrayButActive(bool isgray)
        {
            m_gray = isgray;
            // if (!m_gray)
            //     SetShader(eBtnEffectTpye.None);
            // else
            //     SetShader(eBtnEffectTpye.Gray);
            CanvasGroup g = gameObject.GetComponent<CanvasGroup>();
            if (g == null) g = gameObject.AddComponent<CanvasGroup>();
            g.blocksRaycasts = true;
        }


        // private void SetShader(eBtnEffectTpye type)
        // {
        //     Start();
        //     m_childList.Clear();
        //     GetAllChild(m_obj.transform, ref m_childList);
        //     if (type == eBtnEffectTpye.None && !m_gray)
        //     {
        //         for (int i = 0; i < m_childList.Count; i++)
        //         {
        //             m_childList[i].material = null;
        //         }
        //         m_childList.Clear();
        //     }
        //     else
        //     {
        //         for (int i = 0; i < m_childList.Count; i++)
        //         {
        //             if (m_childList[i] == null || m_childList[i].defaultMaterial == null)
        //                 return;
        //             if (m_gray && type != eBtnEffectTpye.Gray)
        //                 return;
        //             if (m_matGray == null)
        //             {
        //                 Shader shaderG = ShaderManager.Inst.GetShader(eShaderRes.eButtonGray);
        //                 if (shaderG != null)
        //                     m_matGray = new Material(shaderG);
        //             }
        //             if (m_image != null)
        //                 m_image.material = m_matGray;
        //             if (m_rawimage != null)
        //                 m_rawimage.material = m_matGray;
        //             if (m_text != null)
        //                 m_text.material = m_matGray;
        //         }
        //     }
        // }

        private void GetAllChild(Transform parent, ref List<MaskableGraphic> list)
        {
            MaskableGraphic ren = parent.GetComponent<MaskableGraphic>();
            if (ren != null)
            {
                list.Add(ren);
            }
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform item = parent.GetChild(i);
                GetAllChild(item, ref list);
            }
        }

        public static UIGray Get(GameObject go)
        {
            UIGray load = go.GetComponent<UIGray>();
            if (load != null)
            {
                load.Start();
            }
            else
            {
                load = go.AddComponent<UIGray>();
                load.Start();
            }
            return load;
        }

        public void DestroyCurBtn()
        {
            Destroy(this);
        }
    }
}