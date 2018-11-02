using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillJump : SkillBase
    {
        private Vector2 m_startPos;
        private bool m_bJump;
        private bool m_canArriveEnd;

        public SkillJump(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {

        }


        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_bJump)
            {
                Vector2 moveDir = m_curSkillCmd.m_dir.normalized;
                Vector2 nextPos = GetCaster().GetPos() + moveDir * FSPParam.clientFrameScTime * 20000 * 0.001f;
                
                if(!m_canArriveEnd)
                {
                    if(!CMapMgr.m_map.CanMove(nextPos, GetCaster().GetR()))
                    {
                        nextPos = GetCaster().GetPos();
                        OnEndJump();
                        return;
                    }
                }
  
                if(Vector2.Distance(m_startPos, nextPos) >= m_skillInfo.distance)
                {
                    OnEndJump();
                    return;
                }

                GetCaster().SetPos(nextPos);
                GetCaster().m_vCreature.SetPos(nextPos.ToVector3());
                GetCaster().m_vCreature.SetDir(moveDir.ToVector3());
                GetCaster().m_vCreature.SetSpeed(20000 * 0.001f);
                GetCaster().m_vCreature.m_bMoveing = true;
            }
        }

        public override void Launch()
        {
            // 起跳时，逻辑不可用
            SetLogicEnable(false);

            m_startPos = GetCaster().GetPos();
            // 如果终点能跳就直接跳
            Vector2 end = m_startPos + m_curSkillCmd.m_dir.normalized * m_skillInfo.distance;
            m_canArriveEnd = CMapMgr.m_map.CanMove(end, GetCaster().GetR());
 
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

            // 跳跃结束，逻辑可用
            SetLogicEnable(true);
        }
    }
}