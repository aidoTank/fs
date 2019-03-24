//using System.Collections.Generic;

//using UnityEngine;

//namespace Roma
//{
//    // 用于扩展，可以根据类型来设置声音大小，静音
//    public enum SoundType
//    {
//        eBG = 0,    // 背景
//        eEnv,       // 环境 ，比如BOSS出现时
//        eEffect,    // 特效
//        eUI,        // UI
//        eSpeak,     // 说话
//        eMax
//    }

//    public class SoundEntity : DynamicEntity
//    {
//        public float m_volumn = 1.0f;
//        public SoundEntity(uint handle, EntityInitNotify notify, eEntityType eType, EntityBaseInfo entityInfo)
//            : base(handle, notify, eType, entityInfo)
//        {

//        }

//        public override void GetGameObjectByRes(Resource res)
//        {
//            // 声音实体自己创建对象,然后增加组件
//            if(res != null && m_object == null)
//            {
//                m_object = new GameObject(res.GetResInfo().strName + " " + m_handleID);
//                m_audioSource = m_object.AddComponent<AudioSource>();
//            }
//        }

//        public override void UpdateBaseInfo(Resource res)
//        {
//            if (res == null)
//                return;
//            if (null == m_object)
//            {
//                SetLayer((int)LusuoLayer.eEL_Sound);
//            }
//            m_audioSource.clip = ((SoundResource)res).GetAudioClip();
//            SetVolumn(m_volumn);
//            SetMute(false);
//            if (m_type == SoundType.eBG)
//            {
//                SetLoop(true);
//            }
//            else
//            {
//                SetLoop(false);
//            }
//            Play();
//        }

//        public bool Play()
//        {
//            if (null == m_object || null == m_audioSource)
//            {
//                return false;
//            }
//            if (m_audioSource.isActiveAndEnabled)
//                m_audioSource.Play();
//            return true;
//        }

//        public bool Stop(bool stop)
//        {
//            if (null == m_object || null == m_audioSource)
//            {
//                return false;
//            }
//            if (m_audioSource.isActiveAndEnabled)
//            {
//                if (stop)
//                    m_audioSource.Stop();
//                else
//                    m_audioSource.Play();
//            }
//            return true;
//        }

//        public bool IsPlaying()
//        {
//            if (null == m_object || null == m_audioSource)
//            {
//                return false;
//            }
//            return m_audioSource.isPlaying;
//        }

//        public bool IsLoop()
//        {
//            if (m_audioSource == null)
//                return false;
//            return m_audioSource.loop;
//        }

//        public float GetSoundLength()
//        {
//            if (null == m_object || null == m_audioSource)
//            {
//                return 0.0f;
//            }
//            return m_audioSource.clip.length;
//        }

//        public float GetVolumn()
//        {
//            return m_audioSource.volume;
//        }

//        public bool GetMute()
//        {
//            if (null == m_object || null == m_audioSource)
//            {
//                return m_audioSource.mute;
//            }
//            return false;
//        }

//        public void SetLoop(bool bLoop)
//        {
//            if(null == m_object || null == m_audioSource)
//            {
//                return;
//            }
//            m_audioSource.loop = bLoop;
//        }

//        public void SetVolumn(float fVal)
//        {
//            if (m_type == SoundType.eBG)   // 如果是背景默认小一点声音
//                fVal = fVal * 0.9f;
//            m_volumn = fVal;
//            if (null == m_object || null == m_audioSource)
//            {
//                return;
//            }
//            m_audioSource.volume = fVal;
//        }

//        public void SetMute(bool bMute)
//        {
//            if (null == m_object || null == m_audioSource)
//            {
//                return;
//            }
//            m_audioSource.mute = bMute;
//        }

//        public override void Destroy()
//        {
//            base.Destroy();
//            m_audioSource = null;
//        }

//        public SoundType m_type = SoundType.eMax;
//        private AudioSource m_audioSource;
//    }
//}