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
            TextAsset textAsset;
            if(GlobleConfig.m_downLoadType == eDownLoadType.LocalResource)
            {
                textAsset = m_editorRes as TextAsset;
            }
            else
            {
                textAsset = m_assertBundle.LoadAsset<TextAsset>(m_resInfo.strName);  
            }

            m_byte = textAsset.bytes;
          
            return true;
        }

        public byte[] GetData()
        {
            return m_byte;
        }
    }
}
