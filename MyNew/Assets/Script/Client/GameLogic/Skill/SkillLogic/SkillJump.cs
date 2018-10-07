using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillJump : SkillBase
    {
        private Vector2 m_startPos;
        private bool m_bJump;
        public SkillJump(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {
      
        }


        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_bJump)
            {
                Vector2 curPos = GetCaster().GetPos();
                     
                curPos += m_curSkillCmd.m_dir * FSPParam.clientFrameScTime * 20000 * 0.001f;
                GetCaster().SetPos(curPos);
                
                GetCaster().m_vCreature.SetPos(curPos.ToVector3());
                GetCaster().m_vCreature.SetDir(m_curSkillCmd.m_dir.ToVector3());
                GetCaster().m_vCreature.m_bMoveing = true;

                if(Vector2.Distance(m_startPos, curPos) >= m_skillInfo.distance)
                {
                    //OnHit(m_curPos);
                    Debug.Log("跳跃接受。。。。。。。");
                    GetCaster().m_vCreature.PushCommand(new CmdFspStopMove());
                    m_bJump = false;
                }
            }
        }

        public override void Launch()
        {
            m_startPos = GetCaster().GetPos();

            // 逻辑创建特效
            CmdSkillCreate cmd = new CmdSkillCreate();
            m_vSkill.PushCommand(cmd);
            m_bJump = true;
        }

       
        public override void Destory()
        {

        }


    }
}