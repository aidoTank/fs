
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature
    {
        /// <summary>
        /// 技能的施法接口，可以给AI使用
        /// </summary>
       public void UseSkill(int skillId, Vector2 dir, Vector2 pos, bool bDown)
       {
            SkillCsvData skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(skillId);
            
            switch(skillInfo.selectTargetType)
            {

            }
         
       }
    }


}