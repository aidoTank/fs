using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{

    public class CBuffMgr
    {
        private static int m_uid;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffId"></param>
        /// <returns></returns>
        public static BuffBase Create(int buffId)
        {
            SkillBuffCsvData buffData = CsvManager.Inst.GetCsv<SkillBuffCsv>((int)eAllCSV.eAC_SkillBuff).GetData(buffId);
            if(buffData == null)
            {
                Debug.LogError("BUFF为空 id：" + buffId);
                return null;
            }
            if (buffData.logicId == 0)
            {
                Debug.LogError("BUFF配置错误 id：" + buffId + "  类型：" + buffData.logicId);
                return null;
            }
            BuffBase obj = null;
            switch (buffData.logicId)
            {
                case (int)eBuffType.damage:
                    obj = new Buff01(m_uid, buffData);
                break;
                case (int)eBuffType.contDamage:
                    obj = new Buff02(m_uid, buffData);
                    break;
                case (int)eBuffType.pullPos:
                    obj = new Buff03(m_uid, buffData);
                    break;
                case (int)eBuffType.atk:
                    obj = new Buff04(m_uid, buffData);
                    break;
                case (int)eBuffType.speed:
                    obj = new Buff05(m_uid, buffData);
                    break;
                case (int)eBuffType.repel:
                    obj = new Buff06(m_uid, buffData);
                    break;
                case (int)eBuffType.addHp:
                    obj = new Buff07(m_uid, buffData);
                    break;
                case (int)eBuffType.dp:
                    obj = new Buff08(m_uid, buffData);
                    break;
                case (int)eBuffType.modelScale:
                    obj = new Buff09(m_uid, buffData);
                    break;
                case (int)eBuffType.state:
                    obj = new Buff10(m_uid, buffData);
                    break;

                case (int)eBuffType.atkCreate:
                    obj = new Buff11(m_uid, buffData);
                    break;
                case (int)eBuffType.hitCreate:
                    obj = new Buff12(m_uid, buffData);
                    break;

                case (int)eBuffType.createTrigger:
                    obj = new Buff20(m_uid, buffData);
                    break;
                case (int)eBuffType.createSkill:
                    obj = new Buff21(m_uid, buffData);
                    break;
                case (int)eBuffType.createCreature:
                    obj = new Buff22(m_uid, buffData);
                    break;
                case (int)eBuffType.flash:
                    obj = new Buff101(m_uid, buffData);
                    break;
            }
            m_uid++;
            //Add(m_uid++, obj);
            m_listAdd.Add(obj);
            return obj;
        }

        public static BuffBase Get(long uId)
        {
            BuffBase cc;
            if (m_dicSkill.TryGetValue(uId, out  cc))
            {
                return cc;
            }
            return null;
        }

        public static void Add(long uid, BuffBase creature)
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
        /// <param name="uid"></param>
        public static void Destroy(long uid)
        {
            BuffBase buff = Get(uid);
            if(buff != null)
            {
                buff.Destroy();
                Remove(uid);
            }
        }

        /// <summary>
        /// 跨场景时调用
        /// </summary>
        public static void Destroy()
        {
            foreach (KeyValuePair<long, BuffBase> item in m_dicSkill)
            {
                if(item.Value.m_dieDestroy)
                    item.Value.Destroy();
            }
        }

        public static void ExecuteFrame(int frameId)
        {
            for (int i = 0; i < m_listAdd.Count; i++)
            {
                Add(m_listAdd[i].m_uid, m_listAdd[i]);
            }
            m_listAdd.Clear();
            foreach (KeyValuePair<long, BuffBase> item in m_dicSkill)
            {
                item.Value.ExecuteFrame();
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
        }

        public static Dictionary<long, BuffBase> m_dicSkill = new Dictionary<long, BuffBase>();
        public static List<int> m_listDestroy = new List<int>();
        public static List<BuffBase> m_listAdd = new List<BuffBase>();  // 在BUFF心跳中，增加，移除的需求时会用到
    }
}
