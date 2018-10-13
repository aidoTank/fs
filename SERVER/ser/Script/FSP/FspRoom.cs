using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Roma
{
    public enum FspGameState
    {
        None,
        /// <summary>
        /// 游戏创建状态，此时能加入玩家
        /// 所以玩家加入后，进入选人状态
        /// </summary>
        Create,
        /// <summary>
        /// 选人状态
        /// 1.所有玩家选完角色
        /// 2.选人时间结束，进入加载状态
        /// </summary>
        SelectRole,
        /// <summary>
        /// 加载状态
        /// 所有玩家加载完成时，才进入控制状态
        /// </summary>
        StartLoad,
        /// <summary>
        /// 开始游戏状态
        /// </summary>
        StartControl,
        /// <summary>
        /// 游戏结束状态，给所有玩家瞎放结束
        /// </summary>
        End,
    }

    /// <summary>
    /// 一个房间就是一个战场
    /// 房间分为选人阶段 游戏开始阶段,都在房间内处理
    /// </summary>
    public class FspRoom
    {
        public int m_id;
        public List<Player> m_listPlayer = new List<Player>();

        private FspGameState m_state = FspGameState.None;
        private int m_curFrameId;
        private FspFrame m_lockedFrame = new FspFrame();


        public FspRoom(int id)
        {
            m_id = id;
            Console.WriteLine("创建房间：" + id);
            m_state = FspGameState.Create;
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        public void AddPlayer(Player player)
        {
            // 如果包含了
            if(m_listPlayer.Contains(player))
            {
                Console.WriteLine("AddPlayer重复的玩家，新的替换旧的");
                m_listPlayer.Remove(player);
            }

            if(m_listPlayer.Count >= 2)
            {
                Console.WriteLine("AddPlayer达到上限了");
            }
            player.tempData.m_roomId = m_id;
            player.tempData.bReady = false;
            player.tempData.bLoaded = false;
            m_listPlayer.Add(player);
            Console.WriteLine("加入房间：" + player.id);
            // 通知当前所在玩家。更新房间的UI显示
            FspMsgJoinRoom msg = (FspMsgJoinRoom)NetManager.Inst.GetMessage(eNetMessageID.FspMsgJoinRoom);
            //msg.m_enterInfo[0] = m_id;
            // 玩家列表
            for(int i = 0;i < m_listPlayer.Count;  i ++)
            {
                msg.m_enterInfo[i] = (int)m_listPlayer[i].id;
                FspNetRunTime.Inst.Send(m_listPlayer[i].conn, msg);
            }
        }

        public void RemovePlayer(Player player)
        {
            if (m_listPlayer.Contains(player))
            {
                Console.WriteLine("RemovePlayer:" + player.id);
                m_listPlayer.Remove(player);
            }
            if(m_listPlayer.Count == 0)
            {
                FspServerManager.Inst.RemoveRoom(m_id);
            }
        }
        
        /// <summary>
        /// 开始的游戏
        /// </summary>
        public void StartGame()
        {

        }

        /// <summary>
        /// 接受客户端帧消息，转发给所有客户端
        /// </summary>
        public void OnRecvClient(ref Conn conn, FspFrame frames)
        {
            for(int i = 0; i < frames.vkeys.Count; i ++)
            {
                HandleClientCmd(conn.player, frames.vkeys[i]);
            }
        }

        private void HandleClientCmd(Player player, FspVKey cmd)
        {
            cmd.playerId = (uint)player.id;
            m_lockedFrame.vkeys.Add(cmd);
        }

        public void EnterFrame()
        {
            if (m_listPlayer.Count == 0)
                return;

            HandleState();

            // 每一帧的消息发给各客户端
            if(m_lockedFrame.frameId != 0 || !m_lockedFrame.IsEmpty())
            {
                for(int i = 0; i < m_listPlayer.Count; i ++)
                {
                    Player player = m_listPlayer[i];
                    if(m_lockedFrame.vkeys.Count > 0 && m_lockedFrame.vkeys[0].args!= null && m_lockedFrame.vkeys[0].args.Length > 0)
                    {
                        Console.WriteLine("发送指令：" + m_lockedFrame.vkeys[0].ToString());
                    }
                    player.Send(m_lockedFrame);
                }
            }

            // 游戏控制时，帧号才会增加，其实进入房间就算开始了
            if(m_state == FspGameState.StartControl)
            {
                m_curFrameId++;
                //Console.WriteLine("当前帧率：" + m_curFrameId);
                m_lockedFrame = new FspFrame();
                m_lockedFrame.frameId = m_curFrameId;
            }
        }

        private void HandleState()
        {
            switch (m_state)
            {
                case FspGameState.None:
                    // 清空所有数据
                    break;
                case FspGameState.Create:
                    m_state = FspGameState.SelectRole;
                    break;
                case FspGameState.SelectRole:
                    // 时间到，或者都准备了，开始下一个状态
                    foreach (Player item in m_listPlayer)
                    {
                        if (!item.tempData.bReady)
                            return;
                    }
                    // 组合要发送的玩家信息
                    int[] sendData = new int[m_listPlayer.Count];
                    for (int i = 0; i < m_listPlayer.Count; i ++)
                    {
                        sendData[i] = (int)m_listPlayer[i].id;
                    }
                    // 通知客户端开始加载游戏
                    foreach (Player item in m_listPlayer)
                    {
                        FspMsgPlayerData msg = (FspMsgPlayerData)NetManager.Inst.GetMessage(eNetMessageID.FspMsgPlayerData);
                        msg.m_sendData = sendData;
                        FspNetRunTime.Inst.Send(item.conn, msg);
                    }
                    m_state = FspGameState.StartLoad;
                    break;
                case FspGameState.StartLoad:
                    if (m_listPlayer.Count == 0)
                        return;
                    // 临时组合要发送的玩家进度
                    float[] progress = new float[m_listPlayer.Count];
                    for (int i = 0; i < m_listPlayer.Count; i++)
                    {
                        progress[i] = (int)m_listPlayer[i].tempData.m_loadProgress;
                    }
                    foreach (Player item in m_listPlayer)
                    {
                        FspMsgLoadProgress msg = (FspMsgLoadProgress)NetManager.Inst.GetMessage(eNetMessageID.FspMsgLoadProgress);
                        msg.m_sendProgress = progress;
                        FspNetRunTime.Inst.Send(item.conn, msg);
                    }
                    foreach (Player item in m_listPlayer)
                    {
                        if (!item.tempData.bLoaded)
                            return;
                    }
                    Console.WriteLine("接受玩家全部加载完成 发送开始控制");
                    // 如果都加载完成，就发送开始控制,正式时可附带信息
                    foreach (Player item in m_listPlayer)
                    {
                        FspMsgStartControl msg = (FspMsgStartControl)NetManager.Inst.GetMessage(eNetMessageID.FspMsgStartControl);
                        FspNetRunTime.Inst.Send(item.conn, msg);
                    }
                    m_state = FspGameState.StartControl;
                    break;
                case FspGameState.StartControl:

                    break;
            }
        }

        public void Close()
        {

        }
    }
}
