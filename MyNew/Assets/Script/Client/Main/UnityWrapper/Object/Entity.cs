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

    public partial class Entity
    {
        private Action<Entity> m_entityInitNofity;

        public int m_hid;
        public eEntityType m_entityType;
        public GameObject m_object;
        public Transform m_transform;
        private Resource m_res;          // 每一个实体对应一个Resource类，调用资源工厂加载，也会调用资源工厂销毁
        private float m_loadProcess;
        private bool m_bNeedLoadResource = true;
        private bool m_LoadingDestory;  // 下载中需删除
        public EntityBaseInfo m_entityInfo;

        // 渲染器是不可能没有的，开始new没问题
        public List<Renderer> m_listRenderer = new List<Renderer>();
        protected List<Color> m_originalColorList;      //原颜色
        protected List<Shader> m_originalShaderList;    // 原shader

        // 设置特效层级
        protected int[] m_orderList; // 渲染层级
        private bool m_bRevive;  // 下一帧执行重置回调
        public Transform m_highObject;  // 高画质对象
        private Vector3 m_hightPos;  // 高画质对象初始位置

        public float m_headPos; // 头顶位置，因为外部逻辑，这个位置会变动


        public Entity(int handle, Action<Entity> notity, eEntityType tpye, EntityBaseInfo baseInfo)
        {
            if (baseInfo.m_resID == 0)
            {
                Debug.LogError("资源id为0，请检查配置");
                return;
            }
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
                ResInfo resInfo = ResInfosResource.GetResInfo(m_entityInfo.m_resID);
                if (resInfo != null && resInfo.strUrl != null)
                {
                    m_entityInfo.m_strName = resInfo.strName;
                    m_res = ResourceFactory.Inst.LoadResource(m_entityInfo.m_resID, ResourceLoaded);
                }
                else
                {
                    Debug.LogError("创建失败，无资源id：" + m_entityInfo.m_resID + " 现用灰色盒子代替，请注意更换！");
                    m_res = ResourceFactory.Inst.LoadResource(4, ResourceLoaded);
                }
            }
        }

        /// <summary>
        /// 改变模型资源的接口
        /// </summary>
        /// <param name="notity"></param>
        /// <param name="baseInfo"></param>
        public virtual void ChangeResource(Action<Entity> notity, EntityBaseInfo baseInfo)
        {
            m_entityInitNofity = notity;
            m_entityInfo = baseInfo;

            // 先卸载
            if (!m_res.IsLoaded())
            {
                return;
            }
            m_listRenderer.Clear();
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
            if (m_orderList != null)
            {
                m_orderList = null;
            }

            if (m_object != null)
            {
                GameObject.Destroy(m_object);
            }
            ResourceFactory.Inst.UnLoadResource(m_res, true);
            m_object = null;
            m_res = null;

            m_LoadingDestory = false;
            // 开始下载资源
            m_res = ResourceFactory.Inst.LoadResource(m_entityInfo.m_resID, ChangeResourceLoaded);
        }

        public virtual void ChangeResourceLoaded(Resource res)
        {
            ResourceLoaded(res);
        }

        public virtual void Revive(int handleId, Action<Entity> notity, EntityBaseInfo baseInfo)
        {
            if (m_res == null)
                return;
            m_hid = handleId;
            m_entityInitNofity = notity;
            m_entityInfo = baseInfo;
            SetBaseInfo();  // 重复利用的对象池时，不用再去获取组件信息，设置基本信息就行
            m_bRevive = true;
            //OnInited();
        }

        public virtual void Update(float fTime, float fDTime)
        {
            if (m_bRevive)
            {
                OnInited();
                m_bRevive = false;
            }
            if (m_bNeedLoadResource)
            {
                if (m_res != null)
                {
                    m_loadProcess = m_res.GetDownLoadProcess();
                }
            }
            _UpdateHight(fDTime);
            _UpdateWindBlowsUp(fDTime);
            //_UpdateFllow(fDTime);
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
                //Debug.LogError("配置了id和名称，但是没有资源，如果是无用资源请删除相关配置。id:" + m_entityInfo.m_resID);
                return;
            }
            //Debug.LogError("完成加载资源。。。。。。。。。。。。" + res.GetResInfo().nResID + " m_LoadingDestory :" + m_LoadingDestory);
            if (m_LoadingDestory)
            {
                //Debug.LogError("立马删除资源" + res.GetResInfo().nResID + " m_LoadingDestory :" + m_LoadingDestory);
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
            m_listRenderer.Clear();
            Renderer[] render = m_object.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < render.Length; i++)
            {
                Renderer re = render[i];
                m_listRenderer.Add(re);
            }


            if (Application.isEditor)    // 在编辑器时，安卓和苹果平台需要重新赋值
            {
                if (!Client.m_prefix.Equals("pc"))
                {
                    for (int i = 0; i < m_listRenderer.Count; i++)
                    {
                        for (int j = 0; j < m_listRenderer[i].materials.Length; j++)
                        {
                            m_listRenderer[i].materials[j].shader = Shader.Find(m_listRenderer[i].materials[j].shader.name);
                        }
                    }
                }
            }

            // 获取原shader
            GetOriginaShader();
            SetBaseInfo();

            // 刚加载出来时设置一次
            Transform hTransform = m_object.transform.FindChild("high");
            if (hTransform != null)
            {
                m_highObject = hTransform;
                m_hightPos = hTransform.localPosition;
                if (!m_highObject.gameObject.activeSelf)
                {
                    m_highObject.gameObject.SetActive(true);
                }
            }
            //SetQuality(SettingMgr.m_imageQuality);
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
                //m_entityInitNofity = null;
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
                //Debug.LogError("销毁对象。。。。。。。。。。。" + m_object.name);
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
                return;

            GetOriginalColor();
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null)
                {
                    if (m_listRenderer[i].material.HasProperty("_AddTintColor"))
                    {
                        m_listRenderer[i].material.SetColor("_AddTintColor", color);
                    }
                }
            }
        }

        private void GetOriginaShader()
        {
            if (m_originalShaderList == null)
            {
                m_originalShaderList = new List<Shader>();
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    m_originalShaderList.Add(m_listRenderer[i].material.shader);
                }
            }
        }

        private void GetOriginalColor()
        {
            if (m_originalColorList == null)
            {
                m_originalColorList = new List<Color>();
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    Color oldColor = Color.white;
                    if (m_listRenderer[i].material.HasProperty("_AddTintColor"))
                    {
                        oldColor = m_listRenderer[i].material.GetColor("_AddTintColor");
                    }
                    m_originalColorList.Add(oldColor);
                }
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
                    if (m_listRenderer[i].material.HasProperty("_AddTintColor"))
                    {
                        if (i < m_originalColorList.Count)
                            m_listRenderer[i].material.SetColor("_AddTintColor", m_originalColorList[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 设置shader的时候，内部会移除之前的shader
        ///可以把shader接口统一为一个
        ///time：当前shader的变化时间
        ///bAutoEnd：自动结束并移除
        /// </summary>
        public void SetShader(eShaderType type, Color color, float time = 9999999, bool bAutoEnd = true, Action end = null)
        {
            UnityEngine.Profiling.Profiler.BeginSample("SetShader");
            GetOriginalColor();


            EntityShaderInfo info;
            info.entity = this;
            info.type = type;
            info.color = color;
            info.bAutoEnd = bAutoEnd;
            info.showTime = time;
            info.curTime = 0;
            info.showEnd = end;
            ShaderManager.Inst.AddShader(info);

            UnityEngine.Profiling.Profiler.EndSample();
        }

        /// <summary>
        /// 还原shader,并从shader管理器中移除当前ent的shader
        /// </summary>
        public void RemoveShader()
        {
            RestoreShader();
            ShaderManager.Inst.RemoveShader(m_hid);
        }

        public void RestoreShader()
        {
            if (m_originalShaderList == null || m_originalShaderList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                Renderer ren = m_listRenderer[i];
                if (ren != null && ren != null &&
                    ren.material.shader != m_originalShaderList[i])
                    ren.material.shader = m_originalShaderList[i];
            }
            //RestoreColor();  // 还原shader不应该处理颜色
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
                    if (m_listRenderer[i] != null)
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
            if (null != m_transform)
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
                m_transform.position = pos;
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

        public void SetDirection(float y)
        {
            m_entityInfo.m_vRotate = new Vector3(0, y, 0);
            if (null != m_transform)
            {
                m_transform.localEulerAngles
                    = m_entityInfo.m_vRotate;
            }
        }

        public void SetDirection(Vector3 rotate)
        {
            m_entityInfo.m_vRotate = rotate;
            if (null != m_transform)
            {
                //Debug.Log(m_transform.name + "  :" + rotate);
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

        //public void SetScale(float scale, bool bConChild = false)
        //{
        //    m_entityInfo.m_vScale.x = scale;
        //    m_entityInfo.m_vScale.y = scale;
        //    m_entityInfo.m_vScale.z = scale;
        //    if (null != m_transform)
        //    {
        //        if (bConChild)
        //        {
        //            Transform[] itemList = m_transform.GetComponentsInChildren<Transform>();
        //            for (int i = 0; i < itemList.Length; i++)
        //            {
        //                itemList[i].localScale = new Vector3(scale, scale, scale);
        //            }
        //        }
        //        else
        //        {
        //            m_transform.localScale = new Vector3(scale, scale, scale);
        //        }
        //    }
        //}

        public void SetScale(Vector3 scale)
        {
            m_entityInfo.m_vScale = scale;
            if (null != m_transform)
            {
                //Debug.Log(m_transform.name + "  "+ scale);
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
                    m_orderList[i] = m_listRenderer[i].sortingOrder;
                }
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null)
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

        public virtual void SetShow(bool bActive, bool bStop = true)
        {
            m_entityInfo.m_active = bActive;
            if (null == m_transform)
                return;
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                Renderer ren = m_listRenderer[i];
                if (ren == null)
                    continue;

                if (!bActive)
                {
                    if (ren is TrailRenderer)
                    {
                        ((TrailRenderer)ren).Clear();
                    }
                }
                ren.enabled = bActive;
            }
        }

        /// <summary>
        /// 停止实体内部行为，特效播放等
        /// </summary>
        public virtual void Stop()
        {

        }

        /// <summary>
        /// 1.刚加载时设置一次
        /// 2.战斗中切换画质时，遍历一次
        /// 用于特效和场景
        /// </summary>
        public void SetQuality(bool bHigh)
        {
            if (m_highObject == null)
                return;
            if (bHigh)
            {
                m_highObject.localPosition = m_hightPos;
            }
            else
            {
                m_highObject.localPosition = m_hightPos + Vector3.up * 2000;
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


        public void AddBoxCollider()
        {
            if (m_object == null)
                return;
            // 如果美术自己加了碰撞体，程序就不自动生成
            if (m_object.GetComponent<BoxCollider>() != null)
            {
                return;
            }
            BoxCollider boxColl = m_object.AddComponent<BoxCollider>();
            Vector3 LocalAngle = m_transform.localEulerAngles;
            //Zero the rotation before we calculate as bounds are never rotated
            m_transform.localEulerAngles = Vector3.zero;
            //Establish a default center location
            Vector3 center = Vector3.zero;
            //Establish a default empty bound
            Bounds m_bounds = new Bounds(Vector3.zero, Vector3.zero);
            //Count the children with renderers
            int count = 0;
            //We only count childrent with renderers which should be fine as the bounds center is global space
            foreach (Renderer render in m_transform.GetComponentsInChildren<Renderer>())
            {
                center += render.bounds.center;
                count++;
            }
            //Return the average center assuming we have any renderers
            if (count > 0)
                center /= count;
            //Update the parent bound accordingly
            m_bounds.center = center;
            //Again for each and only after updating the center expand via encapsulate
            foreach (Renderer render in m_transform.GetComponentsInChildren<Renderer>())
            {
                m_bounds.Encapsulate(render.bounds);
            }
            //In by case I want to update the parents box collider so first I need to bring things into local space
            m_bounds.center -= m_transform.position;
            boxColl.center = m_bounds.center;
            boxColl.size = new Vector3(
               (int)(m_bounds.size.x * 1.3f),
               (int)(m_bounds.size.y * 1.2f),
               (int)(m_bounds.size.z * 1.3f));
            m_transform.localEulerAngles = LocalAngle;
        }

    }
}
