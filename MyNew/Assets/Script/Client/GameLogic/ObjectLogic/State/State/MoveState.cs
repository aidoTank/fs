using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class MoveState : FSMState
    {
        private Vector2 m_targetPos;
        private List<Vector2> m_movePath = new List<Vector2>();
        private List<float> m_pathTime = new List<float>();
        private int m_curMoveIndex = 0;
        private float m_curMoveTime = 0.0f;
        private Vector2 m_curMovePos = Vector2.zero;
        private bool m_bMovePathing = false;

        private Quaternion m_rotateCurQua = Quaternion.identity;        // 当前
        private Quaternion m_rotateDestQua = Quaternion.identity;       // 目标
        private float m_rotateTime = 0.0f;
        private float m_rotateCurTime = 0.0f;
        private bool m_bRotateing = false;

        private AnimationAction freeMove = new AnimationAction();

        private float m_curSendTime = 0;
        private float m_sendMaxTime = 2f;
        public MoveState(CCreature obj)
            : base(obj)
        {
            m_stateId = StateID.MoveState;
        }
        public override void Enter()
        {
            if(!SceneManager.Inst.IsLoaded() || 
                m_creature.m_moveStateParam.m_movePath.Count == 0)
            {
                m_creature.PushCommand(StateID.IdleState);
                return;
            }
            m_rotateCurQua = m_creature.GetQua();
            freeMove.crossTime = 0.1f;
            freeMove.strFull = AnimationInfo.m_animRun;
            freeMove.playSpeed = m_creature.m_moveStateParam.m_moveSpeed / 10;
            m_creature.Play(freeMove);
            // 通知角色为移动中
            m_creature.m_moveStateParam.m_bMoveing = true;
            // 开启跑步灰尘
            //m_creature.RunDust();

            StartMove(ref m_creature.m_moveStateParam.m_movePath, m_creature.m_moveStateParam.m_moveSpeed);
        }

        public override void Update(float fTime, float fDTime)
        {
            base.Update(fTime, fDTime);
            if (m_bMovePathing)
            {
                _UpdateMove(fTime, fDTime);

                if(Debug.logger.logEnabled)
                {
                    for (int i = 0; i < m_movePath.Count - 1; i++)
                    {
                        float x1 = m_movePath[i].x;
                        float y1 = m_movePath[i].y;
                        float x2 = m_movePath[i + 1].x;
                        float y2 = m_movePath[i + 1].y;
                        Debug.DrawLine(
                            new Vector3(x1, SceneManager.Inst.GetTerrainHeight(x1, y1) + 0.1f, y1),
                            new Vector3(x2, SceneManager.Inst.GetTerrainHeight(x2, y2) + 0.1f, y2),
                            Color.green);
                    }
                }
            }
            if (m_bRotateing)
            {
                _UpdateRotate(fTime, fDTime);
            }
        }

        private void InitMove()
        {
            m_movePath.Clear();
            m_pathTime.Clear();
            m_curMoveIndex = 0;
            m_curMoveTime = 0.0f;
            m_curMovePos = Vector2.zero;
            m_bMovePathing = false;
        }

        public void StartMove(ref List<Vector2> pathList, float fSpeed)
        {
            // 发送目标点
            if(pathList.Count >= 1)
            {
                m_targetPos = pathList[pathList.Count - 1];
                //Debug.Log("开始移动发送位置=========================================="+ m_targetPos);
                SendMasterMoveToServer(10);
            }

            InitMove();
            m_bMovePathing = true;
            // 给路径赋值
            for (int i = 0; i < pathList.Count; i++)
            {
                m_movePath.Add(pathList[i]);
            }
            // 插入玩家当前起点位置
            Vector2 vCurPos = new Vector2(m_creature.GetPos().x, m_creature.GetPos().z);
            m_movePath.Insert(0, vCurPos);

            // 开始获取移动时间
            float runTime = 0.0f;
            for (int i = 0; i < m_movePath.Count - 1; i++)
            {
                float dis = Vector2.Distance(m_movePath[i], m_movePath[i + 1]);
                runTime += dis / fSpeed;
                m_pathTime.Add(runTime);
            }
            m_pathTime.Insert(0, 0);    // 在第一个位置时间当然是0了
        }

        private void _UpdateMove(float fTime, float fDTime)
        {
            float t = 0.0f;
            m_curMoveTime += fDTime;
            int begin = m_curMoveIndex + 1;
            for (int i = begin; i < m_pathTime.Count; i++)    // 索引0,1,2,3,4 取 1-4 作为对比的目标时间
            {
                if (m_curMoveTime > m_pathTime[i])
                {
                    m_curMoveIndex = i;
                    break;
                }
            }

            if (m_curMoveIndex >= m_pathTime.Count - 1)
            {
                // 最后一个地点
                t = 1.0f;
                m_curMovePos = m_movePath[m_movePath.Count - 1];
                // 停止移动
                //m_moveEnd(m_curMovePos);
                m_bMovePathing = false;

                // 没有回调就默认进入待机状态
                if (m_creature.m_moveStateParam.m_moveEnd == null)
                {
                    m_creature.PushCommand(StateID.IdleState);
                }
                else
                {
                    m_creature.m_moveStateParam.m_moveEnd(m_creature);
                    m_creature.m_moveStateParam.m_moveEnd = null;
                }
                SendMasterMoveToServer(10.0f);       // 移动结束
                //Debug.Log("结束移动发送位置==========================================" + m_targetPos);
            }
            else
            {
                t = (m_curMoveTime - m_pathTime[m_curMoveIndex]) / (m_pathTime[m_curMoveIndex + 1] - m_pathTime[m_curMoveIndex]);
                m_curMovePos = Vector2.Lerp(m_movePath[m_curMoveIndex], m_movePath[m_curMoveIndex + 1], t);
            }
            // 设置位置之前控制旋转
            Vector3 dir = new Vector3(
                m_curMovePos.x - m_creature.GetPos().x,
                0,
                m_curMovePos.y - m_creature.GetPos().z);
            if (dir == Vector3.zero) 
                return;

            if (m_curMovePos.x == 0 || m_curMovePos.y == 0)
            {
                return;
            }
            m_creature.SetPos(m_curMovePos.x, m_curMovePos.y);

            StartRotate(dir, 0.2f);
            SendMasterMoveToServer(fDTime); // 移动中
        }

        private void StartRotate(Vector3 dir, float time)
        {
            Quaternion dest = Quaternion.LookRotation(dir);
            if (m_rotateCurQua == dest)
            {
                return;
            }
            m_rotateCurQua = m_creature.GetQua();
            m_bRotateing = true;
            m_rotateDestQua = dest;
            m_rotateTime = time;
            m_rotateCurTime = 0.0f;
        }

        private void _UpdateRotate(float fTime, float fDTime)
        {
            m_rotateCurTime += fDTime;
            float t = m_rotateCurTime/m_rotateTime;

            t = t >= 1.0f ? 1.0f : t;
            Quaternion rot = Quaternion.Slerp(m_rotateCurQua, m_rotateDestQua, t);
            m_creature.SetQua(rot);
            if (t >= 1.0f)
            {
                m_bRotateing = false;
            }
        }

        private void SendMasterMoveToServer(float dTime)
        {
            if (m_creature.IsMaster())
            {
                m_curSendTime += dTime;
                if (m_curSendTime >= m_sendMaxTime)
                {
                    m_curSendTime = 0.0f;
                  //  MsgScenePlayerMove.SendPosToServer(m_targetPos);
                }
            }
        }

        public override void Exit()
        {
            //Debug.Log("结束走路");
            m_creature.m_moveStateParam.m_bMoveing = false;
        }
    }
}