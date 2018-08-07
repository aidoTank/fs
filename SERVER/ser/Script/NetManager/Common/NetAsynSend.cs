using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Roma
{
    public class NetAsynSend
    {
        //发送时的中间变量
        private LusuoStream m_stream = new LusuoStream(new byte[1024]);

        private List<MessageCache> msgList = new List<MessageCache>();
        private bool m_bSending = false;

        public NetAsynSend()
        {
        }

        public void Init()
        {

        }

        public void Send(Conn conn, NetMessage msg)
        {
            MessageCache cache;
            cache.conn = conn;
            cache.msg = msg;
            msgList.Add(cache);

            //m_stream.Reset();
            //msg.ToByte(ref m_stream);
            //try
            //{
            //    Console.WriteLine("发送消息:" + (eNetMessageID)msg.msgID);
            //    conn.socket.BeginSend(m_stream.GetBuffer(), 0, msg.msgMaxLen, SocketFlags.None, null, null);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("[错误]发送消息异常,id:" + (eNetMessageID)msg.msgID + " " + conn.GetAdress() + " : " + e.Message);
            //}
        }

        public void Update()
        {
            SendMessage();
        }

        /// <summary>
        /// 发送消息是发送前压包，再发送
        /// </summary>
        private void SendMessage()
        {
            // 一阵一个
            if (m_bSending)
                return;
            if (msgList.Count > 0)
            {
                MessageCache cache = msgList[0];
                Conn conn = cache.conn;
                NetMessage msg = cache.msg;
                m_stream.Reset();
                msg.ToByte(ref m_stream);
                try
                {
                    Console.WriteLine("发送消息:" + (eNetMessageID)msg.msgID);
                    conn.socket.BeginSend(m_stream.GetBuffer(), 0, msg.msgMaxLen, SocketFlags.None, SendedEnd, conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[错误]发送消息异常,id:" + (eNetMessageID)msg.msgID + " " + conn.GetAdress() + " : " + e.Message);
                }
                msgList.RemoveAt(0);
                m_bSending = true;
            } 
        }

        private void SendedEnd(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            conn.socket.EndSend(ar);
            m_bSending = false;
            ar = null;
            //Debug.Log("发送成功！");
        }


        public void Broadcast(ref Conn[] conns, NetMessage msg)
        {
            for (int i = 0; i < conns.Length; i++)
            {
                if (!conns[i].isUse)
                    continue;
                if (conns[i].player == null)
                    continue;
                Send(conns[i], msg);
            }
        }
    }
}

