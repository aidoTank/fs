using UnityEngine;
namespace Roma
{
    public class StopMoveState : FSMState
    {
        public StopMoveState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.eFspStopMove;
        }

        public override void Enter(IFspCmdType cmd)
        {

        }

        public override void ExecuteFrame(int frameId)
        {

        }



        public override void Exit()
        {

        }


    }
}
