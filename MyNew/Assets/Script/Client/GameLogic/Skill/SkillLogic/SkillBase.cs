using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 技能的逻辑层
    /// </summary>
    public partial class SkillBase : CCreature
    {
        public CmdFspSendSkill m_curSkillCmd;
        public SkillCsvData m_skillInfo;
        
        private bool m_bLaunch;
        private int m_curLaunchTime;

        public VSkillBase m_vSkill;

        public SkillBase(long id, VSkillBase vSkill)
            : base(id)
        {
            m_type = EThingType.Skill;
            m_vSkill = vSkill;
        }

        public override void PushCommand(IFspCmdType cmd)
        {
            switch (cmd.GetCmdType())
            {
                case CmdFspEnum.eFspSendSkill:
                    m_curSkillCmd = cmd as CmdFspSendSkill;
                    m_skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(m_curSkillCmd.m_skillId);
                    m_bLaunch = true;
                    break;
            }
        }        

        public override void ExecuteFrame(int frameId)
        {
            if(m_bLaunch)
            {
                m_curLaunchTime += FSPParam.clientFrameMsTime;
                if(m_curLaunchTime > m_skillInfo.launchTime)
                {
                    m_bLaunch = false;
                    Launch();
                }
            }
        }

        public virtual void Launch()
        {

        }

        public virtual void Hit()
        {

        }

        public override void Destory()
        {
            if(m_vSkill != null)
            {
                m_vSkill.Destory();
                m_vSkill = null;
            }
 
        }

    }
}