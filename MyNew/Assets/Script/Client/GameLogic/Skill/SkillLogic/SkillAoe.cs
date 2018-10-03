using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillAoe : SkillBase
    {
        public bool m_aoeHit;
        public SkillAoe(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {
      
        }


        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_aoeHit)
            {
                m_vSkill.SetPos(m_curSkillCmd.m_endPos);

                AoeHit();
                m_aoeHit = false;
            }
        }

        // AOE和近战的区别是，AOE有区域特效
        public override void Launch()
        {
            // 逻辑创建特效
            CmdSkillCreate cmd = new CmdSkillCreate();
            m_vSkill.PushCommand(cmd);
            m_aoeHit = true;
        }

        private void AoeHit()
        {
            Debug.Log("aoe伤害计算");
        }

    

        public override void Destory()
        {

        }


    }
}