using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    public class EntityManager : Singleton
    {
        public EntityManager()
            : base(true)
        {
        }

        public int CreateEntity(eEntityType eType, EntityBaseInfo baseInfo, Action<Entity> initEnd)
        {
            // 在这里可以让各种Entity继承EntityManager单独做个管理类
            // 然后通过管理类去new实体对象，暂时先用switch了
            // tips：这种静态的公共变量，如果在多个异步调用时，它的值会被最新值替换掉，在下面add时，就不能使用这个变量
            ++s_entityHandleId;
            // 如果缓存中有，就取缓存，如果没有就创建
            Entity entity = TryGetEntityFromCache(s_entityHandleId, initEnd, baseInfo);
            if (entity == null)
            {
                switch (eType)
                {
                    case eEntityType.eNone:
                        entity = new Entity(s_entityHandleId, initEnd, eType, baseInfo);
                        break;
                    case eEntityType.eSceneEntity:
                        entity = new SceneEntity(s_entityHandleId, initEnd, eType, baseInfo);
                        break;
                    case eEntityType.eBoneEntity:
                        entity = new BoneEntity(s_entityHandleId, initEnd, eType, baseInfo);
                        break;
                    case eEntityType.eEffectEntity:
                        entity = new EffectEntity(s_entityHandleId, initEnd, eType, baseInfo);
                        break;
                    //case eEntityType.eSoundEntity:
                    //    entity = new SoundEntity(s_entityHandleId, initEnd, eType, baseInfo);
                    //    break;
                    //case eEntityType.eBattleEntity:
                    //    entity = new BattleEntity(s_entityHandleId, initEnd, eType, baseInfo);
                    //    break;
                }
                if (null == entity)
                {
                    Debug.LogError("error eType：" + eType);
                    return 0;
                }
            }
            m_entityMap.Add(entity.m_hid, entity);
            return entity.m_hid;
        }

        public Entity GetEnity(int handle)
        {
            Entity ent;
            if (m_entityMap.TryGetValue(handle, out ent))
            {
                return ent;
            }
            return null;
        }

        /// <summary>
        /// 默认不加入缓存
        /// </summary>
        public void RemoveEntity(int uHandle)
        {
            RemoveEntity(uHandle, false);
        }

        /// <summary>
        /// 特效，声音加入缓存（ab资源常驻）
        /// </summary>
        public void RemoveEntity(int uHandle, bool bCache)
        {
            Entity ent = null;

            if (!m_entityMap.TryGetValue(uHandle, out ent))
            {
                //Debug.LogWarning("DynamicEntity remove fail :" + uHandle);
                return;
            }
            m_entityMap.Remove(uHandle);
            ent.m_hid = 0;
            if (bCache)
            {
                if (ent != null)
                {
                    // 加入缓存
                    if (!PushToPool(ent))
                    {
                        ent.Destroy();
                        ent = null;
                    }
                }
            }
            else
            {
                ent.Destroy();
                ent = null;
            }
        }

        public override void Update(float fTime, float fDTime)
        {
            Dictionary<int, Entity>.Enumerator dy = m_entityMap.GetEnumerator();
            while (dy.MoveNext())
            {
                m_tempEntityList.Add(dy.Current.Value);
            }

            List<Entity>.Enumerator lis = m_tempEntityList.GetEnumerator();
            while (lis.MoveNext())
            {
                lis.Current.Update(fTime, fDTime);
            }
            m_tempEntityList.Clear();
        }

        private Entity TryGetEntityFromCache(int handleId, Action<Entity> notify, EntityBaseInfo info)
        {
            LinkedList<Entity> entityList;
            if (m_cacheEntityMap.TryGetValue(info.m_resID, out entityList))
            {
                LinkedListNode<Entity> entNode = entityList.First;
                if (entNode != null)
                {
                    Entity ent = entNode.Value;
                    info.m_active = true;
                    ent.Revive(handleId, notify, info);
                    m_curCacheNums--;
                    entityList.RemoveFirst();
                    //Debug.Log("===========================取出缓存" + ent.GetObject() + "  " + ent.GetResource().GetResInfo().nResID);
                    return ent;
                }
            }
            return null;
        }

        private bool PushToPool(Entity ent)
        {
            LinkedList<Entity> lstEntities = null;
            if (!m_cacheEntityMap.TryGetValue(ent.GetEntityBaseInfo().m_resID, out lstEntities))
            {
                lstEntities = new LinkedList<Entity>();
                m_cacheEntityMap.Add(ent.GetEntityBaseInfo().m_resID, lstEntities);
            }
            if (lstEntities.Count >= m_unitMaxCacheNums)
            {
                return false;
            }
            if (!ent.IsInited())
            {
                return false;
            }
            //ent.Bind(null);
            ent.SetShow(false);
            lstEntities.AddLast(ent);
            m_curCacheNums++;
            //Debug.Log("===========================加入缓存" + ent.GetObject() + "  " + ent.GetResource().GetResInfo().nResID);
            return true;
        }

        public void ClearCache()
        {
            foreach (KeyValuePair<int, LinkedList<Entity>> item in m_cacheEntityMap)
            {
                foreach (Entity ent in item.Value)
                {
                    ent.Destroy();
                }
                item.Value.Clear();
            }
            m_cacheEntityMap.Clear();
        }

        public uint GetEntitysNum()
        {
            return (uint)m_entityMap.Count;
        }

        public override void Destroy()
        {
            m_entityMap.Clear();
        }

        public static EntityManager Inst;
        public static int s_entityHandleId;
        public Dictionary<int, Entity> m_entityMap = new Dictionary<int, Entity>();


        private List<Entity> m_tempEntityList = new List<Entity>();
        /// <summary>
        /// 当前资源缓存种类数
        /// </summary>
        public int m_curCacheNums = 0;
        /// <summary>
        /// 单个资源的最大缓存数量,MOBA类型，最大缓存9个差不多了
        /// </summary>
        public int m_unitMaxCacheNums = 9;
        public Dictionary<int, LinkedList<Entity>> m_cacheEntityMap = new Dictionary<int, LinkedList<Entity>>();

        public string GetCahceEntityInfo()
        {
            string info = "\n";
            foreach (KeyValuePair<int, LinkedList<Entity>> item in m_cacheEntityMap)
            {
                if (item.Value.First != null)
                {
                    info += "个数：" + item.Value.Count + " 对象：" + item.Value.First.Value.m_object + "\n";
                }

            }
            return info;
        }

        //public string GetStaticEntityInfo()
        //{
        //    string info = "\n";
        //    int num = 0;
        //    foreach (KeyValuePair<uint, Entity> item in m_staticEntityMap)
        //    {
        //        GameObject obj = item.Value.GetObject();
        //        if (obj != null)
        //        {
        //            info += obj.name + "\n";
        //        }
        //        num++;
        //    }
        //    return " 数量[" + num + "]:" + info;
        //}

        public string GetDynamicEntityInfo()
        {
            string info = "\n";
            int num = 0;
            foreach (KeyValuePair<int, Entity> item in m_entityMap)
            {
                GameObject obj = item.Value.GetObject();
                if (obj != null)
                {
                    info += num + "." + obj.name + "\n";
                }
                else
                {
                    info += num + "." + item.Value.GetEntityBaseInfo().m_strName + "(野对象)\n";

                }
                num++;
            }
            return " 数量[" + num + "]:" + info;
        }

        //private int GetObjectMemory(GameObject go)
        //{
        //    int size = 0;

        //    return size;
        //}
    }
}
