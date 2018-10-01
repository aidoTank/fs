using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillNear : SkillBase
    {
        private int m_curTime;
        private bool m_bHit;

        public SkillNear(long id, int skillId)
            : base(id, skillId)
        {
           
        }

        public override bool InitConfigure()
        {
            // 创建当前技能的表现层
            return true;
        }

        public override void ExecuteFrame(int frameId)
        {
            if(m_bHit)
                return;

            m_curTime += FSPParam.clientFrameMsTime;
            if(m_curTime > m_skillInfo.hitTime - m_skillInfo.launchTime)
            {
                m_bHit = true;
                Debug.Log("近战击中，计算伤害");
            }
        }

        public override void Destory()
        {

        }




    }
}