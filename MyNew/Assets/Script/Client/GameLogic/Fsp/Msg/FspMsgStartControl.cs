using System;
using ProtoBuf;
using Roma;
using UnityEngine;

public class FspMsgStartControl : NetMessage
{
    public FspMsgStartControl()
        : base(eNetMessageID.FspMsgStartControl)
    {
 
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgStartControl();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<int>(0, ref ls);
    }

    public override void OnRecv()
    {
        if (eno == 0)
        {
            Debug.Log("接受开始控制");
        }
    }
}

