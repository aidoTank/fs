using System;
using ProtoBuf;
using Roma;
using UnityEngine;
public class FspMsgCreateRoom : NetMessage
{
    public FspMsgCreateRoom()
        : base(eNetMessageID.FspMsgCreateRoom)
    {
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgCreateRoom();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<CS_CreateRoom>(m_createRoom, ref ls);
    }

    public override void OnRecv()
    {
        if (eno == 0)
        {
            Debug.Log("创建房间成功：" + m_createRoom.roomId);
        }
    }

    public CS_CreateRoom m_createRoom = new CS_CreateRoom();
}

