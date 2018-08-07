using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
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

            if (m_dicPlayer.Count > m_maxPlayerNum)
            {
                Debug.Log("已经大于40人。。。。。。。。。。。。。。。。。。。。。。。。。");
                return null;
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

        public static void Remove(long uid, bool destroy)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                if (destroy)
                {
                    m_dicPlayer[uid].Destory();
                }
                m_dicPlayer.Remove(uid);
            }
        }

        public static void Clear(bool bClearMaster)
        {
            if(bClearMaster)
            {
                List<CPlayer> list = new List<CPlayer>(m_dicPlayer.Values);
                for (int i = 0; i < list.Count; i++)
                {
                    m_dicPlayer.Remove(list[i].GetUid());
                    list[i].Destory();
                    list[i] = null;
                }
                m_dicPlayer.Clear();
            }
            else
            {
                List<CPlayer> list = new List<CPlayer>(m_dicPlayer.Values);
                for (int i = 0; i < list.Count; i++)
                {
                    if (!list[i].IsMaster())
                    {
                        long uid = list[i].GetUid();
                        list[i].Destory();
                        list[i] = null;
                        m_dicPlayer.Remove(uid);
                    }
                }
            }
        }

        public static void Update(float fTime, float fDTime)
        {
            Dictionary<long, CPlayer>.Enumerator ms = m_dicPlayer.GetEnumerator();
            while (ms.MoveNext())
            {
                m_tempListPlayer.Add(ms.Current.Value);
            }

            List<CPlayer>.Enumerator lis = m_tempListPlayer.GetEnumerator();
            while (lis.MoveNext())
            {
                lis.Current.Update(fTime, fDTime);
            }
            m_tempListPlayer.Clear();
        }

        public static void LateUpdate(float fTime, float fDTime)
        {
            Dictionary<long, CPlayer>.Enumerator ms = m_dicPlayer.GetEnumerator();
            while (ms.MoveNext())
            {
                //ms.Current.Value.LateUpdate(fTime, fDTime);
            }
        }

        public static void SetShow(bool bShow)
        {
            Dictionary<long, CPlayer>.Enumerator map = m_dicPlayer.GetEnumerator();
            while (map.MoveNext())
            {
                map.Current.Value.SetShow(bShow);
            }
        }

        /// <summary>
        /// 隐藏其他玩家
        /// </summary>
        /// <param name="bShow"></param>
        public static void SetHideOtherMaster(bool bShow)
        {
            m_bHideOtherPlayer = bShow;
            Dictionary<long, CPlayer>.Enumerator map = m_dicPlayer.GetEnumerator();
            while (map.MoveNext())
            {
                if (!map.Current.Value.IsMaster())
                {
                    map.Current.Value.SetShow(!bShow);
                }
            }
        }


        public static void RefreshHeight()
        {
            Dictionary<long, CPlayer>.Enumerator map = m_dicPlayer.GetEnumerator();
            while (map.MoveNext())
            {
                KeyValuePair<long, CPlayer> key = map.Current;
                key.Value.SetPos(key.Value.GetPos().x, key.Value.GetPos().z);
            }
        }

        public static void RefreshHead()
        {
            Dictionary<long, CPlayer>.Enumerator map = m_dicPlayer.GetEnumerator();
            while (map.MoveNext())
            {
                //map.Current.Value._UpdateHead();
            }
        }

        public static void SetShadow(bool bShow)
        {
            Dictionary<long, CPlayer>.Enumerator map = m_dicPlayer.GetEnumerator();
            while (map.MoveNext())
            {
                //map.Current.Value.SetShadow(bShow);
            }
        }

        public static Dictionary<long, CPlayer> m_dicPlayer = new Dictionary<long, CPlayer>();
        private static List<CPlayer> m_tempListPlayer = new List<CPlayer>();
        public static bool m_bHideOtherPlayer = false;
        private static int m_maxPlayerNum = 100;
        public static int curSeverLevel;      //当前服务器等级 by服务器
    }
}
