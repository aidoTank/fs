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
        bFspMsg = true;
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
            FspFrame frameMsg = GetData<FspFrame>(structBytes);
            Debug.Log("接受帧消息：");

            for (int i = 0; i < frameMsg.vkeys.Count; i++)
            {
                HandleServerCmd(frameMsg.vkeys[i]);
            }

        }
    }


    private void HandleServerCmd(FspVKey cmd)
    {
        switch(cmd.vkey)
        {
            case FspVKeyType.LOAD_START:
                Debug.Log("开始加载场景");
                SelectHeroModule selectHero = (SelectHeroModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelSelectHero);
                selectHero.SetVisible(false);
                break;
        }
    }

    public FspFrame m_frameData = new FspFrame();
    public List<FspFrame> frameMsg = new List<FspFrame>();
}

