using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public enum StateID
    {
        eNone,
        eFspStopMove,       // 停止移动    (可用于状态机)
        eFspMove,           // 移动        (可用于状态机)
        eFspAutoMove,       // 自动移动    (可用于状态机)
        eFspRotation,       // 转向        (可用于状态机)
    }

    public class FSMState
    {
        public FSMState(CCreature creature)
        {
            m_creature = creature;
        }
 
        public virtual void Enter(IFspCmdType cmd)
        {
        }

        public virtual void ExecuteFrame(int frameId)
        {

        }

        public virtual void Exit()
        {
        }

        public StateID m_stateId = StateID.eNone;
        public CCreature m_creature = null;               // 逻辑对象类(属性)
    }

    public class FSMMgr
    {
        public Dictionary<int, FSMState> m_stateList = new Dictionary<int, FSMState>();
        public FSMState m_preState = null;
        public FSMState m_curState = null;

        public StateID m_prePreStateId = StateID.eNone;

        public FSMMgr(CCreature creature)
        {
            StopMoveState stop = new StopMoveState(creature);
            MoveState move = new MoveState(creature);
            AutoMoveState autoMove = new AutoMoveState(creature);
            Add((int)StateID.eFspStopMove, stop);
            Add((int)StateID.eFspMove, move);
            Add((int)StateID.eFspAutoMove, autoMove);

            ChangeState((int)StateID.eFspStopMove, CmdFspStopMove.Inst);
        }

        public void ExecuteFrame(int frameId)
        {
            m_curState.ExecuteFrame(frameId);
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

        public void ChangeState(int stateId, IFspCmdType cmd)
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
            m_curState.Enter(cmd);
            m_preState = m_curState;
        }

        //public void SetPreState()
        //{
        //    ChangeState((int)m_prePreStateId);
        //}

        public StateID GetCurState()
        {
            return m_curState.m_stateId;
        }
    }
}