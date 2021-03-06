﻿using System;
using System.Net;
using System.Net.Sockets;
namespace Roma
{
    public class FspNetRunTime : Singleton
    {
        public static FspNetRunTime Inst;
        public Socket listenfd;
        public Conn[] conns;
        public int maxConn = 50;

        public long heartBeatTime = 10;

        private NetAsynRecv recv;
        private NetAsynSend send;

        public FspNetRunTime()
            : base(true)
        {
        }

        public override void Init()
        {
            Start(GlobalConfig.FSP_SERVER_IP, GlobalConfig.FSP_SERVER_PORT);
        }

        public void Start(string host, int port)
        {
            // 初始化连接池
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            // 初始化socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);
            // 监听
            listenfd.Listen(maxConn);

            recv = new NetAsynRecv(listenfd, ref conns, eServerType.Fsp);
            recv.Init();
            send = new NetAsynSend(eServerType.Fsp);
            send.Init();
            Console.WriteLine("帧服务器网络初始化完毕");
        }

        public void Close()
        {
            for (int i = 0; i < conns.Length; i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                lock (conn)
                {
                    conn.Close();
                }
            }
        }

        public override void Update(long time)
        {
            //处理心跳
            //_HeartBeat();
            //recv.Update();
            //send.Update();
        }

        public void EnterFrame()
        {
            recv.Update();
            send.Update();
        }

        //心跳
        public void _HeartBeat()
        {
            long timeNow = Sys.GetTimeStamp();

            for (int i = 0; i < conns.Length; i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;

                //if (conn.lastTickTime < timeNow - heartBeatTime)
                //{
                //    Console.WriteLine("[心跳引起断开连接]" + conn.GetAdress());
                //    lock (conn)
                //        conn.Close();
                //}
            }
        }

        public void Send(Conn conn, NetMessage msg)
        {
            send.Send(conn, msg);
        }


        public void Broadcast(NetMessage msg)
        {
            send.Broadcast(ref conns, msg);
        }


        public override void UnInit()
        {
            listenfd.Close();
        }
    }

}