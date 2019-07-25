using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{

    public class CBuffTriggerMgr
    {
        private static int m_uid;


        public static CBuffTrigger Create(eBuffTriggerPosType type)
        {
            CBuffTrigger obj = null;
            if (type == eBuffTriggerPosType.Laser)
            {
                obj = new CBuffTrigger_Laser(m_uid++);
            }
            else if(type == eBuffTriggerPosType.SkillEndPos_Curve)
            {
                obj = new CBuffTrigger_Curve(m_uid++);
            }
            else if(type == eBuffTriggerPosType.SkillEndPos)
            {
                obj = new CBuffTrigger_SkillEndPos(m_uid++);
            }
            else if (type == eBuffTriggerPosType.BindCaster)
            {
                obj = new CBuffTrigger_BindCaster(m_uid++);
            }
            else if (type == eBuffTriggerPosType.CasterStartPos_SkillDir)
            {
                obj = new CBuffTrigger_SkillDir(m_uid++);
            }
            else if (type == eBuffTriggerPosType.Lightning)
            {
                obj = new CBuffTrigger_Lightning(m_uid++);
            }
            else
            {
                obj = new CBuffTrigger(m_uid++);
            }
            //Add(m_uid++, obj);

            m_listAdd.Add(obj);
            return obj;
        }

        public static CBuffTrigger Get(long uId)
        {
            CBuffTrigger cc;
            if (m_dicSkill.TryGetValue(uId, out  cc))
            {
                return cc;
            }
            return null;
        }


        public static void Add(long uid, CBuffTrigger creature)
        {
            m_dicSkill[uid] = creature;
        }

        public static void Remove(long uid)
        {
            if (m_dicSkill.ContainsKey(uid))
            {
                m_dicSkill.Remove(uid);
            }
        }

        /// <summary>
        /// 同步移除
        /// 需要外部调用清除的BUFF，比如BUFF消失时间未到，但是人物已经死亡时
        /// </summary>
        public static void Destroy(long uid)
        {
            CBuffTrigger buff = Get(uid);
            if (buff != null)
            {
                buff.Destory();
                //Remove(uid);
            }
        }

        public static void ExecuteFrame(int frameId)
        {
            for (int i = 0; i < m_listAdd.Count; i++)
            {
                Add(m_listAdd[i].GetUid(), m_listAdd[i]);
            }
            m_listAdd.Clear();
            foreach (KeyValuePair<long, CBuffTrigger> item in m_dicSkill)
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


        public static void Destory()
        {
            foreach (KeyValuePair<long, CBuffTrigger> item in m_dicSkill)
            {
                item.Value.Destory();
            }
        }

        public static Dictionary<long, CBuffTrigger> m_dicSkill = new Dictionary<long, CBuffTrigger>();
        public static List<CBuffTrigger> m_listAdd = new List<CBuffTrigger>();
        public static List<int> m_listDestroy = new List<int>();
    }
}
