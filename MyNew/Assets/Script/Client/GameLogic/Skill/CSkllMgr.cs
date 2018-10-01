using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    public class CSkillMgr
    {
        public static SkillBase Create(long uid, int skillId)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                return m_dicPlayer[uid];
            }

            SkillBase player = new SkillBase(uid, skillId);
            Add(uid, player);

            return player;
        }

        public static SkillBase Get(long uId)
        {
            SkillBase cc;
            if (m_dicPlayer.TryGetValue(uId, out  cc))
            {
                return cc;
            }
            return null;
        }

        public static void Add(long uid, SkillBase creature)
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

        public static void ExecuteFrame(int frameId)
        {
            foreach(KeyValuePair<long, SkillBase> item in m_dicPlayer)
            {
                item.Value.ExecuteFrame(frameId);
            }
        }

        public static Dictionary<long, SkillBase> m_dicPlayer = new Dictionary<long, SkillBase>();
    }
}
