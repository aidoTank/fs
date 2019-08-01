
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 玩家当前身上的技能
    /// </summary>
    public class CSkillInfo
    {
        public CCreature m_creature;
        public SkillCsvData m_skillInfo;
        public SkillDataCsvData m_skillDataInfo;
        public int m_skillIndex;
        public int m_skillId;
        public int m_skillLv;

        private bool m_startCd;
        private int m_curCdTime;  // 也表示蓄能值

        private bool m_bCondiSubSkill;
        private CSkillInfo m_mainSkill;
        private CSkillInfo[] m_nearCommon;
        private int m_subSkillNum;
        private int m_subSkillTime;
        private const int m_subSkillMaxTime = 3000;   // 1秒内还原首次攻击
        // 技能附带攻击，受击BUFF
        private List<BuffBase> m_buffList = new List<BuffBase>();

        public CSkillInfo(CCreature creature, int index, int skillId, int lv, bool subSkill = false, CSkillInfo mainSkill = null)
        {
            m_creature = creature;
            m_skillId = skillId;
            m_skillIndex = index;
            SkillCsv skill = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill);
            m_skillInfo = skill.GetData(skillId);
            m_skillLv = lv;

            int skillDataId = skill.GetSkillDataIdByLv(skillId, lv);

            SkillDataCsv dataCsv = CsvManager.Inst.GetCsv<SkillDataCsv>((int)eAllCSV.eAC_SkillData);
            m_skillDataInfo = dataCsv.GetData(skillDataId);
            if (m_skillDataInfo == null)
                return;
            if(GetCDType() == (int)eSkillCdType.Time)
            {
                m_curCdTime = m_skillDataInfo.cd;
            }
            // 技能附加击中BUFF
            for (int i = 0; i < m_skillDataInfo.atkBuffList.Length; i++)
            {
                if (m_skillDataInfo.atkBuffList[i] == 0)
                    continue;
                BuffBase buff = SkillBase.AddBuff(m_creature, m_creature, m_skillDataInfo.atkBuffList[i], Vector2d.zero, Vector2d.zero, Vector2d.zero, index);
                if (buff != null)
                {
                    buff.m_dieDestroy = false;
                    m_buffList.Add(buff);
                }
            }
            // 技能附加受击BUFF
            for (int i = 0; i < m_skillDataInfo.hitBuffList.Length; i++)
            {
                if (m_skillDataInfo.hitBuffList[i] == 0)
                    continue;
                BuffBase buff = SkillBase.AddBuff(m_creature, m_creature, m_skillDataInfo.hitBuffList[i], Vector2d.zero, Vector2d.zero, Vector2d.zero, index);
                if (buff != null)
                {
                    buff.m_dieDestroy = false;
                    m_buffList.Add(buff);
                }
            }

            #region 包含子技能的部分
            m_mainSkill = mainSkill;
            if (subSkill)
                return;
            if(m_skillInfo.subSkill != null && 
                m_skillInfo.subSkill.Length > 0 && 
                m_skillInfo.subSkill[0] != 0)
            {
                m_bCondiSubSkill = true;
                m_nearCommon = new CSkillInfo[m_skillInfo.subSkill.Length];
                for (int i = 0; i < m_skillInfo.subSkill.Length; i ++)
                {
                    int sSkillId = m_skillInfo.subSkill[i];
                    CSkillInfo info = new CSkillInfo(m_creature, index, sSkillId, 1, true, this);
                    m_nearCommon[i] = info;
                }
            }
            #endregion
        }

        /// <summary>
        /// 用于获取正式技能
        /// </summary>
        public CSkillInfo GetCurSkill()
        {
            if (m_bCondiSubSkill)
            {
                return m_nearCommon[m_subSkillNum];
            }
            else
            {
                return this;
            }
        }

        public CSkillInfo GetMainSkill()
        {
            if (m_mainSkill == null)
                return this;
            return m_mainSkill;
        }

        /// <summary>
        /// 调用父技能的使用完成
        /// </summary>
        public void OnUseSkill()
        {
            // 包含子技能，使用时计时器重置
            if (m_bCondiSubSkill)
            {
                m_subSkillNum++;
                if (m_subSkillNum > m_nearCommon.Length - 1)
                {
                    m_subSkillNum = 0;
                    // 关键点：在多段技能结束时，清空玩家技能指令，可用于AI多段技能时进行连续攻击
                    m_creature.m_cmdFspSendSkill = null;  
                }
                m_subSkillTime = 0;
            }


            //if (!Client.Inst().m_bCd)
            //    return;

            m_startCd = true;
            m_curCdTime = 0;

            //if (GetCDType() == (int)eSkillCdType.Charge)
            //{
            //    m_creature.UpdateUI_Charge(m_skillIndex, 0.0f);
            //}
        }

        /// <summary>
        /// 添加蓄能技能蓄力值
        /// </summary>
        public void AddCharge(int val)
        {
            m_curCdTime += val;
            if (m_curCdTime >= m_skillDataInfo.cd)
            {
                m_curCdTime = m_skillDataInfo.cd;
            }
        }

        public void ExecuteFrame()
        {
            if (m_skillDataInfo == null)
                return;

            if (m_skillDataInfo.cdType == (int)eSkillCdType.Time && m_startCd)
            {
                m_curCdTime += FSPParam.clientFrameMsTime;
                if (m_curCdTime >= m_skillDataInfo.cd)
                {
                    m_startCd = false;
                }
                m_creature.UpdateUI_CD(m_skillIndex, m_curCdTime * 0.001f, m_skillDataInfo.cd * 0.001f);
            }

            if(m_bCondiSubSkill)
            {
                m_subSkillTime += FSPParam.clientFrameMsTime;
                if(m_subSkillTime > m_subSkillMaxTime) 
                {
                    m_subSkillNum = 0;
                }
            }
        }

        public bool IsCanUse()
        {
            //if (!Client.Inst().m_bCd)
            //    return true;
            //return true;
            if (m_skillDataInfo == null)
                return false;
            return m_curCdTime >= m_skillDataInfo.cd;
        }

        public bool IsMultiStep()
        {
            return m_bCondiSubSkill;
        }

        public int GetCurSubSkillIndex()
        {
            return m_subSkillNum;
        }

        public int GetSubSkillNum()
        {
            if(m_nearCommon != null)
                return m_nearCommon.Length;
            return 0;
        }

        public int GetCurCdTime()
        {
            return m_curCdTime;
        }

        // 给界面用的
        public float GetChargePct()
        {
            if (m_skillDataInfo.cd == 0)
                return 0;
            return (float)m_curCdTime / m_skillDataInfo.cd;
        }

        public int GetMaxCdTime()
        {
            if (m_skillDataInfo == null)
                return 0;
            return m_skillDataInfo.cd;
        }

        public int GetSkillLv()
        {
            return m_skillLv;
        }

        public int GetSkillIndex()
        {
            return m_skillIndex;
        }

        public int GetSkillId()
        {
            return m_skillId;
        }

        public int GetIcon()
        {
            if (m_skillInfo == null)
            {
                Debug.LogError("获取技能为空：" + m_skillId);
                return 0;
            }
            return m_skillInfo.icon;
        }

        public int GetRange()
        {
            if(m_skillInfo == null)
            {
                Debug.LogError("获取技能为空：" + m_skillId);
                return 0;
            }
            return m_skillInfo.distance;
        }

        public int GetCDType()
        {
            if (m_skillDataInfo == null)
                return 0;
            return m_skillDataInfo.cdType;
        }

        public void Destoty()
        {
            for(int i = 0; i < m_buffList.Count; i ++)
            {
                BuffBase buff = m_buffList[i];
                if(buff != null)
                {
                    buff.Destroy();
                }
            }
        }
    }

    public enum eSkillTypeIndex
    {
        common = 0,
        sprint = 1,       // 冲刺
        heavyWeapons = 2, // 重武器
        grenade = 3,      // 手榴弹
        uniqueSkill = 4,  // 大招
    }

    public partial class CCreature
    {
        /// <summary>
        /// 1.当前角色释放技能的指令
        /// 2.也用于触发器存储的技能指令
        /// </summary>
        public CmdFspSendSkill m_cmdFspSendSkill;  
        /// <summary>
        ///  当前技能列表
        /// </summary>
        public Dictionary<int, CSkillInfo> m_dicSkill;

        public SkillBase m_curSkill;

        /// <summary>
        /// 按下开枪被打斯时调用
        /// </summary>
        public void DestoryDownUpSkill()
        {
            SkillBase dSkill = CSkillMgr.GetDownUpSkill(GetUid());
            if(dSkill != null)
            {
                if (dSkill.m_skillInfo.skillType == (int)eSkillType.Down_Up)
                {
                    //UpdateUI_ResetDownUp();
                    SkillDownUp dp = dSkill as SkillDownUp;
                    dp.OnUp();
                }
            }
        }

        #region 释放技能
        public void EnterSkill()
        {
            CSkillInfo sInfo = GetSkillByIndex(m_cmdFspSendSkill.m_skillIndex);
            if (sInfo == null)
                return;

            // 如果存在引导技能，则不再创建
            SkillBase dSkill = CSkillMgr.GetDownUpSkill(GetUid());
            if (dSkill != null)
            {
                if(sInfo.m_skillInfo.skillType == (int)eSkillType.Down_Up)
                {
                    SkillDownUp dp = dSkill as SkillDownUp;
                    dp.SetCmd(m_cmdFspSendSkill);
                }
                return;
            }

            if (!m_logicSkillEnabled)
            {
                return;
            }
             

            //Debug.Log("技能流程：使用技能：" + sInfo.m_skillInfo.id + " "+ sInfo.m_skillInfo.name);
            if (!sInfo.IsCanUse())
            {
                return;
            }
      
            //Debug.Log("CD 正常进入技能：" + m_cmdFspSendSkill.m_skillIndex + " " + sInfo.m_skillDataInfo.name);

            SkillCsvData m_skillInfo = sInfo.m_skillInfo;
            StartSkill(m_skillInfo);
            sInfo.GetMainSkill().OnUseSkill();
        }

        public void StartSkill(SkillCsvData m_skillInfo)
        {
            switch (m_skillInfo.skillType)
            {
                case (int)eSkillType.None:
                    VSkillBase nSkill = new VSkillBase();
                    //nSkill.PushCommand(m_cmdFspSendSkill);

                    m_curSkill = CSkillMgr.Create(eSkillType.None, nSkill);
                    m_curSkill.PushCommand(m_cmdFspSendSkill);
                    break;
                case (int)eSkillType.Near:
                    VSkillNear vSkill = new VSkillNear();
                    //vSkill.PushCommand(m_cmdFspSendSkill);

                    m_curSkill = (SkillNear)CSkillMgr.Create(eSkillType.Near, vSkill);
                    m_curSkill.PushCommand(m_cmdFspSendSkill);
                    break;
                case (int)eSkillType.Jump:
                    VSkillBase vjump = new VSkillBase();
                    //vjump.PushCommand(m_cmdFspSendSkill);

                    m_curSkill = (SkillJump)CSkillMgr.Create(eSkillType.Jump, vjump);
                    m_curSkill.PushCommand(m_cmdFspSendSkill);
                    break;
                case (int)eSkillType.Down_Up:

                    VSkillDownUp vDp = new VSkillDownUp();
                    //vDp.PushCommand(m_cmdFspSendSkill);

                    m_curSkill = (SkillDownUp)CSkillMgr.Create(eSkillType.Down_Up, vDp);
                    m_curSkill.PushCommand(m_cmdFspSendSkill);
                    break;
            }
        }

        #endregion

        public void AddSkill(int index, int skillId, int skillLv)
        {
            if (m_dicSkill.ContainsKey(index))
            {
                //Debug.Log("添加重复技能索引：" + index);
                RemoveSkill(index);
            }
            CSkillInfo info = new CSkillInfo(this, index, skillId, skillLv);
            m_dicSkill[index] = info;
            //UpdateUI_CD(index, 0, 0);
        }

        public void RemoveSkill(int index)
        {
            if(m_dicSkill.ContainsKey(index))
            {
                CSkillInfo info = m_dicSkill[index];
                if(info != null)
                {
                    info.Destoty();
                    info = null;
                }
                m_dicSkill.Remove(index);
            }
        }

        public CSkillInfo GetSkillByIndex(int index)
        {
            if (m_dicSkill == null)
                return null;
            if (!m_dicSkill.ContainsKey(index))
                return null;

            return m_dicSkill[index].GetCurSkill();
        }

        public void GetCanUseSkillList(ref List<CSkillInfo> list)
        {
            list.Clear();
            for (int nIndex = 0; nIndex < 5; nIndex++)
            {
                if(m_dicSkill.ContainsKey(nIndex))
                {
                    CSkillInfo skill = m_dicSkill[nIndex];
                    if (skill != null && skill.IsCanUse())
                    {
                        list.Add(skill);
                    }
                }
            }
        }

        public void ExecuteFrameSkill()
        {
            //ExecuteFrame_Combo();

            if (m_dicSkill == null)
                return;
            foreach(KeyValuePair<int, CSkillInfo> item in m_dicSkill)
            {
                if(item.Value != null)
                    item.Value.ExecuteFrame();
            }
        }


        /// <summary>
        /// 是否同阵营
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool bCamp(CCreature target)
        {

            if (GetUid() == target.GetUid())
                return true;

            return false;

            if (IsPlayer() && target.IsMonster())   // 玩家对怪物
            {
                return false;
            }
            //else if (IsPartner() && target.IsMonster())
            //{
            //    return false;
            //}
            else if (IsMonster() &&  target.IsPlayer()) // 怪物对玩家
            {
                return false;
            }
            else if (IsNpc() && target.IsMonster())   // 坐骑对怪物
            {
                return false;
            }
            return true;
        }

        #region 主角伤害保护
        public bool m_bMasterProtect;

        public void SetMasterProtect()
        {
            m_bMasterProtect = true;
            CFrameTimeMgr.Inst.RegisterEvent(500, () =>
            {
                m_bMasterProtect = false;
            });
        }

        public bool bMasterProtect()
        {
            return m_bMasterProtect;
        }
        #endregion

        #region 连击相关
        private int m_comboTime;
        private const int m_comboMaxTime = 3000;
        private int m_comboNum;
        public int GetComboNum()
        {
            return m_comboNum;
        }

        public void OnComboAdd()
        {
            if (!IsMaster())
                return;
            m_comboNum++;
            m_comboTime = 0;
            //UpdataUI_ComboNum();
            //Debug.Log("m_comboNum:" + m_comboNum);
        }

        //public void ExecuteFrame_Combo()
        //{
        //    if (!IsMaster())
        //        return;
        //    m_comboTime += FSPParam.clientFrameMsTime;
        //    if (m_comboTime > m_comboMaxTime)
        //    {
        //        m_comboNum = 0;
        //    }
        //}
        #endregion
    }
}