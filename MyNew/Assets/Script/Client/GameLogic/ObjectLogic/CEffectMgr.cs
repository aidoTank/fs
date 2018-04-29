using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngineInternal;

namespace Roma
{
    public delegate void EffectFinishListenerEvent();
    // 特效对象类，类似于CCreature。
    public class CEffect
    {
        /// <summary>
        /// 用于外部设置，识别的唯一标识符，比如场景动画配置需要控制创建和消失
        /// </summary>
        public uint m_uid; 
        public EffectData m_effectData;
        public DynamicEntity m_ent;
        public Vector3 m_pos = new Vector3(0f, 0.01f, 0f);
        public Vector3 m_rota = Vector3.zero;
        public Vector3 m_scale = Vector3.one;
        public int m_layer = (int)LusuoLayer.eEL_Dynamic;
        public int m_order;
        public Vector3 m_offsetPos = Vector3.zero;
        public Transform m_parent;
        public EntityInitNotify m_loaded;
        public EffectFinishListenerEvent m_finished;     //播放结束
        public uint m_soundHandleId = 0;

        public float m_curTime = 0.0f;
        public float m_maxTime = 0.0f;
        // 默认是自动删除，如果开始时间是0，那么就是需要手动删除
        public bool m_isFinish = false;
        private bool m_bAutoDel = true;

        public CEffect(EffectData data)
        {
            m_effectData = data;
            m_maxTime = data.fLiveTime;
            if (m_maxTime.Equals(0.0f))
            {
                m_bAutoDel = false;
            }
        }

        public uint InitConfigure()
        {
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = m_effectData.nResID;
            // 创建一个动态实体
            // 注：如果这里有相同资源，就直接先执行OnLoaded
            uint uH = EntityManager.Inst.CreateEntity(eEntityType.eEffectEntity, OnLoaded, info);
            m_ent = EntityManager.Inst.GetEnity(uH, false) as DynamicEntity;
            return uH;
        }
        private void OnLoaded(Entity entity, object userObject)
        {
            // 设置位置等
            if (null != m_parent)
            {
                ((DynamicEntity)entity).Bind(m_parent);   // 将当期特效绑定到这个骨骼上
                //Debug.Log("这个特效被挂着父类上了:" + m_parent);
            }
            m_pos += m_offsetPos;

            entity.SetPos(m_pos);
            entity.SetDirection(m_rota);
            entity.SetScale(m_scale);
            entity.SetLayer(m_layer);
            entity.SetOrder(m_order);

            if (null != m_loaded)
            {
                m_loaded(entity, userObject);
            }

            // 播放音效
            //Debug.Log("特效实体:" + entity + "特效音效:" + m_effectData + "音效ID:" + m_effectData.uSoundID);
            if (m_effectData != null && !string.IsNullOrEmpty(m_effectData.uSoundID))
            {
                int sndId = 0;
                int.TryParse(m_effectData.uSoundID, out sndId);
                //m_soundHandleId = LogicSystem.Inst.PlaySound(sndId);
            }

            //Debug.Log("特效挂点这里被调用！" + ((DynamicEntity)entity).m_transform.parent);
        }

        public bool IsInited()
        {
            if(m_ent == null)
            {
                return false;
            }
            return m_ent.IsInited();
        }

        public Vector3 GetPos()
        {
            return m_pos;
        }

        public void SetActive(bool bTrue)
        {
            if (m_ent != null && m_ent.IsInited())
            {
                // 特效使用renderer做显影不好重置进行下次播放
                m_ent.SetActive(bTrue);
                //m_ent.GetObject().SetActive(bTrue);
            }
        }

        public void SetLayer(int index)
        {
            if (index == 0)
                return;
            m_layer = index;
            if (m_ent != null && m_ent.IsInited())
            {
                m_ent.SetLayer(index);
            }
        }

        public void SetOrder(int order)
        {
            m_order = order;
            if(m_ent != null && m_ent.IsInited())
            {
                m_ent.SetOrder(order);
            }
        }

        public void SetPos(Vector3 pos)
        {
            m_pos = pos;
            if (m_ent !=null && m_ent.IsInited())
            {
                m_ent.SetPos(pos);
            }
        }

        public void SetOffset(Vector3 vOffset)
        {
            m_offsetPos = vOffset;
        }

        public Vector3 GetDirection()
        {
            if (m_ent != null)
            {
                return m_ent.GetRotate();
            }

            return Vector3.zero;
        }

        public void SetDirection(Vector3 rota)
        {
            m_rota = rota;
            if (m_ent != null && m_ent.IsInited())
            {
                m_ent.SetDirection(rota);
            }
        }

        public void SetScale(Vector3 scale)
        {
            m_scale = scale;
        }

        public void SetBind(Transform bone)
        {
            m_parent = bone;
        }

        public void SetLoaded(EntityInitNotify loaded)
        {
            m_loaded = loaded;
        }

        /// <summary>
        /// 特效播放完成
        /// </summary>
        /// <param name="finished"></param>
        public void SetFinish(EffectFinishListenerEvent finished)
        {
            m_finished = finished;
        }

        public virtual void Update(float fTime, float fDTime)
        {
            if (IsInited() && m_bAutoDel && !m_isFinish)
            {
                m_curTime += fDTime;
                if (m_curTime >= m_maxTime)
                {
                    m_isFinish = true;
                    if (m_finished != null)
                        m_finished();
                }
            }
        }

        public void SetFinish()
        {
            m_isFinish = true;
        }

        public bool IsFinish()
        {
            return m_isFinish;
        }

        /// <summary>
        /// 默认加入缓存， 但是武器特效这些就不加入缓存了
        /// </summary>
        /// <param name="bCache"></param>
        public void Destory(bool bCache = true)
        {
            // 声音用自己的时间销毁
            //SoundManager.Inst.Stop(m_soundHandleId);
            // 后期内存太大，不做缓存了
            EntityManager.Inst.RemoveEntity(m_ent.m_handleID, false, false);
        }
    }
    // 特效作为一个实体对象，可以去控制它在场景中的位置，渲染等等。
    public class CEffectMgr
    {
        /// <summary>
        /// 创建基于场景位置的特效
        /// </summary>
        public static uint Create(uint effectId, Vector3 pos, Vector3 rota, EntityInitNotify initEnd)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectData effectData = effectCsv.GetEffect(effectId);
            if (null == effectData)
            {
                //Debug.LogError("特效不存在：" + effectId);
                return 0;
            }
            CEffect effect = new CEffect(effectData);
            effect.SetPos(pos);
            effect.SetDirection(rota);
            effect.SetLoaded(initEnd);
            uint uH = effect.InitConfigure();
            m_mapEffect.Add(uH, effect);
            return uH;
        }
        //大小
        public static uint Create(uint effectId, Vector3 pos, Vector3 rota, EntityInitNotify initEnd,Vector3 scale)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectData effectData = effectCsv.GetEffect(effectId);
            if (null == effectData)
            {
                //Debug.LogError("特效不存在：" + effectId);
                return 0;
            }
            CEffect effect = new CEffect(effectData);
            effect.SetPos(pos);
            effect.SetDirection(rota);
            effect.SetLoaded(initEnd);
            effect.SetScale(scale);
            uint uH = effect.InitConfigure();
            m_mapEffect.Add(uH, effect);
            return uH;
        }

        /// <summary>
        /// 创建基于玩家身体绑定点的特效,在这里通过外部的技能配置表传入战斗开始，击中等特效的绑定点
        /// </summary>
        public static uint Create(uint effectId, CCreature creature, string bindPoint)
        {
            // 这一层不用再绕到CCreature中处理，直接在这里绑定到角色实体上
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectData effectData = effectCsv.GetEffect(effectId);
            if (null == effectData)
            {
                //Debug.LogError("特效不存在：" + effectId);
                return 0;
            }
            CEffect effect = new CEffect(effectData);
            if (creature == null || creature.GetEntity() == null)
            {
                //Debug.LogError("特效:" + effectId + "绑点不存在");
                return 0;
            }
            //Transform bind = creature.GetBone(bindPoint);
            Vector3 offsetEffect= Vector3.zero;
            //if (bind.Equals("over_head")) offsetEffect = creature.GetEffectOffset();

            effect.SetOffset(offsetEffect);
            //effect.SetBind(bind);
            uint uH = effect.InitConfigure();
            m_mapEffect.Add(uH, effect);
            return uH;
        }

        /// <summary>
        /// 可选是否为战斗监听特效
        /// </summary>
        public static uint Create(uint effectId, CCreature creature, string bindPoint, bool bFightListenerList)
        {
            uint id = Create(effectId, creature, bindPoint);
            return id;
        }

        /// <summary>
        /// 创建基于UI的特效绑定
        /// </summary>     
        public static uint CreateUI(uint effectId, Transform uiBindPoint, int order = 0)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectData effectData = effectCsv.GetEffect(effectId);
            if (null == effectData)
            {
                Debug.Log("特效不存在：" + effectId);
                return 0;
            }
            CEffect effect = new CEffect(effectData);
            effect.SetLayer((int)LusuoLayer.eEL_UI);
            effect.SetOrder((int)order);
            Transform parent = UIItem.GetChild(uiBindPoint, "effect");
            if (parent == null) 
                parent = uiBindPoint;
            effect.SetBind(parent);
            uint uH = effect.InitConfigure();
            m_mapEffect.Add(uH, effect);
            return uH;
        }

        public static uint CreateUI(uint effectId, int order = 0)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectData effectData = effectCsv.GetEffect(effectId);
            if (null == effectData)
            {
                Debug.Log("特效不存在：" + effectId);
                return 0;
            }
            CEffect effect = new CEffect(effectData);
            effect.SetLayer((int)LusuoLayer.eEL_UI);
            effect.SetOrder((int)order);
            uint uH = effect.InitConfigure();
            m_mapEffect.Add(uH, effect);
            return uH;
        }

        public static uint Create(uint effectId, Transform uiBindPoint, EntityInitNotify initEnd = null, int layer = 0)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectData effectData = effectCsv.GetEffect(effectId);
            if (null == effectData)
            {
                //Debug.LogError("特效不存在：" + effectId);
                return 0;
            }
            CEffect effect = new CEffect(effectData);
            effect.SetBind(uiBindPoint);
            effect.SetLoaded(initEnd);
            effect.SetLayer(layer);
            uint uH = effect.InitConfigure();
            m_mapEffect.Add(uH, effect);
            return uH;
        }

        public static uint Create(uint effectId, Transform uiBindPoint, Vector3 pos , Vector3 scale)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectData effectData = effectCsv.GetEffect(effectId);
            if (null == effectData)
            {
                //Debug.LogError("特效不存在：" + effectId);
                return 0;
            }
            CEffect effect = new CEffect(effectData);
            effect.SetBind(uiBindPoint);
            effect.SetPos(pos);
            effect.SetScale(scale);
            uint uH = effect.InitConfigure();
            m_mapEffect.Add(uH, effect);
            return uH;
        }

        public static void Update(float fTime, float fDTime)
        {
            Dictionary<uint, CEffect>.Enumerator ms = m_mapEffect.GetEnumerator();
            while (ms.MoveNext())
            {
                m_tempListEffect.Add(ms.Current.Value);
            }

            for (int i = 0; i < m_tempListEffect.Count; i++)
            {
                CEffect ef = m_tempListEffect[i];
                if (!ef.IsFinish())
                {
                    ef.Update(fTime, fDTime);
                }
                else
                {
                    ef.Destory();
                    m_mapEffect.Remove(ef.m_ent.m_handleID);
                }
            }
            m_tempListEffect.Clear();
        }

        /// <summary>
        /// 让外部可以传入handleID
        /// </summary>
        /// <param name="uHangID"></param>
        /// <param name="effect"></param>
        public static void Add(uint uHangID, CEffect effect)
        {
            if(m_mapEffect.ContainsKey(uHangID))
            {
                //Debug.LogError("添加重复的技能特效");
                return;
            }
            m_mapEffect.Add(uHangID, effect);
        }

        /// <summary>
        /// 特效默认缓存，但是武器特效不加缓存
        /// </summary>
        public static void Destroy(uint handleId, bool bCache = true)
        {
            CEffect effect;
            if (m_mapEffect.TryGetValue(handleId, out effect))
            {
                if (effect.m_ent.IsInited())
                {
                    effect.Destory(bCache);
                    effect.m_effectData = null;
                    m_mapEffect.Remove(handleId);
                }
            }
        }

        /// <summary>
        /// 外部设置UID，并通过UID删除
        /// </summary>
        /// <param name="uid"></param>
        public static void DestroyByUid(uint uid)
        {
            foreach (KeyValuePair<uint, CEffect> item in m_mapEffect)
            {
                if(uid == item.Value.m_uid)
                {
                    Destroy(item.Value.m_ent.m_handleID);
                    return;
                }
            }
        }

        public static CEffect GetEffect(uint effectHandleId)
        { 
            CEffect effect;
            if (m_mapEffect.TryGetValue(effectHandleId, out effect))
            {
                return effect;
            }
            return null;
        }

        public static void Clear()
        {
            foreach (KeyValuePair<uint, CEffect> item in m_mapEffect)
            {
                item.Value.Destory();
            }
            m_mapEffect.Clear();
        }

        public static void ClearNotUI()
        {
            List<CEffect> list = new List<CEffect>(m_mapEffect.Values);
            for(int i = 0; i < list.Count; i ++)
            {
                if (list[i].m_layer != (int)LusuoLayer.eEL_UI)
                {
                    m_mapEffect.Remove(list[i].m_ent.m_handleID);
                    list[i].Destory();
                }
            }
        }

        //public static void OnLoadAllEffect(OnAllShaderLoaded loaded)
        //{
        //    m_onAllEffectLoaded = loaded;

        //    LoadingEffectCsv loadingEffectCsv = CsvManager.Inst.GetCsv<LoadingEffectCsv>((int)eAllCSV.eAC_LoadingEffect);
        //    if(loadingEffectCsv.m_dicData.Count == 0)
        //    {
        //        m_onAllEffectLoaded(m_curNum, m_maxNum);
        //    }
        //    else
        //    {
        //        for (int i = 0; i < loadingEffectCsv.m_dicData.Count; i++)
        //        {
        //            uint csvId = loadingEffectCsv.m_dicData[i];
        //            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
        //            EffectData effectData = effectCsv.GetEffect(csvId);
        //            if (effectData != null)
        //            {
        //                m_maxNum++;
        //                ResourceFactory.Inst.LoadResource(effectData.nResID, OnEffectLoaded, null);
        //            }
        //        }
        //    }
        //}

        //private static void OnEffectLoaded(Resource res)
        //{
        //    m_curNum++;
        //    m_onAllEffectLoaded(m_curNum, m_maxNum);
        //}

        private static Dictionary<uint, CEffect> m_mapEffect = new Dictionary<uint, CEffect>();
        // 临时销毁列表
        private static List<CEffect> m_tempListEffect = new List<CEffect>();

        private static EffectFinishListenerEvent m_fightListenerEvent;
        private static Dictionary<uint, CEffect> m_mapFightListener = new Dictionary<uint, CEffect>();
        //private static bool m_bHandleFLEvent = false;

        // 第一次loading需要加载的
        //private static OnAllShaderLoaded m_onAllEffectLoaded;
        private static int m_curNum;
        private static int m_maxNum;
    }
}
