
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature
    {
        private SkillCsvData m_skillInfo;
        // 本地CSV数据
        // public SkillStepCsvData m_casterData;
        // public SkillStepCsvData m_hitData;
        // public List<SkillStepCsvData> m_flyData;
        // public SkillStepCsvData m_flyHitData;

        public void EnterSkill()
        {
            int skillId = m_cmdFspSendSkill.m_skillId;
            m_skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(skillId);
            //SkillStepCsv step = CsvManager.Inst.GetCsv<SkillStepCsv>((int)eAllCSV.eAC_SkillStep);
            // m_casterData = step.GetCasterData(skillId);
            // m_hitData = step.GetHitData(skillId);
            // step.GetFlyData(ref m_flyData, skillId);
            // m_flyHitData = step.GetFlyHitData(skillId);

            // 毫秒
            int curTime =  GameManager.Inst.GetFspManager().GetCurFrameIndex() * FSPParam.clientFrameMsTime;
            // 获取近战受击时间
            TimeEvent launch = new TimeEvent();
            launch.fun = _TimeEvent_Launch;
            launch.beginTime = curTime + m_skillInfo.launchTime;
            AddTimeEvent(launch);     
        }

        private void _TimeEvent_Launch(int frameId)
        {
            Debug.Log("发射技能");
            switch(m_skillInfo.skillType)
            {
                case (int)eSkillType.Near:
                    // 创建技能逻辑
                    SkillNear sNear = new SkillNear(1, m_skillInfo.id);
                    CSkillMgr.Add(1, sNear);
                    sNear.InitConfigure();
                break;
                case (int)eSkillType.Fly:
                    // 创建飞行技能
                break;
                case (int)eSkillType.Aoe:
                    // 创建AOE技能
                break;
            }

            ClearTimeEvent();
        }
 


        public void TickSkill(int frameId)
        {
            _CheckEvent(frameId);
        }

        public delegate void UpdateEvent(int frameId);
        public class TimeEvent
        {
            public int beginTime;
            public UpdateEvent fun;
            public bool bEnd;
        }

        protected List<TimeEvent> m_listTimeEvent = new List<TimeEvent>();

        protected void AddTimeEvent(TimeEvent EventObj)
        {
            m_listTimeEvent.Add(EventObj);
        }
        protected void ClearTimeEvent()
        {
            m_listTimeEvent.Clear();
        }

        protected void _CheckEvent(int frameId)
        {
            for (int i = 0; i < m_listTimeEvent.Count; i++)
            {
                TimeEvent eventObj = m_listTimeEvent[i];
                // 容器中的元素全部是按照开始时间排序的，所以当找到第一个时间不够的元素就不需要再继续循环了
                // 游戏时间 > 事件的起点时间
                if (frameId * FSPParam.clientFrameMsTime >= eventObj.beginTime)
                {
                    if (!eventObj.bEnd)
                    {
                        eventObj.fun(frameId);
                        eventObj.bEnd = true;
                        continue;
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }


}