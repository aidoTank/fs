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

        private CmdFspEnum m_logicState;
        private Vector2 m_curPos = Vector2.zero;
        private Vector2 m_curDir;
        private float m_moveSpeed = 0.4f;

        private PlayerCsvData m_csv;

        public CPlayer(long id)
            : base(id)
        {
            m_type = EThingType.Player;
        }

        public override bool InitConfigure()
        {
            base.InitConfigure();

            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            m_csv = playerCsv.GetData(1);

            // 测试，需建立表现层
            m_vCreature = new VCreature(m_csv.ModelResId);
            m_vCreature.m_bMaster = this is CMasterPlayer;
            return true;
        }

        /// <summary>
        /// 指令对象转消息内容并发送
        /// </summary>
        /// <param name="cmd"></param>
        public void SendFspCmd(IFspCmdType cmd)
        {
            // 如果本地指令，这里就直接执行指令
            //PushCommand(cmd);
            //return;
            CmdFspEnum type = cmd.GetCmdType();
            FspMsgFrame msg = (FspMsgFrame)NetManager.Inst.GetMessage(eNetMessageID.FspMsgFrame);
            FspVKey key  = new FspVKey();
            key.vkey = (int)type;
            key.playerId = (uint)GetUid();
            switch(type)
            {
                case CmdFspEnum.eFspStopMove:
                    msg.m_frameData.vkeys.Add(key);
                break;
                case CmdFspEnum.eFspMove:
                    CmdFspMove moveCmd = cmd as CmdFspMove;
                    key.args = new int[2];
                    key.args[0] = (int)(moveCmd.m_dir.x * 100);
                    key.args[1] = (int)(moveCmd.m_dir.y * 100);
                    msg.m_frameData.vkeys.Add(key);
                break;
            }
            FspNetRunTime.Inst.SendMessage(msg);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        public void PushCommand(IFspCmdType cmd)
        {
            m_logicState = cmd.GetCmdType();
            if(cmd.GetCmdType() == CmdFspEnum.eFspStopMove)
            {
                //StopMove();
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspMove)
            {
                CmdFspMove moveInfo = cmd as CmdFspMove;
                m_curDir = moveInfo.m_dir;
            }
            m_vCreature.PushCommand(cmd);
        }

        public override void ExecuteFrame()
        {
            if(m_logicState == CmdFspEnum.eFspMove)
            {
                TickMove();
            }
            else if(m_logicState == CmdFspEnum.eFspStopMove)
            {

            }
        }

        public void TickMove()
        {
            m_curPos = m_curPos + m_curDir * m_moveSpeed;
            m_vCreature.SetPos(m_curPos);
            m_vCreature.SetDir(m_curDir);
        }
    }
}

