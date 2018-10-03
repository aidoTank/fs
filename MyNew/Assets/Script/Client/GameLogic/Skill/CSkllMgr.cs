using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    public enum eCSkillType
    {
        None,
        Creature,
        SkllNear,
        SkillSingleFly,
        SkillAoe,
    }

    public class CSkillMgr
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

        public static SkillBase Create(eCSkillType type, VSkillBase vSkill)
        {
            SkillBase obj = null;
            switch (type)
            {
                case eCSkillType.SkllNear:
                    obj = new SkillNear(m_uid, vSkill);
                break;
                case eCSkillType.SkillSingleFly:
                    obj = new SkillSingleFly(m_uid, vSkill);
                break;
                 case eCSkillType.SkillAoe:
                    obj = new SkillAoe(m_uid, vSkill);
                break;
            }
            Add(m_uid ++, obj);
            return obj;
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
