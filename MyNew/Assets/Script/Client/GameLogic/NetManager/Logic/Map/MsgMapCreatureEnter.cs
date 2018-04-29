using System;
using ProtoBuf;
using UnityEngine;
namespace Roma
{

    public class MsgMapCreatureEnter : NetMessage
    {
        public MsgMapCreatureEnter()
            : base(eNetMessageID.MsgMapCreatureEnter)
        {

        }

        public static NetMessage CreateMessage()
        {
            return new MsgMapCreatureEnter();
        }

        public override void ToByte(ref LusuoStream ls)
        {
            eno = 0;
            SetByte<GC_PlayerPublicData>(playerData, ref ls);
        }

        public override void OnRecv()
        {
            if(eno == 0)
            {
                GC_PlayerPublicData data = GetData<GC_PlayerPublicData>(structBytes);
                //CPlayer otherPlayer = CPlayerMgr.Create(data.userName);
                //otherPlayer.publicData = data;
                //LogicSystem.Inst.Enter(otherPlayer, data.x, data.y, data.dir);

                Debug.Log("创建玩家：data.userName："+ data.userName + "  "+data.name);
            }
        }

        public GC_PlayerPublicData playerData = new GC_PlayerPublicData();
    }

}
