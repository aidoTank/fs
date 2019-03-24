using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    public enum eEntityType
    {
        //eStaticEntity,
        //eDynamicEntity,
        eNone,
        eBoneEntity,
        eBattleEntity,
        eSceneEntity,
        eEffectEntity,
        eSoundEntity,
    }

    // 用于区分实体的作用范围
    public enum eEntityTag
    {
        eScene = 0,
        eSmallGame,
    }

    public enum eUserData
    {
        Uid,
        Type,
    }


    public class Entity
    {
        private Action<Entity> m_entityInitNofity;

        public int m_hid;
        public eEntityType m_entityType = eEntityType.eBoneEntity;
        public GameObject m_object;
        public Transform m_transform;
        private Resource m_res;          // 每一个实体对应一个Resource类，调用资源工厂加载，也会调用资源工厂销毁
        private float m_loadProcess;
        private bool m_bNeedLoadResource = true;
        private bool m_LoadingDestory = false;  // 下载中需删除
        public EntityBaseInfo m_entityInfo;

        // 渲染器是不可能没有的，开始new没问题
        public List<Renderer> m_listRenderer = new List<Renderer>();
        protected List<Color> m_originalColorList;      //原颜色
        protected List<Shader> m_originalShaderList;    // 原shader

        // 设置特效层级
        protected int[] m_orderList; // 渲染层级


        public Entity(int handle, Action<Entity> notity, eEntityType tpye, EntityBaseInfo baseInfo)
        {
            m_entityInitNofity = notity;
            m_hid = handle;
            m_entityType = tpye;
            m_entityInfo = baseInfo;
            m_LoadingDestory = false;
            //Debug.LogError("请求加载资源。。。。。。。。。。。。"+ m_entityInfo.m_resID);
            if (!string.IsNullOrEmpty(m_entityInfo.m_strName))
            {
                m_res = ResourceFactory.Inst.LoadResource(m_entityInfo.m_strName, ResourceLoaded);
            }
            else
            {
                m_entityInfo.m_strName = ResInfosResource.GetResInfo(m_entityInfo.m_resID).strName;
                m_res = ResourceFactory.Inst.LoadResource(m_entityInfo.m_resID, ResourceLoaded);
            }
        }

        public virtual void Revive(int handleId, Action<Entity> notity, EntityBaseInfo baseInfo)
        {
            if (m_res == null)
                return;
            m_hid = handleId;
            m_entityInitNofity = notity;
            m_entityInfo = baseInfo;
            SetBaseInfo();  // 重复利用的对象池时，不用再去获取组件信息，设置基本信息就行
            OnInited();
        }

        public virtual void Update(float fTime, float fDTime)
        {
            if (m_bNeedLoadResource)
            {
                if (m_res != null)
                {
                    m_loadProcess = m_res.GetDownLoadProcess();
                }
            }
        }

        public float GetLoadProcess()
        {
            return m_loadProcess;
        }

        public Resource GetResource()
        {
            return m_res;
        }

        private void ResourceLoaded(Resource res)
        {
            if (null == res)
            {
                Debug.LogError("配置了id和名称，但是没有资源，如果是无用资源请删除相关配置。id:" + m_entityInfo.m_resID);
                return;
            }
            //Debug.LogError("完成加载资源。。。。。。。。。。。。" + res.GetResInfo().nResID + " m_LoadingDestory :" + m_LoadingDestory);
            if (m_LoadingDestory)
            {
                ResourceFactory.Inst.UnLoadResource(m_res);
                return;
            }
            m_res = res;
            InstantiateGameObject(m_res);
            UpdateBaseInfo(m_res);
            OnInited();
            m_bNeedLoadResource = false;
        }

        public virtual void InstantiateGameObject(Resource res)
        {
            if (null == m_object)
            {
                m_object = res.InstantiateGameObject();
                if (m_object != null)
                    m_transform = m_object.transform;
            }
        }

        /// <summary>
        /// 执行一次
        /// </summary>
        public virtual void UpdateBaseInfo(Resource res)
        {
            if (m_object == null)
            {
                Debug.Log("资源为空:" + m_entityInfo.m_resID);
                return;
            }
            // 获取渲染器
            if (m_listRenderer.Count <= 0)
            {
                Renderer[] render = m_object.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < render.Length; i++)
                {
                    Renderer re = render[i];
                    m_listRenderer.Add(re);
                }
            }

            if (Application.isEditor)
            {
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    for (int j = 0; j < m_listRenderer[i].materials.Length; j++)
                    {
                        m_listRenderer[i].materials[j].shader = Shader.Find(m_listRenderer[i].materials[j].shader.name);
                    }
                }
            }
            SetBaseInfo();
        }

        public virtual void SetBaseInfo()
        {
            SetPos(m_entityInfo.m_vPos);
            SetParent(m_entityInfo.m_parent);
            SetDirection(m_entityInfo.m_vRotate);
            SetScale(m_entityInfo.m_vScale);
            SetShow(m_entityInfo.m_active);
            SetOrder(m_entityInfo.m_order);
            SetLayer(m_entityInfo.m_ilayer);
        }

        public virtual void OnInited()
        {
            if (null != m_entityInitNofity)
            {
                m_entityInitNofity(this);
                m_entityInitNofity = null;
            }
        }

        public virtual bool IsInited()
        {
            return m_object != null;
        }

        public virtual void Destroy()
        {
            if (m_bNeedLoadResource)    // 下载中,并且上层执行了销毁事件
            {
                //Debug.LogError("下载中就要删除的？Destroy。。。。。。。。。。。" + m_entityInfo.m_resID);
                m_LoadingDestory = true;
                return;
            }
            if (null != m_object)
            {
                //Debug.LogError("销毁对戏。。。。。。。。。。。" + m_object.name);
                // 移除shader
                m_listRenderer.Clear();
                m_listRenderer = null;
                if (m_originalColorList != null)
                {
                    m_originalColorList.Clear();
                    m_originalColorList = null;

                }
                if (m_originalShaderList != null)
                {
                    m_originalShaderList.Clear();
                    m_originalShaderList = null;
                }
                m_orderList = null;

                m_transform = null;
                GameObject.Destroy(m_object);
                ResourceFactory.Inst.UnLoadResource(m_res);
                m_res = null;
                m_object = null;
            }
        }

        public void SetColor(Color color)
        {
            if (m_listRenderer == null)
            {
                return;
            }
            GetOriginalColor();
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null)
                {
                    if (m_listRenderer[i].material.HasProperty("_TintColor"))
                    {
                        m_listRenderer[i].material.SetColor("_TintColor", color);
                    }
                }
            }
        }

        private void GetOriginalColor()
        {
            if (m_originalColorList == null)
                m_originalColorList = new List<Color>();
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                Color oldColor = Color.white;
                if (m_listRenderer[i].material.HasProperty("_TintColor"))
                {
                    oldColor = m_listRenderer[i].material.GetColor("_TintColor");
                }
                m_originalColorList.Add(oldColor);
            }
        }

        public void RestoreColor()
        {
            if (m_originalColorList == null || m_originalColorList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null)
                {
                    if (m_listRenderer[i].material.HasProperty("_TintColor"))
                    {
                        if (i < m_originalColorList.Count)
                            m_listRenderer[i].material.SetColor("_TintColor", m_originalColorList[i]);
                    }
                }
            }
        }

        /// <summary>
        ///可以把shader接口统一为一个
        ///time：当前shader的变化时间
        ///bAutoEnd：自动结束并移除
        /// </summary>
        //public void SetShader(eShaderType type, Color color, float time = 9999999, bool bAutoEnd = true, Action end = null)
        //{
        //    GetOriginalColor();
        //    if (m_originalShaderList == null)
        //    {
        //        m_originalShaderList = new List<Shader>();
        //        for (int i = 0; i < m_listRenderer.Count; i++)
        //        {
        //            m_originalShaderList.Add(m_listRenderer[i].material.shader);
        //        }
        //    }

        //    EntityShaderInfo info = new EntityShaderInfo();
        //    info.entity = this;
        //    info.color = color;
        //    info.bAutoEnd = bAutoEnd;
        //    info.showTime = time;
        //    info.type = type;
        //    info.showEnd = end;
        //    ShaderManager.Inst.AddShader(info);
        //}

        //public void RemoveShader()
        //{
        //    ShaderManager.Inst.RemoveShader(m_hid);
        //}

        public void RestoreShader()
        {
            if (m_originalShaderList == null || m_originalShaderList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null && m_originalShaderList[i] != null)
                    m_listRenderer[i].material.shader = m_originalShaderList[i];
            }
            RestoreColor();
        }

        public virtual void SetLayer(LusuoLayer layer)
        {
            m_entityInfo.m_ilayer = (int)layer;
            if (null != m_object)
            {
                m_object.gameObject.layer = (int)layer;
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    m_listRenderer[i].gameObject.layer = (int)layer;
                }
            }
        }

        public virtual void SetLayer(int layer)
        {
            m_entityInfo.m_ilayer = layer;
            if (null != m_object)
            {
                m_object.gameObject.layer = layer;
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    if(m_listRenderer[i] == null)
                        continue;
                    m_listRenderer[i].gameObject.layer = (int)layer;
                }
            }
        }

        /// <summary>
        /// 将这个对象绑定到parent上
        /// </summary>
        public virtual void SetParent(Transform parent, bool bResetAngle = true)
        {
            if (parent == null)
                return;
            m_entityInfo.m_parent = parent;
            if (null != m_object)
            {
                m_transform.SetParent(parent);
                m_transform.localPosition = Vector3.zero;
                m_transform.localScale = Vector3.one;
                if (bResetAngle)
                {
                    m_transform.localEulerAngles = Vector3.zero;
                }
            }
        }
        public void ClearBind()
        {
            if (null != m_object)
            {
                m_transform.SetParent(null);
            }
        }

        public void SetPos(Vector3 pos)
        {
            m_entityInfo.m_vPos = pos;
            if (null != m_transform)
            {
                //Debug.Log(m_transform.name + " pos:" + pos);
                m_transform.position
                    = new Vector3(pos.x, pos.y, pos.z);
            }
        }

        public void SetLocalPos(Vector3 pos)
        {
            m_entityInfo.m_vPos = pos;
            if (null != m_transform)
            {
                m_transform.localPosition
                    = new Vector3(pos.x, pos.y, pos.z);
            }
        }

        public void SetDirection(Vector3 rotate)
        {
            m_entityInfo.m_vRotate = rotate;
            if (null != m_transform)
            {
                m_transform.localEulerAngles
                    = new Vector3(rotate.x, rotate.y, rotate.z);
            }
        }

        public void SetRot(Quaternion rotate)
        {
            m_entityInfo.m_vRotate = rotate.eulerAngles;
            if (null != m_transform)
            {
                m_transform.rotation = rotate;
            }
        }

        public void SetScale(float scale, bool bConChild = false)
        {
            m_entityInfo.m_vScale.x = scale;
            m_entityInfo.m_vScale.y = scale;
            m_entityInfo.m_vScale.z = scale;
            if (null != m_transform)
            {
                if (bConChild)
                {
                    Transform[] itemList = m_transform.GetComponentsInChildren<Transform>();
                    for (int i = 0; i < itemList.Length; i++)
                    {
                        itemList[i].localScale = new Vector3(scale, scale, scale);
                    }
                }
                else
                {
                    m_transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
        public void SetScale(Vector3 scale)
        {
            m_entityInfo.m_vScale = scale;
            if (null != m_transform)
            {
                m_transform.localScale
                    = new Vector3(scale.x, scale.y, scale.z);
            }
        }

        // 排序层只用设置一次
        public void SetOrder(int layer)
        {
            m_entityInfo.m_order = layer;
            if (null == m_transform)
                return;
            if (m_orderList == null)
            {
                m_orderList = new int[m_listRenderer.Count];
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    if(m_listRenderer[i] == null)
                        continue;
                    m_orderList[i] = m_listRenderer[i].sortingOrder;
                }
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if(m_listRenderer[i] == null)
                    continue;
                m_listRenderer[i].sortingOrder = layer + m_orderList[i];
            }
        }

        public void SetRenderQueue(int queue)
        {
            if (null == m_transform)
                return;
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                for (int j = 0; j < m_listRenderer[i].materials.Length; j++)
                {
                    m_listRenderer[i].materials[j].renderQueue = queue;
                }
            }
        }

        public EntityBaseInfo GetEntityBaseInfo()
        {
            return m_entityInfo;
        }

        public int GetLayer()
        {
            if (null != m_transform)
            {
                return m_entityInfo.m_ilayer;
            }
            return (int)LusuoLayer.eEL_Default;
        }

        public virtual void SetShow(bool bActive)
        {
            m_entityInfo.m_active = bActive;
            if (null == m_transform)
                return;
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if(m_listRenderer[i] == null)
                    continue;
                m_listRenderer[i].enabled = bActive;
            }
        }

        public bool GetActive()
        {
            return m_entityInfo.m_active;
        }

        public Vector3 GetBoxCenter()
        {
            if (m_object != null)
            {
                BoxCollider box = m_object.GetComponent<BoxCollider>();
                if (box != null)
                    return box.center;
            }
            return Vector3.zero;
        }

        public Vector3 GetBoxSize()
        {
            if (m_object != null)
            {
                BoxCollider box = m_object.GetComponent<BoxCollider>();
                if (box != null)
                    return box.size;
            }
            return Vector3.zero;
        }

        public void RemoveBoxCollider()
        {
            if (m_object == null)
                return;
            if (m_object.GetComponent<BoxCollider>() == null)
            {
                return;
            }
            GameObject.Destroy(m_object.GetComponent<BoxCollider>());
        }

        public GameObject GetObject()
        {
            return m_object;
        }
        public virtual Vector3 GetPos()
        {
            if (null != m_transform)
            {
                return m_transform.position;
            }
            return m_entityInfo.m_vPos;
        }
        public virtual Quaternion GetRotateQua()
        {
            if (m_transform == null)
            {
                return Quaternion.identity;
            }
            return m_transform.rotation;
        }
        public virtual Vector3 GetRotate()
        {
            if (null != m_transform)
            {
                return m_transform.localEulerAngles;
            }
            return m_entityInfo.m_vRotate;
        }
        public virtual Vector3 GetScale()
        {
            if (m_transform == null)
            {
                return Vector3.one;
            }
            return m_transform.localScale;
        }

        public eEntityType GetEntityType()
        {
            return m_entityType;
        }
    }
}
