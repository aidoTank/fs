using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{

    public partial class CPlayer : CCreature
    {
        public VCreature m_vCreature;


        public CPlayer(long id)
            : base(id)
        {
            m_type = EThingType.Player;
        }


        public override bool InitConfigure()
        {
            base.InitConfigure();

            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            PlayerCsvData data = playerCsv.GetData(1);

            // 测试，需建立表现层
            m_vCreature = new VCreature(data.ModelResId);
            return true;
        }

        /// <summary>
        /// 指令对象转消息内容
        /// </summary>
        /// <param name="cmd"></param>
        public void SendFspCmd(IFspCmdType cmd)
        {
            CmdFspEnum type = cmd.GetCmdType();
            FspMsgFrame msg = (FspMsgFrame)NetManager.Inst.GetMessage(eNetMessageID.FspMsgFrame);
            FspVKey key  = new FspVKey();
            key.vkey = (int)type;
            key.playerId = (uint)GetUid();
            switch(type)
            {
                case CmdFspEnum.eFspStopMove:
                    msg.m_frameData.vkeys.Add(key);
                    NetRunTime.Inst.SendMessage(msg);
                break;
                case CmdFspEnum.eFspMove:
                    CmdFspMove moveCmd = cmd as CmdFspMove;
                    key.args = new int[2];
                    key.args[0] = (int)(moveCmd.m_dir.x * 100);
                    key.args[1] = (int)(moveCmd.m_dir.y * 100);
                    msg.m_frameData.vkeys.Add(key);
                    NetRunTime.Inst.SendMessage(msg);
                break;
            }
        }

        public void HandleFspCmd(FspVKey vKey)
        {

        }
    }
}

