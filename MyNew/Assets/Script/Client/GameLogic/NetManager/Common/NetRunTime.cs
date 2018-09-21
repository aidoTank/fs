using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace Roma
{
    public class NetRunTime : Singleton
    {
        public delegate void OnConnect();
        public OnConnect dgeconnet; 
        public NetRunTime():base(true)
        {

        }

        public override void Init()
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_send = new NetAsynSend(m_socket);
            m_recv = new NetAsynRecv(m_socket);
            m_send.Start();
        }

        public void ConServer(OnConnect onconnect)
        {
            if (m_netState == NetState.Disconnected)
            {
                Debug.Log("开始连接服务器");
                m_netState = NetState.Connecting;
                int port = 0;
                int.TryParse(GlobleConfig.s_gameServerPort, out port);
                IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(GlobleConfig.s_gameServerIP), port);
                m_socket.BeginConnect(serverIP, ConnSucc, null);
                dgeconnet = onconnect;
                lastTickTime = Time.time;
            }
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        private void ConnSucc(IAsyncResult ar)
        {
            Debug.Log("客户端本地：连接成功");
            m_socket.EndConnect(ar);
            m_netState = NetState.Connected;
            if (dgeconnet != null)
            {
                dgeconnet();
                dgeconnet = null;
            }
            m_recv.Start();
        }

        /// <summary>
        /// 连接失败,并弹出重连对话框
        /// </summary>
        public void ConFail()
        {
            // 断开网络会走这里。
            //Debug.Log("连接失败！！！！！！！！！！！！！！！！！！" + GlobleConfig.m_gameState);
            m_netState = NetState.Disconnected;

        }

        public override void Destroy()
        {
            Stop();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Stop()
        {
            if (m_socket != null)
            {
                m_send.Stop();
                m_recv.Stop();
                m_socket.Close();
                m_socket = null;
            }
            m_netState = NetState.Disconnected;
        }

        /// <summary>
        /// 重新连接服务器
        /// </summary>
        public void ReConServer(OnConnect connend = null)
        {
            Stop();
            ConServer(connend);
        }

        public void SendMessage(NetMessage msg)
        {
            // 如果没连接不处理
            if (m_netState != NetState.Connected)
            {
                Debug.Log("网络没连接，发送失败。ID:" + msg.msgID);
                return;
            }
            m_send.SendMessage(msg);
        }

        //public void SendMessage(LusuoStream stream)
        //{
        //    // 如果没连接不处理
        //    if (m_netState != NetState.Connected)
        //    {
        //        Debug.Log("网络没连接，发送失败:" + stream);
        //        return;
        //    }
        //    m_send.SendMessage(stream);
        //}

        public override void Update(float fTime, float fDTime)
        {
            UpdateState();
            m_send.Update();
            m_recv.Update();
            _HeartBeat();
        }

        // 状态改变
        public void UpdateState()
        {
         
        }

        public void _HeartBeat()
        {
            //心跳
            if (GetState() == NetState.Connected)
            {
                if (Time.time - lastTickTime > heartBeatTime)
                {
                    MsgHeartBeat hb = (MsgHeartBeat)NetManager.Inst.GetMessage(eNetMessageID.MsgHeartBeat);
                    //SendMessage(hb);
                    lastTickTime = Time.time;
                }
            }
        }


        public NetState GetState()
        {
            return m_netState;
        }

        static new public NetRunTime Inst = null;

        public enum NetState
        {
            Disconnected,
            Connecting,
            Connected,
            Queue,
        }
        // 当前网络状态 //
        protected NetState m_netState = NetState.Disconnected;

        protected Socket m_socket = null;

        private NetAsynSend m_send = null;
        private NetAsynRecv m_recv = null;

        private float lastTickTime;
        private float heartBeatTime = 8f;
    }
}