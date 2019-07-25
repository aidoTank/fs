using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

namespace Roma
{

    public class ShaderResource : Resource
    {
        public ShaderResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {
            Shader shader = (Shader)m_assertBundle.LoadAsset(m_resInfo.strName);
            if (Application.isEditor)
            {
                shader = Shader.Find(shader.name);
            }

            return true;
        }
    }
}
