using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Player
{
    public long id;
    public Conn conn;
    public Roma.GC_PlayerPublicData publicData;   // 玩家公共数据，消息结构体
    public TablePlayer data;                      // 玩家数据库数据
    public PlayerTempData tempData;

    //构造函数，给id和conn赋值
    public Player(long id, Conn conn)
    {
        this.id = id;
        this.conn = conn;
        tempData = new PlayerTempData();
    }

    /// <summary>
    /// 给当前玩家发送消息
    /// </summary>
    public void Send(NetMessage proto)
    {
        if (conn == null)
            return;
        NetRunTime.Inst.Send(conn, proto);
    }

    //踢下线
    public static bool KickOff(long id, NetMessage proto)
    {
        Conn[] conns = NetRunTime.Inst.conns;
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            if (conns[i].player == null)
                continue;
            if (conns[i].player.id == id)
            {
                lock (conns[i].player)
                {
                    if (proto != null)
                        conns[i].player.Send(proto);

                    return conns[i].player.Logout();
                }
            }
        }
        return true;
    }

    //下线
    public bool Logout()
    {
        //事件处理，稍后实现
        //ServNet.instance.handlePlayerEvent.OnLogout(this);
        Lobby map = LobbyManager.Inst.GetMap(publicData.mapId);
        if(map != null)
            map.RemovePlayer(this);
        Console.WriteLine("有玩家下线："+ id);
        if (!DBPlayer.Inst.SavePlayer(this))
            return false;
        //下线
        conn.player = null;
        conn.Close();
        return true;
    }
}

