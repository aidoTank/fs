using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    public class EffectEntity : Entity
    {
        private ParticleSystem[] m_listParticle;
        private Animation[] m_listAnimation;
        private Light[] m_light;

        // 链式特效
        private LineRenderer[] m_lineRender;
        private Transform m_lineStart;
        private Transform m_lineEnd;

        public EffectEntity(int handle, Action<Entity> notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {

        }

        public override void UpdateBaseInfo(Resource res)
        {
            base.UpdateBaseInfo(res);
            if (m_object != null)
            {
                m_listParticle = m_object.GetComponentsInChildren<ParticleSystem>();
                m_listAnimation = m_object.GetComponentsInChildren<Animation>();
                m_light = m_object.GetComponentsInChildren<Light>();
            }
        }

        /// <summary>
        /// 特效在重复使用时，需重新播放，在回收时暂停播放，不应该在SetShow中处理
        /// </summary>
        public override void Revive(int handleId, Action<Entity> notity, EntityBaseInfo baseInfo)
        {
            base.Revive(handleId, notity, baseInfo);
            Play(true);
        }

        public override void Stop()
        {
            base.Stop();
            Play(false);
        }

        /// <summary>
        /// 比如玩家隐身，显示时，内部特效不应该激活播放行为
        /// bStop:隐藏时，是否停止特效的行为
        /// </summary>
        public override void SetShow(bool bActive, bool bStop = true)
        {
            base.SetShow(bActive);
            //Debug.Log("name:" + m_transform.name + "  "+ bActive);
        }

        // 设置激活
        public void Play(bool bActive)
        {
            // 粒子重新播放
            if (m_listParticle != null)
            {
                for (int i = 0; i < m_listParticle.Length; i++)
                {
                    ParticleSystem p = m_listParticle[i];
                    if (p == null)
                    {
                        //Debug.LogError("特效粒子为空：" + m_entityInfo.m_resID);
                        continue;
                    }

                    if (bActive)
                    {
                        p.Play();
                    }
                    else
                    {
                        p.Stop();
                        p.Clear();
                    }
                }
            }

            // 部分特效有动画组件，需要重新播放
            if (m_listAnimation != null)
            {
                for (int i = 0; i < m_listAnimation.Length; i++)
                {
                    Animation anima = m_listAnimation[i];
                    if (anima == null)
                    {
                        //Debug.LogError("特效动画为空：" + m_entityInfo.m_resID);
                        continue;
                    }

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

            if (m_light != null)
            {
                for (int i = 0; i < m_light.Length; i++)
                {
                    Light p = m_light[i];
                    if (p == null)
                    {
                        //Debug.LogError("特效粒子为空：" + m_entityInfo.m_resID);
                        continue;
                    }

                    if (bActive)
                    {
                        p.enabled = true;
                    }
                    else
                    {
                        p.enabled = false;
                    }
                }
            }
        }

        public override void Update(float fTime, float fDTime)
        {
            base.Update(fTime, fDTime);
            UpdateLine();
        }

        public override void Destroy()
        {
            base.Destroy();
            m_lineRender = null;
            m_lineStart = null;
            m_lineEnd = null;
        }

        public void UpdateLine()
        {
            if (m_lineStart == null ||
                m_lineEnd == null ||
                m_lineRender == null)
                return;

            for (int i = 0; i < m_lineRender.Length; i++)
            {
                LineRenderer line = m_lineRender[i];
                line.SetPosition(0, m_lineStart.position);
                line.SetPosition(1, m_lineEnd.position);
            }
        }

        public void SetLineStart(Vector3 sPos)
        {
            if (!IsInited())
                return;
            if (m_lineStart == null)
                m_lineStart = GetObject().transform.FindChild("0");
            if (m_lineStart == null)
                return;

            m_lineStart.position = sPos;
        }

        public void SetLineEnd(Vector3 tPos)
        {
            if (!IsInited())
                return;

            if (m_lineStart == null)
                m_lineStart = GetObject().transform.FindChild("0");
            if (m_lineEnd == null)
                m_lineEnd = GetObject().transform.FindChild("1");
            if (m_lineRender == null)
                m_lineRender = GetObject().GetComponentsInChildren<LineRenderer>();

            if (m_lineStart == null ||
                m_lineEnd == null ||
                m_lineRender == null)
                return;

            if (tPos.y == 0)
            {
                m_lineEnd.position = tPos + Vector3.up * m_lineStart.position.y;
            }
            else
            {
                m_lineEnd.position = tPos;
            }

            for (int i = 0; i < m_lineRender.Length; i++)
            {
                LineRenderer line = m_lineRender[i];
                line.numPositions = 2;
                line.SetPosition(0, m_lineStart.position);
                line.SetPosition(1, m_lineEnd.position);
            }
        }

    }
}
