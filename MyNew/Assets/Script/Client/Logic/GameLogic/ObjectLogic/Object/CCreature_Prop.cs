
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

        public virtual void SetPublicPropList()
        {
            SetPropNum(eCreatureProp.Lv, 1);
            SetPropNum(eCreatureProp.Force, m_csvData.force);
            SetPropNum(eCreatureProp.Agility, m_csvData.agility);
            SetPropNum(eCreatureProp.Brain, m_csvData.brain);
            SetPropNum(eCreatureProp.InitAtk, m_csvData.atk);
            SetPropNum(eCreatureProp.InitArmor, m_csvData.armor);
            //SetPropNum(eCreatureProp.InitMoveSpeed, m_csvData.moveSpeed);

            UpdateMaxHp();
            SetPropNum(eCreatureProp.CurHp, GetPropNum(eCreatureProp.Hp));
            UpdateArmor();
            UpdateMaxMp();
            SetPropNum(eCreatureProp.CurMp, GetPropNum(eCreatureProp.MaxMp));
            UpdateAspd();
            UpdateAtk();
            UpdateMoveSpeed();
        }

        public void UpdateMaxHp()
        {
            SetPropNum(eCreatureProp.Hp, 
            GetPropNum(eCreatureProp.Force) * 20);
        }

        public void UpdateArmor()
        {
            SetPropNum(eCreatureProp.Armor, 
            (int)((float)GetPropNum(eCreatureProp.InitArmor) +
            (float)GetPropNum(eCreatureProp.Agility) * 0.17f));     
        }

        public void UpdateMaxMp()
        {
            SetPropNum(eCreatureProp.MaxMp, 
            GetPropNum(eCreatureProp.Brain) * 12);
        }

        public void UpdateAspd()
        {
            SetPropNum(eCreatureProp.Aspd, 
            100 * 1000 +
            GetPropNum(eCreatureProp.Agility));
        }

        public void UpdateAtk()
        {
            SetPropNum(eCreatureProp.Atk, 
            GetPropNum(eCreatureProp.InitAtk) +
            GetPropNum(eCreatureProp.Agility));
        }

        public void UpdateMoveSpeed()
        {
            SetPropNum(eCreatureProp.MoveSpeed, 
            GetPropNum(eCreatureProp.InitMoveSpeed));
        }

        public virtual void SetPropNum(eCreatureProp propType, int newV)
        {
            int index = (int)propType;
            if(index > 0 && index < m_arrProp.Length)
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
            if(index > 0 && index < m_arrProp.Length)
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
            int index = (int)propType;
            if(index > 0 && index < m_arrProp.Length)
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
            if(m_dicPlayerData.TryGetValue(index, out changeAction))
            {
                changeAction(this, newV, oldV);
            }
        }
    }


}