using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 速度增减
    /// </summary>
    public class Buff05 : BuffBase
    {
        private float m_rideSpeed;
        public Buff05(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            if (m_rec == null)
            {
                Destroy();
                return;
            }
          

            base.Init();
            m_rec.UpdateMoveSpeed();

            //CCreature ride = m_rec.GetRide();
            //if (ride != null)
            //{
            //    m_rideSpeed = ride.GetSpeed();
            //    ride.SetSpeed(m_rideSpeed + m_rideSpeed * GetVal1() * 0.01f);
            //}
        }

        public override void Destroy()
        {
            base.Destroy();
            if (m_rec == null)
                return;

            m_rec.UpdateMoveSpeed();

            //CCreature ride = m_rec.GetRide();
            //if (m_rideSpeed != 0 && ride != null)
            //{
            //    ride.SetSpeed(m_rideSpeed);
            //}
        }

    }
}