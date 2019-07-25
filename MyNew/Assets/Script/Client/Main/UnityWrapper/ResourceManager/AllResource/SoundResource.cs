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
            if(GlobleConfig.m_downLoadType == eDownLoadType.LocalResource)
            {
                m_audioClip = m_editorRes as AudioClip;
            }
            else
            {
                object sndObj = m_assertBundle.LoadAsset(m_resInfo.strName);
                if (sndObj != null)
                {
                    m_audioClip = (AudioClip)sndObj;
                }
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
