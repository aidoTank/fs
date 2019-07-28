using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    /// <summary>
    /// 对玩家统一接口的封装，方面map使用
    /// </summary>
    public class CCreatureMgr
    {
        public static CCreature s_masterCreature = null;

        //public static CCreature CreateMaster(long uid)
        //{
        //    if (m_dicPlayer.ContainsKey(uid))
        //    {
        //        return (CMasterPlayer)m_dicPlayer[uid];
        //    }
        //    s_masterCreature = new CMasterPlayer(uid);
        //    //Add(uid, s_masterCreature);
        //    m_listAdd.Add(s_masterCreature);
        //    return s_masterCreature;
        //}

        public static CCreature GetMaster()
        {
            //return s_masterCreature;
            return Get(EGame.m_uid);
        }

        public static CCreature Create(EThingType type, long uid)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                return m_dicPlayer[uid];
            }

            CCreature cc = null;

            switch (type)
            {
                case EThingType.Player:
                    cc = new CPlayer(uid);
                    break;
            }

            Add(uid, cc);
            //m_listAdd.Add(cc);
            return cc;
        }

        public static CCreature Get(long uId)
        {
            CCreature cc;
            if (m_dicPlayer.TryGetValue(uId, out cc))
            {
                return cc;
            }
            return null;
        }

        public static void Add(long uid, CCreature creature)
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

        public static void RemoveAll(bool bConMaster)
        {
            foreach (KeyValuePair<long, CCreature> item in m_dicPlayer)
            {
                if (item.Value.IsMaster())
                {
                    if (bConMaster)
                        item.Value.Destory();
                }
                else
                {
                    item.Value.Destory();
                }
            }
        }

        public static void ExecuteFrame(int frameId)
        {
            for (int i = 0; i < m_listAdd.Count; i++)
            {
                Add(m_listAdd[i].GetUid(), m_listAdd[i]);
            }
            m_listAdd.Clear();
            //Debug.Log(m_dicPlayer.Count);
            foreach (KeyValuePair<long, CCreature> item in m_dicPlayer)
            {
                if (item.Value.m_bActive)
                {
                    item.Value.ExecuteFrame(frameId);
                }
                if (item.Value.m_destroy)
                {
                    m_listDestroy.Add((int)item.Key);
                }
            }

            for (int i = 0; i < m_listDestroy.Count; i++)
            {
                Remove(m_listDestroy[i]);
            }
            m_listDestroy.Clear();
            //Debug.Log("player mgr:" + m_dicPlayer.Count);
        }


        //public static List<long> GetType(EThingType type, int thingCsvId)
        //{
        //    m_tempCreatureList.Clear();
        //    foreach (KeyValuePair<long, CCreature> item in m_dicPlayer)
        //    {
        //        if (item.Value.GetThingType() == type &&
        //            !item.Value.IsDie() &&
        //            item.Value.m_csvData.Id == thingCsvId)
        //        {
        //            m_tempCreatureList.Add(item.Key);
        //        }
        //    }
        //    return m_tempCreatureList;
        //}


        /// <summary>
        /// 需要在外部遍历时，创建，删除，界面调用，用这个
        /// </summary>
        public static List<long> GetType(EThingType type)
        {
            m_tempCreatureList.Clear();
            foreach (KeyValuePair<long, CCreature> item in m_dicPlayer)
            {
                if (!item.Value.m_bActive)
                    continue;
                if (item.Value.GetThingType() == type && !item.Value.IsDie())
                {
                    m_tempCreatureList.Add(item.Key);
                }
            }
            return m_tempCreatureList;
        }

        /// <summary>
        /// 需要在外部遍历时，创建，删除，界面调用，用这个。比如技能
        /// </summary>
        public static List<long> GetCreatureList()
        {
            m_tempCreatureList.Clear();
            foreach (KeyValuePair<long, CCreature> item in m_dicPlayer)
            {
                if (!item.Value.m_bActive)
                    continue;
                m_tempCreatureList.Add(item.Key);
            }
            return m_tempCreatureList;
        }

        //public static void UpdateLanguageName()
        //{
        //    foreach (KeyValuePair<long, CCreature> item in m_dicPlayer)
        //    {
        //        item.Value.UpdateLanguageName();
        //    }
        //}



        private static List<long> m_tempCreatureList = new List<long>();

        private static Dictionary<long, CCreature> m_dicPlayer = new Dictionary<long, CCreature>();
        public static List<CCreature> m_listAdd = new List<CCreature>();
        public static List<int> m_listDestroy = new List<int>();


        public static void OnDrawGizmos()
        {
            //foreach (KeyValuePair<long, CCreature> item in m_dicPlayer)
            //{
            //    if (!item.Value.m_bActive)
            //        continue;
            //    item.Value.Draw();
            //}
        }
    }
}
