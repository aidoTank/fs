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

        private FspManager m_fspMgr;      // FSP管理器
        private int m_gameState; // 游戏状态

        public override void Init()
        {
            FspNetRunTime.Inst = new FspNetRunTime();
            FspNetRunTime.Inst.Init();


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

            LogicSystem.Inst.LoadMap(1, () =>
            {
                CPlayer p = CPlayerMgr.CreateMaster(1);
                p.InitConfigure();
                p.SetPos(60, 27);
            });
        }

        public void FixedUpdate()
        {
            FspNetRunTime.Inst.Update(0,0);
            m_fspMgr.FixedUpdate();
        }

        public void AddFrameMsg(NetMessage msg)
        {
            m_fspMgr.AddServerFrameMsg(msg);
        }
    }
}
