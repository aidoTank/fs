using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public class SkillSingleFly : SkillBase
    {

        private bool m_bFly;
        private Vector2 m_startPos;

        public SkillSingleFly(long id, VSkillBase vSkill)
            : base(id, vSkill)
        {
        
        }

        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_bFly)
            {
                m_curPos = m_curPos + m_curSkillCmd.m_dir * FSPParam.clientFrameScTime * m_skillInfo.flySpeed * 0.001f;
                m_vSkill.SetPos(m_curPos);
                m_vSkill.SetDir(m_curSkillCmd.m_dir);

                if(Vector2.Distance(m_startPos, m_curPos) >= m_skillInfo.distance)
                {
                    CmdSkillHit cmd = new CmdSkillHit();
                    cmd.bPlayer = false;
                    cmd.pos = m_curPos;
                    m_vSkill.PushCommand(cmd);
                    m_bFly = false;
                    Destory();
                    return;
                }
                // 检测碰撞
            }
        }

        public override void Launch()
        {
            Debug.Log("发射技能");
            m_curPos = CPlayerMgr.Get(m_curSkillCmd.m_casterUid).GetPos();
            m_startPos = m_curPos;

            CmdSkillCreate cmd = new CmdSkillCreate();
            m_vSkill.PushCommand(cmd);

            m_bFly = true;
        }




    }
}