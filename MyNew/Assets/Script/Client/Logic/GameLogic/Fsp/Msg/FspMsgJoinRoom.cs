using System;
using ProtoBuf;
using Roma;
using UnityEngine;

public class FspMsgJoinRoom : NetMessage
{
    public FspMsgJoinRoom()
        : base(eNetMessageID.FspMsgJoinRoom)
    {
        //bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgJoinRoom();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<CG_CreateRoom>(m_joinRoom, ref ls);
    }

    public override void OnRecv()
    {
        if (eno == 0)
        {
            int[] joinRoom = GetData<int[]>(structBytes);
            Debug.Log("加入房间成功，切换界面选人界面 : room:" + joinRoom[0]);
            Debug.Log("player id" + joinRoom[1]);
            SelectHeroModule selectHero = (SelectHeroModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelSelectHero);
            selectHero.OnRecvJoinInfo(joinRoom);
            if(selectHero.IsShow())
            {
                selectHero.UpdateJoinInfo();
            }
            else
            {
                selectHero.SetVisible(true);
            }
        }
    }

    public CG_CreateRoom m_joinRoom = new CG_CreateRoom();
}

