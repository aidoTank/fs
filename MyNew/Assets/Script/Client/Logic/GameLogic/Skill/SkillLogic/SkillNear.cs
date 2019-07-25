using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 自己内部检测受击单位
    /// </summary>
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
            //base.Launch();
            //Debug.Log("近战击中，计算伤害");
            // 检测，播放受击动作
            Vector2 pos = CCreatureMgr.Get(m_curSkillCmd.m_casterUid).GetPos().ToVector2();
            Vector2 dir = m_curSkillCmd.m_dir;

            List<long> list = CCreatureMgr.GetCreatureList();
            for (int i = 0; i < list.Count; i++)
            {
                CCreature creature = CCreatureMgr.Get(list[i]);
                if (GetCaster().bCamp(creature) || creature.IsDie())
                    continue;

                Vector2 focusPos = creature.GetPos().ToVector2();
                if(Collide.bSectorInside(pos, dir, m_skillInfo.width, m_skillInfo.length, focusPos))
                {
                    //Debug.Log("近战检测:" + item.GetUid());
                    //OnHit(creature, i++);
                    OnCasterAddBuff(GetCaster(), creature);
                }
            }
        }

    }
}