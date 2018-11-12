using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 帧同步游戏主类，负责最上层接口调用
    /// 1.连接帧服务器
    /// 2.创建房间
    /// 3.加入房间
    /// 4.选角色
    /// 5.点击准备，服务器都OK
    /// 6.同步玩家信息，开始游戏 > 加载场景
    /// 7.同步场景进度
    /// 8.场景加载完毕，进入游戏
    /// </summary>
    public class GameManager : Singleton
    {
        public static GameManager Inst;
        public GameManager() : base(true) { }

        public FspManager m_fspMgr;      // FSP管理器
        private int m_gameState; // 游戏状态

        private bool m_bRunning;
        private float m_sendProgressTime;  // 发送进度间隔
        private bool m_bSendLoaded;        // 是否发送加载结束

        public override void Init()
        {
            FspNetRunTime.Inst = new FspNetRunTime();

            CFrameTimeMgr.Inst = new CFrameTimeMgr();


            CreatureProp.Init();
        }
        /// <summary>
        /// 服务器收到所有人准备后，开始游戏
        /// </summary>
        public void Start(int[] playerData)
        {
            m_fspMgr = new FspManager();
            m_fspMgr.Init();

            Debug.Log("开始加载场景，开始汇报场景进度");

            SelectHeroModule selectHero = (SelectHeroModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelSelectHero);
            selectHero.SetVisible(false);

            MainModule mainModule = (MainModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelMain);
            mainModule.SetVisible(false);

            JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            js.SetVisible(true);
            HeadModule head = (HeadModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelHead);
            head.SetVisible(true);

            
            CMap map = CMapMgr.Create(2);
            map.Create();


            for(int i = 0; i < playerData.Length; i ++)
            {
             
                if(EGame.m_openid.Equals(playerData[i].ToString()))
                {
                    Debug.Log("客户端主角:" + EGame.m_openid);
                    CPlayer master = CPlayerMgr.CreateMaster(playerData[i]);
                    master.Create(playerData[i].ToString(), new Vector2(8, 8), Collide.GetVector(60));
                    master.UpdateUI();
                }
                else
                {
                    Debug.Log("客户端玩家:" + playerData[i]);
                    CPlayer master = CPlayerMgr.Create(playerData[i]);
                    master.Create(playerData[i].ToString(), new Vector2(8, 8), Collide.GetVector(60));
                }
            }

            for(int i = 0; i < 6; i ++)
            {
                CPlayer p1 = CPlayerMgr.Create(i);
                p1.Create("测试：" + i, new Vector2(4 + i * 5, 8), Collide.GetVector(60));
            }
    
            m_bRunning = true;
        }


        public void HandleLoadingProcess()
        {
            if (!m_bRunning)
                return;
            if (m_bSendLoaded)
                return;
            // 进度完成，直接再发送一次
            if (LogicSystem.Inst.GetMapLoadProcess().fPercent >= 1.0f)
            {
                m_bSendLoaded = true;
                m_sendProgressTime = 1;

                // FspMsgLoadProgress msgPro = (FspMsgLoadProgress)NetManager.Inst.GetMessage(eNetMessageID.FspMsgLoadProgress);
                // msgPro.m_progress = 1.0f;
                // FspNetRunTime.Inst.SendMessage(msgPro);

                FspMsgStartControl msg = (FspMsgStartControl)NetManager.Inst.GetMessage(eNetMessageID.FspMsgStartControl);
                FspNetRunTime.Inst.SendMessage(msg);
                return;
            }
            // 场景加载进度不用帧指令,间隔发送
            m_sendProgressTime += Time.deltaTime;
            if(m_sendProgressTime >= 1)
            {
                m_sendProgressTime = 0;
                FspMsgLoadProgress msg = (FspMsgLoadProgress)NetManager.Inst.GetMessage(eNetMessageID.FspMsgLoadProgress);
                msg.m_progress = LogicSystem.Inst.GetMapLoadProcess().fPercent;
                FspNetRunTime.Inst.SendMessage(msg);
            }
        }

        public void FixedUpdate()
        {
            FspNetRunTime.Inst.Update(0,0);
            if(m_fspMgr != null)
                m_fspMgr.FixedUpdate();

            HandleLoadingProcess();
        }

        public void AddFrameMsg(NetMessage msg)
        {
            if (m_fspMgr != null)
                m_fspMgr.AddServerFrameMsg(msg);
        }

        public override void Destroy()
        {
            if(FspNetRunTime.Inst != null)
            {
                FspNetRunTime.Inst.Destroy();
            }
        }

        public FspManager GetFspManager()
        {
            return m_fspMgr;
        }
    }
}
