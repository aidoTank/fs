using UnityEngine;
using System.Collections.Generic;
using System;

namespace Roma
{
    public partial class SkillCurve: SkillBase
    {
        public SkillCurve(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {
      
        }

        // AOE和近战的区别是，AOE有区域特效
        public override void Launch()
        {
            base.Launch();

            // 设置表现层参数
            m_curSkillCmd.m_startPos = GetCaster().GetPos() + (m_curSkillCmd.m_endPos - GetCaster().GetPos()).normalized * 2;
            ((VSkillCurve)m_vSkill).OnStartCurve(0.5f);
            // 逻辑创建特效
            CmdSkillCreate cmd = new CmdSkillCreate();
            m_vSkill.PushCommand(cmd);

            // 到达目的地事件
            CFrameTimeMgr.Inst.RegisterEvent((int)(0.5 * 1000), ()=>
            {
                // 爆炸特效
                OnHit(m_curSkillCmd.m_endPos);

                // 动态检测
                foreach(KeyValuePair<long, CPlayer> item in CPlayerMgr.m_dicPlayer)
                {
                    CCreature creature = item.Value;
                    if(creature.GetUid() ==  GetCaster().GetUid())
                        continue;
                    Sphere flyS = new Sphere();
                    flyS.c = m_curSkillCmd.m_endPos;
                    flyS.r = m_skillInfo.length * 0.5f;

                    Sphere playerS = new Sphere();
                    playerS.c = creature.GetPos();
                    playerS.r = creature.GetR();

                    if(Collide.bSphereSphere(flyS, playerS))
                    {
                        Debug.Log("检测:" + creature.GetUid());
                        OnHit(creature);
                    }
                }

                Destory();
            });
        }
    }
}