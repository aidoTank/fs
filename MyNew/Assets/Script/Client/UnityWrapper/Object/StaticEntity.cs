using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roma
{
	public class StaticEntity : Entity
	{
	    public StaticEntity(uint handle, EntityInitNotify notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
	    {
	        
	    }

        public override bool IsStaticEntity()
        {
            return true;
        }

        public override void UpdateBaseInfo(Resource res)
        {
            base.UpdateBaseInfo(res);
            SetLayer(LusuoLayer.eEL_Static);
            LoadLightMapInfo();

        }

        public override void SetLayer(LusuoLayer layer)
        {
            if (null != m_object)
            {
                //Renderer[] objList = m_object.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < m_listRenderer.Count; i ++)
                {
                    m_listRenderer[i].gameObject.isStatic = true;
                    // 模型遮挡透明这里，取消注释即可，美术的地形名一定要包含dimian，用于区别能点击移动层。
                    //if (!m_listRenderer[i].name.Contains("dimian"))
                    //{
                    //    m_listRenderer[i].gameObject.layer = (int)LusuoLayer.eEL_StaticNoWalking;
                    //}
                    //else
                    //{
                        m_listRenderer[i].gameObject.layer = (int)layer;
                    //}
                }
            }
        }

        private void LoadLightMapInfo()
        {
            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                if (i < m_entityInfo.m_lightMapIndex.Count())
                {
                    Terrain ter = m_listRenderer[i].gameObject.GetComponent<Terrain>();
                    if (ter != null)
                    {
                        ter.lightmapIndex = m_entityInfo.m_lightMapIndex[i];
                        ter.lightmapScaleOffset = m_entityInfo.m_lightMapScaleOffset[i];
                    }
                    else
                    {
                        // 不产生阴影
                        m_listRenderer[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        // 不接受阴影
                        m_listRenderer[i].receiveShadows = false;
                        m_listRenderer[i].useLightProbes = false;
                        m_listRenderer[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                        // 关键坑：在打包时获取的信息来自Renderer组件，那么这里必须是给Renderer赋值
                        m_listRenderer[i].lightmapIndex = m_entityInfo.m_lightMapIndex[i];
                        m_listRenderer[i].lightmapScaleOffset = m_entityInfo.m_lightMapScaleOffset[i];
                    }
                }

                // 增加OCC
                //if (null == m_listRenderer[i].gameObject.GetComponent<MeshCollider>())
                //{
                //    m_listRenderer[i].gameObject.AddComponent<MeshCollider>();
                //}
                //OccEntiy occ = m_listRenderer[i].gameObject.AddComponent<OccEntiy>();
                //occ.Init();

                // tips:解决在编辑器下，自定义shader无效的BUG
                if (Application.isEditor)
                {
                    Terrain ter = m_listRenderer[i].gameObject.GetComponent<Terrain>();
                    if (ter != null)
                    {
                        ter.materialTemplate.shader = Shader.Find(ter.materialTemplate.shader.name);
                    }
                    else
                    {
                        for (int j = 0; j < m_listRenderer[i].materials.Length; j++)
                        {
                            m_listRenderer[i].materials[j].shader = Shader.Find(m_listRenderer[i].materials[j].shader.name);
                        }
                    }
                }
            }
        }


        //private void GetChildAudio(ref List<AudioSource> list, Transform parent)
        //{
        //    AudioSource ren = parent.GetComponent<AudioSource>();
        //    if (ren != null)
        //    {
        //        list.Add(ren);
        //    }
        //    for (int i = 0; i < parent.childCount; i++)
        //    {
        //        Transform item = parent.GetChild(i);
        //        GetChildAudio(ref list, item);
        //    }
        //}

        //private Dictionary<uint, List<AudioSource>> m_3dEnvKeyValue = new Dictionary<uint, List<AudioSource>>();
    }
}
