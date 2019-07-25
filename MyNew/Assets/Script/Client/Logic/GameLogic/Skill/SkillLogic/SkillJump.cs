using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillJump : SkillBase
    {
        private Vector2 m_startPos;
        private bool m_bJump;
        //private bool m_canArriveEnd;

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
                Vector2 nextPos = GetCaster().GetPos().ToVector2() + moveDir * FSPParam.clientFrameScTime * m_skillInfo.flySpeed;

                if (PhysicsManager.Inst.Isblock((int)nextPos.x, (int)nextPos.y))
                {
                    nextPos = GetCaster().GetPos().ToVector2();
                    return;
                }

                GetCaster().SetPos(nextPos.ToVector2d());
                GetCaster().SetDir(moveDir.ToVector2d());
                GetCaster().SetSpeed(new FixedPoint(m_skillInfo.flySpeed));
                VBase vo = GetCaster().m_vCreature;
                if (vo != null)
                    vo.m_bMoveing = true;
            }
        }

        public override void Launch()
        {
            OnCasterAddBuff(GetCaster());

            m_startPos = GetCaster().GetPos().ToVector2();
 
            // 逻辑创建特效
            //CmdSkillCreate cmd = new CmdSkillCreate();
            //m_vSkill.PushCommand(cmd);
            m_bJump = true;
        }

        private void OnEndJump()
        {
            Destory();
        }

        public override void Destory()
        {
            m_bJump = false;
            CCreature cc = GetCaster();
            if(cc != null)
            {
                GetCaster().UpdateMoveSpeed();
            }

            VObject vo = cc.GetVObject();
            if(vo != null)
            {
                vo.PushCommand(new CmdFspStopMove());
                vo.m_bMoveing = false;
            }
   
            base.Destory();
        }
    }
}