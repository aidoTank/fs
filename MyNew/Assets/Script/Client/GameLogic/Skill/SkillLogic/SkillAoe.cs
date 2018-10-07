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
                Vector3 pos = new Vector3(m_curSkillCmd.m_endPos.x, 0, m_curSkillCmd.m_endPos.y);
                m_vSkill.SetPos(pos);
                m_vSkill.m_bMoveing = true;

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

            // 检测播放受击动作
            Vector2 pos = CPlayerMgr.Get(m_curSkillCmd.m_casterUid).GetPos();
            Vector2 dir = m_curSkillCmd.m_dir;
            
            foreach(KeyValuePair<long, CPlayer> item in CPlayerMgr.m_dicPlayer)
            {
                CCreature creature = item.Value;
                if(creature.GetUid() == GetCaster().GetUid())
                    continue;

                Sphere aoeS = new Sphere();
                aoeS.c = m_curSkillCmd.m_endPos;
                aoeS.r = m_skillInfo.length * 0.5f;

                Sphere playerS = new Sphere();
                playerS.c = creature.GetPos();
                playerS.r = creature.GetR();

                if(Collide.bSphereSphere(aoeS, playerS))
                {
                    Debug.Log("检测:" + creature.GetUid());
                    OnHit(creature);
                }
            }
        }

    
        public override void Destory()
        {

        }


    }
}