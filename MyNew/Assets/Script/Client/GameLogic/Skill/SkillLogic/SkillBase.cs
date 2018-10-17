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
                    // 同步施法者方向
                    GetCaster().SetDir(m_curSkillCmd.m_dir);
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

        public CCreature GetCaster()
        {
            return CPlayerMgr.Get(m_curSkillCmd.m_casterUid);
        }

        /// <summary>
        /// 弹道起飞,近战，AOE受击
        /// </summary>
        public virtual void Launch()
        {

        }

        public void OnHit(CCreature creature)
        {
            if(creature.IsDie())
            {
                return;
            }
            int val = (m_skillInfo.ad + m_skillInfo.pd);
            creature.AddPropNum(eCreatureProp.CurHp, - val);
            creature.UpdateHeadHp();
            OnHitHUD(GetCaster(), creature, - (int)(val * 0.001f));

            // 没挂才播放受击动作
            if(creature.GetPropNum(eCreatureProp.CurHp) > 0)
            {
                CmdSkillHit cmd = new CmdSkillHit();
                cmd.bPlayer = true;
                cmd.uid = (int)creature.GetUid();
                m_vSkill.PushCommand(cmd);
            }
        }

        public void OnHit(Vector2 pos)
        {
            CmdSkillHit cmd = new CmdSkillHit();
            cmd.bPlayer = false;
            cmd.pos = new Vector3(pos.x, 1, pos.y);
            m_vSkill.PushCommand(cmd);
        }

   

        public void OnHitHUD(CCreature caster, CCreature target, int hitVal)
        {
            eHUDType type = eHUDType.NONE;

            if(hitVal < 0)
            {
                type = eHUDType.FIGHT_HARM;
            }
            else
            {
                type = eHUDType.FIGHT_ADDBLOOD;
            }

            CmdUIHead cmd = new CmdUIHead();
            cmd.type = 4;
            cmd.hudType = type;
            cmd.hudText = hitVal.ToString();
            target.m_vCreature.PushCommand(cmd);
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