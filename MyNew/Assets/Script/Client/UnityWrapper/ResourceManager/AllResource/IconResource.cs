using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

namespace Roma
{

    public class IconResource : Resource
    {
        private Texture2D m_tex;
        public IconResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {
            m_tex = m_assertBundle.LoadAsset<Texture2D>(m_resInfo.strName);
            return true;
        }

        public Texture2D GetTexture()
        {
            return m_tex;
        }
    }
}
