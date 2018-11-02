using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 当前技能的表现层，可以检测碰撞的弹道
    /// </summary>
    public partial class VSkillBase : VObject
    {
        public CmdFspSendSkill m_curSkillCmd;
        // 本地CSV数据
        public SkillStepCsvData m_casterData;
        public SkillStepCsvData m_hitData;
        public List<SkillStepCsvData> m_flyData = new List<SkillStepCsvData>();
        public SkillStepCsvData m_flyHitData;

        public override void PushCommand(IFspCmdType cmd)
        {
            switch (cmd.GetCmdType())
            {
                case CmdFspEnum.eFspSendSkill:
                    m_curSkillCmd = cmd as CmdFspSendSkill;
                    Start();
                break;
                case CmdFspEnum.eSkillCreate:
                    sVOjectBaseInfo info = new sVOjectBaseInfo();
                    if(m_flyData == null || m_flyData.Count == 0)
                        return;
                    info.m_resId = m_flyData[0].effectId;
                    Create(info);
                    m_state = CmdFspEnum.eFspMove;  // 技能都是移动状态
                break;
                case CmdFspEnum.eSkillHit:
                    CmdSkillHit hit = cmd as CmdSkillHit;
                    if(hit.bPlayer)   // 玩家身上
                    {
                        if (m_hitData == null)
                            return;
                        // 播放动作
                        CPlayer player =  CPlayerMgr.Get(hit.uid);
            
                        AnimationAction anim = new AnimationAction();
                        anim.crossTime = AnimationInfo.m_crossTime;
                        anim.strFull = m_hitData.animaName;
                        anim.eMode = WrapMode.Once;
                        anim.endEvent = (a)=> {
                            CmdFspStopMove stop = new CmdFspStopMove();
                            player.m_vCreature.PushCommand(stop);
                        };
                        player.m_vCreature.GetEnt().Play(anim);

                        // 播放特效
                        int effectId = m_hitData.effectId;
                        string bindPoint = m_hitData.bindPoint;
                        CEffectMgr.Create(effectId, player.m_vCreature.GetEnt().m_hid, bindPoint);
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
            //Debug.Log("播放施法动作" + m_casterData.animaName);

            BoneEntity ent = player.m_vCreature.GetEnt();
        

            ent.SetRot(Quaternion.LookRotation(m_curSkillCmd.m_dir.ToVector3()));

            // 获取施法动作
            AnimationAction anim = new AnimationAction();
            anim.crossTime = AnimationInfo.m_crossTime;
            anim.strFull = m_casterData.animaName;
            anim.eMode = WrapMode.Once;
            anim.endEvent = (a)=> {
                Debug.Log("player.m_vCreature.m_bMoveing:" + player.m_vCreature.m_bMoveing);
                if(player.m_vCreature.m_bMoveing)
                    return;
                CmdFspStopMove stop = new CmdFspStopMove();
                player.m_vCreature.PushCommand(stop);
            };
            ent.Play(anim);

            TimeMgr.Inst.RegisterEvent(m_casterData.startTime * 0.001f, ()=>
            {
                // 播放施法特效
                //Debug.Log("播放施法特效" + m_casterData.effectId);
                CEffectMgr.Create(m_casterData.effectId, ent.GetPos(),ent.GetRotate());
            });
        }
 
    }
}