
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature : CThing
    {
        public CCreature(long id)
            : base(id)
        {
            m_arrProp = new int[(int)eCreatureProp.Max];
        }

        public virtual void ExecuteFrame()
        {

        }

        public virtual bool InitConfigure()
        {
            return false;
        }

        public virtual void PushCommand()
        {
            
        }

        public virtual void SetName(string name)
        {
           
        }


        public virtual void SetPos(int x, int y)
        {

        }

        public virtual void SetDir(int eularAngle)
        {

        }

        public void SetQua(Quaternion vRot)
        {
    
        }

        public virtual void SetPublicPropList()
        {
            
        }

        public virtual void InitPropNum(eCreatureProp propType, int nValue)
        {
            SetPropNum(propType, nValue);
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

        public static Dictionary<int, Action<CCreature, int, int>> m_dicPlayerData;
        public int[] m_arrProp;
    }
}