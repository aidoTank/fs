using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    public class EffectEntity : Entity
    {

        private ParticleSystem[] m_listParticle;
        private Animation[] m_listAnimation;

        private TrailRenderer[] m_listTrail;
        private float[] m_listTrailTime;
        private LineRenderer[] m_listLine;

        public EffectEntity(int handle, Action<Entity> notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {
       
        }

        public override void UpdateBaseInfo(Resource res)
        {
            base.UpdateBaseInfo(res);
            if(m_object != null)
            {
                m_listParticle = m_object.GetComponentsInChildren<ParticleSystem>();
                m_listAnimation = m_object.GetComponentsInChildren<Animation>();
                //m_listTrail = m_object.GetComponentsInChildren<TrailRenderer>();
                //m_listTrailTime = new float[m_listTrail.Length];
                //for (int i = 0; i < m_listTrail.Length; i++)
                //{
                //    m_listTrailTime[i] = m_listTrail[i].time;
                //}
                //m_listLine = m_object.GetComponentsInChildren<LineRenderer>();
            }
        }

        /// <summary>
        /// 特效在重复使用时，需激活
        /// </summary>
        public override void Revive(int handleId, Action<Entity> notity, EntityBaseInfo baseInfo)
        {
            base.Revive(handleId, notity, baseInfo);
            SetActive(true);
        }

        /// <summary>
        /// 比如玩家隐身，显示时，内部特效不应该激活播放行为
        /// </summary>
        public override void SetShow(bool bActive)
        {
            base.SetShow(bActive);
            if(!bActive)
            {
                SetActive(false);
            }
        }

        // 设置激活
        public void SetActive(bool bActive)
        {
            // 粒子重新播放
            if (m_listParticle != null)
            {
                for (int i = 0; i < m_listParticle.Length; i++)
                {
                    if (bActive)
                    {
                        m_listParticle[i].Play(true);
                    }
                    else
                    {
                        m_listParticle[i].Stop();
                    }
                }
            }

            // 部分特效有动画组件，需要重新播放
            if(m_listAnimation != null)
            {
                for (int i = 0; i < m_listAnimation.Length; i++)
                {
                    Animation anima = m_listAnimation[i];
                    if (bActive)
                    {
                        anima.Play();
                    }
                    else
                    {
                        anima.Stop();
                    }
                }
            }

            //if (m_listTrail != null)
            //{
            //    for (int i = 0; i < m_listTrail.Length; i++)
            //    {
            //        TrailRenderer ren = m_listTrail[i];
            //        if (bActive)
            //        {
            //            ren.time = m_listTrailTime[i];
            //        }
            //        else
            //        {
            //            ren.time = 0;
            //        }
            //    }
            //}
        }
    }
}
