using System;
using ProtoBuf;
using Roma;
using UnityEngine;

public class FspMsgLoadProgress : NetMessage
{
    public FspMsgLoadProgress()
        : base(eNetMessageID.FspMsgLoadProgress)
    {
 
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgLoadProgress();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte(m_progress, ref ls);
    }

    public override void OnRecv()
    {
        if (eno == 0)
        {
            
        }
    }

    public float m_progress;
}

