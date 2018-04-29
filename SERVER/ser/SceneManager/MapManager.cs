using System;
using System.Collections.Generic;

public class Map
{
    public int id;
    private MapData mapData;
    private Dictionary<long, Player> dicPlayer = new Dictionary<long, Player>();


    private AOIScene aoiScene;
    public Map(int id, MapData data)
    {
        this.id = id;
        mapData = data;

        aoiScene = new AOIScene();
    }

    public void AddPlayer(Player player)
    {
        if (dicPlayer.ContainsKey(player.id))
            return;
        // 先将其他玩家同步给这个人
        //foreach (KeyValuePair<long, Player> item in dicPlayer)
        //{
        //    //Console.WriteLine("map:发送原有场景人物，地图：" + id + " 玩家：" + item.Value.publicData.name);
        //    MsgMapCreatureEnter otherEnter = (MsgMapCreatureEnter)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureEnter);
        //    otherEnter.playerData = item.Value.publicData;
        //    player.conn.Send(otherEnter);
        //}
        dicPlayer[player.id] = player;
        aoiScene.Add(player.aoiNode, player.publicData.x, player.publicData.y);

        // 新增这个玩家，同步给其他人
        //MsgMapCreatureEnter enter = (MsgMapCreatureEnter)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureEnter);
        //enter.playerData = player.publicData;
        //BroadcastByPlayer(player.id, enter);
    }

    public void RemovePlayer(long playerUid)
    {
        if (!dicPlayer.ContainsKey(playerUid))
            return;

        Player player = dicPlayer[playerUid];

        aoiScene.Leave(player.aoiNode);

        dicPlayer.Remove(playerUid);

        // 通知其他人这个玩家下线
        //MsgMapCreatureLeave enter = (MsgMapCreatureLeave)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureLeave);
        //enter.uid = playerUid;
        //Broadcast(enter);
    }

    public void UpdateMove(long uid, GC_MapCreatureMove moveInfo)
    {
        Player player;
        if (dicPlayer.TryGetValue(uid,out player))
        {
            player.publicData.x = moveInfo.x;
            player.publicData.y = moveInfo.y;
            player.publicData.dir = moveInfo.dir;
        }

        // 同步给其他人
        //MsgMapCreatureMove enter = (MsgMapCreatureMove)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureMove);
        //enter.moveInfo = moveInfo;
        //BroadcastByPlayer(uid, enter);
        aoiScene.Move(player.aoiNode, moveInfo.x, moveInfo.y);
    }


    //public void UpdateState(long uid, )
    //{

    //}


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
        List<AOINode> list = aoiScene.GetList(player.aoiNode);
        foreach (AOINode item in list)
        {
            Player p = item.obj as Player;
            if(p != null)
                p.Send(msg);
        }
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

