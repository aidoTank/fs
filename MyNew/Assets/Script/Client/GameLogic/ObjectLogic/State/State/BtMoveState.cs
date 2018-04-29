using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class BtMoveState : FSMState
    {
        private Vector3 m_startPos;
        private Vector3 m_endPos;
        private float m_moveTime = 0.0f;
        private float m_moveCurTime = 0.0f;
        private bool m_bMoveing = false;

        private Quaternion m_rotateCurQua = Quaternion.identity;        // 当前
        private Quaternion m_rotateDestQua = Quaternion.identity;       // 目标
        private float m_rotateTime = 0.0f;
        private float m_rotateCurTime = 0.0f;
        private bool m_bRotateing = false;

        private AnimationAction freeMove = new AnimationAction();

        public BtMoveState(CCreature obj)
            : base(obj)
        {
            m_stateId = StateID.BtMoveState;
        }

        public override void Enter()
        {
            freeMove.crossTime = 0.1f;
            freeMove.strFull = AnimationInfo.m_animRun;
            freeMove.playSpeed = m_creature.m_moveStateParam.m_moveSpeed / 10;
            //if (m_creature.m_bRideState)
            //{
            //    CCreature petCC = m_creature.GetRidePet();
            //    if (petCC != null)
            //    {
            //        petCC.Play(freeMove);
            //    }
            //}
            //else
            //{
            //    m_creature.Play(freeMove);
            //}
            // 通知角色为移动中
            m_creature.m_moveStateParam.m_bMoveing = true;
            // 开启跑步灰尘
            //m_creature.RunDust();
            StarMove();
        }

        public override void Update(float fTime, float fDTime)
        {
            base.Update(fTime, fDTime);

            if (m_bMoveing)
            {
                _UpdateMove(fTime, fDTime);
            }

            if (m_bRotateing)
            {
                _UpdateRotate(fTime, fDTime);
            }
        }

        private void StarMove()
        {
            m_startPos = m_creature.GetPos();
            m_endPos = m_creature.m_moveStateParam.m_btMoveEndPos;
            m_moveTime = Vector3.Distance(m_startPos, m_endPos) / m_creature.m_moveStateParam.m_moveSpeed;
            m_moveCurTime = 0;
            m_bMoveing = true;

            Debug.Log("开始战斗移动。。。。。。。。。。。。。"+ m_endPos + "          "+ m_startPos);

            Vector3 dir = m_endPos - m_startPos;
            if (dir == Vector3.zero)
                return;
            if(m_creature.m_moveStateParam.m_bRota)
            {
                if(m_moveTime < 0.2f)
                    StartRotate(dir, 0f);
                else
                    StartRotate(dir, 0.2f);
            }
        }

        private void _UpdateMove(float fTime, float fDTime)
        {
            m_moveCurTime += fDTime;
            float t = m_moveCurTime / m_moveTime;
            Vector3 curPos = Vector3.Slerp(m_startPos, m_endPos, t);
            m_creature.SetPos(curPos);
            if (t >= 1.0f)
            {
                m_bMoveing = false;
                m_creature.PushCommand(StateID.IdleState);
                //if (null != m_creature.m_moveStateParam.m_moveEnd)
                //{
                //    m_creature.m_moveStateParam.m_moveEnd(m_creature);
                //    //m_creature.m_moveStateParam.m_moveEnd = null;
                //}
            }
        }

        private void StartRotate(Vector3 dir, float time)
        {
            Debug.Log("开始旋转。。。。。。。。。。。。。。。。。。。。。。。。。。");
            Quaternion dest = Quaternion.LookRotation(dir);
            //if (m_rotateCurQua == dest)
            //{
            //    return;
            //}
            Debug.Log("真的旋转。。。。。。。。。。。。。。。。。。。。。。。。。。");
            m_rotateCurQua = m_creature.GetQua();
            m_bRotateing = true;
            m_rotateDestQua = dest;
            m_rotateTime = time;
            m_rotateCurTime = 0.0f;
        }

        private void _UpdateRotate(float fTime, float fDTime)
        {
            m_rotateCurTime += fDTime;
            float t = 0;
            if (m_rotateTime <= 0)
                t = 1;

            t = m_rotateCurTime/m_rotateTime;

            t = t >= 1.0f ? 1.0f : t;
            Quaternion rot = Quaternion.Slerp(m_rotateCurQua, m_rotateDestQua, t);
            m_creature.SetQua(rot);
            if (t >= 1.0f)
            {
                m_bRotateing = false;
            }
        }

        public override void Exit()
        {
            //Debug.Log("结束走路");
            m_creature.m_moveStateParam.m_bMoveing = false;
        }
    }
}