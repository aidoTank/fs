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
    public class BtJumpState : FSMState
    {

        private AnimationAction anima = new AnimationAction();

        public BtJumpState(CCreature obj)
            : base(obj)
        {
            m_stateId = StateID.BtJumpState;
        }

        public override void Enter()
        {
            m_creature.m_moveStateParam.m_bMoveing = true;
            OnHitBack();
        }

        private void OnHitBack()
        {
            Vector3 startPos = m_creature.GetPos();

           // JumpStateParam param = m_creature.m_jumpStateParam;
            //float dis = MathEx.FastDistance(ref startPos, ref param.m_jumpEndPos);
            //if (param.m_jumpRadian.x > dis)
            //{
            //    param.m_jumpRadian.x = dis;
            //}
            //if (param.m_jumpRadian.y > dis)
            //{
            //    param.m_jumpRadian.y = dis;
            //}
            //float time = dis / param.m_jumpSpeed;
            //m_creature.SetDirection();

            //Vector3 backDir = Quaternion.Euler(m_creature.GetDirection()) * Vector3.back;
            // 在当前角色的后方20米，上方10米
            //Vector3 endPos = startPos + new Vector3(0,10,0) + backDir.normalized * 20f;
            //m_creature.GetEntity().SetLineMove(startPos, 
            //    m_creature.m_jumpStateParam.m_btMoveEndPos,
            //    m_creature.m_jumpStateParam.m_btFlyTime,
            //    UITweener.Method.Linear, CurveMoveEnd);
            //m_creature.GetCCreture().GetEntity().SetCurveMove(startPos,
            //    param.m_jumpEndPos,
            //    param.m_jumpRadian.x,
            //    param.m_jumpRadian.y,
            //    time,
            //    UITweener.Method.EaseIn, CurveMoveEnd);

            // 开始动画
            AnimationAction fly = new AnimationAction();
            //fly.crossTime = AnimationInfo.m_crossTime;
            fly.strFull = AnimationInfo.m_animaJump;
            fly.eMode = WrapMode.Once;
            // 动作的本身时间 / 配置时间 = 速度
            //float animaTime = m_creature.GetEntity().GetAnimaClipTime(AnimationInfo.m_animaDodge);
            //fly.playSpeed = animaTime / time;

            //fly.endTime = m_creature.m_jumpStateParam.m_jumpTime;

           // m_creature.GetCCreture().Play(fly);
            m_creature.GetEntity().SetShadowActive(false);
        }

        private void CurveMoveEnd(object obj)
        {
            m_creature.m_moveStateParam.m_bMoveing = false;
            m_creature.PushCommand(StateID.IdleState);
            m_creature.GetEntity().SetShadowActive(true);
            //CCameraMgr.SetEnterBattleCameraShake(0.1f);
            //if (null != m_creature.m_jumpStateParam.m_jumpEnd)
            //{
            //    m_creature.m_jumpStateParam.m_jumpEnd(m_creature);
            //    //m_creature.m_moveStateParam.m_moveEnd = null;
            //}
        }

        public override void Exit()
        {

        }
    }
}