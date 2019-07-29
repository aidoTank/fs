
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 角色的创建由管理器创建
    /// 销毁由自身死亡状态时，自己调用管理器销毁
    /// 
    /// 1.角色层有必要使用状态机管理待机，移动，自动寻路等比较常用状态，通过切换状态来调用相关心跳，避免类的臃肿
    /// 2.如果是角色常住的功能，可以采用组件式模式，比如属性信息，技能信息，BUFF信息，装备等等，避免类的臃肿
    /// </summary>
    public partial class CCreature : CObject
    {
        public CCreature(long id)
            : base(id)
        {
            m_arrProp = new int[(int)eCreatureProp.Max];

            m_stateMgr = new FSMMgr(this);
        }


        public virtual bool Create(int csvId, string name, Vector2d pos, Vector2d dir)
        {
            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            m_csvData = playerCsv.GetData(csvId);

            InitPublicPropList();

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
                //SetDir(newDir);
                //SetSpeed(GetSpeed());

                if (m_vCreature != null)
                {
                    m_vCreature.SetMove(true);
                    m_vCreature.SetBarrier(true);
                }
            };

            UpdateVO_Create(m_csvData.ModelResId, m_csvData.HeadHeight, eVOjectType.Creature);
            UpdateVO_ShowHead(true);
            UpdateVO_ShowHeadName(name);
            UpdateVO_ShowHeadLv();
            UpdateVO_ShowHeadHp();
            return true;
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
                case CmdFspEnum.eFspAutoMove:
                    CmdFspAutoMove amoveCmd = cmd as CmdFspAutoMove;
                    key.args = new int[2];
                    key.args[0] = (int)(amoveCmd.m_pos.x * 100);
                    key.args[1] = (int)(amoveCmd.m_pos.y * 100);
                    Debug.Log("send :" + key.args[0] + "  "+ key.args[1]);
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

            if(m_logicMoveEnabled && 
                cmd.GetCmdType() == CmdFspEnum.eFspStopMove ||
                cmd.GetCmdType() == CmdFspEnum.eFspMove ||
                cmd.GetCmdType() == CmdFspEnum.eFspAutoMove)
            {
                m_stateMgr.ChangeState((int)cmd.GetCmdType(), cmd);
                m_vCreature.PushCommand(cmd);
            }

            // 属于四种独立状态
            //if (m_logicMoveEnabled && !m_bMovePathing && cmd.GetCmdType() == CmdFspEnum.eFspAutoMove)
            //{
            //    m_cmdFspAutoMove = cmd as CmdFspAutoMove;
            //    EnterAutoMove();
            //    m_vCreature.PushCommand(cmd);
            //}
            //base.PushCommand(cmd);

            //if (m_logicMoveEnabled &&
            //    (cmd.GetCmdType() == CmdFspEnum.eFspAutoMove ||
            //    cmd.GetCmdType() == CmdFspEnum.eFspMove ||
            //    cmd.GetCmdType() == CmdFspEnum.eFspStopMove) ||
            //    cmd.GetCmdType() == CmdFspEnum.eFspRotation)
            //{
            //    SetLogicState(cmd);
            //}
        }

        public override void ExecuteFrame(int frameId)
        {
            if (m_ai != null)
            {
                m_ai.EnterFrame();
            }
            ExecuteFrameSkill();
            if (IsDie())
                return;

            // 独立状态
            //if (m_logicMoveEnabled && m_logicState.GetCmdType() == CmdFspEnum.eFspAutoMove)
            //{
            //    TickAutoMove();
            //}
            //base.ExecuteFrame(frameId);

            m_stateMgr.ExecuteFrame(frameId);

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

        /// <summary>
        /// 复活，用于角色死亡刷新，BUFF刷新
        /// </summary>
        public virtual void OnRevive()
        {
            if (collider != null)
                collider.active = true;

            if (IsMaster())
            {
                m_bornPoint = SceneManager.Inst.GetSceneData().bornPoint.ToVector2();
                CFrameTimeMgr.Inst.RegisterEvent(2800, () =>
                {
                    m_logicMoveEnabled = true;
                    m_logicSkillEnabled = true;
                    PushCommand(CmdFspStopMove.Inst);
                });
            }
            else
            {
                m_logicMoveEnabled = true;
                m_logicSkillEnabled = true;
                PushCommand(CmdFspStopMove.Inst);
            }

            SetPropNum(eCreatureProp.CurHp, GetPropNum(eCreatureProp.Hp));
            //SetPos(m_bornPoint.ToVector2d(), true);
            SetState(eBuffState.Show, true);
            UpdateVO_ShowLife(true);
            UpdateVO_ShowHead(true);
            UpdateVO_ShowFootHalo();
        }

        /// <summary>
        /// 保持统一，死亡复活时不对表现层进行销毁和创建
        /// </summary>
        public virtual void OnDie()
        {
            if (IsMaster())
            {
                StartAi(false);
            }
            DestoryDownUpSkill();
            if (m_curSkill != null)
                m_curSkill.Destory();
            ClearBuff();
            ClearTrigger();
            //StopAutoMove();
            PushCommand(CmdFspStopMove.Inst);
            if (collider != null)
                collider.active = false;

            // 如果是坐骑
            //if (GetMaster() != null)
            //{
            //    GetMaster().DownRide();
            //}
            //// 如果是主人
            //if (GetRide() != null)
            //{
            //    DownRide();
            //}
            m_logicMoveEnabled = false;
            m_logicSkillEnabled = false;
      
            // 逻辑层死亡
            CFrameTimeMgr.Inst.RegisterEvent(m_csvData.dieDelay, () =>
            {

                CFrameTimeMgr.Inst.RegisterEvent(m_refreshTime, () =>
                {
                    OnRevive();
                });
                SetState(eBuffState.Show, false);
            });
            // 播放动作
            UpdateVO_ShowLife(false);
            UpdateVO_ShowHead(false);
        }

        // 切场景时才会真正销毁，也要移除事件监听
        public override void Destory()
        {
            // 当前可能运行的技能，BUFF，触发器
            DestoryDownUpSkill();
            if (m_curSkill != null)
                m_curSkill.Destory();
            ClearBuff();
            ClearTrigger();


            if (m_arrProp != null)
                m_arrProp = null;
            if (m_buffList != null)
            {
                m_buffList.Clear();
                m_buffList = null;
            }
            if (m_dicSkill != null)
            {
                // 清除技能，附带的被动BUFF
                foreach (KeyValuePair<int, CSkillInfo> item in m_dicSkill)
                {
                    if (item.Value != null)
                        item.Value.Destoty();
                }
                m_dicSkill.Clear();
                m_dicSkill = null;
            }
            if (m_listTrigger != null)
            {
                m_listTrigger.Clear();
                m_listTrigger = null;
            }
 
            if (m_ai != null)
            {
                m_ai.Destroy();
                m_ai = null;
            }
            //if (m_aoiNode != null)
            //{
            //    AOIMgr.Inst.Leave(m_aoiNode);
            //    m_aoiNode = null;
            //}
            PhysicsManager.Inst.Remove(collider);
            collider = null;
            base.Destory();
        }

        public bool IsDie()
        {
            if (GetPropNum(eCreatureProp.CurHp) <= 0)
                return true;
            return false;
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
        public string m_name = "null";
        public CreatureCsvData m_csvData;
        public Vector2 m_bornPoint;
        public int m_refreshTime;

        public bool m_bActive = true; // 非主角时，是否处于激活状态
        private Circle collider;

        public CCreatureAI m_ai;
        public eAIType m_aiType = eAIType.Client;
    }
}