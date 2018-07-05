using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma
{
    public class GameManager : Singleton
    {
        public static GameManager Inst;
        public GameManager() : base(true) { }

        private FspManager m_fspMgr;      // FSP管理器

        public override void Init()
        {
            FspNetRunTime.Inst = new FspNetRunTime();
            FspNetRunTime.Inst.Init();
        }

        /// <summary>
        /// 接受玩家信息，表示帧同步开始运行
        /// </summary>
        public void Start()
        {
            // 启动帧同步逻辑
            m_fspMgr = new FspManager();
            m_fspMgr.Start();
        }

        public void FixedUpdate()
        {
            FspNetRunTime.Inst.Update(0,0);

            m_fspMgr.FixedUpdate();
        }
    }
}
