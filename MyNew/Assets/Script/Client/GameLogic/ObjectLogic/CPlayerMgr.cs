using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    /// <summary>
    /// 对玩家统一接口的封装，方面map使用
    /// </summary>
    public class CPlayerMgr
    {
        private static CMasterPlayer s_masterCreature = null;

        public static CMasterPlayer CreateMaster(long uid)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                return (CMasterPlayer)m_dicPlayer[uid];
            }
            s_masterCreature = new CMasterPlayer(uid);
            Add(uid, s_masterCreature);
            return s_masterCreature;
        }

        public static CMasterPlayer GetMaster()
        {
            return s_masterCreature;
        }

        public static CPlayer Create(long uid)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                return m_dicPlayer[uid];
            }

            CPlayer player = new CPlayer(uid);
            Add(uid, player);

            return player;
        }

        public static CPlayer Get(long uId)
        {
            CPlayer cc;
            if (m_dicPlayer.TryGetValue(uId, out  cc))
            {
                return cc;
            }
            return null;
        }

        public static void Add(long uid, CPlayer creature)
        {
            m_dicPlayer[uid] = creature;
        }

        public static void Remove(long uid)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                m_dicPlayer.Remove(uid);
            }
        }

        public static void ExecuteFrame(int frameId)
        {
            foreach(KeyValuePair<long, CPlayer> item in m_dicPlayer)
            {
                item.Value.ExecuteFrame(frameId);
                if(item.Value.m_destroy)
                {
                    m_listDestroy.Add((int)item.Key);
                }
            }
            for(int i = 0; i < m_listDestroy.Count; i ++)
            {
                Remove(m_listDestroy[i]);
            }
            m_listDestroy.Clear();
            //Debug.Log("player mgr:" + m_dicPlayer.Count);
        }

        public static Dictionary<long, CPlayer> m_dicPlayer = new Dictionary<long, CPlayer>();
        public static List<int> m_listDestroy = new List<int>();
    }
}
