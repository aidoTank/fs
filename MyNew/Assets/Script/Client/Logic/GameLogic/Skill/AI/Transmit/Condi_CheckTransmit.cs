
using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    /// <summary>
    /// 检查附近的玩家施法指令
    /// 躲避位置，垂直于施法方向
    /// </summary>
    public class Condi_CheckTransmit : AICondi
    {
        private float m_dis2 = 38 * 38;

        public override void Activate(BtDatabase database)
        {
            base.Activate(database);
        }

        //public override bool Check()
        //{
        //    CCreature leader = CCreatureMgr.Get(m_creature.GetLeader());
        //    if (leader == null)
        //    {
        //        return false;
        //    }
        //    if(Collide.GetDis2(m_creature.GetPos(), leader.GetPos()) > m_dis2)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
       
    }
}
