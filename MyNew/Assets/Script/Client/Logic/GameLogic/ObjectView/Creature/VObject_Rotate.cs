using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
   
    public partial class VObject 
    {
     
        private Quaternion m_rotateCurQua = Quaternion.identity;        // 当前
        private Quaternion m_rotateDestQua = Quaternion.identity;       // 目标
        private float m_rotateTime = 0.0f;
        private float m_rotateCurTime = 0.0f;
        private bool m_bRotateing = false;

        private void StartRotate(Vector3 dir, float time)
        {
            if (dir == Vector3.zero)
                return;
            Quaternion dest = Quaternion.LookRotation(dir);
            //if (m_rotateCurQua == dest)
            //{
            //    return;
            //}

            m_rotateCurQua = GetEnt().GetRotateQua();
            m_bRotateing = true;
            m_rotateDestQua = dest;
            m_rotateTime = time;
            m_rotateCurTime = 0.0f;
        }

        private void _UpdateRotate(float fTime, float fDTime)
        {
            if (!m_bRotateing)
                return;
            m_rotateCurTime += fDTime;
            float t = m_rotateCurTime / m_rotateTime;

            t = t >= 1.0f ? 1.0f : t;
            Quaternion rot = Quaternion.Slerp(m_rotateCurQua, m_rotateDestQua, t);
            GetEnt().SetRot(rot);
            if (t >= 1.0f)
            {
                m_bRotateing = false;
            }
        }
    }
}
