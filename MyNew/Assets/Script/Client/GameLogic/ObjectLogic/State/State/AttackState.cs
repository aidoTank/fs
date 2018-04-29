using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class AttackState : FSMState
    {
        public AttackState(CCreature obj)
            : base(obj)
        {
            m_stateId = StateID.AttackState;
        }
        public override void Enter()
        {
            m_attackTarget = m_creature.m_targetCreature;
            if (m_bExited)
            {
                m_bExited = false;
                UseSkill();
            }
        }

        private bool UseSkill()
        {
            //ClearTimeEvent();
            //float fTime = RealTime.time;
            //GetPetSkillCsv skillCsv = CsvManager.Inst.GetCsv<GetPetSkillCsv>((int)eAllCSV.eAC_GetPetSkill);
            //skillInfo = skillCsv.GetData(m_creature.m_skillCastInfo.m_uSkillID);
            //if (null == skillInfo)
            //{
            //    //Debug.LogError("找到不到技能：" + m_creature.m_skillCastInfo.m_uSkillID);                                                                                                                                                                                                    
            //}

            //// 调整朝向
            //if (skillInfo.targetTpye != (int)SkillTargetType.eSTT_Self && m_attackTarget != null)
            //{
            //    Quaternion lq = MathEx.CalDirection(m_attackTarget.GetPos().x, m_attackTarget.GetPos().z, 
            //        m_creature.GetPos().x, m_creature.GetPos().z);
            //    m_creature.SetQua(lq);
            //}

            //// 1.攻击动作
            //if (!string.IsNullOrEmpty(skillInfo.castAction))
            //{
            //    TimeEvent atkAnima = new TimeEvent();
            //    atkAnima.fun = _TimeEvent_AtkAanima;
            //    atkAnima.fBeginTime = skillInfo.castActionBeginTime + fTime;
            //    AddTimeEvent(atkAnima);
            //}

            //// 2.攻击特效
            //if (skillInfo.castEffect != 0)
            //{
            //    TimeEvent castEffect = new TimeEvent();
            //    castEffect.fun = _TimeEvent_AtkEffect;
            //    castEffect.fBeginTime = skillInfo.castActionBeginTime + fTime + skillInfo.castEffectBeginTime;
            //    AddTimeEvent(castEffect);
            //}

            //// 3.受击时间。支持多受击事件
            //// 这里是近战，受击时间才有意义；远程和区域技能都由其他方式检测受击
            //TimeEvent hitEvent = new TimeEvent();
            //hitEvent.fun = _TimeEvent_HitTarget;
            //hitEvent.fBeginTime = skillInfo.castActionBeginTime + fTime + skillInfo.hitActionBeginTime;
            //AddTimeEvent(hitEvent);

            //// 4.结束状态
            //TimeEvent exitStateEvent = new TimeEvent();
            //exitStateEvent.fun = _TimeEvent_ExitState;
            //exitStateEvent.fBeginTime = skillInfo.castEffectBeginTime + fTime + skillInfo.castActionDuration;
            //AddTimeEvent(exitStateEvent);
            return true;
        }

        private void _TimeEvent_AtkAanima(float fTime, float fDelayTime)
        {
            //AnimationAction atkAction = new AnimationAction();
            //atkAction.crossTime = 0;  // 融合是将此动画时间提前了，如果美术的动作包含了融合效果，这里填0
            //atkAction.endTime = skillInfo.castActionDuration; // 持续时间
            //atkAction.strFull = skillInfo.castAction;
            //atkAction.eMode = WrapMode.Once;
            //atkAction.endEvent = null; // 可注册攻击完成回调事件

            //if (skillInfo.castActionDuration == 0)
            //{
            //    atkAction.playSpeed = 1;
            //}
            //else
            //{
            //    // 动作的本身时间 / 配置时间 = 速度
            //    float animaTime = m_creature.GetEntity().GetAnimaClipTime(skillInfo.castAction);
            //    atkAction.playSpeed = animaTime / skillInfo.castActionDuration;
            //}
            //m_creature.Play(atkAction);
        }

        public void _TimeEvent_AtkEffect(float fTime, float fDelayTime)
        {
           // CEffectMgr.Create(skillInfo.castEffect, m_creature.GetPos() + new Vector3(0.0f, 1.0f, 0.0f), Vector3.zero, null);
        }

        private void _TimeEvent_HitTarget(float fTime, float fDelayTime)
        {
            // tips：受击时通过【SkillAttackType】判断1单体技能2范围技能3子弹技能等
            // 如果是绑定目标
            //if (skillInfo.targetTpye == (int)SkillTargetType.eSTT_Bind)
            //{
            //    switch (skillInfo.attackType)
            //    {
            //        // 单体
            //        case (int)SkillAttackType.eST_BindAttack:
            //            if (m_creature.m_targetCreature == null) return;
            //            HitEvent(m_creature.m_skillCastInfo.m_uSkillID, m_creature.m_targetCreature, skillInfo.hitEffect, skillInfo.hitEffectPoint);
            //            break;
            //        // 子弹
            //        case (int)SkillAttackType.eST_BulletAttack:
            //            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            //            CEffectBullet bullet = new CEffectBullet(
            //                m_creature.m_skillCastInfo.m_uSkillID,
            //                skillInfo.flySpeed,
            //                effectCsv.GetEffect(skillInfo.flyEffect),
            //                m_creature,
            //                m_creature.m_targetCreature,
            //                skillInfo.hitEffectPoint,
            //                BulletHitEvent );
            //            uint handID = bullet.InitConfigure();
            //            CEffectMgr.Add(handID, bullet);
            //            break;
            //        // 区域
            //        case (int)SkillAttackType.eST_WorldAttack:
            //            // 增加击中特效，此时只在目标对象位置播放受击特效
            //            CEffectMgr.Create(skillInfo.hitEffect, m_creature.m_targetCreature.GetPos(), Vector3.zero, null);
            //            for (int i = 0; i < m_creature.m_skillCastInfo.m_lstTarget.Count; i++)
            //            {
            //                CCreature cc = m_creature.m_skillCastInfo.m_lstTarget[i];
            //                if (cc != null)
            //                {
            //                    cc.PushCommand(StateID.StrikeState);
            //                }
            //            }
            //            break;
            //    }
            //}
            //else if (skillInfo.targetTpye == (int)SkillTargetType.eSTT_Self)
            //{
            //    switch (skillInfo.attackType)
            //    {
            //        // 区域，攻击特效为魔法特效
            //        case (int)SkillAttackType.eST_WorldAttack:
            //            for (int i = 0; i < m_creature.m_skillCastInfo.m_lstTarget.Count; i++)
            //            {
            //                CCreature cc = m_creature.m_skillCastInfo.m_lstTarget[i];
            //                if (cc != null)
            //                {
            //                    cc.PushCommand(StateID.StrikeState);
            //                    // 增加击中特效
            //                    //CEffectMgr.Create(skillInfo.uBeAttackEffect, cc, "body", false);
            //                    //CEffectMgr.RegistFightListenerList(EEE);
            //                }
            //            }
            //            break;
            //        // 炸弹类型,目标为鼠标位置
            //        case (int)SkillAttackType.eST_Bomb:
            //            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            //            CEffectBomb bullet = new CEffectBomb(
            //                m_creature.m_skillCastInfo.m_uSkillID,
            //                skillInfo.flySpeed,
            //                effectCsv.GetEffect(skillInfo.flyEffect),
            //                m_creature,
            //                CPointMgr.GetMousePos(),
            //                BombHitEvent);
            //            uint handID = bullet.InitConfigure();
            //            CEffectMgr.Add(handID, bullet);
            //            break;
            //        // 陷阱类型
            //        case (int)SkillAttackType.eST_Trap:
            //            EffectCsv eCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            //            CEffectTrap trap = new CEffectTrap(
            //                m_creature.m_skillCastInfo.m_uSkillID,
            //                skillInfo.attackRange,
            //                eCsv.GetEffect(skillInfo.flyEffect),
            //                m_creature.GetPos(),
            //                BombHitEvent);
            //            uint trapHandID = trap.InitConfigure();
            //            CEffectMgr.Add(trapHandID, trap);
            //            break;
            //    }
            //}
        }

        private void BulletHitEvent(uint skillID, EffectData data, CCreature caster, CCreature target, uint effectHandID, string bindPoint)
        {
            if (m_creature.m_skillCastInfo.m_lstTarget.Count == 0)
            {
                if (m_creature.m_targetCreature == null) 
                    return;
                //HitEvent(skillID, m_creature.m_targetCreature, skillInfo.hitEffect, bindPoint);
                return;
            }
        }

        private void BombHitEvent(uint skillID, EffectData data, List<CCreature> targetList, uint effectHandID)
        {
            for (int i = 0; i < targetList.Count; i++ )
            {
                if (targetList[i] == null)
                {
                    continue;
                }
               // HitEvent(skillID, targetList[i], skillInfo.hitEffect, "chest");
                return;
            }
        }

        private void HitEvent(uint skillID, CCreature cc, uint effectId, string bindPoint)
        {
            if (cc == null || cc.GetState() == StateID.DeadState)
                return;
            //CEffectMgr.Create(skillInfo.hitEffect, m_creature.m_targetCreature, bindPoint, false);
            if (cc.IsMaster())
            {
               // cc.m_stunStateParam.m_stunTime = 1.0f;
                cc.PushCommand(StateID.StunState);
                return;
            }

            //GetPetSkillCsv skillCsv = CsvManager.Inst.GetCsv<GetPetSkillCsv>((int)eAllCSV.eAC_GetPetSkill);
            //GetPetSkillCsvData hitSkillCsv = skillCsv.GetData(skillID);
            //int curHp = cc.GetHp() > 0 ? cc.GetHp() - hitSkillCsv.damagetValue : 0;
            //cc.SetHp(curHp);

            // 测试状态溶解
            cc.PushCommand(StateID.DeadState);
            return;
           // if (curHp <= 0)
            //{
               // cc.PushAICommand(AIStateID.None);
                // 屏蔽掉，抓宠功能已经取消 Qiao 2017.2.4
                //if(GetPetModule.GetPetState())
                //{
                //    cc.PushCommand(StateID.DeadState);
                //}
                //else  
                //{
                //    // 测试状态
                //    //cc.PushCommand(StateID.BtHitFlyState);
                //    //cc.PushCommand(StateID.DeadState);
                //}
           // }
            //else
           // {
                // 播放晕眩动作
            //    if (hitSkillCsv.stunTime != 0)
            //    {
            //      //  cc.m_stunStateParam.m_stunTime = hitSkillCsv.stunTime;
            //        cc.PushCommand(StateID.StunState);
            //    }
            //    else
            //    {
            //        //cc.PushCommand(StateID.StrikeState);
            //    }
            //}
            // 弹出伤害值
           // if (hitSkillCsv.damagetValue != 0)
           // {
                //ComHUDMoudule hud = LayoutMgr.Inst.GetLogicModule<ComHUDMoudule>(LayoutName.S_ComHUD);
                //hud.CreateDamage(eHUDType.FIGHT_HARM, hitSkillCsv.damagetValue.ToString(), cc, eHudShowType.Queue);

                //cc.m_thingHead.SetBattleHUD(eHUDType.FIGHT_HEAD_TEXT, hitSkillCsv.damagetValue.ToString());
            //}
        }

        private void _TimeEvent_ExitState(float fTime, float fDelayTime)
        {
            m_bExited = true;
            m_creature.PushCommand(StateID.IdleState);
            //if(m_creature.m_skillCastInfo.m_attackEnd != null)
            //{
            //    m_creature.m_skillCastInfo.m_attackEnd(m_creature);
            //    m_creature.m_skillCastInfo.m_attackEnd = null;
            //}
        }

        public override void Exit()
        {
        }


        private bool m_bExited = true;
        //private GetPetSkillCsvData skillInfo = null; // 当前技能的配置属性
        public CCreature m_attackTarget = null;
    }
}
