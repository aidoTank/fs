using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 资源管理类
/// </summary>
namespace Roma
{
    public class ResourceManager : Singleton
    {
        public static AssetBundleManifest ABManifest;

        private DownLoadHelper m_downLoadHelper;
        // 请求队列
        private List<Resource> m_DelayQuestDownLoader = new List<Resource>();
        // 下载完成的通知队列
        private List<Resource> m_DelayNotifyDownLoaded = new List<Resource>();
        // 资源列表
        public Dictionary<string, Resource> m_mapResource = new Dictionary<string, Resource>();
        // 卸载队列
        public Dictionary<string, Resource> m_mapUnLoad = new Dictionary<string, Resource>();
        // 临时销毁列表
        private LinkedList<Resource> m_tempDestoryResources = new LinkedList<Resource>();
        // 异步回调
        private List<AsynResLoaded> m_listResLoaded = new List<AsynResLoaded>();
        private class AsynResLoaded
        {
            public Delegate call;
            public Resource res;
        }
        public static ResourceManager Inst = null;

        public ResourceManager()
            : base(true)
        {

        }

        public override void Init()
        {
            GameObject downLoadHelper = new GameObject("downLoadHelper");
            m_downLoadHelper = downLoadHelper.AddComponent<DownLoadHelper>();
        }

        public Resource LoadResource(ResInfo resInfo, Action<Resource> loaded = null)
        {
            Resource outRes = GetResource(resInfo.strName);
            if (outRes != null)
            {
                if (outRes.IsLoaded())
                {
                    AsynResLoaded pair = new AsynResLoaded();
                    pair.call = loaded;
                    pair.res = outRes;
                    m_listResLoaded.Add(pair);
                    //loaded(outRes);
                }
                else
                {
                    outRes.m_loadedEvent += loaded;
                }
                return outRes;
            }
            // 如果下载过,直接取,这里改成工厂模式，根据资源类型new不同类型的资源
            outRes = NewResource(ref resInfo);
            if (outRes == null)
                Debug.LogError("res is null:" + resInfo.strName);
            outRes.AddRef();
            outRes.m_loadedEvent = loaded;
            m_mapResource[resInfo.strName] = outRes;
            m_DelayQuestDownLoader.Add(outRes);

            return outRes;
        }


        public Resource GetResource(string abName)
        {
            Resource res;
            if (m_mapResource.TryGetValue(abName, out res))
            {
                // 如果有资源，引用++
                res.AddRef();
                return res;
            }
            else
            {
                // 如果在获取资源时，卸载队列刚好有，就移除，加到资源列表
                if (m_mapUnLoad.TryGetValue(abName, out res))
                {
                    res.AddRef();
                    m_mapResource[abName] = res;
                    m_mapUnLoad.Remove(abName);
                    return res;
                }
            }
            return null;
        }

        /// <summary>
        /// atOnce立即销毁改资源的所有ab资源
        /// unLoadAllAB销毁该资源的所有ab关联的资源
        /// </summary>
        public bool UnLoadResource(Resource res, bool atOnce)
        {
            if (null == res)
            {
                return false;
            }
            // 如果引用数量为0就卸载掉
            if (res.SubRef() <= 0)
            {
                if (atOnce)
                {
                    string uResID = res.m_resInfo.strName;
                    res.Destroy();
                    m_mapResource.Remove(uResID);   // 从资源队列中移除
                    res = null;
                }
                else
                {
                    string uResID = res.m_resInfo.strName;
                    m_mapResource.Remove(uResID);   // 从资源队列中移除
                    m_mapUnLoad[uResID] = res;      // 加入到卸载对面
                    res.SetCacheTime(res.GetCacheTime());
                }
            }
            return false;
        }

        /// <summary>
        /// 立即销毁
        /// </summary>
        public override void Destroy()
        {
            // 可以让子资源，自己决定是否自动卸载
            m_DelayQuestDownLoader.Clear();
            m_DelayNotifyDownLoaded.Clear();
            foreach (KeyValuePair<string, Resource> key in m_mapResource)
            {
                if (key.Value != null)
                    key.Value.Destroy();
            }

            foreach (KeyValuePair<string, Resource> key in m_mapUnLoad)
            {
                if (key.Value != null)
                    key.Value.Destroy();
            }

            m_mapResource.Clear();
            m_mapUnLoad.Clear();
            m_listResLoaded.Clear();
        }

        public override void Update(float fTime, float fDTime)
        {
            // 只取第一个，直到他下载完成才移除
            if (m_DelayQuestDownLoader.Count > 0)
            {
                Resource res = m_DelayQuestDownLoader[0];
                if (res.GetState() == eResourceState.eRS_None)
                {
                    // 去下载
                    m_downLoadHelper.InitNewDownLoad(res, OnDownLoadend);
                    res.SetState(eResourceState.eRS_Loading);
                }
                else if (res.GetState() == eResourceState.eRS_Loaded ||
                    res.GetState() == eResourceState.eRS_NoFile ||
                    res.GetState() == eResourceState.eRS_Destroy)           // 如果在下载时，状态变成已销毁
                {
                    m_DelayQuestDownLoader.RemoveAt(0);
                }
            }

            // 下载完成后事件处理队列
            if (m_DelayNotifyDownLoaded.Count > 0)
            {
                Resource res = m_DelayNotifyDownLoaded[0];
                m_DelayNotifyDownLoaded.RemoveAt(0);
                if (GlobleConfig.m_gameState == eGameState.Game)
                {
                    res.OnLoadedLogic();
                }
                res.OnLoadedEvent();
            }


            for (int i = 0; i < m_listResLoaded.Count; i++)
            {
                Action<Resource> call = (Action<Resource>)m_listResLoaded[i].call;
                if (null != call)
                {
                    call(m_listResLoaded[i].res);
                }
            }
            m_listResLoaded.Clear();

            //if (m_listResLoaded.Count > 0)
            //{
            //    AsynResLoaded pair = m_listResLoaded.First.Value;
            //    Action<Resource> call = (Action<Resource>) pair.call;
            //    if (null != call)
            //    {
            //        call(pair.res);
            //    }
            //    m_listResLoaded.RemoveFirst();
            //}

            // 卸载资源
            Dictionary<string, Resource>.Enumerator map = m_mapUnLoad.GetEnumerator();
            while (map.MoveNext())
            {
                if (map.Current.Value.UpdatingCacheTime(fDTime))
                {
                    m_tempDestoryResources.AddLast(map.Current.Value);
                }
            }
            LinkedList<Resource>.Enumerator tempMap = m_tempDestoryResources.GetEnumerator();
            while (tempMap.MoveNext())
            {
                Resource res = tempMap.Current;
                m_mapUnLoad.Remove(res.GetResInfo().strName);
                res.Destroy();
                res = null;
            }
            m_tempDestoryResources.Clear();

            // 用于查内存泄漏
            //foreach (KeyValuePair<string, Resource> item in m_mapResource)
            //{
            //    Debug.Log("类型：" + item.Value.GetResInfo().iType + " 资源：" + item.Value.GetResInfo().strName + "  资源数量引用：" + item.Value.m_ref);
            //}
        }

        // 这个资源下载之后，将资源加入到loaded队列
        public void OnDownLoadend(Resource res)
        {
            m_DelayNotifyDownLoaded.Add(res);
        }


        public Resource NewResource(ref ResInfo resInfo)
        {
            Resource resource = null;
            switch (resInfo.iType)
            {
                case ResType.None:
                    resource = new Resource(ref resInfo);
                    break;
                case ResType.ManifestResource:
                    resource = new ManifestResource(ref resInfo);
                    break;
                case ResType.ResInfosResource:
                    resource = new ResInfosResource(ref resInfo);
                    break;
                case ResType.CsvListResource:
                    resource = new CsvListResource(ref resInfo);
                    break;
                case ResType.AllLuaResource:
                    resource = new AllLuaResource(ref resInfo);
                    break;

                case ResType.EffectResource:
                    resource = new EffectResource(ref resInfo);
                    break;
                case ResType.SoundResource:
                    resource = new SoundResource(ref resInfo);
                    break;

                case ResType.PanelResource:
                    resource = new PanelResource(ref resInfo);
                    break;
                case ResType.IconResource:
                    resource = new IconResource(ref resInfo);
                    break;

                case ResType.ModelResource:
                    resource = new ModelResource(ref resInfo);
                    break;
                //case ResType.SceneCfgResource:
                //    resource = new SceneCfgResource(ref resInfo);
                //    break;
                case ResType.LightMapResource:
                    resource = new LightMapResource(ref resInfo);
                    break;
                case ResType.SceneDataResource:
                    resource = new SceneDataResource(ref resInfo);
                    break;

                case ResType.BoneResource:
                    resource = new BoneResource(ref resInfo);
                    break;
                case ResType.ShaderResource:
                    resource = new ShaderResource(ref resInfo);
                    break;
                case ResType.PreloadShaderResource:
                    resource = new PreloadShaderResource(ref resInfo);
                    break;
            }
            return resource;
        }

    }
}



