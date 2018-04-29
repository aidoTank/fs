using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public enum eEntityType
    {
        eStaticEntity,
        eDynamicEntity,
        eBoneEntity,
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

    /// <summary>
    /// 回调参数中，默认为实体信息，上层也可以给m_parameter赋值，回调时取这个值
    /// </summary>
    public delegate void EntityInitNotify(Entity entity, object userObj);
	public class Entity
	{
        private EntityInitNotify m_entityInitNofity;
        public uint m_handleID;
        public eEntityType m_entityType = eEntityType.eStaticEntity;
        public GameObject m_object;
        public Transform m_transform;
        private Resource m_res;          // 每一个实体对应一个Resource类，调用资源工厂加载，也会调用资源工厂销毁
        private float m_loadProcess = 0.0f;
        private bool m_bNeedLoadResource = true;
        public EntityBaseInfo m_entityInfo;
        public Bounds m_bounds;

        public object m_parameter;          // 外部赋值的对象，回调时的参数
        /// <summary>
        /// 实体标签，用于退出小游戏销毁时识别
        /// </summary>
        public eEntityTag m_tag = eEntityTag.eScene;

        // 逻辑传给底层的用户数据， 可以是uid = 1,用于拾取对象
        protected Dictionary<int, object> m_userString = new Dictionary<int, object>();

        public List<Renderer> m_listRenderer = new List<Renderer>();
        protected List<Color> m_originalColorList = new List<Color>();  //原颜色
        protected Shader m_originalShader;                     // 原shader
        public bool m_bShadering = false;                      // 修改着色器状态中，比如长时间的被石化，隐身等

        /// <summary>
        /// 特效渲染列表，用于模型自带特效的渲染控制，比如动态设置渲染层，显隐
        /// </summary>
        public List<Renderer> m_listRendererEffect = new List<Renderer>();

        protected int[] m_orderList = null; // 渲染层级

        public List<Vector2> m_blockInfo = new List<Vector2>();
        protected int m_layer = 0;  // 用于临时记录层信息

        private bool m_LoadingDestory = false;  // 下载中需删除

        public virtual bool IsStaticEntity()
        {
            return false;
        }

        public Entity(uint handle, EntityInitNotify notity, eEntityType tpye, EntityBaseInfo baseInfo)
        {
            m_entityInitNofity = notity;
            m_handleID = handle;
            m_entityType = tpye;
            m_entityInfo = baseInfo;
            m_LoadingDestory = false;
            //Debug.LogError("请求加载资源。。。。。。。。。。。。"+ m_entityInfo.m_resID);
            if(!string.IsNullOrEmpty(m_entityInfo.m_strName))
            {
                m_res = ResourceFactory.Inst.LoadResource(m_entityInfo.m_strName, this.ResourceLoaded);
            }
            else
            {
                m_res = ResourceFactory.Inst.LoadResource(m_entityInfo.m_resID, this.ResourceLoaded);
            }
        }


  

        public virtual void Revive(uint handleId, EntityInitNotify notity, EntityBaseInfo baseInfo)
        {
            if(m_res == null)
                return;
            m_handleID = handleId;
            m_entityInitNofity = notity;
            m_entityInfo = baseInfo;
            ResourceLoaded(m_res);
        }

        public void SetUserString(eUserData key, object val)
	    {
	        m_userString[(int)key] = val;
	    }

        public object GetUserString(eUserData key)
	    {
            object val = null;
	        if (m_userString.TryGetValue((int)key, out val))
	        {
	            return val;
	        }
            return val;
	    }

        public virtual bool Update(float fTime, float fDTime)
        {
            if (m_bNeedLoadResource)
            {
				if(m_res != null)
				{
					m_loadProcess = m_res.GetDownLoadProcess();
				}
            }
            //for (int i = 0; i < m_blockInfo.Count; i ++ )
            //{
            //    int x = (int)m_blockInfo[i].x;
            //    int z = (int)m_blockInfo[i].y;
            //    Debug.DrawLine(
            //        new Vector3(x * TerrainBlockData.nodesize, 4, z * TerrainBlockData.nodesize),
            //        new Vector3(x * TerrainBlockData.nodesize, 0, z * TerrainBlockData.nodesize),
            //        Color.red);
            //}
            return true;
        }

        private void ResourceLoaded(Resource res)
        {
            //Debug.LogError("完成加载资源。。。。。。。。。。。。" + res.GetResInfo().nResID);
            if (m_LoadingDestory)
            {
                ResourceFactory.Inst.UnLoadResource(m_res);
                return;
            }
            m_res = res;
            GetGameObjectByRes(m_res);
            UpdateBaseInfo(m_res);
            OnInited();
            m_bNeedLoadResource = false;
        }

        public float GetLoadProcess()
        {
            return m_loadProcess;
        }

        public Resource GetResource()
        {
            return m_res;
        }

        public virtual void GetGameObjectByRes(Resource res)
        {
            if (null == res)
            {
                Debug.LogError("资源为空:" + m_entityInfo.m_resID);
                return;
            }
            if (null == m_object)
            {
                m_object = res.InstantiateGameObject();
                m_transform = m_object.transform;
                // 添加对象助手
                if (m_object != null)
                {
                    GameObjectHelper ghelp = m_object.GetComponent<GameObjectHelper>();
                    if (ghelp == null)
                    {
                        ghelp = m_object.AddComponent<GameObjectHelper>();
                    }
                    ghelp.SetHelpObject(this);
                }
            }
        }

        public virtual void UpdateBaseInfo(Resource res)
        {
            if (m_object == null)
            {
                Debug.Log("资源为空:" + m_entityInfo.m_resID);
                return;
            }

            m_object.isStatic = m_entityInfo.m_bStatic;

            m_listRenderer.Clear();
            m_listRendererEffect.Clear();
            Renderer[] render = m_object.GetComponentsInChildren<Renderer>();
            for(int i = 0; i < render.Length; i ++)
            {
                Renderer re = render[i];
                re.gameObject.isStatic = m_entityInfo.m_bStatic;
                if (re.name.Equals("tx")) // 因为模型身上加了一些特效，这些特效不参与模型变色等，排除渲染列表
                {
                    m_listRendererEffect.Add(re);
                }
                else
                {
                    m_listRenderer.Add(re);
                }
            }
            //m_listRenderer = new List<Renderer>(render);

            SetPos(m_entityInfo.m_vPos);
            SetDirection(m_entityInfo.m_vRotate);
            SetScale(m_entityInfo.m_vScale);
            SetActive(m_entityInfo.m_active);
        }

        public virtual void OnInited()
        {
            if (null != m_entityInitNofity)
            {
                if (m_parameter == null)            // 回调参数中默认为实体信息
                    m_parameter = m_entityInfo;
                m_entityInitNofity(this, m_parameter);
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
                //Debug.LogError("下载中就要删除的？Destroy。。。。。。。。。。。" + m_res.GetResInfo().nResID);
                m_LoadingDestory = true;
                return;
            }
            if (null != m_object)
            {
                //Debug.LogError("销毁对戏。。。。。。。。。。。" + m_object.name);

                // 移除shader


                m_originalColorList.Clear();
                m_originalColorList = null;
                m_originalShader = null;
                m_orderList = null;
                m_listRenderer.Clear();
                m_listRenderer = null;
                m_listRendererEffect.Clear();
                m_listRendererEffect = null;

                m_transform = null;
                GameObject.Destroy(m_object);
                ResourceFactory.Inst.UnLoadResource(m_res);
                m_res = null;
                m_object = null;
            }
        }

        public virtual void SetOcc(bool bActive)
        {

        }

        public void SetColor(Color color)
        {
            if (m_listRenderer == null)
            {
                return;
            }
            if(m_originalColorList.Count == 0)
            {
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    Color oldColor = Color.white;
                    if (m_listRenderer[i].material.HasProperty("_Color"))
                    {
                        oldColor = m_listRenderer[i].material.GetColor("_Color");
                    }
                    m_originalColorList.Add(oldColor);
                }
            }

        }

        public void SetColorForever(Color color)
        {
            if (m_listRenderer == null)
            {
                return;
            }
            if (m_originalColorList.Count == 0)
            {
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    Color oldColor = Color.white;
                    if (m_listRenderer[i].material.HasProperty("_Color"))
                    {
                        oldColor = m_listRenderer[i].material.GetColor("_Color");
                    }
                    m_originalColorList.Add(oldColor);
                }
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null)
                {
                    if (m_listRenderer[i].material.HasProperty("_Color"))
                    {
                        m_listRenderer[i].material.SetColor("_Color", color);
                    }
                }
            }
        }

        public void RestoreColor()
        {
            if (m_listRenderer == null || m_originalColorList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null)
                {
                    if (m_listRenderer[i].material.HasProperty("_Color"))
                    {
                        if(i < m_originalColorList.Count)
                            m_listRenderer[i].material.SetColor("_Color", m_originalColorList[i]);
                    }
                }
            }
        }



        public void RestoreShader()
        {
            if (m_listRenderer == null || m_originalShader == null)
            {
                return;
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (m_listRenderer[i] != null)
                    m_listRenderer[i].material.shader = m_originalShader;
            }
            RestoreColor();
            m_bShadering = false;
        }

        public virtual void SetLayer(LusuoLayer layer)
        {
            m_entityInfo.m_ilayer = (int)layer;
            if (null != m_object)
            {
                m_object.gameObject.layer = (int)layer;
                //Renderer[] objList = m_object.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    m_listRenderer[i].gameObject.layer = (int)layer;
                }
                for (int i = 0; i < m_listRendererEffect.Count; i++)
                {
                    m_listRendererEffect[i].gameObject.layer = (int)layer;
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
                    m_listRenderer[i].gameObject.layer = (int)layer;
                }
                for (int i = 0; i < m_listRendererEffect.Count; i++)
                {
                    m_listRendererEffect[i].gameObject.layer = (int)layer;
                }
            }
        }

        /// <summary>
        /// 将这个对象绑定到parent上
        /// </summary>
        public virtual void Bind(Transform parent)
        {
        }

        public void SetPos(Vector3 pos)
        {
            m_entityInfo.m_vPos = pos;
            if (null != m_object)
            {
                //m_transform.localPosition
                m_transform.localPosition
                    = new Vector3(pos.x, pos.y, pos.z); 
            }
        }
        public void SetPos(float x, float z)
        {
            m_entityInfo.m_vPos.x = x;
            m_entityInfo.m_vPos.z = z;
            m_entityInfo.m_vPos.y = SceneManager.Inst.GetTerrainHeight(x, z);
            SetPos(m_entityInfo.m_vPos);
        }

        public void SetDirection(Vector3 rotate)
        {
            m_entityInfo.m_vRotate = rotate;
            if (null != m_object)
            {
                m_transform.localEulerAngles
                //m_transform.localEulerAngles
                    = new Vector3(rotate.x, rotate.y, rotate.z);
            }
        }

        public void SetRot(Quaternion rotate)
        {
            m_entityInfo.m_vRotate = rotate.eulerAngles;
            if (null != m_object)
            {
                m_transform.rotation = rotate;
                //m_transform.rotation = rotate;
            }
        }

        public void SetScale(float scale, bool bConChild = false)
        {
            m_entityInfo.m_vScale.x = scale;
            m_entityInfo.m_vScale.y = scale;
            m_entityInfo.m_vScale.z = scale;
            if (null != m_object)
            {
                if(bConChild)
                {
                    Transform[] itemList = m_transform.GetComponentsInChildren<Transform>();
                    for(int i= 0;i < itemList.Length; i ++)
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
            if (null != m_object)
            {
                m_transform.localScale
                    = new Vector3(scale.x, scale.y, scale.z);
            }
        }

        // 排序层只用设置一次
        public void SetOrder(int layer)
        {
            m_entityInfo.m_order = layer;
            if (null == m_object)
                return;
            if(m_orderList == null)
            {
                m_orderList = new int[m_listRenderer.Count];
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    m_orderList[i] = m_listRenderer[i].sortingOrder;
                }
            }
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                m_listRenderer[i].sortingOrder = layer + m_orderList[i];
            }
        }

        public void ClearDynamicBlock()
        {
            // 检测之前将上次的障碍信息清空
            for (int i = 0; i < m_blockInfo.Count; i++)
            {
                Vector2 info = m_blockInfo[i];
                SceneManager.Inst.GetMap().SetTerrainBlock((int)info.x, (int)info.y, false);
            }
            m_blockInfo.Clear();
        }

        public void SetDynamicBlock()
        {
            //return;
            ClearDynamicBlock();

            //// 适用于规则并且不旋转的动态障碍
            //Vector3 pos = GetPos() + GetBoxCenter();
            //Vector3 size = GetBoxSize();
            //int x0 = (int)(MathEx.RoundToInt(pos.x - size.x * 0.6f) / TerrainBlockData.nodesize);
            //int x1 = (int)(MathEx.RoundToInt(pos.x + size.x * 0.6f) / TerrainBlockData.nodesize);
            //int z0 = (int)(MathEx.RoundToInt(pos.z - size.z * 0.6f) / TerrainBlockData.nodesize);
            //int z1 = (int)(MathEx.RoundToInt(pos.z + size.z * 0.6f) / TerrainBlockData.nodesize);
            //for (int x = x0; x <= x1; x++)
            //{
            //    for (int z = z0; z <= z1; z++)
            //    {
            //        // 这个值是以0.5为单元格，所以在物体计算时，边界起点终点都要*2
            //        if (SceneManager.Inst.GetMap().CanArriveDoubleInt(x, z))
            //        {
            //            SceneManager.Inst.GetMap().SetTerrainBlock(x, z, true);
            //            // 保存障碍点信息
            //            m_blockInfo.Add(new Vector2(x, z));
            //        }
            //    }
            //}
            //return;
            
            // 适用于不规则动态障碍，物体在移动并旋转时，频繁的检测障碍信息时会比较消耗
            // DOTA类游戏英雄的碰撞体积是固定的圆形，也就无需检测障碍信息，只需读写障碍信息
            //int sceneW = SceneManager.Inst.GetMapData().sizeW;
            m_layer = GetObject().layer;
            // 当前检测最大范围，当前动态障碍物体的最大直径
            int range = 10;
            SetLayer(LusuoLayer.eEL_NavMesh);
            Vector3 pos = GetPos() + GetBoxCenter();

            // 真实坐标映射到寻路坐标,位置全部乘以2，也就是0.5米检测一个点，考虑到边界的问题，在检测时将碰撞体增加0.1
            m_object.GetComponent<BoxCollider>().size += Vector3.one * 0.1f;
            int x0 = (int)(MathEx.RoundToInt(pos.x - range) / TerrainBlockData.nodesize);
            int x1 = (int)(MathEx.RoundToInt(pos.x + range) / TerrainBlockData.nodesize);
            int z0 = (int)(MathEx.RoundToInt(pos.z - range) / TerrainBlockData.nodesize);
            int z1 = (int)(MathEx.RoundToInt(pos.z + range) / TerrainBlockData.nodesize);
            for (int x = x0; x <= x1; x++)
            {
                for (int z = z0; z <= z1; z++)
                {
                    // 检测碰撞时还原到真实坐标
                    if (Physics.Raycast(new Vector3(x * TerrainBlockData.nodesize, 10, z * TerrainBlockData.nodesize), -Vector3.up, 100, (int)LusuoLayerMask.eEL_NavMesh))
                    {
                        SceneManager.Inst.GetMap().SetTerrainBlock(x, z, true);
                        // 保存障碍点信息
                        m_blockInfo.Add(new Vector2(x, z));
                    }
                }
            }
            m_object.GetComponent<BoxCollider>().size -= Vector3.one * 0.1f;
            SetLayer(m_layer);
        }

        public EntityBaseInfo GetEntityBaseInfo()
        {
            return m_entityInfo;
        }

        public int GetLayer()
        {
            if (null != m_object)
            {
                return m_entityInfo.m_ilayer;
            }
            return (int)LusuoLayer.eEL_Default;
        }

        public virtual void SetActive(bool bActive)
        {
            m_entityInfo.m_active = bActive;
            if (null == m_object)
                return;

            if(m_entityType == eEntityType.eEffectEntity || m_entityType == eEntityType.eSoundEntity)
            {
                m_object.SetActive(bActive);
            }
            else
            {
                for (int i = 0; i < m_listRenderer.Count; i++)
                {
                    m_listRenderer[i].enabled = bActive;
                }
                for (int i = 0; i < m_listRendererEffect.Count; i++)
                {
                    m_listRendererEffect[i].enabled = bActive;
                }
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

        public void AddBoxCollider()
        {
            if (m_object == null)
                return;
            // 如果美术自己加了碰撞体，程序就不自动生成
            if(m_object.GetComponent<BoxCollider>() != null)
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
            m_bounds = new Bounds(Vector3.zero, Vector3.zero);
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
                MathEx.RoundToInt(m_bounds.size.x * 1.3f),
                MathEx.RoundToInt(m_bounds.size.y * 1.2f), 
                MathEx.RoundToInt(m_bounds.size.z * 1.3f));
            m_transform.localEulerAngles = LocalAngle;
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
            if(null != m_object)
            {
                return m_transform.position;
            }
            return m_entityInfo.m_vPos;
        }
        public virtual Quaternion GetRotateQua()
        {
            if (m_object == null)
            {
                return Quaternion.identity;
            }
            return m_transform.rotation;
        }
        public virtual Vector3 GetRotate()
        {
            if (null != m_object)
            {
                return m_transform.localEulerAngles;
            }
            return m_entityInfo.m_vRotate;
        }
        public virtual Vector3 GetScale()
        {
            if(m_object == null)
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
