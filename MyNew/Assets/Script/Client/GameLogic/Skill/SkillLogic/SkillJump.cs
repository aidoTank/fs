using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillJump : SkillBase
    {
        private Vector2 m_startPos;
        private bool m_bJump;
        private bool m_canJump;

        public SkillJump(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {

        }


        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_bJump)
            {
                m_tempPos = GetCaster().GetPos();
                Vector2 moveDir = m_curSkillCmd.m_dir.normalized;
                m_tempPos += moveDir * FSPParam.clientFrameScTime * 20000 * 0.001f;
                
                if(!m_canJump)
                {
                    if(!CMapMgr.m_map.bCanMove((int)m_tempPos.x, (int)m_tempPos.y))
                    {
                        m_tempPos = GetCaster().GetPos();
                        OnEndJump();
                    }
                    // if(!CMapMgr.m_map.CanMove(m_tempPos, GetCaster().GetR()))
                    // {
                    //     m_tempPos = GetCaster().GetPos();
                    //     OnEndJump();
                    // }
                }
  
                if(Vector2.Distance(m_startPos, m_tempPos) >= m_skillInfo.distance)
                {
                    OnEndJump();
                    return;
                }

                GetCaster().SetPos(m_tempPos);
                GetCaster().m_vCreature.SetPos(m_tempPos.ToVector3());
                GetCaster().m_vCreature.SetDir(moveDir.ToVector3());
                GetCaster().m_vCreature.SetSpeed(20000 * 0.001f);
                GetCaster().m_vCreature.m_bMoveing = true;
            }
        }

        public override void Launch()
        {
            m_startPos = GetCaster().GetPos();
            // 如果终点能跳就直接跳
            Vector2 end = m_startPos + m_curSkillCmd.m_dir.normalized * m_skillInfo.distance;
            m_canJump = CMapMgr.m_map.bCanMove((int)end.x, (int)end.y);
            //m_canJump = CMapMgr.m_map.CanMove(end, GetCaster().GetR());
 
            // 逻辑创建特效
            CmdSkillCreate cmd = new CmdSkillCreate();
            m_vSkill.PushCommand(cmd);
            m_bJump = true;
        }

        private void OnEndJump()
        {
            GetCaster().m_vCreature.PushCommand(new CmdFspStopMove());
            GetCaster().m_vCreature.m_bMoveing = false;
            m_bJump = false;
        }

    }
}