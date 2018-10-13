
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
            SetPropNum(eCreatureProp.MoveSpeed, m_csv.moveSpeed);
            SetPropNum(eCreatureProp.Force, m_csv.force);
            SetPropNum(eCreatureProp.Agility, m_csv.agility);
            SetPropNum(eCreatureProp.Brain, m_csv.brain);
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