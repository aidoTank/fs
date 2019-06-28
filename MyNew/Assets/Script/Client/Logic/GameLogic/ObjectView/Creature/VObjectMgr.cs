using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    public enum eVOjectType
    {
        None,
        Creature,
        SkillTrigger,
    }

    /// <summary>
    /// 断线重连时，表现层只用创建角色，不用创建技能，并且停止执行表现层命令和心跳
    /// </summary>
    public class VObjectMgr
    {
        private static int m_uid;

        public static VBase Create(eVOjectType type)
        {
            VBase obj = null;
            switch (type)
            {
                case eVOjectType.Creature:
                    obj = new VObject();
                break;
                //case eVOjectType.SkillTrigger:
                //    obj = new VTrigger();
                //    break;
            }
            obj.m_id = m_uid++;
            Add(obj.m_id, obj);
            return obj;
        }

        public static VBase Get(int uId)
        {
            VBase cc;
            if (m_dicPlayer.TryGetValue(uId, out  cc))
            {
                return cc;
            }
            return null;
        }

        public static void Add(int uid, VBase creature)
        {
            m_dicPlayer[uid] = creature;
        }

        public static void Remove(int uid)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                m_dicPlayer.Remove(uid);
            }
        }

        public static void Update(float time, float fdTime)
        {
            foreach (KeyValuePair<int, VBase> item in m_dicPlayer)
            {
                item.Value.Update(time, fdTime);
                if(item.Value.m_destroy)
                {
                    m_listDestroy.Add(item.Key);
                }
            }

            for (int i = 0; i < m_listDestroy.Count; i ++)
            {
                Remove(m_listDestroy[i]);
            }
            m_listDestroy.Clear();
            //Debug.Log("voject mgr:" + m_dicPlayer.Count);
        }

        public static Dictionary<int, VBase> m_dicPlayer = new Dictionary<int, VBase>();
        public static List<int> m_listDestroy = new List<int>();
    }
}
