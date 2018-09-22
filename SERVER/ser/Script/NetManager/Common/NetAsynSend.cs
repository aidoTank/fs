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
        private List<MessageCache> m_msgList = new List<MessageCache>();
        private eServerType m_serverType = eServerType.Lobby;

        public NetAsynSend(eServerType type)
        {
            m_serverType = type;
        }

        public void Init()
        {

        }

        public void Send(Conn conn, NetMessage msg)
        {
            MessageCache cache;
            cache.conn = conn;
            cache.msg = msg;
            m_msgList.Add(cache);
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
            for (int i = 0; i < m_msgList.Count; i++)
            {
                MessageCache cache = m_msgList[i];
                Conn conn = cache.conn;
                if (conn != null && conn.socket != null && conn.socket.Connected)
                {
                    NetMessage msg = cache.msg;
                    m_stream.Reset();
                    msg.ToByte(ref m_stream);
                    try
                    {
                        if (msg.msgID != 200)
                        {
                            Console.WriteLine(m_serverType + ":发送消息:" + (eNetMessageID)msg.msgID);
                        }
                        conn.socket.BeginSend(m_stream.GetBuffer(), 0, msg.msgMaxLen, SocketFlags.None, null, conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(m_serverType + ":[错误]发送消息异常,id:" + (eNetMessageID)msg.msgID + " " + conn.GetAdress() + " : " + e.Message);
                    }
                }
            }
            m_msgList.Clear();
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

