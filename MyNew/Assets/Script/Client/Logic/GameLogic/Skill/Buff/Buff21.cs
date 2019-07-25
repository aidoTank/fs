using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// BUFF触发-技能
    /// </summary>
    public class Buff21 : BuffBase
    {
        private int m_curIntervalTime;

        private int m_delayEventHid;

        public Buff21(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();
            CreateSkill();
        }

        public override void ExecuteFrame()
        {
        }

        /// <summary>
        /// 支持创建多个不同的触发器
        /// </summary>
        private void CreateSkill()
        {
            CFrameTimeMgr.Inst.RegisterEvent(m_buffData.IntervalTime, () => {

                m_caster.m_cmdFspSendSkill.m_skillId = m_buffData.ParamValue1;

                SkillCsv skillCsv = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill);
                SkillCsvData m_skillInfo = skillCsv.GetData(m_buffData.ParamValue1);
                m_caster.StartSkill(m_skillInfo);
            });
        }

        public override void Destroy()
        {
            base.Destroy();
        }

    }
}