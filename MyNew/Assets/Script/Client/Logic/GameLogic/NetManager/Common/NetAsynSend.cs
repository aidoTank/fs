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
            //SendMessageNew();
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
                m_bSending = true;
                NetMessage msg = m_listMsg[0];
                msg.ToByte(ref m_stream);
                m_socket.BeginSend(m_stream.GetBuffer(), 0, msg.msgMaxLen, SocketFlags.None, SendedEnd, null);
                m_listMsg.RemoveAt(0);
                //Debug.Log("最后发送消息：" + (eNetMessageID)msg.msgID);
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


        #region 优化为消息注册方式
        public void SendMessage<T>(int msgID, T t)
        {
            LusuoStream stream = new LusuoStream(new byte[m_uBufferSize]);

            stream.WriteInt(0);                   // 预留总字节数
            stream.WriteInt((int)msgID);          // 写消息编号
            byte[] bytes = ProtobufHelper.Serialize<T>(t);
            byte[] md5 = GetMd5Str(bytes);

            stream.Write(ref md5);
            stream.Write(ref bytes);              // 写具体结构体
            stream.Seek(0);
            // 内容字节数
            int contentLen = StringHelper.s_IntSize + md5.Length + bytes.Length;
            stream.WriteInt(contentLen);          // 再次写内容长度
            stream.m_byteLen = StringHelper.s_IntSize + contentLen; // 长度字节数 + 内容字节数

            _m_listMsg.Add(stream);
        }

        private void SendMessageNew()
        {
            // 一次发送一条
            if (_m_bSending)
                return;
            if (_m_listMsg.Count > 0)
            {
                _m_bSending = true;
                LusuoStream msg = _m_listMsg[0];
                m_socket.BeginSend(msg.GetBuffer(), 0, msg.m_byteLen, SocketFlags.None, _SendedEnd, null);
                _m_listMsg.RemoveAt(0);
                //Debug.LogWarning("最后发送消息：" + (eNetMessageID)msg.msgID);
            }
        }

        private void _SendedEnd(IAsyncResult ar)
        {
            m_socket.EndSend(ar);
            _m_bSending = false;
            //m_stream.Reset();
            ar = null;
            Debug.Log("发送成功！");
        }

        public byte[] GetMd5Str(byte[] ConvertString)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string value = System.BitConverter.ToString(md5.ComputeHash(ConvertString), 4, 8);
            value = value.Replace("-", "");
            value = value.ToLower();
            return System.Text.Encoding.UTF8.GetBytes(value);
        }

        protected List<LusuoStream> _m_listMsg = new List<LusuoStream>();
        protected bool _m_bSending = false;
        #endregion
    }
}