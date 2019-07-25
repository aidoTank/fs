using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 回复生命
    /// </summary>
    public class Buff07 : BuffBase
    {
        private int m_curIntervalTime;

        public Buff07(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();
            int val = m_buffData.ParamValue1; // 百分比
            int maxHp = m_rec.GetPropNum(eCreatureProp.Hp);
            int add = (int)((val * 0.01f) * maxHp);

            AddHp(add, maxHp);
            UpdateUI_ShowHpHUD(m_rec, add);
        }

        public override void ExecuteFrame()
        {
            if(IsCont())
            {
                if (m_rec == null || m_rec.IsDie())
                    return;

                m_curIntervalTime += FSPParam.clientFrameMsTime;
                if (m_curIntervalTime >= m_buffData.IntervalTime)   // 间隔加血
                {
                    m_curIntervalTime = 0;

                    int val = m_buffData.listParam[1];
                    int maxHp = m_rec.GetPropNum(eCreatureProp.Hp);
                    int add = (int)((val * 0.01f) * maxHp);

                    AddHp(add, maxHp);
                    UpdateUI_ShowHpHUD(m_rec, add);
                }
            }
        }

        private void AddHp(int add, int maxHp)
        {
            int curHp = m_rec.GetPropNum(eCreatureProp.CurHp);
            if (curHp + add > maxHp)
            {
                m_rec.SetPropNum(eCreatureProp.CurHp, maxHp);
            }
            else
            {
                m_rec.AddPropNum(eCreatureProp.CurHp, add);
            }
        }
    }
}