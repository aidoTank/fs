﻿
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
        public void SetLogicState(IFspCmdType state)
        {
            m_logicState = state;
        }

        public IFspCmdType GetLogicState()
        {
            if (m_logicState == null)
                return CmdFspStopMove.Inst;
            return m_logicState;
        }

     

        /// <summary>
        /// 执行本地指令,push的指令，要更加当前情况，才能去设置玩家状态
        /// </summary>
        public override void PushCommand(IFspCmdType cmd)
        {
            if (IsDie())
                return;

            if (cmd.GetCmdType() == CmdFspEnum.eFspSendSkill)
            {
                m_cmdFspSendSkill = cmd as CmdFspSendSkill;
                //EnterSkill();
            }

            // 属于四种独立状态
            if (m_logicMoveEnabled && cmd.GetCmdType() == CmdFspEnum.eFspAutoMove)
            {
                //EnterAutoMove();
                m_vCreature.PushCommand(cmd);
            }
            base.PushCommand(cmd);

            if (m_logicMoveEnabled &&
                (cmd.GetCmdType() == CmdFspEnum.eFspAutoMove ||
                cmd.GetCmdType() == CmdFspEnum.eFspMove ||
                cmd.GetCmdType() == CmdFspEnum.eFspStopMove) ||
                cmd.GetCmdType() == CmdFspEnum.eFspRotation)
            {
                SetLogicState(cmd);
            }
        }

        public override void ExecuteFrame(int frameId)
        {
            //ExecuteFrameSkill();

            if (IsDie())
                return;

            // 独立状态
            if (m_logicMoveEnabled && m_logicState.GetCmdType() == CmdFspEnum.eFspAutoMove)
            {
                //TickAutoMove();
            }
            base.ExecuteFrame(frameId);

            //if (collider == null)
            //    return;
            //if (IsPlayer() || IsMonster() || IsPartner())
            //    collider.Update();
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



        public CmdFspMove m_cmdFspMove;
        public CmdFspSendSkill m_cmdFspSendSkill;

        // 属性
        public PlayerCsvData m_csv;
    }
}