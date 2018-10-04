using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillNear : SkillBase
    {
   
        public SkillNear(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {
      
        }


        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
        }

        public override void Launch()
        {
            Debug.Log("近战击中，计算伤害");
            // 检测，播放受击动作
            Vector2 pos = CPlayerMgr.Get(m_curSkillCmd.m_casterUid).GetPos();
            Vector2 dir = m_curSkillCmd.m_dir;
            
            foreach(KeyValuePair<long, CPlayer> item in CPlayerMgr.m_dicPlayer)
            {
                Vector2 focusPos = item.Value.GetPos();
                if(Collide.bSectorInside(pos, dir, m_skillInfo.width, m_skillInfo.length, focusPos))
                {
                    Debug.Log("检测:" + item.Value.GetUid());
                    CmdSkillHit cmd = new CmdSkillHit();
                    cmd.bPlayer = true;
                    cmd.uid = (int)item.Value.GetUid();
                    m_vSkill.PushCommand(cmd);
                }
            }
        }

        public override void Destory()
        {

        }


    }
}