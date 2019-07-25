
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 角色的创建由管理器创建
    /// 销毁由自身死亡状态时，自己调用管理器销毁
    /// </summary>
    public partial class CCreature : CObject
    {
        // UI相关
        public void UpdateUI_Base()
        {
            UpdateUI_Icon();
            UpdateUI_Lv();
            UpdateUI_Hp();
            UpdateUI_Exp();
            UpdateUI_Buff();
            UpdataUI_FightingVal();
        }

        public void UpdateUI_Icon()
        {
            if (!IsMaster())
                return;
            //BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            //if (bm.IsShow())
            //{
            //    bm.SetMasterIcon(m_csvData.Icon);
            //}
        }

        public void UpdateUI_Lv()
        {
            if (!IsMaster())
                return;
            //BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            //if (bm.IsShow())
            //{
            //    int lv = GetPropNum(eCreatureProp.Lv);
            //    bm.UpdateLv(lv);
            //}
            //BagModule bag = (BagModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBag);
            //if (bag.IsShow())
            //{
            //    //bag.ShowLV();
            //    bag.ShowLV();
            //}
        }

        public void UpdateUI_Hp()
        {
            if (!IsMaster())
                return;
            //BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            //if (bm.IsShow())
            //{
            //    float hp = GetPropNum(eCreatureProp.CurHp);
            //    float maxHp = GetPropNum(eCreatureProp.Hp);
            //    bm.UpdateHp(hp, maxHp);
            //}
        }
        //战力
        public void UpdataUI_FightingVal()
        {
            if (!IsMaster())
                return;
            //int Val = GetFightingVal(GetPropNum(eCreatureProp.Hp), GetPropNum(eCreatureProp.Ap), GetPropNum(eCreatureProp.Dp), GetPropNum(eCreatureProp.CritChance), GetPropNum(eCreatureProp.CritDamage));
            //BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            //if (bm.IsShow())
            //{                
            //    bm.UpdataFightingVal(GetPropNum(eCreatureProp.Fighting));
            //}
            //BagModule bag = (BagModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBag);
            //if (bag.IsShow())
            //{
            //    //bag.UpdataZhanli();
            //    bag.UpdataZhanli();
            //}
        }


        public void UpdateUI_Exp()
        {
            if (!IsMaster())
                return;
            //BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            //if (bm.IsShow())
            //{
            //    int lv = GetPropNum(eCreatureProp.Lv);
            //    PlayerExpCsv playerCsv = CsvManager.Inst.GetCsv<PlayerExpCsv>((int)eAllCSV.eAC_PlayerExp);
            //    int curLvExp = playerCsv.GetMaxExpByLv(lv);


            //    float curExp = GetPropNum(eCreatureProp.CurExp) - curLvExp;
            //    float maxExp = GetPropNum(eCreatureProp.Exp) - curLvExp;
            //    bm.UpdateExp(curExp, maxExp);
            //}
        }

        public void UpdateUI_Buff()
        {
            if (!IsMaster())
                return;
            //BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            //if (bm.IsShow())
            //{
            //    bm.UpdateBuff(m_buffList);
            //}
        }

        public void UpdateUI_Master(CCreature cc)
        {
            if (!IsMaster())
                return;
            //JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            //js.SetMaster(cc);
            //cc.UpdateUI_Skill();
            //js.InitCd();
        }

        public void UpdateUI_Skill()
        {
            if (m_dicSkill == null)
                return;
            // 如果是主角，坐骑主人是主角
            if (IsMaster())
            {
                JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                if (js.IsShow())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        js.SetIcon(i, -1);
                    }
                    foreach (KeyValuePair<int, CSkillInfo> item in m_dicSkill)
                    {
                        CSkillInfo info = item.Value;
                        if (info != null)
                        {
                            js.SetIcon(item.Key, info.GetIcon());
                        }
                    }
                    //js.SetLock(false);
                }
            }
        }

        public void UpdateUI_SkillLock(bool bLock)
        {
            //if (IsMaster() || (GetMaster() != null && GetMaster().IsMaster()))
            //{
            //    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            //    if (js.IsShow())
            //    {
            //        foreach (KeyValuePair<int, CSkillInfo> item in m_dicSkill)
            //        {
            //            CSkillInfo info = item.Value;
            //            if (info != null)
            //            {
            //                js.SetLock(item.Key, bLock);
            //            }
            //        }
            //    }
            //}
        }

        public void UpdateUI_CD(int index, float time, float maxTime)
        {
            // 如果是主角，坐骑主人是主角
            //if (IsMaster() || (GetMaster() != null && GetMaster().IsMaster()))
            //{
            //    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            //    if (js.IsShow())
            //    {
            //        js.SetCD(index, time, maxTime);
            //    }
            //}
        }

        /// <summary>
        /// 更新能量
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pct"></param>
        public void UpdateUI_Charge(int index, float pct)
        {
            // 如果是主角，坐骑主人是主角
            //if (IsMaster() || (GetMaster() != null && GetMaster().IsMaster()))
            //{
            //    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            //    if (js.IsShow())
            //    {
            //        js.SetCharge(index, pct);
            //    }
            //}
        }

        /// <summary>
        /// 重置摇杆，在放完技能时
        /// </summary>
        public void UpdateUI_ResetJoyStick(bool bTrue)
        {
            if (!IsMaster())
                return;
            JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            js.m_isFirstJoyStick = bTrue;
        }

        /// <summary>
        /// 重置摇杆，在放完技能时
        /// </summary>
        public void UpdateUI_ResetDownUp()
        {
            if (!IsMaster())
                return;
            //JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            //js.m_curSkillJoyStick  = eJoyStickEvent.None;
        }

        public void UpdateUI_AutoAi(bool run)
        {
            if (!IsMaster())
                return;

            //m_bornPoint = GetPos();
            //// 控制界面
            //BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            //if (bm.IsShow())
            //{
            //    bm.SetAi(run);
            //}
        }

        ///// <summary>
        ///// 获得战力值 生命，攻击，防御，暴击率 暴击伤害倍率
        ///// </summary>
        //public int GetFightingVal(int hp,int ap,int dp, float CritChance, float CritDamage)
        //{

        //    double val = hp + ap * 2 + dp + ap * ap / (ap + 1 * dp) *( CritChance / 1000 )* (CritDamage / 1000);
        //    return (int)Math.Ceiling(val);
        //}
    }
}