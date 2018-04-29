using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// ս���л���״̬
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
            Debug.Log("������Ч-------------------------------------------------------");
            m_curTime = 0;
        }

        public override void Update(float fTime, float fDTime)
        {
            //m_curTime += fDTime;
            //if (m_curTime > 0.6f)
            //{
            //    Debug.Log("�ص�������-------------------------------------------------------");
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