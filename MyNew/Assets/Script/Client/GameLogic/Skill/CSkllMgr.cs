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
        SkillJump,
        SkillCurve,
    }

    public class CSkillMgr
    {
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
                case eCSkillType.SkillJump:
                    obj = new SkillJump(m_uid, vSkill);
                break;
                case eCSkillType.SkillCurve:
                    obj = new SkillCurve(m_uid, vSkill);
                break;
            }
            Add(m_uid++, obj);
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

        public static void Remove(long uid)
        {
            if (m_dicPlayer.ContainsKey(uid))
            {
                m_dicPlayer.Remove(uid);
            }
        }

        public static void ExecuteFrame(int frameId)
        {
            foreach(KeyValuePair<long, SkillBase> item in m_dicPlayer)
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
            //Debug.Log("skill mgr:" + m_dicPlayer.Count);
        }

        public static Dictionary<long, SkillBase> m_dicPlayer = new Dictionary<long, SkillBase>();
        public static List<int> m_listDestroy = new List<int>();
    }
}
