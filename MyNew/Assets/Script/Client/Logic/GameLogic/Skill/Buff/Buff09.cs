using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 缩放
    /// </summary>
    public class Buff09 : BuffBase
    {
        private float m_preScale;

        public Buff09(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();

            m_preScale = m_rec.GetScale();
            m_rec.SetScale(m_preScale * (1 + GetVal1() * 0.01f));

            //if (m_rec.GetRide() != null)
            //{
            //    m_rec.GetRide().SetScale(m_preScale * (1 + GetVal1() * 0.01f));
            //}
        }

        public override void Destroy()
        {
            base.Destroy();

            m_rec.SetScale(m_preScale);
            //if (m_rec.GetRide() != null)
            //{
            //    m_rec.GetRide().SetScale(m_preScale);
            //}
        }
    }
}