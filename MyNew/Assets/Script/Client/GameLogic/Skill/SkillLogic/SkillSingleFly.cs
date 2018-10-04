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
            if(m_vSkill != null && m_bFly)
            {
                m_curPos = m_curPos + m_curSkillCmd.m_dir * FSPParam.clientFrameScTime * m_skillInfo.flySpeed * 0.001f;
                Vector3 pos = new Vector3(m_curPos.x, 1, m_curPos.y);
                m_vSkill.SetPos(pos);
                m_vSkill.SetDir(m_curSkillCmd.m_dir);

                if(Vector2.Distance(m_startPos, m_curPos) >= m_skillInfo.distance)
                {
                    CmdSkillHit cmd = new CmdSkillHit();
                    cmd.bPlayer = false;
                    cmd.pos = new Vector3(m_curPos.x, 1, m_curPos.y);
                    m_vSkill.PushCommand(cmd);
                    m_bFly = false;
                    Destory();
                    return;
                }
                // 检测碰撞
                foreach(KeyValuePair<long, CPlayer> item in CPlayerMgr.m_dicPlayer)
                {
                    Vector2 focusPos = item.Value.GetPos();
                    if(Collide.bSphereInside(m_curPos, m_skillInfo.length * 0.5f, focusPos))
                    {
                        Debug.Log("检测:" + item.Value.GetUid());
                        CmdSkillHit cmd = new CmdSkillHit();
                        cmd.bPlayer = true;
                        cmd.uid = (int)item.Value.GetUid();
                        m_vSkill.PushCommand(cmd);

                        Destory();
                        return;
                    }
                }
            }
        }

        public override void Launch()
        {
            Debug.Log("发射技能");
            m_curPos = CPlayerMgr.Get(m_curSkillCmd.m_casterUid).GetPos() + m_curSkillCmd.m_dir.normalized;
            m_startPos = m_curPos;

            CmdSkillCreate cmd = new CmdSkillCreate();
            m_vSkill.PushCommand(cmd);

            m_bFly = true;
        }




    }
}