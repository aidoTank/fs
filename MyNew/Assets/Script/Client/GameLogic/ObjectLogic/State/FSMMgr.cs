using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    /// <summary>
    /// 主角的输入管理
    /// 状态机
    /// 任何动画都是通过命令进行执行，播放
    /// 输入也只是达到条件后执行命令
    /// </summary>
    public class FSMState
    {
        public FSMState(CCreature creature)
        {
            m_creature = creature;
        }
        // 状态进入时，处理的逻辑
        public virtual void Enter()
        {
        }

        public virtual void Update(float fTime, float fDTime)
        {
            // 选择目标
            _CheckEvent(fTime, fDTime);
        }

        public virtual void LateUpdate(float fTime, float fDTime)
        {
        }

        // 状态退出时，处理的逻辑
        public virtual void Exit()
        {
        }

        public StateID m_stateId =  StateID.IdleState;
        public CCreature m_creature = null;               // 逻辑对象类(属性)

        public delegate void UpdateEvent(float fTime, float deltaTime);
        public class TimeEvent
        {
            public float fBeginTime = 0.0f;
            public UpdateEvent fun = null;
            public bool bEnd = false;
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

        protected void _CheckEvent(float fTime, float fDeltaTime)
        {
            for (int i = 0; i < m_listTimeEvent.Count; i++)
            {
                TimeEvent eventObj = m_listTimeEvent[i];
                // 容器中的元素全部是按照开始时间排序的，所以当找到第一个时间不够的元素就不需要再继续循环了
                // 游戏时间 > 事件的起点时间
                if (fTime >= eventObj.fBeginTime)
                {
                    // 时间达到触发时间，查看是否已经触发过
                    if (false == eventObj.bEnd)
                    {
                        eventObj.fun(fTime, fDeltaTime);
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
    // 一个对象身上放一个状态机
    public class FSMMgr
    {
        //private CCreature m_creature;
        public Dictionary<int, FSMState> m_stateList = new Dictionary<int, FSMState>();
        public FSMState m_preState = null;
        public FSMState m_curState = null;

        public StateID m_prePreStateId = StateID.IdleState;

        public FSMMgr(CCreature creature)
        {
            //m_creature = creature;
            IdleState idle = new IdleState(creature);
            Add((int)StateID.IdleState, idle);
            Add((int)StateID.MoveState, new MoveState(creature));
            Add((int)StateID.JumpState, new Jump(creature));
            Add((int)StateID.DeadState, new Dead(creature));
            Add((int)StateID.AttackState, new AttackState(creature));
            Add((int)StateID.StrikeState, new StrikeState(creature));
            Add((int)StateID.StunState, new StunState(creature));
            Add((int)StateID.DuckState, new DuckState(creature));
            Add((int)StateID.NearState, new NearState(creature));
            Add((int)StateID.RotateState, new RotateState(creature));

            Add((int)StateID.BtMoveState, new BtMoveState(creature));
            Add((int)StateID.BtHitBackState, new BtHitBackState(creature));
            Add((int)StateID.BtHitFlyState, new BtHitFlyState(creature));
            Add((int)StateID.BtStrikeState, new BtStrikeState(creature));
            //Add((int)StateID.BtRotateState, new BtRotateState(creature));
            Add((int)StateID.BtJumpState, new BtJumpState(creature));
            Add((int)StateID.BtFlashState, new BtFlashState(creature));

            m_preState = idle;
            m_curState = idle;
            ChangeState((int)StateID.IdleState);
        }

        public void Update(float fTime, float fDTime)
        {
            //Profiler.BeginSample("FSM_Update");
            m_curState.Update(fTime, fDTime);
            //Profiler.EndSample();

        }

        public void Add(int stateId, FSMState state)  // 添加状态
        {
            if (!m_stateList.ContainsKey(stateId))
                m_stateList.Add(stateId, state);
        }
        public void Remove(int stateId)  // 移除状态
        {
            if (m_stateList.ContainsKey(stateId))
                m_stateList.Remove(stateId);
        }

        public void ChangeState(int stateId)
        {
            // 如果当前的不存在
            if (!m_stateList.ContainsKey(stateId))
            {
                return;
            }

            if (null != m_preState)
            {
                m_preState.Exit();
                m_prePreStateId = m_preState.m_stateId;
            }
            m_curState = m_stateList[stateId];
            m_curState.Enter();
            m_preState = m_curState;
        }

        public void SetPreState()
        {
            ChangeState((int)m_prePreStateId);
        }

        public StateID GetCurState()
        {
            return m_curState.m_stateId;
        }
    }
}