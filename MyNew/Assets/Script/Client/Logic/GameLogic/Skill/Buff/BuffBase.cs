using UnityEngine;
using System.Collections.Generic;
using System;

namespace Roma
{
    /// <summary>
    /// 技能的创建由管理器创建
    /// 销毁由自身死亡状态时，自己调用管理器销毁
    /// </summary>
    public class BuffBase
    {
        public bool m_dieDestroy = true;  // 当前持有BUFF者死亡时清除
        public bool m_destroy;
        public int m_uid;
        public CCreature m_caster;
        public CCreature m_rec;    // 纯位置的BUFF，是没有接受者的，比如触发器BUFF
        public int m_triUid;

        public Vector2 m_startPos;
        public int m_skillIndex;     // 当前BUFF的技能id，一般情况是有的
        /// <summary>
        /// 1.用于AOE触发器的位置
        /// 2.用于拉扯性BUFF的结束位置
        /// </summary>
        public Vector2 m_skillPos;
        public Vector2 m_skillDir;
        public object m_extendParam;  // 扩展参数

        public SkillBuffCsvData m_buffData;

        public int m_lifeEventHid;

        public BuffBase(int uid, SkillBuffCsvData data)
        {
            m_uid = uid;
            m_buffData = data;
        }

        public virtual eBuffType GetBuffType()
        {
            return (eBuffType)m_buffData.logicId;
        }

        public bool IsCont()
        {
            return m_buffData.IsCont;
        }

        /// <summary>
        /// 是状态BUFF
        /// </summary>
        public virtual bool IsStateBuff()
        {
            return false;
        }

        public int GetVal1()
        {
            return m_buffData.ParamValue1;
        }

        public virtual void Init()
        {
            UpdateVO_ShowBuffEffect(m_rec, true, m_buffData.effectId, m_buffData.effectPoint, m_uid);
            if(m_buffData.ContinuanceTime != 0)
            {
                m_lifeEventHid = CFrameTimeMgr.Inst.RegisterEvent(m_buffData.ContinuanceTime, () =>
                {
                    Destroy();
                });
            }
        }

        public void ResetTime()
        {
            CFrameEvent ev = CFrameTimeMgr.Inst.GetEvent(m_lifeEventHid);
            if (ev != null)
                ev.curTime = 0;
        }


        public virtual void ExecuteFrame()
        {

        }

        /// <summary>
        ///  1.BUFF的销毁，统一通过m_destroy控制，在管理类心跳中销毁
        ///  2.玩家身上的BUFF信息，也由BUFF类自己调用移除
        /// </summary>
        public virtual void Destroy()
        {
            //Debug.Log("销毁BUFF：" + m_buffData.logicId + "   uid:" + m_uid);

            m_destroy = true;

            // BUFF自己销毁时，如果玩家绑定了，就要移除
            if(m_caster != null)
            {
                m_caster.RemoveBuff(this);
            }
            if(m_rec != null)
            {
                m_rec.RemoveBuff(this);
            }
            CFrameTimeMgr.Inst.RemoveEvent(m_lifeEventHid);

            UpdateVO_ShowBuffEffect(m_rec, false, m_buffData.effectId, m_buffData.effectPoint, m_uid);
        }

        /// <summary>
        /// 实际战斗时取的数值
        /// 技能为0和4时，取武器攻击
        /// 1时，取手雷
        /// 2时，取火炮
        /// </summary>
        public int GetPlayerAp()
        {
            PlayerCsvData pCsv = (PlayerCsvData)m_caster.m_csvData;
            int lv = m_caster.GetPropNum(eCreatureProp.Lv);
            int maxAp = 0;
            //int maxAp = m_caster.GetPlayerPropVal(pCsv.BaseAp, pCsv.ApGrow, lv);
            //if (m_skillIndex == -1 || m_skillIndex == 0 || m_skillIndex == 4)
            //    maxAp += m_caster.GetEquipAp(eEquipType.HandRight);
            //else if (m_skillIndex == 1)
            //    maxAp += m_caster.GetEquipAp(eEquipType.Grenade);
            //else if (m_skillIndex == 2)
            //    maxAp += m_caster.GetEquipAp(eEquipType.Back);
            maxAp += m_caster.GetBuffAp(maxAp);
            return maxAp;
        }

        /// <summary>
        /// 1.技能伤害数值，是通过普攻*val%计算得出的
        /// 1.如果施法者是载具，其实还是用的主人的数值
        /// 返回1：需要播放受击动作
        /// </summary>
        public int OnHurt(int skillVal)
        {
            if (m_rec == null)
                return 0;

            if (m_rec.CheckState(eBuffState.God))
            {
                return 0;
            }

            // 设置主角保护
            if (m_rec.IsMaster() && m_rec.bMasterProtect())
            {
                return 1;
            }
            if (m_rec.IsMaster())
            {
                m_rec.SetMasterProtect();
            }

            // 施法者是载具时，数值结算用主人的
            //if (m_caster.GetMaster() != null)
            //{
            //    m_caster = m_caster.GetMaster();
            //}

            // 触发施法者的攻击命中BUFF，触发受击者的受击BUFF，在伤害之前
            m_caster.OnCreateAtkBuff(m_skillIndex, m_rec);
            m_rec.OnCreateHitBuff(m_caster);

            bool bCrit = false;

            //伤害 = 攻击 * 攻击 /（攻击 + k * 防御）*（0.9 / 1.1）
            int k = 1;
            int ap = m_caster.GetPropNum(eCreatureProp.Ap);
            int dp = m_rec.GetPropNum(eCreatureProp.Dp);
            // 玩家普攻伤害
            int hurtVal = (int)Math.Ceiling((float)ap * ap / (ap + k * dp));
            // 技能伤害
            hurtVal = (int)Math.Ceiling(hurtVal * (1 + skillVal * 0.01));
            // 计算暴击
            float CritChance = m_caster.GetPropNum(eCreatureProp.CritChance);
            if (GameManager.Inst.GetRand(0, 100) <= CritChance * 0.1f)
            {
                bCrit = true;
                float CritDamage = m_caster.GetPropNum(eCreatureProp.CritDamage);
                hurtVal = (int)Math.Ceiling((float)hurtVal * (1 + CritDamage * 0.001f));
            }
            // 连击累加
            int comboNum = m_caster.GetComboNum();
            if (comboNum > 300)
                comboNum = 300;
            hurtVal = (int)(hurtVal * (comboNum * 0.01f + 1.0f));

            int curHp = m_rec.GetPropNum(eCreatureProp.CurHp);
            int nextHp = curHp - hurtVal;
            if (nextHp <= 0)
                nextHp = 0;
            m_rec.SetPropNum(eCreatureProp.CurHp, nextHp);

          

            // 充能
            UpdateChargeSkill(m_caster, hurtVal);
            // 解除受击者睡眠BUFF
            UpdateBuff(m_rec);

            // 主角相关
            // 经验计算
            GetExp(m_rec);
            // 主角连击
            if (m_caster != null && m_caster.IsMaster())
            {
                m_caster.OnComboAdd();
            }
            UpdateUI_ShowHpHUD(m_rec, -hurtVal, bCrit);

            return 1;
        }

        /// <summary>
        /// 更新充能技能
        /// </summary>
        private void UpdateChargeSkill(CCreature m_caster, int hurtVal)
        {
            if (m_caster == null || m_caster.m_cmdFspSendSkill == null)
                return;

            CSkillInfo cInfo = m_caster.GetSkillByIndex(m_caster.m_cmdFspSendSkill.m_skillIndex);
            if (cInfo == null)
                return;

            // 如果施法者，当前是放的蓄能大招，则不进行充能
            if (cInfo.GetCDType() == (int)eSkillCdType.Charge)
                return;

            foreach (KeyValuePair<int, CSkillInfo> item in m_caster.m_dicSkill)
            {
                CSkillInfo info = item.Value;
                if (info.GetCDType() == (int)eSkillCdType.Charge)
                {
                    info.AddCharge(hurtVal);
                    // 更新界面
                    m_caster.UpdateUI_Charge(info.GetSkillIndex(), info.GetChargePct());
                }
            }
        }

        private void UpdateBuff(CCreature rec)
        {
            BuffBase buff = rec.GetStateBuff(eBuffState.sleep);
            if (buff != null)
            {
                CBuffMgr.Destroy(buff.m_uid);
            }
        }

        private void GetExp(CCreature m_rec)
        {
            if (m_rec.IsMonster() && m_rec.IsDie())
            {

            }
        }
        
      
        public void UpdateUI_ShowExpHud(int exp)
        {
            //HudModule hud = (HudModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelHud);
            //hud.SetVisible(true);
            //hud.SetHUD(eHUDType.FIGHT_EXP,"EXP" + "+" + exp.ToString(), m_caster);
        }

        public void UpdateUI_ShowHpHUD(CCreature target, int hitVal, bool bCrit = false)
        {
            eHUDType type = eHUDType.NONE;

            if (hitVal < 0)
            {
                if(bCrit)
                {
                    type = eHUDType.FIGHT_CRIT;
                }
                else
                {
                    if(target.IsMaster())
                    {
                        type = eHUDType.FIGHT_SELFHARM;
                    }
                    else
                    {
                        type = eHUDType.FIGHT_HARM;
                    }
                }
            }
            else
            {
                type = eHUDType.FIGHT_ADDBLOOD;
            }

            HudModule hud = (HudModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelHud);
            hud.SetVisible(true);
            hud.SetHUD(type, hitVal.ToString(), target);

            return;
            if (target.m_vCreature == null)
                return;
            CmdUIHead cmd = new CmdUIHead();
            cmd.type = 4;
            cmd.hudType = type;
            cmd.hudText = hitVal.ToString();
            target.m_vCreature.PushCommand(cmd);
        }

        public void UpdateVO_ShowBuffEffect(CCreature cc, bool bAdd, int effectId, int effectPoint, int uid)
        {
            if (cc == null)
                return;

            //if(bAdd) // 添加时，没有同种BUFF则创建特效，有则不创建
            //{
            //    BuffBase buffBase = cc.GetBuffByCsvId(m_buffData.id);
            //    if (buffBase != null)
            //        return;
            //}
            //else // 移除时，没有同种BUFF则销毁特效，有同种BUFF则不销毁
            //{
            //    BuffBase buffBase = cc.GetBuffByCsvId(m_buffData.id);
            //    if (buffBase != null)
            //        return;
            //}

            if (cc.m_vCreature == null)
            {
                return;
            }

            CmdFspBuff buff = new CmdFspBuff();
            buff.bAdd = bAdd;
            buff.effectId = effectId;
            buff.bindType = effectPoint;
            buff.color = m_buffData.hitColor;
            cc.m_vCreature.PushCommand(buff);
        }


        // 创建玩家身上的特效，非技能情况，比如升级，吃F爆点等等，不需要逻辑控制生命周期的
        // bindPos = SBindPont.eBindType
        public void UpdateVO_ShowEffect_One(CCreature cc, int effect, int bindPos)
        {
            if (cc == null || cc.m_vCreature == null)
            {
                return;
            }

            CmdUIHead cmd = new CmdUIHead();
            cmd.type = 11;
            cmd.effectId = effect;
            cmd.effectBindPos = bindPos;
            cc.m_vCreature.PushCommand(cmd);
        }

        public void UpdateVO_HitAnima(CCreature cc, int animaId)
        {
            if (cc == null || cc.m_vCreature == null)
            {
                return;
            }

            CmdUIHead cmd = new CmdUIHead();
            cmd.type = 10;
            cmd.animaId = animaId;
            cc.m_vCreature.PushCommand(cmd);

            //cc.UpdateVO_BuffState(eBuffState.Hit, true);
            //CFrameTimeMgr.Inst.RegisterEvent(200, () =>
            //{
            //    cc.UpdateVO_BuffState(eBuffState.Hit, false);
            //});
        }
        
    }
}