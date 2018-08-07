using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngineInternal;

namespace Roma
{
    public class CEffect
    {
        private Entity m_ent;
        private EffectData m_effectData;   // 如果是场景特效，需要控制声音的销毁，UI特效就不控制
        public float m_curTime;
        public float m_maxTime;
        // 默认是自动删除，如果开始时间是0，那么就是需要手动删除
        public bool m_isFinish = false;
        private bool m_bAutoDel = true;

        public BoneEntity m_parentEnt; // 挂载的父对象
        private Action<CEffect> m_playEnd;

        private Entity m_soundEnt;

        public CEffect(int csvId, Vector3 start, Action<Entity> loaded = null)
        {
            Create(csvId, start, loaded);
        }

        public CEffect(int csvId, Action<Entity> loaded = null)
        {
            if (csvId == 0)
                return;
            Create(csvId, Vector3.zero, loaded);
        }

        private void Create(int csvId, Vector3 startPos, Action<Entity> loaded = null)
        {
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            m_effectData = effectCsv.GetData(csvId);
            if (m_effectData == null || m_effectData.nResID == 0)
            {
                Debug.LogError("找不到特效id:" + csvId);
                return;
            }
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_vPos = startPos;
            info.m_resID = m_effectData.nResID;
            int handldId = EntityManager.Inst.CreateEntity(eEntityType.eEffectEntity, info, loaded);
            m_ent = EntityManager.Inst.GetEnity(handldId);

            m_maxTime = m_effectData.fLiveTime;
            if (m_maxTime.Equals(0.0f))
            {
                m_bAutoDel = false;
            }

            //int soundHid = SoundManager.Inst.PlaySound(m_effectData.uSoundID, startPos);
            //m_soundEnt = EntityManager.Inst.GetEnity(soundHid);

        }

        public void AddPlayEnd(Action<CEffect> end)
        {
            m_playEnd = end;
        }

        public virtual void Update(float fTime, float fDTime)
        {
            if (m_ent.IsInited() && m_bAutoDel && !m_isFinish)
            {
                m_curTime += fDTime;
                if (m_curTime >= m_maxTime)
                {
                    m_isFinish = true;
                    if (m_playEnd != null)
                        m_playEnd(this);
                }
                // 声音位置和特效位置一致
                if (m_soundEnt != null)
                {
                    m_soundEnt.SetPos(m_ent.GetPos());
                }
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
            // if effect have parent then parent object need clear bind objects
            if (m_parentEnt != null)
                m_parentEnt.RemoveBindObject(m_ent);
            //EntityManager.Inst.RemoveEntity(m_ent.m_hid, true);
            //if (m_soundEnt != null)
            //{
            //    SoundManager.Inst.Remove(m_soundEnt.m_hid);
            //}
        }

        public void SetBind(int bindHandleId, string bingPoint)
        {
            BoneEntity bindEnt = (BoneEntity)EntityManager.Inst.GetEnity(bindHandleId);
            Transform bindTransform = bindEnt.GetBone(bingPoint);
            if (bindTransform == null)
            {
                m_ent.SetPos(bindEnt.GetPos());
                //ent.SetParent(bindEnt.GetObject().transform);
            }
            else
            {
                m_ent.SetParent(bindTransform);
            }
        }

        public void SetLayer(LusuoLayer lay)
        {
            m_ent.SetLayer(lay);
        }

        public void SetShow(bool bShow)
        {
            m_ent.SetShow(bShow);
        }
    }

    public class CEffectMgr
    {
        public static int Create(int effectId, Vector3 pos, Vector3 rota, Action<Entity> loaded = null)
        {
            if (effectId == 0)
                return 0;
            CEffect effect = new CEffect(effectId, pos, loaded);
            Entity ent = effect.GetEntity();
            ent.SetLayer((int)LusuoLayer.eEL_Dynamic);
            ent.SetPos(pos);
            ent.SetDirection(rota);
            m_mapEffect.Add(ent.m_hid, effect);
            return ent.m_hid;
        }

        /// <summary>
        /// 创建基于绑定实体身上的特效
        /// </summary>     
        public static int Create(int effectId, int bindHandleId, string bingPoint, Action<Entity> loaded = null)
        {
            if (effectId == 0)
                return 0;
            CEffect effect = new CEffect(effectId, loaded);
            Entity ent = effect.GetEntity();
            if (ent == null)
            {
                return 0;
            }
            ent.SetLayer((int)LusuoLayer.eEL_Dynamic);

            BoneEntity bindEnt = (BoneEntity)EntityManager.Inst.GetEnity(bindHandleId);
            Transform bindTransform = bindEnt.GetBone(bingPoint);
            if (bindTransform == null)
            {
                ent.SetPos(bindEnt.GetPos());
                //ent.SetParent(bindEnt.GetObject().transform);
            }
            else
            {
                ent.SetParent(bindTransform);
                effect.m_parentEnt = bindEnt;
                bindEnt.AddBindObject(ent);
            }
            m_mapEffect.Add(ent.m_hid, effect);
            return ent.m_hid;
        }

        /// <summary>
        /// 创建基于实体位置跟随的特效
        /// </summary>
        public static int CreateByCreaturePos(int effectId, int bindHandleId, Action<Entity> loaded = null)
        {
            if (effectId == 0)
                return 0;
            CEffect effect = new CEffect(effectId, loaded);
            Entity ent = effect.GetEntity();
            if (ent == null)
            {
                return 0;
            }
            ent.SetLayer((int)LusuoLayer.eEL_Dynamic);

            BoneEntity bindEnt = (BoneEntity)EntityManager.Inst.GetEnity(bindHandleId);
            effect.m_parentEnt = bindEnt;
            bindEnt.AddBindObject(ent);

            m_mapEffect.Add(ent.m_hid, effect);

            m_followPos.Add(effect, bindEnt);
            return ent.m_hid;
        }


        /// <summary>
        /// 创建基于UI的特效绑定
        /// </summary>     
        public static int CreateUI(int effectId, Transform uiBindPoint, int order = 0, Action<CEffect> playend = null)
        {
            if (effectId == 0)
                return 0;
            CEffect effect = new CEffect(effectId, null);
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
                    m_followPos.Remove(ef);
                    m_mapEffect.Remove(ef.GetEntity().m_hid);
                    ef.Destory();
                }
            }
            m_tempListEffect.Clear();

            foreach (KeyValuePair<CEffect, Entity> item in m_followPos)
            {
                Entity ent = item.Key.GetEntity();
                if (ent.IsInited())
                {
                    ent.SetPos(item.Value.GetPos());
                }
            }
        }

        private static Dictionary<int, CEffect> m_mapEffect = new Dictionary<int, CEffect>();
        // 临时销毁列表
        private static List<CEffect> m_tempListEffect = new List<CEffect>();

        /// <summary>
        /// 跟随角色位置的特效
        /// </summary>
        private static Dictionary<CEffect, Entity> m_followPos = new Dictionary<CEffect, Entity>();



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
