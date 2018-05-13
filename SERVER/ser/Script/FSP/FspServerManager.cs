using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Roma
{
    
    public class FspServerManager : Singleton
    {

        public static FspServerManager Inst;

        public FspServerManager():base(true)
        { }

        private FspRoom m_room;

        //线程模块
        private Thread m_loopThread;
        public bool m_bRuning = true;

        private long m_logicLastTicks;
        /// <summary>
        /// 帧间隔...
        /// </summary>
        private long FRAME_TICK_INTERVAL = 666666;


        /// <summary>
        /// 帧服务器一开始就启动，只是暂时主要写一个房间，也在开始就创建吧(正式写法，由服务器创建房间)
        /// </summary>
        public override void Init()
        {
            m_room = new FspRoom(1);
            Console.WriteLine("帧服务器启动成功");

            m_logicLastTicks = DateTime.Now.Ticks;

            m_loopThread = new Thread(OnLoopThread)
            {
                IsBackground = true
            };
            m_loopThread.Start();
        }

        public FspRoom GetRoom(int id)
        {
            return m_room;
        }

        public void Close()
        {
            m_room.Close();
        }


        private void OnLoopThread()
        {
            while (m_bRuning)
            {
                try
                {
                    Update();
                }
                catch(Exception e)
                {
                    Console.WriteLine("帧心态异常：" + e.Message);
                    Thread.Sleep(10);
                }
            }
        }

        public void Update()
        {
            long nowticks = DateTime.Now.Ticks;
            long interval = nowticks - m_logicLastTicks;
       
            if (interval > FRAME_TICK_INTERVAL)
            {
                TimeSpan elapsedSpan = new TimeSpan(interval);
                m_logicLastTicks = nowticks - (nowticks % FRAME_TICK_INTERVAL);
                EnterFram();
            }
        }

        public void EnterFram()
        {
            //Console.WriteLine("帧心跳");
            // 网络的心跳在这里
            FspNetRunTime.Inst.EnterFrame();
            m_room.EnterFrame();
        }

    }
}
