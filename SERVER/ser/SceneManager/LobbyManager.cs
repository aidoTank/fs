using System;
using System.Collections.Generic;

public class Map
{
    public int id;
    private MapData mapData;
    private Dictionary<long, Player> dicPlayer = new Dictionary<long, Player>();


    public Map(int id, MapData data)
    {
        this.id = id;
        mapData = data;
    }

    public void AddPlayer(Player player)
    {
        if (dicPlayer.ContainsKey(player.id))
            return;
        dicPlayer[player.id] = player;

        Console.WriteLine(player.publicData.name + " 加入" + " 当前房间人数" + dicPlayer.Count);
    }

    public void RemovePlayer(Player player)
    {
        if (!dicPlayer.ContainsKey(player.id))
            return;
        dicPlayer.Remove(player.id);

        Console.WriteLine(player.publicData.name + " 离开" + " 当前房间人数" + dicPlayer.Count);
    }

    public void UpdateMove(long uid, GC_MapCreatureMove moveInfo)
    {

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

public class MapManager : Singleton
{
    public MapManager() 
        : base(true)
    {
    }

    public override void Init()
    {
        MapCsv mapCsv = CsvManager.Inst.GetCsv<MapCsv>(eCSV.eMap);
        //MapData data = mapCsv.m_mapDataDic[3];

        foreach(KeyValuePair<int, MapData> item in mapCsv.m_mapDataDic)
        {
            Map map = new Map(item.Value.id, item.Value);
            dicMap[item.Value.id] = map;
        }
        Console.WriteLine("地图初始化完毕，数量："+ mapCsv.m_mapDataDic.Count);
    }

    public Map GetMap(int id)
    {
        Map map = null;
        if (dicMap.TryGetValue(id, out map))
        {
            return map;
        }
        return null;
    }

    public override void Update(long time)
    {
        foreach (KeyValuePair<int, Map> item in dicMap)
        {
            item.Value.Update(time, 0);
        }
    }

    public override void UnInit()
    {
       
    }

    public static MapManager Inst;
    private Dictionary<int, Map> dicMap = new Dictionary<int, Map>();
}

