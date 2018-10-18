using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    public enum eVOjectType
    {
        None,
        Creature,
        SkillBase,
        SkllNear,
        SkillSingleFly,
        SkillAoe,
    }

    public class VObjectMgr
    {
        // public static SkillBase Create(long uid, int skillId)
        // {
        //     if (m_dicPlayer.ContainsKey(uid))
        //     {
        //         return m_dicPlayer[uid];
        //     }

        //     SkillBase player = new SkillBase(uid, skillId);
        //     Add(uid, player);

        //     return player;
        // }

        private static int m_uid;

        public static VObject  Create(eVOjectType type)
        {
            VObject obj = null;
            switch (type)
            {
                case eVOjectType.Creature:
                    obj = new VObject();
                break;
                case eVOjectType.SkillBase:
                    obj = new VSkillBase();
                break;
                case eVOjectType.SkllNear:
                    obj = new VSkillNear();
                break;
                case eVOjectType.SkillSingleFly:
                    obj = new VSkillSingleFly();
                break;
                case eVOjectType.SkillAoe:
                    obj = new VSkillBase();
                break;
            }
            obj.m_id = m_uid ++;
            Add(obj.m_id, obj);
            return obj;
        }

        public static VObject Get(int uId)
        {
            VObject cc;
            if (m_dicPlayer.TryGetValue(uId, out  cc))
            {
                return cc;
            }
            return null;
        }

        public static void Add(int uid, VObject creature)
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
            foreach(KeyValuePair<int, VObject> item in m_dicPlayer)
            {
                item.Value.Update(time, fdTime);
                if(item.Value.m_destroy)
                {
                    m_listDestroy.Add(item.Key);
                }
            }
            for(int i = 0; i < m_listDestroy.Count; i ++)
            {
                Remove(m_listDestroy[i]);
            }
            m_listDestroy.Clear();
            Debug.Log("voject mgr:" + m_dicPlayer.Count);
        }

        public static Dictionary<int, VObject> m_dicPlayer = new Dictionary<int, VObject>();
        public static List<int> m_listDestroy = new List<int>();
    }
}
