using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

public class Conn
{
    public const int BUFFER_SIZE = 1024;
    public Socket socket;
    public bool isUse = false;

    public byte[] readBuff = new byte[BUFFER_SIZE];
    public int buffCount = 0;
    // 粘包分包
    public byte[] lenBytes = new byte[sizeof(UInt32)];
    public Int32 msgLength = 0;
    // 心跳时间
    public long lastTickTime = long.MinValue;
    public Player player;

    public Conn()
    {
        readBuff = new byte[BUFFER_SIZE];
    }

    public void Init(Socket socket)
    {
        this.socket = socket;
        isUse = true;
        buffCount = 0;
        lastTickTime = Sys.GetTimeStamp();
    }

    /// <summary>
    /// 剩余的BUFF
    /// </summary>
    /// <returns></returns>
    public int BuffRemain()
    {
        return BUFFER_SIZE - buffCount;
    }

    // 获取客户端地址
    public string GetAdress()
    {
        if (!isUse)
            return "无法获取地址";
        return socket.RemoteEndPoint.ToString();
    }

    public void Close()
    {
        if (!isUse)
            return;
        if (player != null)
        {
            // 玩家退出，稍后实现
            player.Logout();
            return;
        }
        Console.WriteLine("【断开连接】"+GetAdress());
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        socket = null;
        isUse = false;
    }

    //发送协议，相关内容稍后实现
    public void Send(NetMessage protocol)
    {
        NetRunTime.Inst.Send(this, protocol);
    }
}

