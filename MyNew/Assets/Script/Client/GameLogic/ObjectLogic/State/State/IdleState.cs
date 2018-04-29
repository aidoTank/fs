using UnityEngine;
namespace Roma
{
    public class IdleState : FSMState
    {
        public IdleState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.IdleState;
        }
        public override void Enter()
        {

            //if (m_creature.GetBuffType() == (int)star_def.BUFF_SUB_TYPE.BUFF_SUB_TYPE_ACCUMULATE)
            //{
            //    anima.strFull = AnimationInfo.m_animaXuli;
            //    anima.eMode = WrapMode.Loop;
            //}

                anima.eMode = WrapMode.Loop;

            anima.crossTime = AnimationInfo.m_crossTime;


                anima.strFull = AnimationInfo.m_animaStand;
            m_creature.Play(anima);
    
             m_clearRunDust = true;
        }
        public override void Update(float fTime, float fDTime)
        {
            //if (BattleControl.GetSingle().itfBattleType && !BattleControl.GetSingle().itfStartBattle && !m_creature.GetBoolDead())
            //{
            //    m_curTime += fDTime;
            //    if (m_curTime > m_maxTime)
            //    {
            //        anima.crossTime = AnimationInfo.m_crossTime;
            //        anima.strFull = AnimationInfo.m_animaStand1;
            //        anima.eMode = WrapMode.Once;
            //        anima.endEvent = Stand1End;
            //        if (null != m_creature.GetEntity())
            //            m_creature.GetEntity().Play(anima);
            //        m_curTime = 0;
            //        m_maxTime = Random.Range(8, 20);
            //    }
            //}
            if (m_clearRunDust)
            {
                m_curClearRunDustTime += fDTime;
                if (m_curClearRunDustTime > m_clearRunDustTime)
                {
                   // m_creature.StopDust();
                    m_clearRunDust = false;
                    m_curClearRunDustTime = 0f;
                }
            }
        }

        private void Stand1End(AnimationAction action)
        {
            Enter();
        }

        public override void Exit()
        {
           
        }

        private AnimationAction anima = new AnimationAction();

        private bool m_clearRunDust = true;
        private float m_clearRunDustTime = 0.3f;
        private float m_curClearRunDustTime;
    }
}
