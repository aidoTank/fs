using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class DuckState : FSMState
    {
        public DuckState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.DuckState;
        }

        public override void Enter()
        {
            AnimationAction ride = new AnimationAction();
            ride.crossTime = AnimationInfo.m_crossTime;
            //ride.strFull = m_creature.m_duckAnimaParam;
            //ride.eMode = WrapMode.Loop;
            //CPlayerMgr.GetMaster().Play(ride);
        }


        public override void Exit()
        {

        }
    }
}