using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngineInternal;

namespace Roma
{
    /// <summary>
    /// 如果是场景特效需循环的，时间配0，并且声音配循环，同特效一起销毁
    /// </summary>
    public class CEffect
    {
        private Entity m_ent;
        private EffectCsvData m_effectData;
        private EntityBaseInfo m_entBaseInfo;
        public float m_curTime;
        public float m_maxTime;
        // 默认是自动删除，如果开始时间是0，那么就是需要手动删除
        public bool m_isFinish = false;
        private bool m_bAutoDel = true;

        public BoneEntity m_parentEnt; // 挂载的父对象
        private Action<CEffect> m_playEnd;

        private bool m_playSound;
        private Entity m_soundEnt;


        public float m_offsetPos; // 跟随位置的偏移位置
        public int m_followType;  // 跟随类型的特效，1脚底，2头顶


        public CEffect(int csvId, EntityBaseInfo entInfo, Action<Entity> loaded = null)
        {
            m_entBaseInfo = entInfo;
            Create(csvId, m_entBaseInfo, loaded);
        }

        public CEffect(int csvId, Action<Entity> loaded = null)
        {
            if (csvId == 0)
                return;
            Create(csvId, m_entBaseInfo, loaded);
        }

        private void Create(int csvId, EntityBaseInfo entInfo, Action<Entity> loaded = null)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            m_effectData = effectCsv.GetData(csvId);
            if (m_effectData == null || m_effectData.resId == 0)
            {
                Debug.LogError("找不到特效id:" + csvId);
                return;
            }
            if (entInfo == null)
                entInfo = new EntityBaseInfo();
            entInfo.m_resID = m_effectData.resId;
            int handldId = EntityManager.Inst.CreateEntity(eEntityType.eEffectEntity, entInfo, loaded);
            m_ent = EntityManager.Inst.GetEnity(handldId);

            m_maxTime = m_effectData.lifeTime;
            if (m_maxTime.Equals(0.0f))
            {
                m_bAutoDel = false;
            }

            //if (GameManager.Instance.m_speedUpInLoading)
            //    return;
            if (m_effectData == null)
                return;
            int soundHid = SoundManager.Inst.PlaySound(m_effectData.soundId, m_ent.GetPos());
            m_soundEnt = EntityManager.Inst.GetEnity(soundHid);
            if (m_effectData.shakeCamera == 1)
            {
                CameraMgr.Inst.OnShake();
            }
        }

        public void AddPlayEnd(Action<CEffect> end)
        {
            m_playEnd = end;
        }

        public virtual void Update(float fTime, float fDTime)
        {
            //if (!m_ent.IsInited())
            //    return;

            if (m_bAutoDel && !m_isFinish)
            {
                m_curTime += fDTime;
                if (m_curTime >= m_maxTime)
                {
                    m_isFinish = true;
                    if (m_playEnd != null)
                        m_playEnd(this);
                }
            }

            // 声音位置和特效位置一致
            if (m_soundEnt != null && m_soundEnt.IsInited())
            {
                m_soundEnt.SetPos(m_ent.GetPos());
            }
        }

        public bool IsFinish()
        {
            return m_isFinish;
        }

        public Entity GetEntity()
        {
            return m_ent;
        }

        public void Destory()
        {
            if (m_parentEnt != null)
                m_parentEnt.RemoveBindObject(m_ent);
            EntityManager.Inst.RemoveEntity(m_ent.m_hid, true);
            if (m_soundEnt != null)
            {
                SoundManager.Inst.Remove(m_soundEnt.m_hid);
            }
        }

        public void SetBind(int bindHandleId, string bingPoint)
        {
            if (m_parentEnt != null)
                m_parentEnt.RemoveBindObject(m_ent);  // 移除之前的绑定

            BoneEntity bindEnt = (BoneEntity)EntityManager.Inst.GetEnity(bindHandleId);
            if (bindEnt == null || bindEnt.GetObject() == null)
                return;

            Transform bindTransform = bindEnt.GetBone(bingPoint);
            if (bindTransform == null)
            {
                //m_ent.SetPos(bindEnt.GetPos());
                bindTransform = bindEnt.GetObject().transform;
            }
            //else
            //{
            m_ent.SetParent(bindTransform);
            m_parentEnt = bindEnt;
            bindEnt.AddBindObject(m_ent);
            //}
        }

        public void SetLayer(LusuoLayer lay)
        {
            m_ent.SetLayer(lay);
        }

        public void SetShow(bool bShow)
        {
            m_ent.SetShow(bShow);
        }

        /// <summary>
        /// 拖尾特效在最后要脱离跟随
        /// </summary>
        public void ClearFollow()
        {
            if (CEffectMgr.m_followPos.ContainsKey(this))
            {
                CEffectMgr.m_followPos.Remove(this);
            }
        }
    }

    public class CEffectMgr
    {
        public static int Create(int effectId, Vector3 pos, Vector3 rota, Action<Entity> loaded = null)
        {
            if (effectId == 0)
                return 0;
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectCsvData m_effectData = effectCsv.GetData(effectId);
            if (m_effectData == null)
            {
                Debug.LogError("特效为空：" + effectId);
                return 0;
            }
            EntityBaseInfo entInfo = new EntityBaseInfo();
            entInfo.m_resID = m_effectData.resId;
            entInfo.m_vPos = pos;
            entInfo.m_vRotate = rota;
            CEffect effect = new CEffect(effectId, entInfo, loaded);
            Entity ent = effect.GetEntity();
            ent.SetLayer((int)LusuoLayer.eEL_Dynamic);
            ent.SetPos(pos);
            ent.SetDirection(rota);
            m_mapEffect.Add(ent.m_hid, effect);
            return ent.m_hid;
        }

        /// <summary>
        /// 创建基于绑定实体身上的特效,和角色一起显隐，无绑定点的在角色位置，不和角色一起显隐,比如刺客的施法特效
        /// 挂载特效可能需要缩放，在设置挂载之后
        /// </summary>     
        public static int Create(int effectId, Entity newEnt, string bingPoint, Action<Entity> loaded = null, float scale = 1.0f)
        {
            if (effectId == 0)
                return 0;
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectCsvData m_effectData = effectCsv.GetData(effectId);
            if (m_effectData == null)
                return 0;
            EntityBaseInfo baseInfo = new EntityBaseInfo();
            baseInfo.m_resID = m_effectData.resId;
            baseInfo.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            baseInfo.m_vScale = Vector3.one * scale;
            // 获取绑定点
            BoneEntity boneEnt = newEnt as BoneEntity;
            Transform bindTransform = boneEnt.GetBone(bingPoint);

            CEffect effect;
            Entity ent;
            if (bindTransform == null)
            {
                //baseInfo.m_vPos = boneEnt.GetPos();
                //baseInfo.m_vRotate = boneEnt.GetRotate();
                //effect = new CEffect(effectId, baseInfo, loaded);
                //ent = effect.GetEntity();
                GameObject obj = boneEnt.GetObject();
                if (obj == null)
                    return 0;
                bindTransform = obj.transform;
            }
            //else
            //{
            baseInfo.m_parent = bindTransform;
            effect = new CEffect(effectId, baseInfo, loaded);
            ent = effect.GetEntity();
            effect.m_parentEnt = boneEnt;
            boneEnt.AddBindObject(ent);
            //}

            if (ent == null)
                return 0;
            m_mapEffect.Add(ent.m_hid, effect);
            return ent.m_hid;
        }

        /// <summary>
        /// 创建基于实体位置跟随的特效，followType一般就2种
        /// 1.脚底
        /// 2.头顶
        /// </summary>
        public static int CreateByCreaturePos(int effectId, Entity newEnt, int followType, Action<Entity> loaded = null)
        {
            if (effectId == 0)
                return 0;
            //BoneEntity bindEnt = (BoneEntity)EntityManager.Inst.GetEnity(bindHandleId);
            // 获取绑定点
            BoneEntity bindEnt = newEnt as BoneEntity;
            EntityBaseInfo baseInfo = new EntityBaseInfo();
            baseInfo.m_vPos = bindEnt.GetPos();
            CEffect effect = new CEffect(effectId, baseInfo, loaded);
            Entity ent = effect.GetEntity();
            if (ent == null)
            {
                return 0;
            }
            ent.SetLayer((int)LusuoLayer.eEL_Dynamic);

            effect.m_parentEnt = bindEnt;
            effect.m_followType = followType;
            bindEnt.AddBindObject(ent);

            m_mapEffect.Add(ent.m_hid, effect);

            m_followPos.Add(effect, bindEnt);
            return ent.m_hid;
        }


        public static int Create(int effectId, Transform uiBindPoint)
        {
            if (effectId == 0)
                return 0;
            EntityBaseInfo baseInfo = new EntityBaseInfo();
            baseInfo.m_parent = uiBindPoint;
            baseInfo.m_vScale = Vector3.one;

            CEffect effect = new CEffect(effectId, baseInfo, null);
            Entity ent = effect.GetEntity();
            ent.SetLayer((int)LusuoLayer.eEL_Dynamic);
            ent.SetParent(uiBindPoint);
            m_mapEffect.Add(ent.m_hid, effect);
            return ent.m_hid;
        }

        /// <summary>
        /// 创建基于UI的特效绑定
        /// </summary>     
        public static int CreateUI(int effectId, Transform uiBindPoint, int order = 0, Action<CEffect> playend = null, Action<Entity> loaded = null, float scale = 1.0f)
        {
            if (effectId == 0)
                return 0;
            EntityBaseInfo baseInfo = new EntityBaseInfo();
            baseInfo.m_parent = uiBindPoint;
            baseInfo.m_vScale = new Vector3(scale, scale, scale);

            CEffect effect = new CEffect(effectId, baseInfo, loaded);
            Entity ent = effect.GetEntity();
            ent.SetLayer((int)LusuoLayer.eEL_UI);
            ent.SetOrder(order);
            ent.SetParent(uiBindPoint);
            m_mapEffect.Add(ent.m_hid, effect);
            effect.AddPlayEnd(playend);
            return ent.m_hid;
        }

        public static void Destroy(int hid)
        {
            if (hid == 0)
                return;
            CEffect effect;
            if (m_mapEffect.TryGetValue(hid, out effect))
            {
                //Debug.Log("移除特效:" + effect.GetEntity().m_transform.name + " hid:" + hid);
                if (m_followPos.ContainsKey(effect))
                {
                    m_followPos.Remove(effect);
                }

                effect.Destory();
                effect = null;
                m_mapEffect.Remove(hid);
            }
        }

        public static CEffect GetEffect(int handleId)
        {
            CEffect effect;
            if (m_mapEffect.TryGetValue(handleId, out effect))
            {
                return effect;
            }
            return null;
        }

        public static void Update(float fTime, float fDTime)
        {
            Dictionary<int, CEffect>.Enumerator ms = m_mapEffect.GetEnumerator();
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
                    //m_mapEffect.Remove(ef.GetEntity().m_hid);
                    //ef.Destory();
                    Destroy(ef.GetEntity().m_hid);
                }
            }
            m_tempListEffect.Clear();

            foreach (KeyValuePair<CEffect, Entity> item in m_followPos)
            {
                Entity ent = item.Key.GetEntity();
                if (ent.IsInited())
                {
                    if (item.Key.m_followType == 1)
                    {
                        ent.SetPos(item.Value.GetPos());
                    }
                    else
                    {
                        ent.SetPos(item.Value.GetPos() + Vector3.up * item.Value.m_headPos);
                    }

                }
            }
        }

        public static void UnInit()
        {
            m_mapEffect.Clear();
            m_tempListEffect.Clear();
            m_followPos.Clear();
        }

        private static Dictionary<int, CEffect> m_mapEffect = new Dictionary<int, CEffect>();
        // 临时销毁列表
        private static List<CEffect> m_tempListEffect = new List<CEffect>();

        /// <summary>
        /// 跟随角色位置的特效
        /// </summary>
        public static Dictionary<CEffect, Entity> m_followPos = new Dictionary<CEffect, Entity>();



        public static string GetEffectInfo()
        {
            string info = "\n";
            int num = 0;
            foreach (KeyValuePair<int, CEffect> item in m_mapEffect)
            {
                Entity ent = item.Value.GetEntity();
                if (ent == null)
                {
                    info += "当前特效的ent为空" + "\n";
                }
                else
                {
                    if (ent.GetObject() == null)
                    {
                        info += num + ".当前特效的object为空了" + " " + ent.GetEntityBaseInfo().m_strName + "\n";
                    }
                    else
                    {
                        info += num + "." + ent.GetObject().name + "\n";
                    }
                }
                num++;
            }
            return " 数量[" + num + "]:" + info;
        }
    }
}
