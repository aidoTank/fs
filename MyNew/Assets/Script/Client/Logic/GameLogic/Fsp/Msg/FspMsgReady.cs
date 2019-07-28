using System;
using ProtoBuf;
using Roma;
using UnityEngine;

public class FspMsgReady : NetMessage
{
    public FspMsgReady()
        : base(eNetMessageID.FspMsgReady)
    {
        //bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgReady();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<int>(m_heroIndex, ref ls);
    }

    public override void OnRecv()
    {
        if (eno == 0)
        {
            //int[] joinRoom = GetData<int[]>(structBytes);
            //Debug.Log("加入房间成功，切换界面选人界面 : room:" + joinRoom[0]);
            //Debug.Log("player id" + joinRoom[1]);
            //SelectHeroModule selectHero = (SelectHeroModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelSelectHero);
            //selectHero.OnRecvJoinInfo(joinRoom);
        }
    }
    public int m_heroIndex;
    //public CG_CreateRoom m_joinRoom = new CG_CreateRoom();
}

