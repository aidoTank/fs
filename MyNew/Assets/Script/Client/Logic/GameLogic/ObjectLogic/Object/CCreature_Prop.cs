
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature
    {
        
        public static Dictionary<int, Action<CCreature, int, int>> m_dicPlayerData;
        public int[] m_arrProp;


        public virtual void InitPublicPropList()
        {
            if (IsPlayer() || IsMaster())
            {
                SetPropNum(eCreatureProp.Lv, 1);
                UpdateProp();
            }
            UpdateMoveSpeed();
            SetPropNum(eCreatureProp.CurHp, GetPropNum(eCreatureProp.Hp));
        }

        public int GetPlayerPropVal(int baseVal, float growVal, int lv)
        {
            double preVal = baseVal;
            for (int i = 1; i <= lv; i++)
            {
                double d = (baseVal * growVal * i);
                preVal = preVal + d;
            }
            return (int)Math.Ceiling(preVal);
        }

        /// <summary>
        /// 等级，装备，BUFF改变时更新属性
        /// 并不改变UI
        /// </summary>
        public virtual void UpdateProp()
        {
            UpdateMaxHp();
            UpdateAp();
            UpdateDp();
            UpdateCritChance();
            UpdateCritDamage();
        }

        public virtual void UpdateMaxHp()
        {
            if (IsPlayer() || IsMaster())
            {
                PlayerCsvData pCsv = (PlayerCsvData)m_csvData;
                int lv = GetPropNum(eCreatureProp.Lv);
                int maxHp = GetPlayerPropVal(pCsv.BaseHp, pCsv.HpGrow, lv);
                //maxHp += GetEquipHp();
                SetPropNum(eCreatureProp.Hp, maxHp);
            }
            UpdataFightingVal();
        }

        /// <summary>
        /// 用于显示界面上的主武器攻击数值
        /// 更新攻击,自身属性 + 装备 + BUFF
        /// </summary>
        public virtual void UpdateAp()
        {
            if (IsPlayer() || IsMaster())
            {
                PlayerCsvData pCsv = (PlayerCsvData)m_csvData;
                int lv = GetPropNum(eCreatureProp.Lv);
                int maxAp = GetPlayerPropVal(pCsv.BaseAp, pCsv.ApGrow, lv);
                //maxAp += GetEquipAp(eEquipType.HandRight);      // 主手装备面板
                maxAp += GetBuffAp(maxAp);
                SetPropNum(eCreatureProp.Ap, maxAp);
            }
            UpdataFightingVal();
        }

        /// <summary>
        /// 更新防御 自身属性 + 装备 + BUFF
        /// </summary>
        public virtual void UpdateDp()
        {
            if (IsPlayer() || IsMaster())
            {
                PlayerCsvData pCsv = (PlayerCsvData)m_csvData;
                int lv = GetPropNum(eCreatureProp.Lv);
                int maxDp = GetPlayerPropVal(pCsv.BaseDp, pCsv.DpGrow, lv);
                //maxDp += GetEquipDp();
                maxDp += GetBuffDp(maxDp);
                SetPropNum(eCreatureProp.Dp, maxDp);
            }
            UpdataFightingVal();
        }

        public virtual void UpdateCritChance()
        {
            if (IsPlayer() || IsMaster())
            {
                PlayerCsvData pCsv = (PlayerCsvData)m_csvData;
                float cc = pCsv.CritChance;
                //cc += GetEquipCritChance();
                cc *= 1000;
                SetPropNum(eCreatureProp.CritChance, (int)cc);
            }
            UpdataFightingVal();
        }

        public virtual void UpdateCritDamage()
        {
            if (IsPlayer() || IsMaster())
            {
                PlayerCsvData pCsv = (PlayerCsvData)m_csvData;
                float cc = pCsv.CritDamage;
                //cc += GetEquipCritDamage();
                cc *= 1000;
                SetPropNum(eCreatureProp.CritDamage, (int)cc);
            }
            UpdataFightingVal();
        }

        public void UpdateMoveSpeed()
        {
            float speed = m_csvData.moveSpeed;
            speed += GetBuffSpeed(speed);
            SetSpeed(new FixedPoint(speed));
        }

        public virtual void UpdataFightingVal()
        {
            if (IsPlayer() || IsMaster())
            {
                int cc = GetFightingVal(GetPropNum(eCreatureProp.Hp), GetPropNum(eCreatureProp.Ap), GetPropNum(eCreatureProp.Dp), GetPropNum(eCreatureProp.CritChance), GetPropNum(eCreatureProp.CritDamage));
                SetPropNum(eCreatureProp.Fighting, (int)cc);
            }
            UpdataUI_FightingVal();
        }


        public virtual void SetPropNum(eCreatureProp propType, int newV)
        {
            if (m_arrProp == null)
            {
                //Debug.LogError("找不到属性，检测是不是UID重复时被移除的类：" + m_uId);
                return;
            }

            int index = (int)propType;
            if (index < m_arrProp.Length)
            {
                int oldV = m_arrProp[index];
                m_arrProp[index] = newV;
                ApplyAttrs(propType, newV, oldV);
            }
            else
            {
                Debug.LogError("属性索引越界" + index);
            }
        }

        public virtual void AddPropNum(eCreatureProp propType, int addV)
        {
            int index = (int)propType;
            if (index < m_arrProp.Length)
            {
                int oldV = m_arrProp[index];
                int newV = oldV + addV;
                m_arrProp[index] = newV;
                ApplyAttrs(propType, newV, oldV);
            }
            else
            {
                Debug.LogError("属性索引越界" + index);
            }
        }

        public virtual int GetPropNum(eCreatureProp propType)
        {
            if (m_arrProp == null)
                return 0;
            int index = (int)propType;
            if (index < m_arrProp.Length)
            {
                return m_arrProp[index];
            }
            else
            {
                Debug.LogError("属性索引越界" + index);
            }
            return 0;
        }

        public virtual void ApplyAttrs(eCreatureProp propType, int newV, int oldV)
        {
            int index = (int)propType;
            Action<CCreature, int, int> changeAction;
            if (m_dicPlayerData.TryGetValue(index, out changeAction))
            {
                changeAction(this, newV, oldV);
            }
        }

        public bool bMaxLv()
        {
            PlayerExpCsv playerCsv = CsvManager.Inst.GetCsv<PlayerExpCsv>((int)eAllCSV.eAC_PlayerExp);
            int lv = GetPropNum(eCreatureProp.Lv);
            if (lv >= playerCsv.m_maxLv.Level)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得战力值 生命，攻击，防御，暴击率 暴击伤害倍率
        /// </summary>
        public int GetFightingVal(int hp, int ap, int dp, float CritChance, float CritDamage)
        {
            double val = 0;
            if (ap <= 0 && dp <= 0)
            {
                val = hp + ap * 2 + dp;
            }
            else
            {
                val = hp + ap * 2 + dp + ap * ap / (ap + 1 * dp) * (CritChance / 1000) * (CritDamage / 1000);
            }
            return (int)Math.Ceiling(val);
        }
    }



}