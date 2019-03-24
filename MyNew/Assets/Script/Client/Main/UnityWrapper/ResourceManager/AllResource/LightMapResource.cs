using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
namespace Roma
{
    public class LightMapResource : Resource
    {
        public LightMapResource(ref ResInfo res)
            : base(ref res)
        {
        }

        public override void Destroy()
        {
            if (null != m_Tex)
            {
                foreach (Texture2D tex in m_Tex)
                {
                    Texture2D.DestroyImmediate(tex, true);
                }
                m_Tex = null;
            }

            if (m_assertBundle != null)
            {
                m_assertBundle.Unload(true);
                Object.Destroy(m_assertBundle);
                m_assertBundle = null;
            }
        }

        public override bool OnLoadedLogic()
        {
            Object[] Objs = m_assertBundle.LoadAllAssets(typeof(Texture2D));

            m_Tex = new Texture2D[Objs.Length];
            for (int i = 0; i < Objs.Length; i++)
            {
                m_Tex[i] = (Texture2D)Objs[i];
                //Debug.Log("Load light Tex:" + m_Tex[i].name.ToString());
            }
            return null != m_assertBundle || null != m_Tex;
        }

        public Texture2D[] GetTex()
        {
            return m_Tex ;
        }

        private Texture2D[] m_Tex = null;
    }
}