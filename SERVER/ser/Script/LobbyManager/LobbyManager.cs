using System;
using System.Collections.Generic;

public class Lobby
{
    public int id;
    private MapData mapData;
    private Dictionary<long, Player> dicPlayer = new Dictionary<long, Player>();
    //private List<>

    public Lobby(int id, MapData data)
    {
        this.id = id;
        mapData = data;
    }

    public void AddPlayer(Player player)
    {
        if (dicPlayer.ContainsKey(player.id))
            return;
        dicPlayer[player.id] = player;

        Console.WriteLine(player.publicData.name + " 加入" + " 当前大厅人数" + dicPlayer.Count);
    }

    public void RemovePlayer(Player player)
    {
        if (!dicPlayer.ContainsKey(player.id))
            return;
        dicPlayer.Remove(player.id);

        Console.WriteLine(player.publicData.name + " 离开" + " 当前大厅人数" + dicPlayer.Count);
    }

    public void Update(float time, float fTime)
    {

    }

    /// <summary>
    /// 广播给当前玩家的周围
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="msg"></param>
    public void BroadcastByPlayer(long uid, NetMessage msg)
    {
        if (!dicPlayer.ContainsKey(uid))
            return;

        Player player = dicPlayer[uid];
        player.Send(msg);
    }

    /// <summary>
    /// 广播给当前地图所有人
    /// </summary>
    public void Broadcast(NetMessage msg)
    {
        foreach(KeyValuePair<long, Player> item in dicPlayer)
        {
            item.Value.Send(msg);
        }
    }
}

public class LobbyManager : Singleton
{
    public LobbyManager() 
        : base(true)
    {
    }

    public override void Init()
    {
        MapCsv mapCsv = CsvManager.Inst.GetCsv<MapCsv>(eCSV.eMap);
        //MapData data = mapCsv.m_mapDataDic[3];

        Lobby map = new Lobby(3, null);
        dicLobby[3] = map;

        Console.WriteLine("大厅初始化完毕");
    }

    public Lobby GetMap(int id)
    {
        Lobby map = null;
        if (dicLobby.TryGetValue(id, out map))
        {
            return map;
        }
        return null;
    }

    public override void Update(long time)
    {
        foreach (KeyValuePair<int, Lobby> item in dicLobby)
        {
            item.Value.Update(time, 0);
        }
    }

    public override void UnInit()
    {
       
    }

    public static LobbyManager Inst;
    private Dictionary<int, Lobby> dicLobby = new Dictionary<int, Lobby>();
}

