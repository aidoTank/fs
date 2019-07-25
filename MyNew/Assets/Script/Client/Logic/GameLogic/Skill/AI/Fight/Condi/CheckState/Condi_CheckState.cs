using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 检测自身状态
    /// </summary>
    public partial class Condi_CheckState : AICondi
    {

        public override bool Check()
        {
            // 主角机枪可用时
            //if (Check_DownUpSkill())
            //{
            //    return false;
            //}
            //// 小兵机枪
            //if (Check_DownUpSkill_Partent())
            //{
            //    return false;
            //}
            //// 多段技能
            //if (Check_MultiStepSkill())
            //{
            //    return false;
            //}
            //// 小兵多段
            //if (Check_MultiStepSkill_Partner())
            //{
            //    return false;
            //}
            //// 自己使用了站立可旋转的技能时，转向目标
            //if (Check_RotaSkill())
            //{
            //    return false;
            //}

            // CD间隔
            int skillInterval = m_dataBase.GetData<int>((int)eAIParam.INT_SKILL_INTERVAL);
            if (skillInterval > 0)
            {
                return false;
            }
            return true;
        }

        //private bool Check_RotaSkill()
        //{
        //    // 可旋转技能的状态
        //    if(m_creature.m_logicSkillRotationEnabled)
        //    {
        //        int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
        //        CCreature targetCC = CCreatureMgr.Get(targetUid);
        //        if(targetCC != null)
        //        {
        //            Vector2 dir = targetCC.GetPos() - m_creature.GetPos();
        //            CmdFspRotation cmd = new CmdFspRotation();
        //            cmd.m_rotation = dir.ToVector3();
        //            m_creature.PushCommand(cmd);
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        //private bool Check_MultiStepSkill()
        //{
        //    if (!m_creature.IsMaster())
        //        return false;

        //    if (m_creature.GetSkillByIndex(1).IsCanUse() || m_creature.GetSkillByIndex(2).IsCanUse())
        //    {
        //        return false;
        //    }

        //    // 如果当前技能是多段技能，并且还没结束，继续多段技能，多段攻击结束时m_cmdFspSendSkill会清空
        //    CmdFspSendSkill cmd = m_creature.m_cmdFspSendSkill;
        //    if (cmd == null)
        //        return false;
        //    CSkillInfo skillInfo = m_creature.GetSkillByIndex(cmd.m_skillIndex);
        //    CSkillInfo mainInfo = skillInfo.GetMainSkill();
        //    if (mainInfo.IsMultiStep())
        //    {
        //        if (mainInfo.GetCurSubSkillIndex() < mainInfo.GetSubSkillNum())
        //        {
        //            m_dataBase.SetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX, skillInfo.m_skillIndex);
        //            AIParam.SendSkill(m_creature, m_dataBase, false);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private bool Check_MultiStepSkill_Partner()
        //{
        //    if (!m_creature.IsPartner())
        //        return false;

        //    // 如果当前技能是多段技能，并且还没结束，继续多段技能，多段攻击结束时m_cmdFspSendSkill会清空
        //    CmdFspSendSkill cmd = m_creature.m_cmdFspSendSkill;
        //    if (cmd == null)
        //        return false;
        //    CSkillInfo skillInfo = m_creature.GetSkillByIndex(cmd.m_skillIndex);
        //    CSkillInfo mainInfo = skillInfo.GetMainSkill();
        //    if (mainInfo.IsMultiStep())
        //    {
        //        if (mainInfo.GetCurSubSkillIndex() < mainInfo.GetSubSkillNum())
        //        {
        //            m_dataBase.SetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX, skillInfo.m_skillIndex);
        //            AIParam.SendSkill(m_creature, m_dataBase, false);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

    }
}


