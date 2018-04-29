using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 负责所有资源的下载
/// 1.创建下载器对象，下载在资源管理类通过队列进行管理
/// 2.依赖资源，主资源下载完成时，才是真的下载完毕
/// 3.资源通过名称下载
/// </summary>
namespace Roma
{

    public class DPResourceManager : Singleton
    {
        public static new DPResourceManager Inst = null;
        public Dictionary<string, DPResource> m_mapResource = new Dictionary<string, DPResource>();

        public DPResourceManager()
            : base(true)
        {

        }

        public override void Update(float fTime, float fDTime)
        {
            base.Update(fTime, fDTime);
        }

        public string GetDPResource()
        {
            string curInfo = "\n";

            string uintInfo = "";

            int num = 0;
            foreach (KeyValuePair<string, DPResource> res in m_mapResource)
            {
                num++;
                string iRef = "[引用：" + res.Value.m_ref + "]  ";
                uintInfo += "\n" + iRef + res.Value.m_assertBundle.LoadAllAssets()[0].GetType() + " -- " + res.Value.m_assertBundle.name;
            }
            if (!string.IsNullOrEmpty(uintInfo))
            {
                curInfo += "数量：" + num + uintInfo + "\n";
            }

            return curInfo;
        }


        /// <summary>
        /// 下载时添加
        /// </summary>
        public void Add(string name, AssetBundle ab)
        {
            if (m_mapResource.ContainsKey(name))
            {
                Debug.LogError("添加依赖资源重复错误："+ name);
                return;
            }
            DPResource res = new DPResource();
            res.m_assertBundle = ab;
            res.AddRef();
            m_mapResource.Add(name, res);
        }

        /// <summary>
        /// 资源移除时
        /// </summary>
        public void Remove(string name)
        {
            DPResource res;
            if (m_mapResource.TryGetValue(name, out res))
            {
                res.SubRef();
                if(res.m_ref == 0)
                {
                    res.m_assertBundle.Unload(true);
                    m_mapResource.Remove(name);
                }
            }
        }

        public DPResource GetResource(string name)
        {
            DPResource res;
            if (m_mapResource.TryGetValue(name, out res))
            {
                return res;
            }
            return null;
        }
    }

    public class DPResource
    {
        public AssetBundle m_assertBundle;
        public int m_ref = 0;    // 当前资源的引用数量

        public int AddRef()
        {
            return ++m_ref;
        }

        public int SubRef()
        {
            return --m_ref;
        }
    }

    public class DownLoadHelper : MonoBehaviour
    {
        private bool m_bStart = false;
        private Resource m_res;
        private WWW m_www;

        public void Clear()
        {
            if (null != m_www)
            {
                m_www = null;
            }
        }

        public void InitNewDownLoad(Resource res
            , Action<Resource> loaded)
        {
            m_res = res;
            m_bStart = true;
            Clear();
            StartCoroutine(DownloadAssetBundle(res, loaded));
        }

        /// <summary>
        /// 下载AssetBundle资源包，并保存在内存中
        /// </summary>
        public IEnumerator DownloadAssetBundle(Resource res, 
             Action<Resource> loaded)
        {
            string[] dependences = null;
            if (res.GetResInfo().m_bDepend) //先下载依赖资源
            {
                dependences = ResourceManager.ABManifest.GetAllDependencies(res.GetResInfo().strUrl);
                if (dependences != null && dependences.Length > 0)
                {
                    for (int i = 0; i < dependences.Length; i++)
                    {
                        string name = dependences[i];
                        // 如果包含就不下载，而是添加引用
                        DPResource dpRes = DPResourceManager.Inst.GetResource(name);
                        if (dpRes != null)
                        {
                            dpRes.AddRef();
                            continue;
                        }

                        string dpURL = GlobleConfig.GetPersistentPath() + name;
                        if (!File.Exists(dpURL))
                        {
                            dpURL = GlobleConfig.GetStreamingPath() + name;
                        }

                        WWW dwww = new WWW(dpURL);
                        yield return dwww;
                        if (dwww.error != null)
                        {
                            m_bStart = false;
                            Debug.Log("依赖资源加载出错：" + name + "  " + dpURL);
                        }
                        else
                        {
                            DPResourceManager.Inst.Add(name, dwww.assetBundle);
                        }
                    }
                }
            }

            //下载主资源
            Debug.Log("加载资源：" + res.m_fullUrl);
            m_www = new WWW(res.m_fullUrl);
            yield return m_www;
            if (m_www.error != null)
            {
                m_bStart = false;
                Debug.LogError("下载AssetBundle出错！地址：" + res.m_fullUrl + "  "+ m_www.error);
                res.SetState(eResourceState.eRS_NoFile);

                ResourceFactory.Inst.UnLoadResource(m_res, true);
                if (res.m_resInfo.strName.Equals(ExportDefine.m_prefix))
                {
                    m_bStart = false;
                    m_res = null;
                    Client.Inst().m_uiResInitDialog.OpenPanel(true);
                    Client.Inst().m_uiResInitDialog.SetText("重连", "退出", "连接资源服务器失败，请联系客服。");
                    Client.Inst().m_uiResInitDialog.AddEvent((bOk, a, b) =>
                    {
                        if (bOk)
                        {
                            Debug.Log("重连资源服");
                            Client.Inst().m_gameInit.OnCheckFirst();
                        }
                        else
                        {
                            Application.Quit();
                        }
                    });
                }
                else          // 下载资源错误
                {
                    m_bStart = false;
                    m_res = null;
                    Client.Inst().m_uiResInitDialog.OpenPanel(true);
                    Client.Inst().m_uiResInitDialog.SetText("退出", "", "获取资源错误：" + m_res.m_fullUrl);
                    Client.Inst().m_uiResInitDialog.AddEvent((bOk, a, b) =>
                    {
                        if (bOk)
                        {
                            Application.Quit();
                        }
                    });
                }
            }
            else
            {
                res.SetDPResource(dependences);
                res.SetResource(ref m_www);
                res.SetState(eResourceState.eRS_Loaded);  // 这里只是表示资源加载完成，并没有执行资源内部的下载后的逻辑
                res.SetDownLoadProcess(1.0f);
            }

            if (loaded != null)  
                loaded(res);

            m_bStart = false;
        }

        private void Update()
        {
            if (!m_bStart)
            {
                return;
            }
            if(m_res != null && m_www != null && !m_www.isDone)
            {
                m_res.SetDownLoadProcess(m_www.progress);
            }
        }
    }
}



