using System;
using ProtoBuf;

namespace Roma
{

    public class MsgExit : NetMessage
    {
        public MsgExit()
            : base(eNetMessageID.MsgExit)
        {

        }
        public static NetMessage CreateMessage()
        {
            return new MsgExit();
        }

        public override void ToByte(ref LusuoStream ls)
        {
            //SetByte<Roma.GC_PlayerPublicData>(playerData, ref ls);
            SetByte<int>(0, ref ls);
        }

        public override void OnRecv()
        {

        }
    }
}

