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

        // 发送给其他人uid下线
        public override void ToByte(ref LusuoStream ls)
        {
            eno = 0;
            SetByte<long>(uid, ref ls);
        }

        // 接受客户端消息，这个人下线
        public override void OnRecv(ref Conn conn)
        {
            if (eno == 0)
            {
                long uid = GetData<long>(structBytes);
                if (conn.player.id == uid)
                {
                    conn.Close();
                }
            }
        }

        public long uid;
    }

}