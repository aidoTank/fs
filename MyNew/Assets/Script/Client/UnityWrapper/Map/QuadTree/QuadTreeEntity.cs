using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class QuadTreeEntity : IQtUserData
    {
        public EntityBaseInfo m_baseInfo;
        private uint m_handle;
        private Entity m_ent;

        public Vector3 GetCenter()
        {
            return m_baseInfo.m_vPos;
        }

        /// <summary>
        /// 获取边界框的一半，其实这里直接就1米即可，也不用再去计算
        /// </summary>
        public Vector3 GetExtends()
        {
            return Vector2.one;
        }

        public void SwapIn()
        {
            if (m_ent == null)
            {
                m_handle = EntityManager.Inst.CreateEntity(eEntityType.eStaticEntity, OnLoadedStaticEntity, m_baseInfo);
                m_ent = EntityManager.Inst.GetEnity(m_handle, true);
            }
        }

        private void OnLoadedStaticEntity(Entity ent, object obj)
        {
            ent.GetObject().transform.SetParent(SceneManager.Inst.GetSceneRoot().transform);
        }

        public void SwapOut()
        {
            if (m_ent == null || m_handle == 0)
                return;

            EntityManager.Inst.RemoveEntity(m_handle, true);
            m_handle = 0;
            m_ent = null;
        }

        public bool IsSwapInCompleted()
        {
            if (m_ent == null || !m_ent.IsInited())
                return false;

            return m_ent.GetActive();
        }

        public bool IsSwapOutCompleted()
        {
            if (m_ent == null || !m_ent.IsInited())
                return false;

            return !m_ent.GetActive();
        }

        public void Destory()
        {
            SwapOut();
        }
    }
}
