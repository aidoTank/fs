//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//namespace Roma
//{
//    public class BtRotateState : FSMState
//    {
//        public BtRotateState(CCreature go)
//            : base(go)
//        {
//            m_stateId = StateID.BtRotateState;
//        }

//        public override void Enter()
//        {
//            anima.crossTime = 0.2f;
//            anima.strFull = AnimationInfo.m_animRun;
//            anima.playSpeed = 0.5f;
//            CCreature petCC = m_creature.GetCCreture();
//            if (petCC != null)
//            {
//                petCC.Play(anima);
//            }

//            GameObject obj = m_creature.GetRideEntity().GetObject();
//            Vector3 dir = m_creature.GetDirection();
//            TweenRotation rota = TweenRotation.Get(obj);
//            rota.duration = 0.15f;
//            rota.from = dir;
//            rota.to = new Vector3(dir.x, m_creature.m_moveStateParam.m_btEndDir.y, dir.z);
//            rota.onFinished = onFinished;
//            rota.Reset();
//            rota.Play(true);


//            Debug.Log("start:" + dir + " end:" +rota.to);
//        }

//        private void onFinished(UITweener ui)
//        {
//            m_creature.PushCommand(StateID.IdleState);
//            if (null != m_creature.m_moveStateParam.m_rotaEnd)
//            {
//                m_creature.m_moveStateParam.m_rotaEnd(m_creature);
//                m_creature.m_moveStateParam.m_rotaEnd = null;
//            }
//        }


//        public override void Exit()
//        {
           
//        }


//        private AnimationAction anima = new AnimationAction();
//    }
//}
