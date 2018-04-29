using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 战斗中击退状态
    /// </summary>
    public class BtFlashState : FSMState
    {
        private AnimationAction anima = new AnimationAction();

        public BtFlashState(CCreature obj)
            : base(obj)
        {
            m_stateId = StateID.BtFlashState;
        }

        public override void Enter()
        {
            CEffectMgr.Create(29, m_creature.GetPos(), Vector3.one, null);
            Debug.Log("播放特效-------------------------------------------------------");
            m_curTime = 0;
        }

        public override void Update(float fTime, float fDTime)
        {
            //m_curTime += fDTime;
            //if (m_curTime > 0.6f)
            //{
            //    Debug.Log("回到结束点-------------------------------------------------------");
            //    m_creature.SetPos(m_creature.m_jumpStateParam.m_jumpEndPos.x,
            //        m_creature.m_jumpStateParam.m_jumpEndPos.z);

            //    if (null != m_creature.m_jumpStateParam.m_jumpEnd)
            //    {
            //        m_creature.m_jumpStateParam.m_jumpEnd(m_creature);
            //    }
            //    else
            //    {
            //        m_creature.PushCommand(StateID.IdleState);
            //    }
            //}
        }

        public override void Exit()
        {

        }

        private float m_curTime;
    }
}