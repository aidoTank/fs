using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
/// <summary>
/// 客户端的生物属性用Dictionary<CreatureProp, int>存储的好处
/// 1.支持服务器通过键值对的方式，单独更新单个属性
/// 2.支持客户端单独更新属性值时，单独调用显示前端显示
/// 3.支持其他模块通过key的方法获取属性值
/// </summary>
namespace Roma
{
    public class NetAsynRecv : NetAsynBase
    {
        public NetAsynRecv(Socket tgcp)
            : base(tgcp)
        {

        }

        public override void Start()
        {
            m_recvBuffer = new byte[m_uBufferSize];
            RecvMessage();
        }

        public override void Update()
        {
            _HandleMsg();
        }

        public void RecvMessage()
        {
            // 如果连接了就发送
            m_socket.BeginReceive(m_recvBuffer, buffCount,
                BUFFER_SIZE - buffCount, SocketFlags.None
                , ReceviceEnd, null);
        }

        private void ReceviceEnd(IAsyncResult ar)
        {
            int count = m_socket.EndReceive(ar);
            buffCount = buffCount + count; // 加上此次接受的大小
            ProcessData();

            m_socket.BeginReceive(m_recvBuffer, buffCount,
                 BUFFER_SIZE - buffCount, SocketFlags.None
                , ReceviceEnd, null);
        }

        private void ProcessData()
        {
            if (buffCount < sizeof(Int32))
                return;
            // 取出长度的字节，并获取内容长度
            Array.Copy(m_recvBuffer, lenBytes, sizeof(Int32));
            msgLength = BitConverter.ToInt32(lenBytes, 0);
            // 如果真实的数据长度 < 总长度 ，那么是被截断的数据，等待下一次取buff
            if (buffCount < msgLength + sizeof(Int32))
                return;
            // 开始解析消息
            HandleMsg();

            // 如果消息过长
            int count = buffCount - msgLength - sizeof(Int32);
            Array.Copy(m_recvBuffer, sizeof(Int32) + msgLength, m_recvBuffer, 0, count);
            buffCount = count;
            if(buffCount > 0)
            {
                ProcessData();
            }
        }

        private void HandleMsg()
        {
            LusuoStream stream = new LusuoStream(m_recvBuffer);
            int msgLen = stream.ReadInt();
            ushort msgId = stream.ReadUShort();
            Debug.Log("接受消息长度：" + msgLen);
            Debug.Log("接受消息ID：" + msgId);
            // 通过消息头，获取消息体
            //NetMessage msg = NetManager.Inst.GetMessage((eNetMessageID)msgId);
            //msg.OnRecv(msgLen, ref stream);
            ////msg.OnRecv();
            //m_msgList.Add(msg);


            // 如果id段大于1000，那么就是lua协议
            if(msgId > s_luaStartId)
            {
                LuaMsgStruct data;
                data.id = msgId;
                data.msgLen = msgLen;
                data.stream = stream;
                m_luaMsgList.Add(data);
            }
        }

        private const int s_luaStartId = 1000;

        private List<LuaMsgStruct> m_luaMsgList = new List<LuaMsgStruct>();
        private struct LuaMsgStruct
        {
            public int id;
            public int msgLen;
            public Roma.LusuoStream stream;
        }

        private void _HandleMsg()
        {
            for (int i = 0; i < m_msgList.Count; i ++)
            {
                m_msgList[i].OnRecv();
                m_msgList.RemoveAt(i);
            }

            for (int i = 0; i < m_luaMsgList.Count; i++)
            {
                LuaMsgStruct data = m_luaMsgList[i];
                CSharpCallLua.NetManager_OnRecv(data.id, data.msgLen, data.stream);
                m_luaMsgList.RemoveAt(i);
            }
        }


        public override void Stop()
        {

        }

        private const int BUFFER_SIZE = 1024;
        protected byte[] m_recvBuffer = null;
        private int buffCount = 0;
        // 粘包分包
        private int msgLength = 0;
        private byte[] lenBytes = new byte[sizeof(Int32)];

        // 接受的消息队列，不能再异步回调的线程中操作模型
        private List<NetMessage> m_msgList = new List<NetMessage>();
    }
}