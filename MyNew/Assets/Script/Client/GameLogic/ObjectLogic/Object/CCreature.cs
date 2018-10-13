
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature : CThing
    {
        public CCreature(long id)
            : base(id)
        {
            m_arrProp = new int[(int)eCreatureProp.Max];
        }


        public virtual bool Create(Vector2 pos, Vector2 dir)
        {
            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            m_csv = playerCsv.GetData(1);

            SetPublicPropList();

            // 数据
            SetPos(pos);
            SetDir(dir);

            // 表现
            m_vCreature = VObjectMgr.Create(eVOjectType.Creature);
            m_vCreature.m_bMaster = this is CMasterPlayer;
            sVOjectBaseInfo info = new sVOjectBaseInfo();
            info.m_resId = m_csv.ModelResId;
            info.m_pos = pos.ToVector3();
            info.m_dir = dir.ToVector3();
            m_vCreature.Create(info);
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
                case CmdFspEnum.eFspSendSkill:
                    CmdFspSendSkill skill = cmd as CmdFspSendSkill;
                    key.args = new int[7];
                    key.args[0] = (int)skill.m_casterUid;
                    key.args[1] = (int)skill.m_skillId;
                    key.args[2] = (int)skill.m_targetId;

                    key.args[3] = (int)(skill.m_dir.x * 100);
                    key.args[4] = (int)(skill.m_dir.y * 100);

                    key.args[5] = (int)(skill.m_endPos.x * 100);
                    key.args[6] = (int)(skill.m_endPos.y * 100);

                    msg.m_frameData.vkeys.Add(key);
                break;
            }
            FspNetRunTime.Inst.SendMessage(msg);
        }

        /// <summary>
        /// 执行本地指令
        /// </summary>
        public virtual void PushCommand(IFspCmdType cmd)
        {
            m_logicState = cmd.GetCmdType();
            if(cmd.GetCmdType() == CmdFspEnum.eFspStopMove)
            {
                EnterStopMove();
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspMove)
            {
                m_cmdFspMove = cmd as CmdFspMove;
                EnterMove();
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspSendSkill)
            {
                m_cmdFspSendSkill = cmd as CmdFspSendSkill;
                //Debug.Log("切换技能状态：");
                EnterSkill();
            }
            m_vCreature.PushCommand(cmd);
        }

        public virtual void ExecuteFrame(int frameId)
        {
            if(m_logicState == CmdFspEnum.eFspMove)
            {
                TickMove();
            }
            else if(m_logicState == CmdFspEnum.eFspStopMove)
            {
                TickStopMove();
            }
            else if(m_logicState == CmdFspEnum.eFspSendSkill)
            {
                //TickSkill(frameId);
            }
        }


        public virtual void SetName(string name)
        {
           
        }

       
        public override void Destory()
        {
            if(m_vCreature != null)
            {
                m_vCreature.Destory();
                m_vCreature = null;
            }
        }


        // 逻辑状态数据
        public CmdFspEnum m_logicState;
        public CmdFspMove m_cmdFspMove;
        public CmdFspSendSkill m_cmdFspSendSkill;

        // 属性
        public PlayerCsvData m_csv;

        // 表现层
        public VObject m_vCreature;
    }
}