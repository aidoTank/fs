using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public class SkillSingleFly : SkillBase
    {

        private bool m_bFly;
        private Vector2 m_startPos;

        public SkillSingleFly(long id, int skillId, VSkillBase vSkill)
            : base(id, skillId, vSkill)
        {
       
        }


        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            if(m_bFly)
            {
                m_curPos = m_curPos + m_curSkillCmd.m_dir * FSPParam.clientFrameScTime * 8000 * 0.001f;
                m_vCreature.SetPos(m_curPos);
                m_vCreature.SetDir(m_curSkillCmd.m_dir);

                if(Vector2.Distance(m_startPos, m_curPos) >= m_skillInfo.distance)
                {
                    CmdFspHit cmd = new CmdFspHit();
                    cmd.bPlayer = false;
                    cmd.pos = m_curPos;
                    m_vCreature.PushCommand(cmd);
                    m_bFly = false;
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

            m_vCreature = new VCreature(20022);
            m_vCreature.Init();
            m_bFly = true;
        }

        public override void Destory()
        {

        }




    }
}