using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 击退BUFF
    /// 如果是作为释放技能的位移，这个时间不能超过技能时间
    /// 因为取下一次释放技能时的位移会出现方向错乱的问题
    /// </summary>
    public class Buff06 : BuffBase
    {
        private Vector2 m_recDir;
        private bool m_bBack;

        public Buff06(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();

            if (m_rec == null)
                return;
            m_recDir = m_rec.GetDir().ToVector2();
            // 击退表现层，不做转向效果
            if (m_rec.m_vCreature != null)
            {
                m_rec.m_vCreature.m_bRotate = false;
            }
        }

        public override void ExecuteFrame()
        {
            if (m_rec == null || m_destroy)
                return;


            int speed = GetVal1();
            int dirDelta = 1;
            if (speed > 0)             // 配置的方向
            {
                m_bBack = false;
                dirDelta = 1;
            }
            else
            {
                m_bBack = true;
                dirDelta = -1;
            }
            speed = Mathf.Abs(speed);   // 配置的速度

            Vector2 moveDir = m_recDir * dirDelta; // 最终角色方向
            if (m_rec != m_caster)   // 属于被打击退
            {
                moveDir = m_rec.GetPos().ToVector2() - m_skillPos;
                moveDir = (m_rec.GetPos() - m_caster.GetPos()).ToVector2();
            }
            // 逻辑层移动位置
            moveDir.Normalize();
            Vector2 nextPos = m_rec.GetPos().ToVector2() + moveDir * FSPParam.clientFrameScTime * speed;
            m_rec.SetPos(nextPos.ToVector2d());
            m_rec.SetDir(moveDir.ToVector2d());
            m_rec.SetSpeed(new FixedPoint(speed));

            // 通知表现层移动
            if (m_rec.m_vCreature != null)
            {
                m_rec.m_vCreature.SetMove(true);
                // 击退是否带高度          
                if (m_rec != m_caster)   // 属于被打击退
                {
                    if (m_buffData.listParam.Length == 2)
                    {
                        int hight = m_buffData.listParam[1];
                        m_rec.m_vCreature.GetEnt().AddHight(hight);
                    }
                }
            }
        }

        public override void Destroy()
        {
            if (m_rec == null)
                return;
            m_rec.UpdateMoveSpeed();
            // 后退才需要重置逻辑层方向，因为逻辑方向是向后，击退结束时，方向要重置
            if(m_bBack)
            {
                m_rec.SetDir(m_rec.GetDir() * -1);
            }

            if (m_rec.m_vCreature != null)
            {
                m_rec.m_vCreature.m_bRotate = true;
                m_rec.m_vCreature.SetMove(false);
            }

            base.Destroy();
            //m_rec.m_logicMoveEnabled = true;
        }

    }
}