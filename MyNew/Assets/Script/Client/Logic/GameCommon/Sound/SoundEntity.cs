using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    // 1.UI声音（包含UI特效）自己播放完销毁，
    // 如果声音做了缓存，而且声音时间比特效短，
    // 声音先会放入缓存池，
    // 连续播放特效会发生声音不完整的情况,因为是UI连续的操作所以不太影响
    // 2.背景声音由程序功能控制，
    // 3.其他特效声音由特效创建和销毁
    public enum SoundType
    {
        None = 1,
        eUI,        // UI声音，按钮点击，UI特效，英雄展示说话
        eBG,        // 主界面背景，地图背景
        eSceneEffect, // 场景中的特效
        eSpeak,     // 战斗中英雄说话
        eEnv,       // 环境 ，比如BOSS出现时
        eMax
    }

    public class SoundEntity : Entity
    {
        public bool m_bBgmRemove;
        private float m_curBgmRemoveTime = 0;
        public Action m_playEnd;

        public SoundEntity(int handle, Action<Entity> notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {

        }


        public override void Update(float fTime, float fDTime)
        {
            base.Update(fTime, fDTime);

            if (m_bBgmRemove)
            {
                m_curBgmRemoveTime += fDTime;
                if (m_curBgmRemoveTime >= 2)
                {
                    m_bBgmRemove = false;
                    m_curBgmRemoveTime = 0;
                    SetVolumn(0.6f);

                    SoundManager.Inst.RemoveLast(m_hid);
                    return;
                }
                float val = Mathf.Lerp(0.6f, 0, m_curBgmRemoveTime / 2);
                SetVolumn(val);
            }
        }

        public override void InstantiateGameObject(Resource res)
        {
            if (res != null && m_object == null)
            {
                m_object = new GameObject(res.GetResInfo().strName + " " + m_hid);
                if (m_object != null)
                {
                    m_audioSource = m_object.AddComponent<AudioSource>();
                    m_transform = m_object.transform;
                }
            }
        }

        public override void UpdateBaseInfo(Resource res)
        {
            if (res == null)
                return;
            if (!(res is SoundResource))
            {
                Debug.LogError(res.GetResInfo().strName + " 不是声音资源");
                return;
            }
            m_audioSource.clip = ((SoundResource)res).GetAudioClip();
            if (m_entityInfo.m_soundLoop)
            {
                SetLoop(true);
            }
            else
            {
                SetLoop(false);
            }
            if (m_entityInfo.m_soundType == (int)SoundType.eBG)
            {
                SetLoop(true);
                SetVolumn(0.6f);
            }
            else if (m_entityInfo.m_soundType == (int)SoundType.eUI)
            {
                SetVolumn(0.6f);
            }

            // 特效3D音效,这里并没有区分是界面特效声音，还是场景特效声音
            if (m_entityInfo.m_soundType == (int)SoundType.eSceneEffect ||
                m_entityInfo.m_soundType == (int)SoundType.eSpeak)
            {
                m_audioSource.dopplerLevel = 0;
                m_audioSource.spread = 360;
                m_audioSource.rolloffMode = AudioRolloffMode.Linear;
                m_audioSource.spatialBlend = 1;
                m_audioSource.minDistance = 0;
                m_audioSource.maxDistance = 30;
            }
            else
            {
                m_audioSource.spatialBlend = 0;  // 2D音效
            }
            SetBaseInfo();
            Play();
        }

        public override void SetBaseInfo()
        {
            base.SetBaseInfo();
            SetMute(m_entityInfo.m_soundMute);
        }

        public bool Play()
        {
            if (null == m_object || null == m_audioSource)
            {
                return false;
            }
            if (m_audioSource.enabled && !GetMute() && !IsPlaying())
                m_audioSource.Play();
            return true;
        }

        public bool Stop(bool stop)
        {
            if (null == m_object || null == m_audioSource)
            {
                return false;
            }
            if (m_audioSource.enabled)
            {
                if (stop)
                    m_audioSource.Stop();
                else
                    m_audioSource.Play();
            }
            return true;
        }

        public bool IsPlaying()
        {
            if (null == m_object || null == m_audioSource)
            {
                return false;
            }
            return m_audioSource.isPlaying;
        }

        public bool IsLoop()
        {
            if (m_audioSource == null)
                return false;
            return m_audioSource.loop;
        }

        public float GetSoundLength()
        {
            if (null == m_object || null == m_audioSource)
            {
                return 0.0f;
            }
            return m_audioSource.clip.length;
        }

        public float GetVolumn()
        {
            return m_audioSource.volume;
        }

        public bool GetMute()
        {
            if (null == m_object || null == m_audioSource)
            {
                return false;
            }
            return m_audioSource.mute;
        }

        public void SetLoop(bool bLoop)
        {
            if (null == m_object || null == m_audioSource)
            {
                return;
            }
            m_audioSource.loop = bLoop;
        }

        public void SetVolumn(float fVal)
        {
            m_entityInfo.m_soundVolume = fVal;
            if (null == m_object || null == m_audioSource)
            {
                return;
            }
            m_audioSource.volume = fVal;
        }

        public void SetMute(bool bMute)
        {
            m_entityInfo.m_soundMute = bMute;
            if (null == m_object || null == m_audioSource)
            {
                return;
            }
            m_audioSource.mute = bMute;
            if (!bMute && m_entityInfo.m_soundType == (int)SoundType.eBG)
            {
                Play();
            }
        }

        public override void SetShow(bool bActive, bool bStop = true)
        {
            if (null == m_audioSource)
                return;
            m_audioSource.enabled = bActive;
            if (bActive)
            {
                Play();
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            m_audioSource = null;
        }

        private AudioSource m_audioSource;
    }
}