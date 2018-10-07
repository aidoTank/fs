
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature
    {
        private SkillCsvData m_skillInfo;
        private int m_vSkillHid;
        private int m_skillUid;

        public void EnterSkill()
        {
            int skillId = m_cmdFspSendSkill.m_skillId;
            m_skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(skillId);

            // 如果是多弹道子弹，也是在这里创建多个，每个元素都是一个技能对象
            switch(m_skillInfo.skillType)
            {
                case (int)eSkillType.Near:
                    VSkillNear vSkill = (VSkillNear)VObjectMgr.Create(eVOjectType.SkllNear);
                    vSkill.PushCommand(m_cmdFspSendSkill);

                    SkillNear sNear = (SkillNear)CSkillMgr.Create(eCSkillType.SkllNear, vSkill);
                    sNear.PushCommand(m_cmdFspSendSkill);
                break;
                case (int)eSkillType.Fly:
                    // 创建飞行技能
                    VSkillSingleFly sing = (VSkillSingleFly)VObjectMgr.Create(eVOjectType.SkillSingleFly);
                    sing.PushCommand(m_cmdFspSendSkill);

                    SkillSingleFly singFly = (SkillSingleFly)CSkillMgr.Create(eCSkillType.SkillSingleFly, sing);
                    singFly.PushCommand(m_cmdFspSendSkill);
                break;
                case (int)eSkillType.Aoe:
                    // 创建AOE技能
                    // 创建飞行技能
                    VSkillNear vAoe = (VSkillNear)VObjectMgr.Create(eVOjectType.SkllNear);
                    vAoe.PushCommand(m_cmdFspSendSkill);

                    SkillAoe aoe = (SkillAoe)CSkillMgr.Create(eCSkillType.SkillAoe, vAoe);
                    aoe.PushCommand(m_cmdFspSendSkill);
                break;
                case (int)eSkillType.Jump:
                    VSkillBase vjump = (VSkillBase)VObjectMgr.Create(eVOjectType.SkillBase);
                    vjump.PushCommand(m_cmdFspSendSkill);

                    SkillJump jump = (SkillJump)CSkillMgr.Create(eCSkillType.SkillJump, vjump);
                    jump.PushCommand(m_cmdFspSendSkill);
                break;
            }  
        }
    }
}