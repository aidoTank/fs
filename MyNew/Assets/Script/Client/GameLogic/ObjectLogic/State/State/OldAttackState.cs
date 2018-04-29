//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//namespace Roma
//{
//    public class AttackState : FSMState
//    {
//        public AttackState(CCreature obj)
//            : base(obj)
//        {
//            m_stateId = StateID.AttackState;
//        }
//        public override void Enter()
//        {
//            m_attackTarget = m_creature.m_targetCreature;
//            if (m_bExited)
//            {
//                m_bExited = false;
//                UseSkill();
//            }
//        }

//        private bool UseSkill()
//        {
//            ClearTimeEvent();
//            float fTime = RealTime.time;
//            SkillCsv skillCsv = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill);
//            skillInfo = skillCsv.GetSkillInfo(m_creature.m_skillCastInfo.m_uSkillID);
//            if (null == skillInfo)
//            {
//                //Debug.LogError("找到不到技能：" + m_creature.m_skillCastInfo.m_uSkillID);
//            }

//            // 调整朝向
//            if (skillInfo.eTargetType != SkillTargetType.eSTT_Self && m_attackTarget != null)
//            {
//                Quaternion lq = MathEx.CalDirection(m_attackTarget.GetPos().x, m_attackTarget.GetPos().z, 
//                    m_creature.GetPos().x, m_creature.GetPos().z);
//                m_creature.SetQua(lq);
//            }

//            // 播放攻击动画
//            if (!string.IsNullOrEmpty(skillInfo.strCastAction))
//            {
//                TimeEvent atkAnima = new TimeEvent();
//                atkAnima.fun = _TimeEvent_AtkAanima;
//                atkAnima.fBeginTime = skillInfo.fCastActionBeginTime + fTime;
//                AddTimeEvent(atkAnima);
//            }

//            // 播放攻击特效
//            if (skillInfo.uCastEffect !=0)
//            {
//                TimeEvent castEffect = new TimeEvent();
//                castEffect.fun = _TimeEvent_AtkEffect;
//                castEffect.fBeginTime = skillInfo.fCastEffectBeginTime + fTime;
//                AddTimeEvent(castEffect);
//            }

//            // 击中处理
//            foreach (float item in skillInfo.lstHitTime)
//            {
//                TimeEvent hitEvent = new TimeEvent();
//                hitEvent.fun = _TimeEvent_HitTarget;
//                hitEvent.fBeginTime = skillInfo.fCastActionBeginTime + item + fTime;
//                AddTimeEvent(hitEvent);
//            }

//            // 结束状态
//            TimeEvent exitStateEvent = new TimeEvent();
//            exitStateEvent.fun = _TimeEvent_ExitState;
//            exitStateEvent.fBeginTime = skillInfo.fCastActionBeginTime + fTime + skillInfo.fCastActionDuration;
//            AddTimeEvent(exitStateEvent);
//            return true;
//        }

//        public override void Exit()
//        {
//        }

//        private void _TimeEvent_AtkAanima(float fTime, float fDelayTime)
//        {
//            AnimationAction atkAction = new AnimationAction();
//            atkAction.crossTime = 0;  // 融合是将此动画时间提前了，如果美术的动作包含了融合效果，这里填0
//            atkAction.endTime = skillInfo.fCastActionDuration; // 持续时间
//            atkAction.strFull = skillInfo.strCastAction;
//            atkAction.eMode = WrapMode.Once;
//            atkAction.endEvent = null; // 可注册攻击完成回调事件

//            // 动作的本身时间 / 配置时间 = 速度
//            float animaTime = m_creature.GetEntity().GetAnimaClipTime(skillInfo.strCastAction);
//            atkAction.playSpeed = animaTime / skillInfo.fCastActionDuration;
//            m_creature.Play(atkAction);
//        }

//        public void _TimeEvent_AtkEffect(float fTime, float fDelayTime)
//        {
//            // 攻击特效位置为发动者的位置
//            CEffectMgr.Create(skillInfo.uCastEffect, m_creature.GetPos() + new Vector3(0.0f, 1.0f, 0.0f), Vector3.zero, null);
//        }

//        private void _TimeEvent_HitTarget(float fTime, float fDelayTime)
//        {
//            // tips：受击时通过【SkillAttackType】判断1单体技能2范围技能3子弹技能等
//            // 如果是绑定目标
//            if (skillInfo.eTargetType == SkillTargetType.eSTT_Bind)
//            {
//                switch (skillInfo.eAttackType)
//                {
//                    // 单体
//                    case SkillAttackType.eST_BindAttack:
//                        if (m_creature.m_targetCreature == null) return;
//                        m_creature.m_targetCreature.PushCommand(StateID.StrikeState);
//                        CEffectMgr.Create(skillInfo.uBeAttackEffect, m_creature.m_targetCreature, "chest", false);
//                        CEffectMgr.RegistFightListenerList(EEE);
//                        break;
//                    // 子弹
//                    case SkillAttackType.eST_BulletAttack:
//                        EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
//                        CEffectTrackBullet bullet = new CEffectTrackBullet(
//                            m_creature.m_skillCastInfo.m_uSkillID,
//                            skillInfo.fFlySpeed,
//                            effectCsv.GetEffect(skillInfo.uFlyEffect),
//                            m_creature,
//                            m_creature.m_targetCreature,
//                            BulletHitEvent);
//                        uint handID = bullet.InitConfigure();
//                        CEffectMgr.Add(handID, bullet);
//                        break;
//                    // 区域
//                    case SkillAttackType.eST_WorldAttack:
//                        // 增加击中特效，此时只在目标对象位置播放受击特效
//                        CEffectMgr.Create(skillInfo.uBeAttackEffect, m_creature.m_targetCreature.GetPos(), Vector3.zero, null);
//                        CEffectMgr.RegistFightListenerList(EEE);
//                        for (int i = 0; i < m_creature.m_skillCastInfo.m_lstTarget.Count; i++)
//                        {
//                            CCreature cc = m_creature.m_skillCastInfo.m_lstTarget[i];
//                            if (cc != null)
//                            {
//                                cc.PushCommand(StateID.StrikeState);
//                            }
//                        }
//                        break;
//                }
//            }
//            else if (skillInfo.eTargetType == SkillTargetType.eSTT_Self)
//            {
//                switch (skillInfo.eAttackType)
//                {
//                    // 区域，攻击特效为魔法特效
//                    case SkillAttackType.eST_WorldAttack:
//                        for (int i = 0; i < m_creature.m_skillCastInfo.m_lstTarget.Count; i++)
//                        {
//                            CCreature cc = m_creature.m_skillCastInfo.m_lstTarget[i];
//                            if (cc != null)
//                            {
//                                cc.PushCommand(StateID.StrikeState);
//                                // 增加击中特效
//                                //CEffectMgr.Create(skillInfo.uBeAttackEffect, cc, "body", false);
//                                //CEffectMgr.RegistFightListenerList(EEE);
//                            }
//                        }
//                        break;
//                }
//            }
//        }

//        private void BulletHitEvent(uint skillID, EffectData data, CCreature caster, CCreature target, uint effectHandID)
//        {
//            if (m_creature.m_skillCastInfo.m_lstTarget.Count == 0)
//            {
//                if (m_creature.m_targetCreature == null) return;
//                m_creature.m_targetCreature.PushCommand(StateID.StrikeState);
//                CEffectMgr.Create(skillInfo.uBeAttackEffect, m_creature.m_targetCreature, "chest", false);
//                return;
//            }
//        }

//        private void _TimeEvent_ExitState(float fTime, float fDelayTime)
//        {
//            m_bExited = true;
//            m_creature.PushCommand(StateID.IdleState);
//        }

//        private void EEE()
//        {

//        }


//        private bool m_bExited = true;
//        private SkillInfo skillInfo = null; // 当前技能的配置属性
//        public CCreature m_attackTarget = null;
//    }
//}
