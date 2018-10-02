using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 技能的逻辑层
    /// </summary>
    public partial class VSkillBase : VCreature
    {
        public CmdFspSendSkill m_curSkillCmd;
        // 本地CSV数据
        public SkillStepCsvData m_casterData;
        public SkillStepCsvData m_hitData;
        public List<SkillStepCsvData> m_flyData = new List<SkillStepCsvData>();
        public SkillStepCsvData m_flyHitData;

        public VSkillBase(int resId, int skillId)
            : base(resId)
        {

        }

        public override void PushCommand(IFspCmdType cmd)
        {
            switch (cmd.GetCmdType())
            {
                case CmdFspEnum.eFspSendSkill:
                    m_curSkillCmd = cmd as CmdFspSendSkill;
                    Start();
                    break;
                case CmdFspEnum.eHit:
                    CmdFspHit hit = cmd as CmdFspHit;
                    if(hit.bPlayer)   // 玩家身上
                    {
                        if (m_hitData == null)
                            return;
                        // 播放动作
                        // BattleEntity ent = EntityManager.Inst.GetEnity(hit.hid) as BattleEntity;
                        // int animaId = 0;
                        // int.TryParse(m_hitData.animId, out animaId);
                        // ent.PlayAnima(animaId);
                        // // 播放特效
                        // int effectId = m_hitData.effectId;
                        // string bindPoint = m_hitData.bindPoint;
                        // CEffectMgr.Create(effectId, hit.hid, bindPoint);
                    }
                    else // 子弹自爆
                    {
                        if (m_flyHitData == null)
                            return;
                        int effectId = m_flyHitData.effectId;
                        CEffectMgr.Create(effectId, hit.pos + Vector3.up, Vector3.zero);
                    }
                break;
            }
        }

        public void Start()
        {
            int skillId = m_curSkillCmd.m_skillId;
            SkillStepCsv step = CsvManager.Inst.GetCsv<SkillStepCsv>((int)eAllCSV.eAC_SkillStep);
            m_casterData = step.GetCasterData(skillId);
            m_hitData = step.GetHitData(skillId);
            step.GetFlyData(ref m_flyData, skillId);
            m_flyHitData = step.GetFlyHitData(skillId);

            CPlayer player =  CPlayerMgr.Get(m_curSkillCmd.m_casterUid);
            Debug.Log("播放施法动作" + m_casterData.animaName);
            AnimationAction anim = new AnimationAction();
            anim.strFull = m_casterData.animaName;
            anim.eMode = WrapMode.Once;
            player.m_vCreature.GetEnt().Play(anim);
            // 获取施法动作
   
            // 播放施法特效
            Debug.Log("播放施法特效" + m_casterData.effectId);
            CEffectMgr.Create(m_casterData.effectId, player.m_vCreature.GetEnt().GetPos(), Vector3.zero);
        }

        

       




    }
}