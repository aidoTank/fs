using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace Roma
{
    
    public class FspServerManager : Singleton
    {

        public static FspServerManager Inst;

        public FspServerManager():base(true)
        { }

        //线程模块
        //private Thread m_loopThread;
        public bool m_bRuning = true;

        private long m_logicLastTicks;
        /// <summary>
        /// 帧间隔...
        /// </summary>
        private long FRAME_TICK_INTERVAL = 666666;


        private Timer timer = new Timer(32);

        /// <summary>
        /// 帧服务器一开始就启动，只是暂时主要写一个房间，也在开始就创建吧(正式写法，由服务器创建房间)
        /// </summary>
        public override void Init()
        {

            Console.WriteLine("帧服务器启动成功");

            m_logicLastTicks = DateTime.Now.Ticks;

            //m_loopThread = new Thread(OnLoopThread)
            //{
            //    IsBackground = true
            //};
            //m_loopThread.Start();


            // 定时器
            timer.Elapsed += new ElapsedEventHandler((sender, e) =>
            {
                //Update(Sys.GetTimeStamp());
                EnterFram();
            });
            timer.AutoReset = true;    // 一直自动执行
            timer.Enabled = true;
        }



        public void Close()
        {
          
        }


        private void OnLoopThread()
        {
            while (m_bRuning)
            {
                //try
                //{
                    Update();
               // }
               // catch(Exception e)
               // {
               //     Console.WriteLine("帧心态异常：" + e.Message);
                //    Thread.Sleep(10);
               // }
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

            // 后续会拆分为房间管理器
            _UpdateRoom();
        }


        private Dictionary<int, FspRoom> m_listRoom = new Dictionary<int, FspRoom>();

        public void CreateRoom(int id)
        {
            FspRoom room = new FspRoom(id);
            m_listRoom[id] = room;
        }

        public void _UpdateRoom()
        {
            foreach(KeyValuePair<int, FspRoom> item in m_listRoom)
            {
                item.Value.EnterFrame();
            }
        }

        public FspRoom GetRoom(int id)
        {
            return m_listRoom[id];
        }

        public void RemoveRoom(int id)
        {
            if(m_listRoom.ContainsKey(id))
            {
                m_listRoom[id] = null;
                m_listRoom.Remove(id);
                Console.WriteLine("移除房间:" + id);
            }
        }

    }
}
