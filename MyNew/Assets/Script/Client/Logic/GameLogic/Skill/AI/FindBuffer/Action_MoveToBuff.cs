using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{


    public class Action_MoveToBuff_ : AIAction_
    {
        private int m_curTime;

        public override void Enter()
        {
            //m_curTime = 10000;
        }

        public override BtResult Execute()
        {
            int buffId = m_dataBase.GetData<int>((int)eAIParam.INT_BUFF_ID);
            CCreature buff = CCreatureMgr.Get(buffId);
            if (buff != null)
            {
                //float dis2 = Collide.GetDis2(buff.GetPos(), m_creature.GetPos());
                //if (dis2 < 1.0f)
                //{
                //    return BtResult.Ended;
                //}
                //else
                //{
                //    m_curTime += FSPParam.clientFrameMsTime;
                //    if (m_curTime >= 30 * FSPParam.clientFrameMsTime)
                //    {
                //        m_curTime = 0;
                //        Vector2 target = buff.GetPos();
                //        m_creature.GoTo(target);
                //    }
                //    return BtResult.Running;
                //}
            }
            else
            {
                m_curTime = 0;
                return BtResult.Ended;
            }
            return BtResult.Ended;
        }
    }
}
