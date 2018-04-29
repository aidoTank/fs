using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class RotateState : FSMState
    {
        public RotateState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.RotateState;
        }

        public override void Enter()
        {
            float angle =  Quaternion.Angle(m_creature.GetQua(), m_creature.m_moveStateParam.m_btEndQua);
            if(angle < 15)
            {
                m_creature.PushCommand(StateID.IdleState);
                //if (null != m_creature.m_moveStateParam.m_rotaEnd)
                //{
                //   // m_creature.m_moveStateParam.m_rotaEnd(m_creature);
                //   // m_creature.m_moveStateParam.m_rotaEnd = null;
                //}
                return;
            }

            if (StartRotate(m_creature.m_moveStateParam.m_btEndQua, 0.14f))
            {
                PlayAnima();
            }
        }

        private void PlayAnima()
        {
            AnimationAction freeMove = new AnimationAction();
            freeMove.crossTime = 0.1f;
            freeMove.playSpeed = 0.2f;
            freeMove.strFull = AnimationInfo.m_animRun;

                m_creature.Play(freeMove);
   
            Vector3 dir = m_creature.GetDirection();
        }

        private bool StartRotate(Quaternion endQua, float time)
        {
            Debug.Log("旋转状态开始旋转。。。。。。。。。。。。。。。。。。。。。。。。。。");
            //Quaternion dest = Quaternion.LookRotation(dir);
            m_rotateCurQua = m_creature.GetQua();

            m_bRotateing = true;
            m_rotateDestQua = endQua;
            m_rotateTime = time;
            m_rotateCurTime = 0.0f;
            return true;
        }

        public override void Update(float fTime, float fDTime)
        {
            if(m_bRotateing)
                _UpdateRotate(fTime, fDTime);
        }

        private void _UpdateRotate(float fTime, float fDTime)
        {
            m_rotateCurTime += fDTime;
            float t = 0;
            if (m_rotateTime <= 0)
                t = 1;

            t = m_rotateCurTime / m_rotateTime;

            t = t >= 1.0f ? 1.0f : t;
            Quaternion rot = Quaternion.Slerp(m_rotateCurQua, m_rotateDestQua, t);
            m_creature.SetQua(rot);
            if (t >= 1.0f)
            {
                m_bRotateing = false;
                m_creature.PushCommand(StateID.IdleState);
                //if (null != m_creature.m_moveStateParam.m_rotaEnd)
                //{
                //    m_creature.m_moveStateParam.m_rotaEnd(m_creature);
                //    m_creature.m_moveStateParam.m_rotaEnd = null;
                //}
            }
        }

        public override void Exit()
        {

        }


        private Quaternion m_rotateCurQua = Quaternion.identity;        // 当前
        private Quaternion m_rotateDestQua = Quaternion.identity;       // 目标
        private float m_rotateTime = 0.0f;
        private float m_rotateCurTime = 0.0f;
        private bool m_bRotateing = false;
    }
}