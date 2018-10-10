using System.Text;
using UnityEngine;

namespace Roma
{

    public class SceneDataResource : Resource
    {
        public SceneDataResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {
            m_byte = (m_assertBundle.LoadAsset<TextAsset>(m_resInfo.strName)).bytes;
          
            return true;
        }

        public byte[] GetData()
        {
            return m_byte;
        }
    }
}
