using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace Roma
{
    public class NetAsynSend : NetAsynBase
    {
        public NetAsynSend(Socket socekt)
            : base(socekt)
        {

        }

        public override void Start()
        {
            // 填入IP等，进行连接初始化
            m_stream = new LusuoStream(new byte[m_uBufferSize]);
        }

        public void SendMessage(NetMessage msg)
        {
            m_listMsg.Add(msg);
            Debug.Log("发送消息：" + (eNetMessageID)msg.msgID);
            //m_stream.Reset();
            //msg.ToByte(ref m_stream);
            //m_socket.BeginSend(m_stream.GetBuffer(), 0, msg.msgMaxLen, SocketFlags.None, null, null);
        }

        //public void SendMessage(LusuoStream stream)
        //{
        //    m_socket.BeginSend(stream.GetBuffer(), 0, stream.m_byteLen, SocketFlags.None, null, null);
        //}


        // 发送消息要单独在心跳中发送
        // 帧同步时，可能需在一帧发送很多消息，待处理
        public override void Update()
        {
            SendMessage();
        }

        /// <summary>
        /// 发送消息是发送前压包，再发送
        /// </summary>
        private void SendMessage()
        {
            // 一次发送一条
            if (m_bSending)
                return;
            if(m_listMsg.Count > 0)
            {
                NetMessage msg = m_listMsg[0];
                msg.ToByte(ref m_stream);
                m_socket.BeginSend(m_stream.GetBuffer(), 0, msg.msgMaxLen, SocketFlags.None, SendedEnd, null);
                m_listMsg.RemoveAt(0);
                m_bSending = true;
            }
        }

        private void SendedEnd(IAsyncResult ar)
        {
            m_socket.EndSend(ar);
            m_bSending = false;
            m_stream.Reset();
            ar = null;
            //Debug.Log("发送成功！");
        }

        public override void Stop()
        {

        }

        private LusuoStream m_stream;
        protected bool m_bSending = false;
    }
}