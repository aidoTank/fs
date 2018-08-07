using System;
using ProtoBuf;
using Roma;

public class FspMsgLoadProgress : NetMessage
{
    public FspMsgLoadProgress()
        : base(eNetMessageID.FspMsgLoadProgress)
    {
        bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgLoadProgress();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<float[]>(m_sendProgress, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        // 接受玩家的加入房间信息，
        if (eno == 0)
        {
            float p = GetData<float>(structBytes);
            conn.player.tempData.m_loadProgress = p;
            Console.WriteLine("接受玩家进度：" + p);

        }
    }

    public float[] m_sendProgress; // 临时为玩家进度

    
}

