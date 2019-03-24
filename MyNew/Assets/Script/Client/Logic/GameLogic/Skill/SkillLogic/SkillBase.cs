using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 技能的创建由管理器创建
    /// 销毁由自身死亡状态时，自己调用管理器销毁
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
                    // 注册最长生命周期
                    CFrameTimeMgr.Inst.RegisterEvent(m_skillInfo.lifeTime, ()=>{
                        Destory();
                    });
                    // 释放技能，逻辑不可用
                    SetLogicEnable(false);
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
            // 一般的技能，在释放完成时，逻辑就可以，但是跳跃除外
            SetLogicEnable(true);
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

        /// <summary>
        /// 技能的动作施法时，逻辑是不可用的
        /// </summary>
        public void SetLogicEnable(bool bTrue)
        {
            GetCaster().SetLogicEnabled(bTrue);
            if(true)
            {
                if(GetCaster() is CMasterPlayer)
                {
                    (GetCaster() as CMasterPlayer).ResetJoyStick(true);
                }
            }
        }

        public override void Destory()
        {
            m_destroy = true;
            // 销毁表现层
            if(m_vSkill != null)
            {
                m_vSkill.Destory();
            }
        }
    }
}