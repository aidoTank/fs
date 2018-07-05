using System;
using System.Collections.Generic;


namespace Roma
{
    public class MatchInfo
    {
        public int roomId;
        public Player playerA;
        public Player playerB;
    }

    public class Lobby
    {
        public int m_id;
        private MapData m_mapData;
        private Dictionary<long, Player> m_dicPlayer = new Dictionary<long, Player>();
        // 匹配池
        private Dictionary<long, Player> m_dicMatch = new Dictionary<long, Player>();
        private List<MatchInfo> m_listMatch = new List<MatchInfo>();
        private int m_matchRoomId;

        public Lobby(int id, MapData data)
        {
            this.m_id = id;
            m_mapData = data;
        }

        public void AddPlayer(Player player)
        {
            if (m_dicPlayer.ContainsKey(player.id))
                return;
            m_dicPlayer[player.id] = player;

            Console.WriteLine(player.publicData.name + " 加入" + " 当前大厅人数" + m_dicPlayer.Count);
        }

        public void RemovePlayer(Player player)
        {
            if (!m_dicPlayer.ContainsKey(player.id))
                return;
            m_dicPlayer.Remove(player.id);

            Console.WriteLine(player.publicData.name + " 离开" + " 当前大厅人数" + m_dicPlayer.Count);
            // 如果玩家在匹配中，就移除
        }

        public Player GetPlayer(int uid)
        {
            return m_dicPlayer[uid];
        }

        public void Update(float time, float fTime)
        {
            bool isOk = true;
            foreach (KeyValuePair<long, Player> itemA in m_dicMatch)
            {
                foreach (KeyValuePair<long, Player> itemB in m_dicMatch)
                {
                    Player playerA = itemA.Value;
                    Player playerB = itemB.Value;
                    if (playerA.id == playerB.id &&
                        playerA.tempData.m_matchType == playerB.tempData.m_matchType)
                    {
                        MatchInfo info = new MatchInfo();
                        info.roomId = m_matchRoomId;
                        info.playerA = playerA;
                        info.playerB = playerB;
                        m_listMatch.Add(info);
                        isOk = true;
                        break;
                    }
                }
                if (isOk)
                    break;
            }

            for (int i = 0; i < m_listMatch.Count; i ++)
            {
                MatchInfo item = m_listMatch[i];
                Console.WriteLine("匹配成功 a:" + item.playerA.publicData.userName
                    + " b:" + item.playerB.publicData.userName);

                MsgStartMatch msg = (MsgStartMatch)NetManager.Inst.GetMessage(eNetMessageID.MsgStartMatch);
                msg.m_matchResult.roomId = 1;
                //msg.m_matchResult.matchType = 1;
                msg.m_matchResult.serverIp = "127.0.0.1";
                msg.m_matchResult.serverPort = 6001;
                item.playerA.Send(msg);
                item.playerB.Send(msg);

                m_dicMatch.Remove(item.playerA.id);
                m_dicMatch.Remove(item.playerB.id);
                m_listMatch.RemoveAt(i);
            }
        }

        public void OnJoinMatch(Player player)
        {
            if(!m_dicMatch.ContainsKey(player.id))
            {
                m_dicMatch.Add(player.id, player);
            }
        }

        /// <summary>
        /// 广播给当前玩家
        /// </summary>
        public void BroadcastByPlayer(long uid, NetMessage msg)
        {
            if (!m_dicPlayer.ContainsKey(uid))
                return;

            Player player = m_dicPlayer[uid];
            player.Send(msg);
        }

        /// <summary>
        /// 广播给当前地图所有人
        /// </summary>
        public void Broadcast(NetMessage msg)
        {
            foreach(KeyValuePair<long, Player> item in m_dicPlayer)
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

        public Lobby GetLobby(int id)
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

}