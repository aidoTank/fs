using UnityEngine;
namespace Roma
{
    public class BtStrikeState : FSMState
    {
        public BtStrikeState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.BtStrikeState;
        }

        public override void Enter()
        {
            Debug.Log("进入受击。。。。。");
            //anima.crossTime = AnimationInfo.m_crossTime;
            //anima.strFull = string.IsNullOrEmpty(m_creature.m_strikeStateParam.m_hitAnimaName) ? 
            //    AnimationInfo.m_animStrike : m_creature.m_strikeStateParam.m_hitAnimaName;
            anima.eMode = WrapMode.Once;
            anima.endEvent = AnimaEnd;
            //m_creature.GetCCreture().Play(anima);
        }

        public override void Exit()
        {
            Debug.Log("退出受击。。。。。。。。。");
        }
        
        private void AnimaEnd(AnimationAction action)
        {
            m_creature.PushCommand(StateID.IdleState);
        }

        AnimationAction anima = new AnimationAction();
    }
}