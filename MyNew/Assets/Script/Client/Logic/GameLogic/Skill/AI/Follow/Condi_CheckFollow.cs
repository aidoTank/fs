
using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    /// <summary>
    /// 检查附近的玩家施法指令
    /// 躲避位置，垂直于施法方向
    /// </summary>
    public class Condi_CheckFollow : AICondi
    {

        public override void Activate(BtDatabase database)
        {
            base.Activate(database);
        }

        //public override bool Check()
        //{
        //    int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
        //    CCreature targetCC = CCreatureMgr.Get(targetUid);
        //    if (targetUid != 0 && targetCC != null && !targetCC.IsDie())
        //    {
        //        return false;
        //    }

        //    CCreature leader = CCreatureMgr.Get(m_creature.GetLeader());
        //    if (leader == null)
        //    {
        //        return false;
        //    }
        //    Vector2 tPos = leader.GetFollowPos((int)m_creature.GetUid());
        //    m_dataBase.SetData<Vector2>((int)eAIParam.V3_MOVE_TO_POS, tPos);
        //    return true;
        //}
       
    }
}
