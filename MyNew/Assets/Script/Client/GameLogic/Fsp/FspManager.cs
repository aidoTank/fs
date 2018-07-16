using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class FspManager
    {

        private Dictionary<int, FspFrame> m_dicFrame = new Dictionary<int, FspFrame>();
        private int m_curFrameIndex;
        /// <summary>
        /// 客户端收到的最新帧
        /// </summary>
        private int m_clientNewFrameIndex;

        public void Start()
        {
            Time.fixedDeltaTime = FSPParam.clientFrameScTime;
        }

        public void Stop()
        {
            m_dicFrame.Clear();
        }

        public void FixedUpdate()
        {
           if(m_curFrameIndex < m_clientNewFrameIndex)
           {
                m_curFrameIndex++;

                FspFrame frameMsg;
                if(m_dicFrame.TryGetValue(m_curFrameIndex, out frameMsg))
                {
                    ExecuteFrame(m_curFrameIndex, frameMsg);
                }
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
                //for(int i = 0; i < frameMsg.frameMsg.Count; i ++)
                //{
                //    AddServerFrameUnit(frameMsg.frameMsg[i]);
                //}
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
            // 场景帧心跳等
        }

        private void HandleServerCmd(FspVKey cmd)
        {
            switch (cmd.vkey)
            {
                case FspVKeyType.LOAD_START:
                    Debug.Log("开始加载场景，开始汇报场景进度");
                    SelectHeroModule selectHero = (SelectHeroModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelSelectHero);
                    selectHero.SetVisible(false);

                    MainModule mainModule = (MainModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelMain);
                    mainModule.SetVisible(false);

                    LogicSystem.Inst.LoadMap(1, () =>
                    {
                        CPlayer p = CPlayerMgr.CreateMaster(1);
                        p.InitConfigure();
                        p.SetPos(60, 27);
                    });
                    break;
                case FspVKeyType.CONTROL_START:
                    Debug.Log("开始控制");
                    break;
            }
        }
    }
}
