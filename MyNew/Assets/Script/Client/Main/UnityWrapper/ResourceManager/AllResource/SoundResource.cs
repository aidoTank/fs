using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{

    public class SoundResource : Resource
    {
        public SoundResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override void Destroy()
        {
            base.Destroy();
            m_audioClip = null;
        }

        public override bool OnLoadedLogic()
        {
            AudioClip m_audioClip;
            if(GlobleConfig.m_downLoadType == eDownLoadType.LocalResource)
            {
                m_audioClip = m_editorRes as AudioClip;
            }
            else
            {
                m_audioClip = m_assertBundle.LoadAsset<AudioClip>(m_resInfo.strName);
            }

            return null != m_audioClip;
        }
       

        public AudioClip GetAudioClip()
        {
            return m_audioClip;
        }

        private AudioClip m_audioClip = null;
    }
}
