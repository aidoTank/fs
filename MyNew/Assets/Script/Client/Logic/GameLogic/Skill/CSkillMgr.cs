using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{

    public class CSkillMgr
    {
        private static int m_uid;

        #region �������
        /// <summary>
        /// ����+������ת����������ӿڱ�̣���Ȼ������õ��Ǹ��ࣩ
        /// ԭ�����Ʒ�ת��Inversion of Control��IoC�����������õ��� ����ע�루���캯��ע�룩��Dependency injection�� DI
        /// ���ã����ھ�,�����
        /// </summary>
        public static SkillBase Create(eSkillType type, VSkillBase vSkill)
        {
            SkillBase obj = null;
            switch (type)
            {
                case eSkillType.None:
                    obj = new SkillBase(m_uid, vSkill);
                break;
                case eSkillType.Near:
                    obj = new SkillNear(m_uid, vSkill);
                break;
                case eSkillType.Jump:
                    obj = new SkillJump(m_uid, vSkill);
                break;
                case eSkillType.Down_Up:
                    obj = new SkillDownUp(m_uid, vSkill);
                    break;
            }
            Add(m_uid++, obj);
            return obj;
        }

        public static SkillBase Get(long uId)
        {
            SkillBase cc;
            if (m_dicSkill.TryGetValue(uId, out  cc))
            {
                return cc;
            }
            return null;
        }

        public static SkillBase GetDownUpSkill(long casterUid)
        {
            foreach (KeyValuePair<long, SkillBase> item in m_dicSkill)
            {
                if (casterUid == item.Value.m_curSkillCmd.m_casterUid &&
                    item.Value.m_skillInfo.skillType == (int)eSkillType.Down_Up && 
                    !item.Value.m_destroy)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static void Add(long uid, SkillBase creature)
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
        #endregion

        public static void ExecuteFrame(int frameId)
        {
            foreach(KeyValuePair<long, SkillBase> item in m_dicSkill)
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
            foreach (KeyValuePair<long, SkillBase> item in m_dicSkill)
            {
                item.Value.Destory();
            }
        }

        public static Dictionary<long, SkillBase> m_dicSkill = new Dictionary<long, SkillBase>();
        public static List<int> m_listDestroy = new List<int>();
    }
}
