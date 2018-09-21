using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roma
{




    public class Player
    {
        public long id;
        public Conn conn;
        public GC_PlayerPublicData publicData;   // 玩家公共数据，消息结构体
        public TablePlayer data;    // 逻辑不用的，在创建玩家时，先从数据中取tabData给到PublicData，保存时PublicData转tabData
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
            Lobby map = LobbyManager.Inst.GetLobby(publicData.mapId);
            if (map != null)
                map.RemovePlayer(this);
            Console.WriteLine("有玩家下线：" + id);
            if (!DBPlayer.Inst.SavePlayer(this))
                return false;
            //下线
            conn.player = null;
            conn.Close();
            return true;
        }

        #region 帧同步的玩家接口
        private Queue<FspFrame> m_frameList = new Queue<FspFrame>();
        public void Send(FspFrame frame)
        {
            if (frame == null)
                return;

            if(!m_frameList.Contains(frame))
            {
                m_frameList.Enqueue(frame);
            }

            while(m_frameList.Count > 0)
            {
                FspFrame fsp = m_frameList.Peek();

                FspMsgFrame msg = (FspMsgFrame)NetManager.Inst.GetMessage(eNetMessageID.FspMsgFrame);
                msg.frameMsg = fsp;
                FspNetRunTime.Inst.Send(conn, msg);

                m_frameList.Dequeue();
            }
        }

        #endregion
    }
}

