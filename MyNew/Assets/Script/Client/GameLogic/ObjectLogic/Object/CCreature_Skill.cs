
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

        public void EnterSkill()
        {
            int skillId = m_cmdFspSendSkill.m_skillId;
            m_skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(skillId);

            // 如果是多弹道子弹，也是在这里创建多个，每个元素都是一个技能对象
            switch(m_skillInfo.skillType)
            {
                case (int)eSkillType.Near:
                
                    VSkillNear vSkill = new VSkillNear(1, m_skillInfo.id);
                    vSkill.PushCommand(m_cmdFspSendSkill);

                    SkillNear sNear = new SkillNear(1, m_skillInfo.id, vSkill);
                    CSkillMgr.Add(1, sNear);
                    sNear.PushCommand(m_cmdFspSendSkill);
                break;
                case (int)eSkillType.Fly:
                    // 创建飞行技能
                    VSkillSingleFly sing = new VSkillSingleFly(1, m_skillInfo.id);
                    sing.PushCommand(m_cmdFspSendSkill);

                    SkillSingleFly singFly = new SkillSingleFly(1, m_skillInfo.id, sing);
                    CSkillMgr.Add(1, singFly);
                    singFly.PushCommand(m_cmdFspSendSkill);
                break;
                case (int)eSkillType.Aoe:
                    // 创建AOE技能
                break;
            }  
        }
    }
}