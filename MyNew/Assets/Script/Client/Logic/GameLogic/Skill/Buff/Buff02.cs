using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 持续伤害的BUFF
    /// </summary>
    public class Buff02 : BuffBase
    {
        private int m_curIntervalTime;
 
        public Buff02(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

  
        public override void ExecuteFrame()
        {
            if (m_rec == null || m_rec.IsDie())
                return;

            m_curIntervalTime += FSPParam.clientFrameMsTime;
            if (m_curIntervalTime >= m_buffData.IntervalTime)
            {
                m_curIntervalTime = 0;

                int val = m_buffData.ParamValue1;
                OnHurt(val);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
        }

    }
}