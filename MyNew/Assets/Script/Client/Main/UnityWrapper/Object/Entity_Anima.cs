using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
   
    public partial class Entity
    {
        public void SetGhostShadow(bool bShow)
        {
            if (m_object == null)
                return;

            GhostShadow gs = GhostShadow.Get(m_object);
            gs.SetShow(bShow);
        }

        #region 模拟抛物线(模拟重力)
        /// <summary>
        /// 模拟重力效果，只控制Y轴
        /// </summary>
        public float power;
        public float Gravity = -200;
        private Vector3 moveSpeed;
        private float m_curHight;
        private float m_cutTime;
        private bool m_up;

        /// <summary>
        /// 模拟重力的击飞高度
        /// </summary>
        public void AddHight(int val)
        {
            power = val;
            moveSpeed = Vector3.up * power;
            m_curHight = GetPos().y;
            m_cutTime = 0;
            m_up = true;
        }

        public void _UpdateHight(float fdTime)
        {
            if (!m_up)
                return;
            float upY = Gravity * (m_cutTime += fdTime);
            m_curHight += (moveSpeed.y + upY) * fdTime;
            if (m_curHight >= 5)
            {
                m_curHight = 5;
            }
            m_transform.localPosition = new Vector3(GetPos().x, m_curHight, GetPos().z);
            if(m_curHight <= 0)
            {
                m_up = false;
            }
        }
        #endregion

        #region 抛物线
        /// <summary>
        /// 设置抛物线（贝塞尔曲线）
        /// </summary>
        public void SetCurve(Vector3 end, float time, float hight)
        {
            if (m_object != null)
            {
                TweenCurve cur = TweenCurve.Get(m_object);
                cur.duration = time * 0.001f;
                cur.from = GetPos();
                cur.front = new Vector3(0, hight, 0);
                cur.back = new Vector3(0, hight, 0);
                cur.to = end;
                cur.method = UITweener.Method.Linear;
                cur.Reset();
                cur.Play(true);

                TweenFloat rota = TweenFloat.Get(m_object);
                rota.duration = time * 0.001f;
                rota.from = 0;
                rota.to = -360;
                rota.method = UITweener.Method.EaseOut;
                rota.Reset();
                rota.Play(true);
                rota.FloatUpdateEvent = (val) => {
                    SetDirection(val * Vector3.one);
                };
            }
        }
        #endregion


        #region 跟随目标ent
        public Entity m_targetEnt;
        public Action m_fllowFinish;
        public void SetFllowEnt(Entity ent, Action finish)
        {
            m_targetEnt = ent;
            m_fllowFinish = finish;
        }

        public void _UpdateFllow(float fdTime)
        {
            if (m_targetEnt == null)
                return;

            Vector3 dir = m_targetEnt.GetPos() - GetPos();
            SetPos(GetPos() + dir.normalized * 2);
            if(Vector3.Dot(dir, dir) < 16)
            {
                m_targetEnt = null;
                if (m_fllowFinish != null)
                {
                    m_fllowFinish();
                    m_fllowFinish = null;
                }
                 
            }
        }

        #endregion

    }
}
