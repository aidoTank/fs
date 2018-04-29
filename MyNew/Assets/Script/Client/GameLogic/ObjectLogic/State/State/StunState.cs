using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class StunState : FSMState
    {
        public StunState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.StunState;
        }

        public override void Enter()
        {
            //m_creature.StopDust(); 

            anima.crossTime = AnimationInfo.m_crossTime;
            // 如果是抓宠，并且是宠物蛋就播放晕眩动作
            anima.strFull = AnimationInfo.m_animaYun;
            anima.eMode = WrapMode.Loop;
            m_creature.Play(anima);

            //CEffectMgr.Destroy(m_stunHandId);
            //m_stunHandId = CEffectMgr.Create(62, m_creature, "over_head01");
            m_curTime = 0;
           // m_maxTime = m_creature.m_stunStateParam.m_stunTime;
            //Debug.Log("生成时间：" + m_creature.m_stunStateParam.m_stunTime);
        }

        private void AnimaEnd(AnimationAction action)
        {
            // 播放待机动作
            m_creature.PushCommand(StateID.IdleState);
        }

        public override void Update(float fTime, float fDTime)
        {
            //if (!m_creature.IsMaster() && m_creature.GetHp() <= 0)
            //{
            //    //CEffectMgr.Destroy(m_stunHandId);
            //    m_creature.PushCommand(StateID.DeadState);
            //    return;
            //}
            //m_curTime += fDTime;
            //if (m_curTime > m_maxTime)
            //{
            //    m_creature.PushCommand(StateID.IdleState);
            //    m_curTime = 0;
            //}
        }

        public override void Exit()
        {
            //Debug.Log("销毁：" + m_stunHandId);
            //CEffectMgr.Destroy(m_stunHandId);
        }

        private float m_curTime = 0;
        private float m_maxTime = 1.4f;
        //private uint m_stunHandId = 0;
        AnimationAction anima = new AnimationAction();
    }
}