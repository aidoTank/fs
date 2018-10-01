using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 技能的逻辑层
    /// </summary>
    public partial class SkillBase : CCreature
    {
        public int m_skillId;
        public SkillCsvData m_skillInfo;
        public SkillBase(long id, int skillId)
            : base(id)
        {
            m_type = EThingType.Skill;

            m_skillId = skillId;
            m_skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(skillId);
        }

        public override bool InitConfigure()
        {
            return true;
        }

        public override void ExecuteFrame(int frameId)
        {

        }

        public override void Destory()
        {

        }




    }
}