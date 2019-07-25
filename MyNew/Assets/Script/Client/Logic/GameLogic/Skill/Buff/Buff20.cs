using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// BUFF触发器
    /// </summary>
    public class Buff20 : BuffBase
    {
        public static long BUFFTRIGGER_UID = 10000;
        private int m_curIntervalTime;

        private int m_delayEventHid;

        public Buff20(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            if(!m_buffData.IsInterval)  // 如果不是间隔触发，那么直接调用父类
            {
                base.Init();
            }
            else   // 间隔触发器，每次播放触发一次性特效，如机枪
            {
                UpdateVO_ShowEffect_One(m_caster, m_buffData.effectId, m_buffData.effectPoint);
            }
            CreateTrigger();
        }

        public override void ExecuteFrame()
        {
            if (m_destroy)
                return;

            if(m_buffData.IsInterval)
            {
                m_curIntervalTime += FSPParam.clientFrameMsTime;
                if (m_curIntervalTime >= m_buffData.IntervalTime)
                {
                    m_curIntervalTime = 0;

                    CreateTrigger();
                    // 间隔触发器，每次播放触发一次性特效，如机枪
                    UpdateVO_ShowEffect_One(m_caster, m_buffData.effectId, m_buffData.effectPoint);
                }
            }
        }

        /// <summary>
        /// 支持创建多个不同的触发器
        /// </summary>
        private void CreateTrigger()
        {
            //int val = m_buffData.ParamValue1;
            BuffTriggerCsv buffCsv = CsvManager.Inst.GetCsv<BuffTriggerCsv>((int)eAllCSV.eAc_BuffTrigger);
            for (int i = 0; i < m_buffData.listParam.Length; i ++)
            {
                int triId = m_buffData.listParam[i];
                if (triId == 0)
                    continue;
                BuffTriggerCsvData triggerData = buffCsv.GetData(triId);
                if(triggerData == null)
                {
                    Debug.LogError("触发器为空：" + triId);
                    return; 
                }
                if(triggerData.DelayTime == 0)
                {
                    CreateTrigger(triggerData);
                }
                else
                {
                    m_delayEventHid = CFrameTimeMgr.Inst.RegisterEvent(triggerData.DelayTime, () =>
                    {
                        CreateTrigger(triggerData);
                    });
                }
                //Debug.Log("技能流程：创建检测器：" + triId + "  " + triggerData.Name);
            }
        }

        private void CreateTrigger(BuffTriggerCsvData data)
        {
            CBuffTrigger buffTri = CBuffTriggerMgr.Create((eBuffTriggerPosType)data.PosType);
            buffTri.m_caster = m_caster;
            buffTri.m_rec = m_rec;
            buffTri.m_skillIndex = m_skillIndex;
            buffTri.m_skillPos = m_skillPos;
            buffTri.m_extendParam = m_extendParam;
            buffTri.Create(data.Id, string.Empty, m_startPos, m_skillDir);

            if(m_caster != null)
                m_caster.AddTrigger((int)buffTri.GetUid());
        }

        public override void Destroy()
        {
            base.Destroy();
            //CFrameTimeMgr.Inst.RemoveEvent(m_delayEventHid);
        }

    }
}