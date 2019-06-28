
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
    // 优化一下类的层级
    /// <summary>
    /// 角色的创建由管理器创建
    /// 销毁由自身死亡状态时，自己调用管理器销毁
    /// </summary>
    public partial class CCreature : CObject
    {
        public CCreature(long id)
            : base(id)
        {
            m_arrProp = new int[(int)eCreatureProp.Max];
        }


        public virtual bool Create(string name, Vector2 pos, Vector2 dir)
        {
            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            m_csv = playerCsv.GetData(1);

            SetPublicPropList();

            // 数据
            SetPos(pos);
            SetDir(dir);

            // 表现
            //m_vCreature = VObjectMgr.Create(eVOjectType.Creature);
            //sVOjectBaseInfo info = new sVOjectBaseInfo();
            //info.m_resId = m_csv.ModelResId;
            //info.m_pos = pos.ToVector3();
            //info.m_dir = dir.ToVector3();
            //info.m_headHeight = m_csv.headHeight;
            //m_vCreature.Create(info);

            UpdateVO_Create(m_csv.ModelResId, 5, eVOjectType.Creature);

            UpdateHeadName(name);
            UpdateHeadLv();
            UpdateHeadHp();
            return true;
        }

        public void UpdateHeadName(string sName)
        {
            CmdUIHead name = new CmdUIHead();
            name.type = 1;
            name.name = sName;
            m_vCreature.PushCommand(name);
        }

        public void UpdateHeadLv()
        {
            CmdUIHead lv = new CmdUIHead();
            lv.type = 2;
            lv.lv = GetPropNum(eCreatureProp.Lv);
            m_vCreature.PushCommand(lv);
        }

        public void UpdateHeadHp()
        {
            CmdUIHead hp = new CmdUIHead();
            hp.type = 3;
            hp.curHp = (int)(GetPropNum(eCreatureProp.CurHp) * 0.001f);
            hp.maxHp = (int)(GetPropNum(eCreatureProp.MaxHp) * 0.001f);
            m_vCreature.PushCommand(hp);

            if(GetPropNum(eCreatureProp.CurHp) <= 0)
            {
                CmdLife life = new CmdLife();
                life.state = false;
                m_vCreature.PushCommand(life);

                CFrameTimeMgr.Inst.RegisterEvent(m_csv.dieDelay, ()=>{
                    Destory();
                });
            }
        }

        public bool IsDie()
        {
            if(GetPropNum(eCreatureProp.CurHp) <= 0)
                return true;
            return false;
        }

    
        /// <summary>
        /// 指令对象转消息内容并发送
        /// </summary>
        /// <param name="cmd"></param>
        public void SendFspCmd(IFspCmdType cmd)
        {
            // 如果本地单机测试
            if (Client.Inst().isSingleTest)
            {
                PushCommand(cmd);
                return;
            }

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

        public void SetLogicState(CmdFspEnum state)
        {
            m_logicState = state;
        }

        public CmdFspEnum GetLogicState()
        {
            return m_logicState;
        }

        public void SetLogicEnabled(bool enabled)
        {
            m_logicCmdEnabled = enabled;
        }

        public bool GetLogicEnabled()
        {
            return m_logicCmdEnabled;
        }



        /// <summary>
        /// 执行本地指令,push的指令，要更加当前情况，才能去设置玩家状态
        /// </summary>
        public virtual void PushCommand(IFspCmdType cmd)
        {            
            if(!GetLogicEnabled())
            {
                return;
            }

            Debug.Log("==============" + cmd.GetCmdType());
            if(cmd.GetCmdType() == CmdFspEnum.eFspStopMove)
            {
                //EnterStopMove();
                m_vCreature.PushCommand(cmd);
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspMove)
            {
                m_cmdFspMove = cmd as CmdFspMove;
                EnterMove();
                m_vCreature.PushCommand(cmd);
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspSendSkill)
            {
                PushCommand(new CmdFspStopMove()); // 中止移动

                m_cmdFspSendSkill = cmd as CmdFspSendSkill;
                //Debug.Log("切换技能状态：");
                //EnterSkill();
                m_vCreature.PushCommand(cmd);
            }
            SetLogicState(cmd.GetCmdType());
        }

        public virtual void ExecuteFrame(int frameId)
        {
            if(!GetLogicEnabled())
            {
                return;
            }

            if(m_logicState == CmdFspEnum.eFspMove)
            {
                TickMove();
            }
            else if(m_logicState == CmdFspEnum.eFspStopMove)
            {
                //TickStopMove();
            }
            else if(m_logicState == CmdFspEnum.eFspSendSkill)
            {
                //TickSkill(frameId);
            }
        }
       
        public override void Destory()
        {
            m_destroy = true;
            if(m_vCreature != null)
            {
                m_vCreature.Destory();
                m_vCreature = null;
            }
        }


        // 逻辑状态数据
        public CmdFspEnum m_logicState;
        // 逻辑命令是否可用，比如游戏开始前，技能释放动作时，逻辑都是不可以的
        public bool m_logicCmdEnabled = true;

        public CmdFspMove m_cmdFspMove;
        public CmdFspSendSkill m_cmdFspSendSkill;

        // 属性
        public PlayerCsvData m_csv;
    }
}