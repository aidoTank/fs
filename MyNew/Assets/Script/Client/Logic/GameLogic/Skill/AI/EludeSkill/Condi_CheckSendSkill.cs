
using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    /// <summary>
    /// 检查附近的玩家施法指令
    /// 躲避位置，垂直于施法方向
    /// </summary>
    public class Condi_CheckSendSkill_ : AICondi
    {
        public long m_lookDis = 10;
        private List<CCreature> m_foeSkillList;


        private int m_newbieAIMoveTime;

        public override void Activate(BtDatabase database)
        {
            base.Activate(database);
        }

        public override bool Check()
        {
            //m_lookDis = FixedMath.Create(AIParam.GetCheckSkillDis(m_player.m_ai.m_level));
            //// 飞起状态，不躲弹道
            //if (m_creature.CheckState((int)DState.State_Fly))
            //{
            //    return false;
            //}

            //if (CheckBatBuff())
            //{
            //    return true;
            //}

            if (CheckFlySkill())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查自身BUFF，根据类型确定目标点，中了蝙蝠BUFF应该不能动
        /// </summary>
        private bool CheckBatBuff()
        {
            // 如果中了蝙蝠的2技能，并且不在熔岩，就原地不动站撸
            //if (m_player.ImpactHaveImpactOfSpecificImpactId(DCombat.BatLeechBuffId) &&
            //    !CMapMgr.Get().GetAutohurtMapAreaMgr().CheckIsHurtPos(m_player.GetPos())
            //    )
            //{
            //    m_dataBase.SetData<Vector2d>(AIParam.V3_MOVE_TO_POS, m_creature.GetPos());
            //    // 选择附近单位
            //    if (AIParam.GetAtkTarget(m_player, m_dataBase, 20))
            //    {
            //        // 选择可用技能
            //        if (AIParam.RandomSelectSkill(m_player, m_dataBase))
            //        {
            //            // 如果此时是钩到人了，则不再施法
            //            if (m_player.IsHookBuff())
            //                return true;
            //            // 发动站立攻击
            //            AIParam.SendSkill(m_player, m_dataBase, false);
            //        }
            //    }
            //    return true;
            //}
            return false;
        }

        private bool CheckFlySkill()
        {
            // 判断最近弹道, 判断是否是敌人释放的弹道
            int minDisCreatureUID = 0;
            //if (m_foeSkillList == null)
            //    m_foeSkillList = new List<CCreature>();
            // 技能对象，并没有区分陷阱和飞行弹道，陷阱用更短的距离检测即可
           // CSkillObjectMgr.GetEnemySkillObj(m_creature, ref m_foeSkillList);



            float minDis2 = 999999;
            //int count = m_foeSkillList.Count;
            //foreach(KeyValuePair<long, SkillBase> skill in CSkillMgr.m_dicSkill)
            //{
            //    SkillBase item = skill.Value;
                //if (item.IsSkillSpecial() || m_creature == item.GetOwner())
                //    continue;

                // 新入AI，不是飞行物不处理，也就是ai会自动走向飞行物
                //if (m_creature.m_ai.m_level == eAILevel.NEWBIE && !item.IsFlySkill())
                //{
                //    continue;
                //}

                //float abDis2 = Collide.GetDis2(m_creature.GetPos() ,item.GetPos());
                //if (abDis2 < m_lookDis * m_lookDis)
                //{
                //    if (minDis2 > abDis2)
                //    {
                //        minDis2 = abDis2;
                //        minDisCreatureUID = (int)item.GetUid();
                //        //Debug.Log("检测到最近技能对象：" + minDisCreatureUID + "   skillid:" + item.m_skillId);
                //    }
                //}
            //}

            //// 获取躲避技能的位置
            //if (minDisCreatureUID != 0)
            //{
            //    SkillBase obj = CSkillMgr.Get(minDisCreatureUID);
                //// 新入AI，走向飞行物
                //if (m_creature.m_ai.m_level == eAILevel.NEWBIE)
                //{
                //    int x = 0;
                //    if (m_creature.m_aiType == eAIType.Client)
                //    {
                //        x = GameManager.Instance.GetRand(-1, 1, 514);
                //    }
                //    else
                //    {
                //        x = GameManager.Instance.m_clientRand.Next(-1, 1);
                //    }
                //    m_dataBase.SetData<Vector2d>(AIParam.V3_MOVE_TO_POS, obj.GetPos() + new Vector2d(x, x));
                //    return true;
                //}


                // 获取垂直于弹道的方向
                //Vector2 dir = Vector3.Cross(new Vector3(0, 1, 0), obj.GetDir().ToVector3()).ToVector2();
                //// 当前方向是否面朝弹道
                //Vector2 offset = obj.GetPos() - m_creature.GetPos();
                //float idot = Vector2.Dot(dir, offset);
                ////int idot = FixedMath.ToInt(dot);
                //if (idot >= 0)   // 面对弹道
                //{
                //    dir = dir * -1;
                //}
                //dir.Normalize();
                //Vector2 pos = m_creature.GetPos() + dir * 6;
                // 如果当前是伤害区域|或者不能走的区域，那么就去安全位置
                //bool bHurt = CMapMgr.Get().GetAutohurtMapAreaMgr().CheckIsHurtPos(pos);
                //if (bHurt || !CMapMgr.Get().IsCanGo(ref pos))
                //{
                //    pos = CMapMgr.Get().GetAutohurtMapAreaMgr().GetSaftePos(m_player);
                //}
                //m_dataBase.SetData<Vector2>((int)eAIParam.V3_MOVE_TO_POS, pos);
               // return true;
            //}
            return false;
        }
    }
}
