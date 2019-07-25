using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public partial class Condi_CheckState
    {
        private bool m_bDownUpSkill;
        private int m_downUpCurTime;

        /// <summary>
        /// 如果开启机枪技能，按下四秒再抬起
        /// </summary>
        //private bool Check_DownUpSkill()
        //{
        //    if (!m_creature.IsMaster())
        //        return false;

        //    SkillBase dSkill = CSkillMgr.GetDownUpSkill(m_creature.GetUid());
        //    // 如果2-3技能能用，则取消当前的机枪
        //    if (m_creature.GetSkillByIndex(1).IsCanUse() || m_creature.GetSkillByIndex(2).IsCanUse())
        //    {
        //        if(dSkill != null)
        //        {
        //            m_downUpCurTime = 0;
        //            m_bDownUpSkill = false;

        //            //((SkillDownUp)dSkill).OnUp();
        //            m_creature.DestoryDownUpSkill();
        //        }
        //        return false;
        //    }
              
        //    if (dSkill == null)
        //        return false;

        //    if (!m_bDownUpSkill)
        //    {
        //        m_bDownUpSkill = true;
        //    }
        //    if(m_bDownUpSkill)
        //    {
        //        // 更新攻击方向
        //        int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
        //        CCreature targetCC = CCreatureMgr.Get(targetUid);
        //        if (targetCC != null)
        //        {
        //            Vector2 dir = targetCC.GetPos() - m_creature.GetPos();
        //            ((SkillDownUp)dSkill).m_curSkillCmd.m_dir = dir;
        //            ((SkillDownUp)dSkill).OnDown();
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //// 小兵机枪
        //private bool Check_DownUpSkill_Partent()
        //{
        //    if (!m_creature.IsPartner())
        //        return false;

        //    SkillBase dSkill = CSkillMgr.GetDownUpSkill(m_creature.GetUid());
        //    if (dSkill == null)
        //        return false;

        //    if (!m_bDownUpSkill)
        //    {
        //        m_bDownUpSkill = true;
        //    }
        //    if (m_bDownUpSkill)
        //    {
        //        // 更新攻击方向
        //        int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
        //        CCreature targetCC = CCreatureMgr.Get(targetUid);
        //        if (targetCC != null)
        //        {
        //            Vector2 dir = targetCC.GetPos() - m_creature.GetPos();
        //            ((SkillDownUp)dSkill).m_curSkillCmd.m_dir = dir;
        //        }
        //        ((SkillDownUp)dSkill).OnDown();

        //        m_downUpCurTime += FSPParam.clientFrameMsTime;
        //        if (m_downUpCurTime >= 1 * 30 * FSPParam.clientFrameMsTime)
        //        {
        //            m_downUpCurTime = 0;
        //            m_bDownUpSkill = false;

        //            //((SkillDownUp)dSkill).OnUp();
        //            m_creature.DestoryDownUpSkill();
        //        }
        //        return true;
        //    }
        //    return false;
        //}


    }
}


