
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature
    {
        public List<BuffBase> m_buffList;
        public List<int> m_listTrigger;

        private int m_buffState;             // BUFF状态eBuffState


        public void AddTrigger(int tUid)
        {
            if (m_listTrigger == null)
                return;

            if (m_listTrigger.Contains(tUid))
            {
                return;
            }
            m_listTrigger.Add(tUid);
        }

        public void ClearTrigger()
        {
            if (m_listTrigger != null)
            {
                // 倒序，可以避免在遍历中玩家移除了BUFF导致的元素丢失问题
                for (int i = m_listTrigger.Count - 1; i >= 0; i--)
                {
                    CBuffTriggerMgr.Destroy(m_listTrigger[i]);
                }
                m_listTrigger.Clear();
            }
        }

        public void AddBuff(BuffBase buffUid)
        {
            if (m_buffList.Contains(buffUid))
            {
                return;
            }
            m_buffList.Add(buffUid);

            //UpdateUI_Buff();
            //if(buffUid.m_buffData.hasColor)
            //    UpdateVO_ColorByBuff();
        }

        public void RemoveBuff(BuffBase buffUid)
        {
            if(m_buffList == null || !m_buffList.Contains(buffUid))
            {
                return;
            }
            m_buffList.Remove(buffUid);

            UpdateUI_Buff();
            if (buffUid.m_buffData.hasColor)
                UpdateVO_ColorByBuff();
        }

        /// <summary>
        /// 包含当前BUFF类型
        /// </summary>
        public bool bConBuffLogicType(eBuffType type)
        {
            if (m_buffList == null)
                return false;

            for(int i = 0; i < m_buffList.Count; i ++)
            {
                if(m_buffList[i].m_buffData.logicId == (int)type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 通过配置id获取
        /// </summary>
        public BuffBase GetBuffByCsvId(int id)
        {
            for (int i = 0; i < m_buffList.Count; i++)
            {
                if (m_buffList[i].m_buffData.id == id)
                {
                    return m_buffList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 死亡时移除的BUFF
        /// </summary>
        public void ClearBuff()
        {
            if (m_buffList != null)
            {
                // 倒序，可以避免在遍历中玩家移除了BUFF导致的元素丢失问题
                for (int i = m_buffList.Count - 1; i >= 0; i--)
                {
                    BuffBase buff = m_buffList[i];
                    if (buff.m_dieDestroy)
                    {
                        buff.Destroy();
                    }
                }
            }
        }

        /// <summary>
        /// 通过【属性增益BUFF】获取的属性增益值
        /// 
        /// </summary>
        public int GetBuffDp(int cur)
        {
            if (m_buffList == null)
                return 0;
            int newVal = 0;
            for (int i = 0; i < m_buffList.Count; i++)
            {
                BuffBase buff = m_buffList[i];
                if (buff.GetBuffType() == eBuffType.dp)
                {
                    FixedPoint pct = buff.GetVal1() * new FixedPoint(0.01f);
                    newVal += (int)(pct * cur).value;
                }
            }
            return newVal;
        }

        /// <summary>
        /// 通过BUFF的攻击增减
        /// </summary>
        public int GetBuffAp(int cur)
        {
            if (m_buffList == null)
                return 0;
            int newVal = 0;
            for (int i = 0; i < m_buffList.Count; i ++)
            {
                BuffBase buff = m_buffList[i];
                if(buff.GetBuffType() == eBuffType.atk)
                {
                    FixedPoint pct = buff.GetVal1() * new FixedPoint(0.01f);
                    newVal += (int)(cur * pct).value;
                }
            }
            return newVal;
        }

        /// <summary>
        /// 通过BUFF的攻击增减
        /// </summary>
        public FixedPoint GetBuffSpeed(FixedPoint cur)
        {
            if (m_buffList == null)
                return FixedPoint.N_0;
            FixedPoint newVal = FixedPoint.N_0;
            for (int i = 0; i < m_buffList.Count; i++)
            {
                BuffBase buff = m_buffList[i];
                if (buff.GetBuffType() == eBuffType.speed)
                {
                    FixedPoint pct = buff.GetVal1() * new FixedPoint(0.01f);
                    newVal += cur * pct;
                }
            }
            return newVal;
        }

        /// <summary>
        /// 设置状态，支持多状态
        /// </summary>
        public void SetState(eBuffState type, bool bSet)
        {
            int iType = (int)type;
            if (bSet)
            {
                m_buffState = m_buffState | (1 << iType);   // 一个为1都为1
            }
            else
            {
                m_buffState = m_buffState & ~(1 << iType);  // 两个都为1才是1
            }

            UpdateVO_BuffState(type, bSet);
        }

        public bool CheckState(eBuffState type)
        {
            int iType = (int)type;
            return ((m_buffState >> iType) & 1) > 0;
        }

        /// <summary>
        /// 通过状态BUFF的类型获取当前BUFF对象
        /// </summary>
        public BuffBase GetStateBuff(eBuffState type)
        {
            for(int i = 0; i < m_buffList.Count; i ++)
            {
                BuffBase buff = m_buffList[i];
                if(buff.IsStateBuff() && buff.GetVal1() == (int)type)
                {
                    return buff;
                }
            }
            return null;
        }

        /// <summary>
        /// 包含状态类BUFF
        /// </summary>
        public bool bStateBuff(eBuffState type)
        {
            if (m_buffList == null)
                return false;
            for (int i = 0; i < m_buffList.Count; i++)
            {
                BuffBase buff = m_buffList[i];
                if (buff.IsStateBuff() && buff.GetVal1() == (int)type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 命中目标时，释放的技能包含击中BUFF，则触发
        /// </summary>
        public void OnCreateAtkBuff(int skillIndex, CCreature foeCc)
        {
            if (m_buffList == null || skillIndex == -1)
                return;

            for (int i = 0; i < m_buffList.Count; i++)
            {
                BuffBase buff = m_buffList[i];
                // 施法者身上包含命中BUFF，并且刚好是当前技能所包含的BUFF，则触发
                if (buff.GetBuffType() == eBuffType.atkCreate)
                {
                    if(m_dicSkill[skillIndex].m_skillDataInfo.ContainAtkBuff(buff.m_buffData.id))
                    {
                        ((Buff11)buff).CreateBuff(foeCc);
                    }
                }
            }
        }

        public void OnCreateHitBuff(CCreature foeCc)
        {
            for (int i = 0; i < m_buffList.Count; i++)
            {
                BuffBase buff = m_buffList[i];
                if (buff.GetBuffType() == eBuffType.hitCreate)
                {
                    ((Buff12)buff).CreateBuff(foeCc);
                }
            }
        }

        public void UpdateVO_BuffState(eBuffState type, bool bAdd)
        {
            if (m_vCreature == null)
                return;
            CmdFspState fspState = new CmdFspState();
            fspState.type = (eVObjectState)type;
            fspState.bAdd = bAdd;
            m_vCreature.PushCommand(fspState);
        }
    }
}