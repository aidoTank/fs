using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillNear : SkillBase
    {
        private int m_curHitTime;
        private bool m_bHit;

        public SkillNear(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {
           m_bHit = true;
        }

        public override bool InitConfigure()
        {
            // 创建当前技能的表现层
            return true;
        }

        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_bHit)
            {
                m_curHitTime += FSPParam.clientFrameMsTime;
                if(m_curHitTime > m_skillInfo.hitTime)
                {
                    m_bHit = false;
                    Hit();
                }
            }
        }

        public override void Hit()
        {
            Debug.Log("近战击中，计算伤害");
        }

        public override void Destory()
        {

        }


    }
}