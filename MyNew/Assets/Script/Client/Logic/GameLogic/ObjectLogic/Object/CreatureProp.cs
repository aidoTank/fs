using System.Collections.Generic;
using System;
namespace Roma
{

    public enum eCreatureProp
    {
        Lv,
        Fighting,
        Hp,
        Ap,  // 攻击
        Dp,  // 防御

        CritChance, // 千分比
        CritDamage, // 千分比

        CurHp,
        CurExp,
        Exp,

        Max
    }

    public class CreatureProp
    {
        public static void Init()
        {
            CCreature.m_dicPlayerData = new Dictionary<int, Action<CCreature, int, int>>()
            {
                 {(int)eCreatureProp.CurHp,  CreatureProp.OnChangeCurHp},
                 {(int)eCreatureProp.Hp,  CreatureProp.OnChangeMaxHp},
                 {(int)eCreatureProp.CurExp,  CreatureProp.OnChangeCurExp},
                 {(int)eCreatureProp.Lv,  CreatureProp.OnChangeLv},
            };
        }

        public static void OnChangeMaxHp(CCreature obj, int newV, int oldV)
        {
            // 表现层
            obj.UpdateVO_ShowHeadHp();
            obj.UpdateUI_Hp();
        }

        public static void OnChangeCurHp(CCreature obj, int newV, int oldV)
        {
            if (oldV == 0 && newV == 0)
                return;
            // 逻辑层
            if (newV <= 0)
            {
                obj.OnDie();
            }
            // 表现层
            obj.UpdateVO_ShowHeadHp();
            // UI
            obj.UpdateUI_Hp();
        }

        public static void OnChangeCurExp(CCreature obj, int newV, int oldV)
        {
            // 逻辑层
            // 到达满级时，不再执行
            if (newV == oldV && newV != 0)
                return;

            // 通过当前总经验值获取等级。
            PlayerExpCsv playerCsv = CsvManager.Inst.GetCsv<PlayerExpCsv>((int)eAllCSV.eAC_PlayerExp);
            int lv = playerCsv.GetLevelByExp(newV);
            if (lv >= playerCsv.m_maxLv.Level)  // 已满级
            {
                int maxExp = playerCsv.GetMaxExpByLv(lv);
                obj.SetPropNum(eCreatureProp.CurExp, maxExp);
                obj.SetPropNum(eCreatureProp.Exp, maxExp);
            }
            else
            {
                int maxExp = playerCsv.GetMaxExpByLv(lv + 1);  // 下一级所需经验
                obj.SetPropNum(eCreatureProp.Exp, maxExp);
            }
            obj.SetPropNum(eCreatureProp.Lv, lv);

            // 表现层
            obj.UpdateUI_Exp();
        }

        public static void OnChangeLv(CCreature obj, int newV, int oldV)
        {
            if (newV == oldV)
                return;

            // 逻辑层
            obj.UpdateProp();
            int maxHp = obj.GetPropNum(eCreatureProp.Hp);
            // 主角升级才回满血
            //if (obj.IsMaster())
            //{
            //    obj.SetPropNum(eCreatureProp.CurHp, maxHp);
            //}

            // 表现层
            obj.UpdateVO_ShowHeadLv();
            obj.UpdateVO_ShowHeadHp();
            obj.UpdateUI_Lv();

            if (oldV != 0 && obj.IsMaster())
            {
                if (obj.m_vCreature != null)
                {
                    CEffectMgr.Create(21160, obj.m_vCreature.GetEnt(), "origin");
                }
            }
        }
    }
     
}