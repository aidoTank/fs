using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class StrikeState : FSMState
    {
        public StrikeState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.StrikeState;
        }
        public override void Enter()
        {
            Debug.Log("播放受击动作。。。。。。。。。。。。。");
            anima.crossTime = AnimationInfo.m_crossTime;
            anima.strFull = AnimationInfo.m_animStrike;
            anima.eMode = WrapMode.Once;
            anima.endEvent = AnimaEnd;

                if (null != m_creature.GetEntity())
                    m_creature.Play(anima);
      
        }
        public override void Update(float fTime, float fDTime)
        {
            // 开始倒计时，多久消失
            //m_curTime += Time.deltaTime;
            //if (m_curTime > m_maxTime)
            //{
            //    m_creature.PushCommand(StateID.IdleState);
            //}
        }
        public override void Exit()
        {
            //Debug.Log("结束Strike");
        }
        
        private void AnimaEnd(AnimationAction action)
        {
            m_creature.PushCommand(StateID.IdleState);
        }

        AnimationAction anima = new AnimationAction();

        //private float m_curTime = 0;
        //private float m_maxTime = 0.5f;
    }
}