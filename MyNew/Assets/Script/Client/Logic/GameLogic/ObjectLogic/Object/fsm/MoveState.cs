using UnityEngine;
namespace Roma
{
    public class MoveState : FSMState
    {
        public CmdFspMove m_cmdFspMove;

        public MoveState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.eFspMove;
        }
        public override void Enter(IFspCmdType cmd)
        {
            m_cmdFspMove = cmd as CmdFspMove;
        }

        public override void ExecuteFrame(int frameId)
        {
            Vector2d moveDir = m_cmdFspMove.m_dir.normalized;
            FixedPoint delta = new FixedPoint(FSPParam.clientFrameScTime) * m_creature.GetSpeed();
            Vector2d nextPos = m_creature.m_curPos + moveDir * delta;

            m_creature.SetPos(nextPos);
            m_creature.SetDir(moveDir);  // 技能前摇时，移动时，模型表现方向失效，比如机枪移动时射击
            m_creature.SetSpeed(m_creature.GetSpeed());

            //Debug.Log("无障碍移动");
            m_creature.m_vCreature.SetBarrier(false);
        }



        public override void Exit()
        {

        }


    }
}
