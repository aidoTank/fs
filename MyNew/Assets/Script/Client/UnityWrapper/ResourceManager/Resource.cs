using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// 资源管理类
/// </summary>
namespace Roma
{
    public enum eResourceState
    {
        eRS_None,
        eRS_Loading,
        eRS_Loaded,
        eRS_NoFile,
    }

    public class Resource
    {
        public ResInfo m_resInfo;
        public string m_fullUrl;
        public eResourceState m_state;
        public Action<Resource> m_loadedEvent;

        public AssetBundle m_assertBundle;
        public string[] m_dpResource;  // 依赖资源 

        public byte[] m_byte = null; //这个是资源解压后的内存数据,用于文本解析

        public byte[] m_wwwByte;    // 用于更新,在需要保存到磁盘时才给他赋值，不然就会有产生多余的堆内存

        protected float m_fDownLoadProcess;

        public int m_ref = 0;         // 当前资源的引用数量
        protected float m_fCache = 10.0f;

        public Resource(ref ResInfo resInfo)
        {
            m_resInfo = resInfo;

            if(GlobleConfig.m_downLoadType == eDownLoadType.WWW)
            {
                m_fullUrl = GlobleConfig.GetFileServerPath() + m_resInfo.strUrl;
            }
            else if (GlobleConfig.m_downLoadType == eDownLoadType.StreamingAssetsPath)
            {
                m_fullUrl = GlobleConfig.GetStreamingPath() + m_resInfo.strUrl;
            }
            else if(GlobleConfig.m_downLoadType == eDownLoadType.PersistentDataPath)
            {
                m_fullUrl = GlobleConfig.GetPersistentPath() + m_resInfo.strUrl;
            }
            else if(GlobleConfig.m_downLoadType == eDownLoadType.None)  // 无加载模式就是正式游戏环境
            {
                // 先从沙盒获取，如果没有就从steam目录获取
                m_fullUrl = GlobleConfig.GetPersistentPath_File() + m_resInfo.strUrl;
                if (File.Exists(m_fullUrl))
                {
                    m_fullUrl = GlobleConfig.GetPersistentPath() + m_resInfo.strUrl;
                }
                else
                {
                    m_fullUrl = GlobleConfig.GetStreamingPath() + m_resInfo.strUrl;
                }
            }
        }

        public void SetDPResource(string[] dp)
        {
            m_dpResource = dp;
        }

        public virtual bool SetResource(ref WWW www)
        {
            if (www != null)
            {
                if (GlobleConfig.m_gameState == eGameState.Game)
                {
                    m_assertBundle = www.assetBundle;
                }
                else if (GlobleConfig.m_gameState == eGameState.First ||
                         GlobleConfig.m_gameState == eGameState.Update)
                {
                    m_wwwByte = www.bytes;
                }
                return true;
            }
            return false;
        }

        public eResourceState SetState(eResourceState state)
        {
            return m_state = state;
        }

        public eResourceState GetState()
        {
            return m_state;
        }

        public bool IsLoaded()
        {
            return m_state == eResourceState.eRS_Loaded;
        }

        public void SetDownLoadProcess(float fp)
        {
            m_fDownLoadProcess = fp;
        }
        public float GetDownLoadProcess()
        {
            return m_fDownLoadProcess;
        }


        /// <summary>
        /// 资源下载后的内部逻辑处理，各资源不同
        /// </summary>
        public virtual bool OnLoadedLogic()
        {
            return true;
        }

        /// <summary>
        /// 资源下载后的外部事件处理
        /// </summary>
        public void OnLoadedEvent()
        {
            if (m_state == eResourceState.eRS_Loaded && m_loadedEvent != null)
            {
                m_loadedEvent(this);
            }
        }

        public virtual GameObject InstantiateGameObject()
        {
            if (null == this.m_assertBundle)
            {
                return null;
            }
            GameObject obj = m_assertBundle.LoadAsset<GameObject>(m_resInfo.strName);
            if (obj == null)
            {
                Debug.LogWarning("实例化失败：" + m_resInfo.strName);
                return null;
            }
            obj = GameObject.Instantiate(obj) as GameObject;
            return obj;
        }

        /// <summary>
        // 但是依赖打包时，Unload(false)就用不着了
        /// </summary>
        public virtual void Destroy()
        {
            if (null != m_assertBundle)
            {
                m_assertBundle.Unload(true);
                GameObject.Destroy(m_assertBundle);
                m_assertBundle = null;
            }
            m_byte = null;
            m_wwwByte = null;
            // 如果完全销毁，才需要移除依赖资源的引用
            if (m_dpResource != null)
            {
                for(int i = 0; i < m_dpResource.Length; i ++)
                {
                    DPResourceManager.Inst.Remove(m_dpResource[i]);
                }
            }
            m_state = eResourceState.eRS_NoFile;
        }

        public ResInfo GetResInfo()
        {
            return m_resInfo;
        }

        public int AddRef()
        {
            return ++m_ref;
        }

        public int SubRef()
        {
            return --m_ref;
        }

        public float GetCacheTime()
        {
            return m_fCache;
        }

        public void SetCacheTime(float time)
        {
            m_fCache = time;
        }

        public bool UpdatingCacheTime(float fDTime)
        {
            //没下载完不要进入卸载队列，免得出乱子
            if (!IsLoaded())
            {
                return false;
            }
            m_fCache -= fDTime;
            return m_fCache < 0.0f;
        }

        public bool SaveToDisk()
        {
            string savePath = GlobleConfig.GetPersistentPath_File() + m_resInfo.strUrl;
            if(!File.Exists(savePath))
            {
                string dir = Path.GetDirectoryName(savePath);
                dir = dir.Replace("\\", "/");
                Directory.CreateDirectory(dir);
            }
            else
            {
                File.Delete(savePath);
            }

            try
            {
                FileStream file = new FileStream(savePath, FileMode.OpenOrCreate);
                file.Write(m_wwwByte, 0, m_wwwByte.Length);
                file.Close();
            }
            catch(IOException e)
            {
                Client.Inst().m_uiResInitDialog.OpenPanel(true);
                Client.Inst().m_uiResInitDialog.SetText("退出", "", "磁盘空间不足或文件异常，请清理空间后重启游戏。\n" + e.StackTrace);
                Client.Inst().m_uiResInitDialog.AddEvent((bOk, a, b) =>
                {
                    if (bOk)
                    {
                        Debug.Log("磁盘满了，退出");
                        Application.Quit();
                    }
                });
                return false;
            }

            return true;
        }
    }
}



