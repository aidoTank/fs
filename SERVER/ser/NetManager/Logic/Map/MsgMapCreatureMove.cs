using System;
using ProtoBuf;
using UnityEngine;

public class MsgMapCreatureMove : NetMessage
{
    public MsgMapCreatureMove()
        : base(eNetMessageID.MsgMapCreatureMove)
    {

    }
    public static NetMessage CreateMessage()
    {
        return new MsgMapCreatureMove();
    }

    // 发送给其他人发送移动
    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<GC_MapCreatureMove>(moveInfo, ref ls);
    }

    // 接受客户端移动消息
    public override void OnRecv(ref Conn conn)
    {
        if (eno == 0)
        {
            //GC_MapCreatureMove moveInfo = GetData<GC_MapCreatureMove>(structBytes);
            //Map map = MapManager.Inst.GetMap(conn.player.publicData.mapId);
            //map.UpdateMove(conn.player.id, moveInfo);
        }
    }

    public GC_MapCreatureMove moveInfo;
}

