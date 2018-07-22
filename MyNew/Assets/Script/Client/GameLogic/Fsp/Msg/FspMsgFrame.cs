using System;
using ProtoBuf;
using Roma;
using System.Collections.Generic;
using UnityEngine;

public class FspMsgFrame : NetMessage
{
    public FspMsgFrame()
        : base(eNetMessageID.FspMsgFrame)
    {

    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgFrame();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<FspFrame>(m_frameData, ref ls);
    }

    public override void OnRecv()
    {
        if (eno == 0)
        {
            m_frameData = GetData<FspFrame>(structBytes);
            Debug.Log("接受帧消息：");

            //for (int i = 0; i < frameMsg.vkeys.Count; i++)
            //{
            //    HandleServerCmd(frameMsg.vkeys[i]);
            //}

        }
    }


    public FspFrame m_frameData = new FspFrame();
    //public List<FspFrame> frameMsg = new List<FspFrame>();
}

