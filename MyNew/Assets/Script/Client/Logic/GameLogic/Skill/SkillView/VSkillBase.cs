using UnityEngine;
using System.Collections.Generic;
using System;

namespace Roma
{
    /// <summary>
    /// 当前技能的表现层，弹道等
    /// </summary>
    public partial class VSkillBase // : VObject
    {
        public VObject m_casterObject;
        public SkillCsvData m_skillInfo;

        public CmdFspSendSkill m_curSkillCmd;
        // 本地CSV数据
        public SkillStepCsvData m_casterData;

        private AnimationCsvData m_curAnima;

        public int m_curCasterEffectHid;

        public virtual void PushCommand(IFspCmdType cmd)
        {
            switch (cmd.GetCmdType())
            {
                case CmdFspEnum.eFspSendSkill:
                    m_curSkillCmd = cmd as CmdFspSendSkill;
                    Init();
                break;
            }
        }

        public void Init()
        {
            CCreature player = CCreatureMgr.Get(m_curSkillCmd.m_casterUid);
            if (m_curSkillCmd.m_skillId == 0)
            {
                m_skillInfo = player.GetSkillByIndex(m_curSkillCmd.m_skillIndex).m_skillInfo;
            }
            else
            {
                // 组合技能
                SkillCsv skillCsv = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill);
                m_skillInfo = skillCsv.GetData(m_curSkillCmd.m_skillId);
            }

            int skillId = m_skillInfo.id;

            SkillStepCsv step = CsvManager.Inst.GetCsv<SkillStepCsv>((int)eAllCSV.eAC_SkillStep);
            m_casterData = step.GetCasterData(skillId);


            //Debug.Log("播放施法动作" + m_casterData.animaName);
            if (player.m_vCreature == null)
                return;
               
            if(m_casterData == null)
            {
                Debug.LogError("技能子表 施法配置为空：" + skillId);
                return;
            }

            m_casterObject = player.GetVObject();
            m_casterObject.StartRotate(m_curSkillCmd.m_dir.ToVector3());

            Start();
        }

        /// <summary>
        /// 常规技能的开始阶段
        /// 1.也包含了循环动作的技能，如旋风斩
        /// 2.支持背后武器火炮的攻击时，挂点的改变
        /// </summary>
        public virtual void Start()
        {
            BattleEntity ent = m_casterObject.GetEnt() as BattleEntity;
            int animaId = m_casterData.animaName;
            AnimationCsv anima = CsvManager.Inst.GetCsv<AnimationCsv>((int)eAllCSV.eAC_Animation);
            m_curAnima = anima.GetData(animaId);
            if (m_curAnima != null)
            {
                CmdSkillAnimaPriority cmd = new CmdSkillAnimaPriority();
                m_casterObject.PushCommand(cmd);
                if (m_curAnima.mode == (int)WrapMode.Loop)
                {
                    ent.PlayAnima(animaId, null);
                }
                else
                {
                    if (m_curAnima.switchHand)  // 如果是火炮，特殊处理一下，施法时武器切换到手上
                    {
                        ent.BackEquipStart();
                    }
                    ent.PlayAnima(animaId, () =>
                    {
                        ent.BackEquipEnd();
                    });
                }
            }

            int effectId = m_casterData.effectId;
            if (m_casterData.startTime == 0)
            {
                m_curCasterEffectHid = CEffectMgr.Create(effectId, ent, m_casterData.bindPoint);
            }
            else
            {
                TimeMgr.Inst.RegisterEvent(m_casterData.startTime * 0.001f, () =>
                {
                    m_curCasterEffectHid = CEffectMgr.Create(effectId, ent, m_casterData.bindPoint);
                });
            }

            if (m_casterObject == null || 
                m_curSkillCmd == null || 
                m_casterObject.m_cmdUpdateEquip == null)
                return;

            // 播放语音,1手雷 2火炮
            //if (m_curSkillCmd.m_skillIndex == 0)
            //{
            //    if (m_casterObject.m_cmdUpdateEquip.m_armsType == (int)eArmsType.Near)
            //    {
            //        m_casterObject.PlaySpeak(eRoleSpeakCsv.skill1Near);
            //    }
            //    else
            //    {
            //        m_casterObject.PlaySpeak(eRoleSpeakCsv.skill1Far);
            //    }
            //}
            //else if(m_curSkillCmd.m_skillIndex == 1)
            //{
            //    m_casterObject.PlaySpeak(eRoleSpeakCsv.skill2);
            //}
            //else if(m_curSkillCmd.m_skillIndex == 2)
            //{
            //    m_casterObject.PlaySpeak(eRoleSpeakCsv.skill3);
            //}
        }

        public virtual void Destory()
        {
            // 还原动作
            if (m_curSkillCmd == null)
                return;

            CCreature player = CCreatureMgr.Get(m_curSkillCmd.m_casterUid);
            if (player != null && player.m_vCreature != null)
            {
                if (m_curAnima != null && m_curAnima.switchHand)  // 如果是火炮，特殊处理一下，施法时武器切换到手上
                {
                    ((BattleEntity)player.m_vCreature.GetEnt()).BackEquipEnd();
                }
            }
            if(m_curCasterEffectHid != 0)
            {
                CEffectMgr.Destroy(m_curCasterEffectHid);
            }
        }
    }
}