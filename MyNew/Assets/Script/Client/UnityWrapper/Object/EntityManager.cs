using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class EntityManager : Singleton
	{
        public EntityManager()
            : base(true)
        {
        }

        public uint CreateEntity(eEntityType eType, EntityInitNotify notify, EntityBaseInfo baseInfo)
        {
            // 在这里可以让各种Entity继承EntityManager单独做个管理类
            // 然后通过管理类去new实体对象，暂时先用switch了
            // tips：这种静态的公共变量，如果在多个异步调用时，它的值会被最新值替换掉，在下面add时，就不能使用这个变量
            ++s_entityHandleId;
            // 如果缓存中有，就取缓存，如果没有就创建
            Entity entity = TryGetEntityFromCache(s_entityHandleId, notify, baseInfo);
            if (entity == null)
            {
                switch (eType)
                {
                    case eEntityType.eStaticEntity:
                        entity = new StaticEntity(s_entityHandleId, notify, eType, baseInfo);
                        break;
                    case eEntityType.eDynamicEntity:
                    case eEntityType.eEffectEntity:
                        entity = new DynamicEntity(s_entityHandleId, notify, eType, baseInfo);
                        break;
                    case eEntityType.eBoneEntity:
                        entity = new BoneEntity(s_entityHandleId, notify, eType, baseInfo);
                        break;
                    case eEntityType.eSoundEntity:
                        entity = new SoundEntity(s_entityHandleId, notify, eType, baseInfo);
                        break;
                }
                if (null == entity)
                {
                    Debug.LogError("error eType：" + eType);
                    return 0;
                }
            }
            if (entity.IsStaticEntity())
            {
                m_staticEntityMap.Add(entity.m_handleID, entity);
            }
            else
            {

                m_dynamicEntityMap.Add(entity.m_handleID, entity);
            }
            return entity.m_handleID;
        }

        public Entity GetEnity(uint uHandle, bool bStatic)
        {
            Entity ent;
            if (bStatic)
            {
                if (m_staticEntityMap.TryGetValue(uHandle, out ent))
                {
                    return ent;
                }
            }
            else
            {
                if (m_dynamicEntityMap.TryGetValue(uHandle, out ent))
                {
                    return ent;
                }
            }
            return null;
        }

        /// <summary>
        /// 默认不加入缓存
        /// </summary>
        /// <param name="uHandle"></param>
        /// <param name="bStatic"></param>
        public void RemoveEntity(uint uHandle, bool bStatic)
        {
            RemoveEntity(uHandle, bStatic, false);
        }

        /// <summary>
        /// 特效，声音加入缓存（ab资源常驻）
        /// </summary>
        public void RemoveEntity(uint uHandle, bool bStatic, bool bCache)
        {
            Entity ent = null;
            if (bStatic)
            {
                if (!m_staticEntityMap.TryGetValue(uHandle, out ent))
                {
                    //Debug.LogWarning("StaticEntity remove fail :" + uHandle);
                    return;
                }
               // if (ent.IsInited())
                //{
                    m_staticEntityMap.Remove(uHandle);
               // }
            }
            else
            {
                if (!m_dynamicEntityMap.TryGetValue(uHandle, out ent))
                {
                    //Debug.LogWarning("DynamicEntity remove fail :" + uHandle);
                    return;
                }
                //if (ent.IsInited())
                //{
                    m_dynamicEntityMap.Remove(uHandle);
                //}
            }

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
            //Dictionary<uint, Entity>.Enumerator map = m_dynamicEntityMap.GetEnumerator();
            //while (map.MoveNext())
            //{
            //    map.Current.Value.Update(fTime, fDTime);
            //}

            //Dictionary<uint, Entity>.Enumerator smap = m_staticEntityMap.GetEnumerator();
            //while (smap.MoveNext())
            //{
            //    smap.Current.Value.Update(fTime, fDTime);
            //}

            Dictionary<uint, Entity>.Enumerator dy = m_dynamicEntityMap.GetEnumerator();
            while (dy.MoveNext())
            {
                m_tempEntityList.Add(dy.Current.Value);
            }

            Dictionary<uint, Entity>.Enumerator st = m_staticEntityMap.GetEnumerator();
            while (st.MoveNext())
            {
                m_tempEntityList.Add(st.Current.Value);
            }

            List<Entity>.Enumerator lis = m_tempEntityList.GetEnumerator();
            while (lis.MoveNext())
            {
                lis.Current.Update(fTime, fDTime);
            }
            m_tempEntityList.Clear();

            //m_tempEntityList = new List<Entity>(m_dynamicEntityMap.Values);
            //List<Entity>.Enumerator dynamicList = m_tempEntityList.GetEnumerator();
            //while (dynamicList.MoveNext())
            //{
            //    dynamicList.Current.Update(fTime, fDTime);
            //}
            //m_tempEntityList.Clear();

            //m_tempEntityList = new List<Entity>(m_staticEntityMap.Values);
            //List<Entity>.Enumerator staticList = m_tempEntityList.GetEnumerator();
            //while (staticList.MoveNext())
            //{
            //    staticList.Current.Update(fTime, fDTime);
            //}
            //m_tempEntityList.Clear();

            //var entityList = new List<Entity>(m_dynamicEntityMap.Values);
            //for (int i = 0; i < entityList.Count; i++)
            //{
            //    entityList[i].Update(fTime, fDTime);
            //}

            //var sentityList = new List<Entity>(m_staticEntityMap.Values);
            //for (int i = 0; i < sentityList.Count; i++)
            //{
            //    sentityList[i].Update(fTime, fDTime);
            //}
        }

        private Entity TryGetEntityFromCache(uint handleId, EntityInitNotify notify, EntityBaseInfo info)
        {
            LinkedList<Entity> entityList;
            if (m_cacheEntityMap.TryGetValue(info.m_resID, out entityList))
            {
                LinkedListNode<Entity> entNode = entityList.First;
                if (entNode != null)
                {
                    Entity ent = entNode.Value;
                    ent.Revive(handleId,notify, info);
                    m_curCacheNums--;
                    entityList.RemoveFirst();
                    ent.SetActive(true);
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
            ent.Bind(null);
            ent.SetActive(false);
            lstEntities.AddLast(ent);
            m_curCacheNums++;
            //Debug.Log("===========================加入缓存" + ent.GetObject() + "  " + ent.GetResource().GetResInfo().nResID);
            return true;
        }

        public void ClearCache()
        {
            foreach(KeyValuePair<int, LinkedList<Entity>> item in m_cacheEntityMap)
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
            return (uint)m_staticEntityMap.Count + (uint)m_dynamicEntityMap.Count;
        }

        public override void Destroy()
        {
            m_staticEntityMap.Clear();
            m_dynamicEntityMap.Clear();
        }

        public static new EntityManager Inst;
        public static uint s_entityHandleId = 0;
        public Dictionary<uint, Entity> m_staticEntityMap = new Dictionary<uint, Entity>();
        public Dictionary<uint, Entity> m_dynamicEntityMap = new Dictionary<uint, Entity>();

        private List<Entity> m_tempEntityList = new List<Entity>();
        /// <summary>
        /// 当前资源缓存种类数
        /// </summary>
        public int m_curCacheNums = 0;
        /// <summary>
        /// 单个资源的最大缓存数量
        /// </summary>
        public int m_unitMaxCacheNums = 10;
        public Dictionary<int, LinkedList<Entity>> m_cacheEntityMap = new Dictionary<int, LinkedList<Entity>>();

        public string GetCahceEntityInfo()
        {
            string info = "\n";
            foreach (KeyValuePair<int, LinkedList<Entity>> item in m_cacheEntityMap)
            {
                if (item.Value.First != null)
                {
                    info +=  "个数：" + item.Value.Count + " 对象：" + item.Value.First.Value.m_object + "\n";
                }

            }
            return info;
        }

        public string GetStaticEntityInfo()
        {
            string info = "\n";
            int num = 0;
            foreach (KeyValuePair<uint, Entity> item in m_staticEntityMap)
            {
                GameObject obj = item.Value.GetObject();
                if (obj != null)
                {
                    info += obj.name + "\n";
                }
                num ++;
            }
            return " 数量[" + num + "]:" + info;
        }

        public string GetDynamicEntityInfo()
        {
            string info = "\n";
            int num = 0;
            foreach (KeyValuePair<uint, Entity> item in m_dynamicEntityMap)
            {
                GameObject obj = item.Value.GetObject();
                if (obj != null)
                {
                    info += obj.name + "\n";
                }
                else
                {
                    info += item.Value.GetResource().GetResInfo().strName + "(野对象)\n";

                }
                num++;
            }
            return " 数量[" + num + "]:" + info;
        }

        private int GetObjectMemory(GameObject go)
        {
            int size = 0;

            return size;
        }
	}
}
