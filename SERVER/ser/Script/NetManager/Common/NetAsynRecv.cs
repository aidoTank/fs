using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
namespace Roma
{
    public class NetAsynRecv
    {
        public Socket listenfd;
        public Conn[] conns;
        public int maxConn = 50;
        // 主定时器
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        public long heartBeatTime = 180;


        public NetAsynRecv(Socket socket, ref Conn[] conn)
        {
            listenfd = socket;
            conns = conn;
        }

        // 获取链接池索引，返回负数表示获取失败
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Init()
        {
            // 接受
            listenfd.BeginAccept(AcceptCb, null);
        }

        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.Write("[警告]链接池已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAdress();
                    Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                    conn.socket.BeginReceive(conn.readBuff,
                        conn.buffCount, conn.BuffRemain(),
                        SocketFlags.None, ReceiveCb, conn);
                }
                // 继续握手下一个客户端
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("接受客户端失败:" + e.Message);
            }
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            // 获取用户定义的数据
            Conn conn = (Conn)ar.AsyncState;
            lock (conn)
            {
                try
                {
                    int count = conn.socket.EndReceive(ar);
                    // 如果<=0，就是客户端关闭了
                    if (count <= 0)
                    {
                        Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接");
                        conn.Close();
                        return;
                    }
                    conn.buffCount += count;
                    // 处理数据
                    ProcessData(conn);
                    // 继续接受，起点是缓冲区的当前位置，大小是缓冲区的剩余位置，用于分包时的接受
                    if (conn.socket == null)   // 该conn客户端断开连接时，不在继续接受
                        return;
                    conn.socket.BeginReceive(conn.readBuff,
                        conn.buffCount, conn.BuffRemain(),
                        SocketFlags.None, ReceiveCb, conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine("收到异常消息 [" + conn.GetAdress() + "] 断开链接:" + e);
                    conn.Close();
                }
            }
        }
        // 粘包分包
        private void ProcessData(Conn conn)
        {
            if (conn.buffCount < sizeof(Int32))
                return;

            // 消息长度
            Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));    // 将缓冲区buff的消息复制到lenBytes中(只是复制长度的bytes)
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);    // 通过lenBytes获取这条消息的内容长度
            if (conn.buffCount < conn.msgLength + sizeof(Int32))        // 如果消息总长度 < 长度+内容长度
            {
                return;                                                 // （分包时的处理）被截断的为下一条消息，此时它的总长度肯定不是一个完整的消息长度，所有不处理，等下一次消息的接受
            }

            //处理消息，此时的readBuff是ok的
            HandleMsg(conn);

            // 处理剩余的消息，可能是粘包了，也可能是需要分包
            int count = conn.buffCount - sizeof(Int32) - conn.msgLength;     // 总长度 - 内容长度 - 长度 = 下一条消息长度
            Array.Copy(                                                      // 将剩余的内容移到缓冲区最开始的位置
                conn.readBuff,
                sizeof(Int32) + conn.msgLength,      // 源的开始位置
                conn.readBuff,
                0, count                             // 目标的开始位置和大小
                );
            conn.buffCount = count;
            if (conn.buffCount > 0)                   // 如果截取的之前的消息，还有剩余的
            {
                ProcessData(conn);
            }
        }

        private void HandleMsg(Conn conn)
        {
            LusuoStream stream = new LusuoStream(conn.readBuff);
            int contentLen = stream.ReadInt();
            ushort msgId = stream.ReadUShort();
            NetMessage msg = NetManager.Inst.GetMessage(msgId);
            if (msgId != (int)eNetMessageID.MsgHeartBeat)
            {
                Console.WriteLine("接受消息：" + (eNetMessageID)msgId);
            }
            msg.OnRecv(ref conn, contentLen, ref stream);
            //msg.OnRecv(ref conn);

            MessageCache cache;
            cache.conn = conn;
            cache.msg = msg;
            // 每次接受存起来，然后一阵一个
            msgList.Add(cache);
        }

        private void _HandleMsg()
        {
            for (int i = 0; i < msgList.Count; i++)
            {
                //Console.WriteLine("处理消息：" + msgList[i].msg + " num:" + msgList.Count);
                MessageCache msg = msgList[i];
                msgList.RemoveAt(i);
                msg.msg.OnRecv(ref msg.conn);
            }

            //LinkedListNode<MessageCache> cache = msgList.First;
            //if (cache == null)
            //    return;
            //Conn conn = cache.Value.conn;
            //cache.Value.msg.OnRecv(ref conn);
            //if (msgList.First != null)
            //    msgList.RemoveFirst();
        }

        public void Update()
        {
            _HandleMsg();
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


        private List<MessageCache> msgList = new List<MessageCache>();
        // 接受的消息队列，不能再异步回调的线程中操作模型
        // private Dictionary<Conn, NetMessage> m_msgList = new Dictionary<Conn, NetMessage>();


        //打印信息
        public void Print()
        {
            Console.WriteLine("===服务器登录信息===");
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;

                string str = "连接[" + conns[i].GetAdress() + "] ";
                if (conns[i].player != null)
                    str += "玩家id " + conns[i].player.id;

                Console.WriteLine(str);
            }
        }

    }
    public struct MessageCache
    {
        public Conn conn;
        public NetMessage msg;
    }

}