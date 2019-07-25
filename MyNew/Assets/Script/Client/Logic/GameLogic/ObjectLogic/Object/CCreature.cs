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


        public virtual bool Create(string name, Vector2d pos, Vector2d dir)
        {
            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            m_csvData = playerCsv.GetData(1);

            SetPublicPropList();

            // 数据
            SetPos(pos);
            SetDir(dir);

            // 碰撞
            collider = new Circle();
            collider.isObstacle = false;
            collider.c = pos;
            collider.r = GetR();
            if (IsMaster() || IsPlayer() || IsNpc())
            {
                collider.notPush = true;
            }
            else if (IsMonster())
            {
                collider.notPush = false;
            }
            PhysicsManager.Inst.Add(collider);
            collider.m_updatePosEvent = (newPos, newDir) => {

                SetPos(newPos);
                SetSpeed(GetSpeed());

                if (m_vCreature != null)
                {
                    m_vCreature.m_bMoveing = true;
                    m_vCreature.SetBarrier(true);
                }
            };

            UpdateVO_Create(m_csvData.ModelResId, 5, eVOjectType.Creature);
            UpdateVO_ShowHeadName(name);
            UpdateVO_ShowHeadLv();
            UpdateVO_ShowHeadHp();
            return true;
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
                EnterSkill();
            }

            // 属于四种独立状态
            if (m_logicMoveEnabled && cmd.GetCmdType() == CmdFspEnum.eFspAutoMove)
            {
                EnterAutoMove();
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

            if (m_ai != null)
            {
              
                    m_ai.EnterFrame();
                
            }
            ExecuteFrameSkill();
            if (IsDie())
                return;

            // 独立状态
            if (m_logicMoveEnabled && m_logicState.GetCmdType() == CmdFspEnum.eFspAutoMove)
            {
                TickAutoMove();
            }
            base.ExecuteFrame(frameId);

            if (collider == null)
                return;
            if (IsPlayer() || IsMonster())
                collider.Update();
        }

        public override void SetPos(Vector2d pos, bool bTeleport = false)
        {
            base.SetPos(pos, bTeleport);
            if (collider != null)
            {
                collider.c = pos;
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


        public void StartAi(bool bRun)
        {
            if (m_ai == null)
            {
                m_ai = new CCreatureAI(this, eAILevel.HARD);
            }

            // 取消AI时，如果之前开启了AI，则停止按下技能
            if (!bRun && m_ai.IsRun())
            {
               // DestoryDownUpSkill();
            }
            m_ai.SetRun(bRun);

           // UpdateUI_AutoAi(bRun);
        }

        // 属性
   
        public CreatureCsvData m_csvData;

        public bool m_bActive = true; // 非主角时，是否处于激活状态
        private Circle collider;

        public CCreatureAI m_ai;
    }
}