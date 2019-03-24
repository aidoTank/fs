using System;
using System.Collections.Generic;
using System.Net.Sockets;


namespace Roma
{
    public class NetAsynBase
    {
        public NetAsynBase(Socket socket)
        {
            m_socket = socket;
        }

        public virtual void Start()
        {
            // 填入IP等，进行连接初始化
        }

        public virtual void Update()
        {

        }

        public virtual void Stop()
        {

        }

        protected Socket m_socket = null;
        protected List<NetMessage> m_listMsg = new List<NetMessage>();
        protected const int m_uBufferSize = 1024;
    }
}