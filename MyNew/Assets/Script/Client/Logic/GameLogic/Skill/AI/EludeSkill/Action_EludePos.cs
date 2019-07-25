using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public class Action_EludePos_ : AIAction_
    {
        private int m_curTime;

        // 躲子弹要快，找BUFF不用立即触发
        public override void Enter()
        {
            base.Enter();
            m_curTime = 999999;
        }

        public override BtResult Execute()
        {
            //Vector2 moveToPos = m_dataBase.GetData<Vector2>((int)eAIParam.V3_MOVE_TO_POS);
            //float dis2 =  Collide.GetDis2(moveToPos, m_creature.GetPos());
            //if (dis2 < 0.5f * 0.5f)
            //{
            //    return BtResult.Ended;
            //}
            //else
            //{
            //    m_curTime += FSPParam.clientFrameMsTime;
            //    if (m_curTime >= 30 * FSPParam.clientFrameMsTime)
            //    {
            //        m_curTime = 0;

            //        Vector2 pos2d = moveToPos;
            //        // 取周围可以行走到的位置
            //        //CMapMgr.Get().FindEmptyPos(ref pos2d, 2);
            //        //m_creature.SendMoveMessageToTarget(ref pos2d);
            //        m_creature.GoTo(pos2d);
            //        //AIParam.OnMove(m_creature, ref pos2d);
            //    }
            //    return BtResult.Running;
            //}
            return BtResult.Running;
        }
    }
}


