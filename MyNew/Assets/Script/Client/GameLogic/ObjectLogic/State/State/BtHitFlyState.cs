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
    public class BtHitFlyState : FSMState
    {

        private AnimationAction anima = new AnimationAction();

        public BtHitFlyState(CCreature obj)
            : base(obj)
        {
            m_stateId = StateID.BtHitFlyState;
        }

        public override void Enter()
        {
            OnHitBack();
        }

        private void OnHitBack()
        {
            Vector3 startPos = m_creature.GetPos();
            //Vector3 backDir = Quaternion.Euler(m_creature.GetDirection()) * Vector3.back;
            // 在当前角色的后方20米，上方10米
            //Vector3 endPos = startPos + new Vector3(0,10,0) + backDir.normalized * 20f;
            //m_creature.GetEntity().SetLineMove(startPos, 
            //    m_creature.m_hitFlyStateParam.m_btMoveEndPos,
            //    m_creature.m_hitFlyStateParam.m_btFlyTime,
            //    UITweener.Method.Linear, CurveMoveEnd);

            // 开始动画
            AnimationAction fly = new AnimationAction();
            fly.crossTime = AnimationInfo.m_crossTime;
            fly.strFull = AnimationInfo.m_animaFlying;
            fly.eMode = WrapMode.Loop;
            //m_creature.GetCCreture().Play(fly);
            m_creature.GetEntity().SetShadowActive(false);
        }

        private void CurveMoveEnd(object obj)
        {
            m_creature.PushCommand(StateID.IdleState);
            //if (null != m_creature.m_hitFlyStateParam.m_btMoveEnd)
            //{
            //    m_creature.m_hitFlyStateParam.m_btMoveEnd(m_creature);
            //    //m_creature.m_moveStateParam.m_moveEnd = null;
            //}
        }

        public override void Exit()
        {

        }
    }
}