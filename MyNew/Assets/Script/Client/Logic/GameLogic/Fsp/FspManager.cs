using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 帧同步管理类，负责游戏开始后，消息缓存，逻辑处理，加速处理等
    /// </summary>
    public class FspManager
    {
        private FSPFrameController m_frameCtrl;
        private Dictionary<int, FspFrame> m_dicFrame = new Dictionary<int, FspFrame>();
        private int m_curFrameIndex;
        /// <summary>
        /// 客户端收到的最新帧
        /// </summary>
        private int m_clientNewFrameIndex;

        public bool m_bStartCtl;
        public void Init()
        {
            Time.fixedDeltaTime = FSPParam.clientFrameScTime;
            m_frameCtrl = new FSPFrameController();
            m_frameCtrl.Start();
        }

        public void Stop()
        {
            m_dicFrame.Clear();
            m_frameCtrl.Close();
        }

        public void FixedUpdate()
        {
            if(Client.Inst().isSingleTest)
            {
                ExecuteFrame(0, null);
                m_curFrameIndex++;
                return;
            }

            if(!m_bStartCtl)
                return;

            // 简化版的追帧
            //int speed = m_frameCtrl.m_NewestFrameId - m_curFrameIndex;
            // 差帧非常多时，8倍速度，减少同一帧的计算量
            //speed = speed > 100 ? 8 : speed;
               
            int speed = m_frameCtrl.GetFrameSpeed(m_curFrameIndex);
            //Debug.Log("speed:"+speed);
            while (speed > 0)
            {
                if(m_curFrameIndex < m_clientNewFrameIndex)
                {
                    m_curFrameIndex++;
                    //Debug.LogWarning("当前帧率：" + m_curFrameIndex + " max:" + m_clientNewFrameIndex);
                    FspFrame frameMsg;
                    if(m_dicFrame.TryGetValue(m_curFrameIndex, out frameMsg))
                    {
                        ExecuteFrame(m_curFrameIndex, frameMsg);
                    }
                }
                speed--;
            }
        }

        /// <summary>
        /// 添加FSP消息，问题：接受FSP消息时，是存储List<FspFrame> ，还是FspFrame
        /// 应该是List,正常情况一次只有一个帧消息，但是断线重连可以扩充为list
        /// </summary>
        public void AddServerFrameMsg(NetMessage msg)
        {
            FspMsgFrame frameMsg = msg as FspMsgFrame;
            if (frameMsg != null)
            {
                AddServerFrameUnit(frameMsg.m_frameData);
            }
        }

        // 添加一帧的指令列表
        public void AddServerFrameUnit(FspFrame frame)
        {
            if(frame.frameId <= 0)
            {
                ExecuteFrame(0, frame);
                return;
            }

            m_clientNewFrameIndex = frame.frameId;
            m_dicFrame.Add(frame.frameId, frame);
            // 添加最新帧
            m_frameCtrl.AddNewFrameId(frame.frameId);
        }

        // 正式的游戏逻辑帧
        private void ExecuteFrame(int frameId, FspFrame frameMsg)
        {
            if (frameMsg != null && frameMsg.vkeys != null)
            {
                for (int i = 0; i < frameMsg.vkeys.Count; i++)
                {
                    FspVKey cmd = frameMsg.vkeys[i];
                    // 根据keyid做不同逻辑处理
                    HandleServerCmd(cmd);
                }
            }
            
            CFrameTimeMgr.Inst.FixedUpdate();
            CCreatureMgr.ExecuteFrame(frameId);
            CSkillMgr.ExecuteFrame(frameId);
            CBuffMgr.ExecuteFrame(frameId);
            CBuffTriggerMgr.ExecuteFrame(frameId);
        }

        /// <summary>
        /// 消息内容转指令对象
        /// </summary>
        private void HandleServerCmd(FspVKey cmd)
        {
            CmdFspEnum type = (CmdFspEnum)cmd.vkey;
            uint uid = cmd.playerId;
            if(uid == 0)
                return;
            CCreature player = CCreatureMgr.Get(uid);
            IFspCmdType logicCmd = null;
            switch (type)
            {
                case CmdFspEnum.eFspStopMove:
                    //Debug.Log(uid + " 停止移动");
                    logicCmd = new CmdFspStopMove();
                break;
                case CmdFspEnum.eFspMove:
                    //Debug.Log(uid + " 客户端调用移动命令 " + cmd.args[0] + " " + cmd.args[1]);
                    Vector2d v = new Vector2d(cmd.args[0] * 0.01f, cmd.args[1] * 0.01f);
                    logicCmd = new CmdFspMove(ref v);
                break;
                case CmdFspEnum.eFspAutoMove:
                    Vector2d v1 = new Vector2d(cmd.args[0] * 0.01f, cmd.args[1] * 0.01f);
                    logicCmd = new CmdFspAutoMove(ref v1);
                    break;
                case CmdFspEnum.eFspSendSkill:
                    //Debug.Log(uid + " 客户端调用技能 " + cmd.args[0] + " " + cmd.args[1]);
                    
                    CmdFspSendSkill skill = new CmdFspSendSkill();
                    skill.m_casterUid = cmd.args[0];
                    skill.m_skillIndex = cmd.args[1];
                    skill.m_targetId = cmd.args[2];
                    Vector2d dir = new Vector2d(cmd.args[3] * 0.01f, cmd.args[4] * 0.01f);
                    Vector2d endPos = new Vector2d(cmd.args[5] * 0.01f, cmd.args[6] * 0.01f);
                    skill.m_dir = dir;
                    skill.m_endPos = endPos;
                    logicCmd = skill;
                break;
            }
            player.PushCommand(logicCmd);
        }

        public int GetCurFrameIndex()
        {
            return m_curFrameIndex;
        }

    }
}
