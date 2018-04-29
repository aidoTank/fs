using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 战斗中击退状态
    /// </summary>
    public class BtHitBackState : FSMState
    {

        private AnimationAction anima = new AnimationAction();
        private bool m_startHitBackDelay = false;

        public BtHitBackState(CCreature obj)
            : base(obj)
        {
            m_stateId = StateID.BtHitBackState;
        }

        public override void Enter()
        {
            OnHitBack();
        }

        private void OnHitBack()
        {
            anima.crossTime = AnimationInfo.m_crossTime;
            anima.strFull = AnimationInfo.m_animaHit;
            anima.eMode = WrapMode.Once;
            anima.endEvent = OnHitBackEnd;
          //  m_creature.GetCCreture().Play(anima);

            Vector3 startPos = m_creature.GetPos();
            Vector3 backDir = Quaternion.Euler(m_creature.GetDirection()) * Vector3.back;
            Vector3 endPos = startPos + new Vector3(0, 0, 0) + backDir * 1.5f;
            //m_creature.GetCCreture().GetEntity().SetCurveMove(startPos, endPos, 0f, 0f, 0.1f, UITweener.Method.Linear, null);
            m_creature.m_moveStateParam.m_bMoveing = true;
        }

        private float m_cutTime = 0;
        

        public override void Update(float fTime, float fDTime)
        {
            if(m_startHitBackDelay)
            {
                m_cutTime += fDTime;
                if (m_cutTime >= m_creature.m_moveStateParam.m_hitBackDelayEndTime - 0.3f)
                {
                    OnStartBack();
                    m_startHitBackDelay = false;
                }
            }
        }

        /// <summary>
        /// 击退到后退结束,，开始回到原位，如果是被反击致死，不需要回到原位
        /// </summary>
        public void OnHitBackEnd(AnimationAction action)
        {
            m_startHitBackDelay = true;
            m_cutTime = 0;
            // 播放结束暂停，2秒后继续
            m_creature.GetEntity().Play(false);

            //OnStartBack();
        }

        private void OnStartBack()
        {
            //Debug.Log("受击击退后开始返回" + m_creature.m_name);
            //Vector3 startPos = m_creature.GetCCreture().GetPos();
            //Vector3 endPos = m_creature.m_moveStateParam.m_btMoveEndPos;
            //// 如果死亡，就慢慢走回去
            //bool dead = m_creature.GetBoolDead();
            //float speed = dead ? 8f : 12f;
            //float distance = Vector3.Distance(startPos, endPos);
            //float time = distance / speed;
            //m_creature.GetCCreture().GetEntity().SetCurveMove(startPos, endPos, 0f, 0f, time, UITweener.Method.Linear, OnHitBackReturnEnd);
            //m_creature.GetCCreture().SetDirection(Quaternion.LookRotation(endPos - startPos).eulerAngles);
            //m_creature.m_moveStateParam.m_bMoveing = true;

            //// 开始动画
            //AnimationAction fly = new AnimationAction();
            //fly.crossTime = AnimationInfo.m_crossTime;
            //fly.strFull = AnimationInfo.m_animRun;
            //fly.eMode = WrapMode.Loop;
            //fly.playSpeed = 0.7f;
            //fly.endEvent = null;
            //m_creature.GetCCreture().Play(fly);
        }

        private void OnHitBackReturnEnd(UITweener ui)
        {
            //m_creature.GetCCreture().PushCommand(StateID.IdleState);
            //// 被击退的表现都是坐骑，但是数据一定要记录在玩家身上
            //m_creature.SetHitBackType(false);
            //if (m_creature.GetBoolDead())
            //{
            //    m_creature.Dead();
            //}
            //if (null != m_creature.m_moveStateParam.m_moveEnd)
            //{
            //    m_creature.m_moveStateParam.m_moveEnd(m_creature);
            //    //m_creature.m_moveStateParam.m_moveEnd = null;
            //}
        }



        public override void Exit()
        {

        }
    }
}