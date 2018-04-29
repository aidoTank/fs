using System;
using ProtoBuf;
using UnityEngine;
namespace Roma
{

    public class MsgMapCreatureLeave : NetMessage
    {
        public MsgMapCreatureLeave()
            : base(eNetMessageID.MsgMapCreatureLeave)
        {

        }
        public static NetMessage CreateMessage()
        {
            return new MsgMapCreatureLeave();
        }
        // 用于发送离开
        public override void ToByte(ref LusuoStream ls)
        {
            eno = 0;
            //long uid = CPlayerMgr.GetMaster().publicData.userName;
            //SetByte<long>(uid, ref ls);
        }

        public override void OnRecv()
        {
            if (eno == 0)
            {
                long uid = GetData<long>(structBytes);
                //CPlayerMgr.Remove(uid, true);
            }
        }
    }
}
