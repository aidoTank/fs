using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

/// pc:
/// 1.pc上网络地址就是stream目录，是最新资源
/// 2.新建一个目录，用于模拟移到平台的沙盒目录
/// 3.首次进入游戏将主配置文件解压到沙盒
/// 4.下载网络主配置文件，加载沙盒主配置文件，进行版本对比，然后更新资源
/// 5.进入游戏，优先获取沙盒资源，如果没有去stram目录获取
namespace Roma
{
    public partial class GameInit
    {
        //public void InitAppVersion()
        //{
        //    Client.Inst().m_uiResInit.OpenPanel(true);
        //}

        public void OnCheckFirst()
        {
            Client.Inst().m_uiResInit.OpenPanel(true);

            bool bFirst = true;
            string preVersion = PlayerPrefs.GetString("version", "0");
            if(Application.version.Equals(preVersion))
            {
                Debug.Log("版本号相同，不是第一次进入游戏");
                bFirst = false;
            }
            if(bFirst)
            {
                OnStartFirst();
            }
            else
            {
                OnCheckResource();
            }
        }

        private void OnStartFirst()
        {
            Client.Inst().m_uiResInit.SetText("首次进入");
            Debug.Log("第一次进入游戏，从steam加载主配置");
            GlobleConfig.m_gameState = eGameState.First;
            GlobleConfig.m_downLoadType = eDownLoadType.StreamingAssetsPath;

            ResInfo rInfo = new ResInfo();
            rInfo.m_bDepend = false;
            rInfo.strName = ExportDefine.m_prefix;
            rInfo.strUrl = ExportDefine.m_prefix;
            rInfo.iType = ResType.ManifestResource;
            ResourceFactory.Inst.LoadResource(rInfo, (res)=> {

                res.SaveToDisk();
                ResourceFactory.Inst.UnLoadResource(res, true);

                PlayerPrefs.SetString("version", Application.version);
                Debug.Log("第一次进入游戏，保存主配置到沙盒成功，写入版本号到本地:" + Application.version);

                OnCheckResource();
            });
        }

        private void OnCheckResource()
        {
            Client.Inst().m_uiResInit.SetText("检查更新");

            Debug.Log("开始检测资源更新，1.加载本地老的主配置");
            GlobleConfig.m_gameState = eGameState.Update;
            GlobleConfig.m_downLoadType = eDownLoadType.PersistentDataPath;

            ResInfo rInfo = new ResInfo();
            rInfo.m_bDepend = false;
            rInfo.strName = ExportDefine.m_prefix;
            rInfo.strUrl = ExportDefine.m_prefix;
            rInfo.iType = ResType.ManifestResource;
            ResourceFactory.Inst.LoadResource(rInfo, OnOldInfoEnd);
        }

        private void OnOldInfoEnd(Resource res)
        {
            Dictionary<string, string> oldInfo = (res as ManifestResource).GetNameAndHashId();
            ResourceFactory.Inst.UnLoadResource(res, true);

            Debug.Log("开始检测资源更新，2.加载服务器上的主配置");
            GlobleConfig.m_downLoadType = eDownLoadType.WWW;
            ResInfo rInfo = new ResInfo();
            rInfo.m_bDepend = false;
            rInfo.strName = ExportDefine.m_prefix;
            rInfo.strUrl = ExportDefine.m_prefix;
            rInfo.iType = ResType.ManifestResource;
            ResourceFactory.Inst.LoadResource(rInfo, (newRes)=> {

                m_newManifest = newRes;
                Dictionary<string, string> newInfo = (newRes as ManifestResource).GetNameAndHashId();

                int maxCount = newInfo.Count;
                List<string> updateList = new List<string>();
                foreach (string newName in newInfo.Keys)
                {
                    if(!oldInfo.ContainsKey(newName))   // 1.本地不包含的资源
                    {
                        updateList.Add(newName);
                    }
                    else                                // 2.本地包含，但是hashid不相同时
                    {
                        string oldHashId = oldInfo[newName];
                        string newHashId = newInfo[newName];
                        if (!oldHashId.Equals(newHashId))
                        {
                            updateList.Add(newName);
                        }
                    }
                }
                Debug.Log("检查完毕，开始更新：需要更新数量：" + updateList.Count);
                if (updateList.Count != 0)
                {
                    OnStartUpdate(ref updateList);
                }
                else
                {
                    StartGame();
                }
            });
        }

        private void OnStartUpdate(ref List<string> updateList)
        {
            int totalSize = 0;
            ResInfo[] templist = new ResInfo[updateList.Count];
            for(int i = 0; i < updateList.Count; i ++)
            {
                ResInfo resInfo = new ResInfo();
                resInfo.m_bDepend = false;
                resInfo.strName = updateList[i];
                resInfo.strUrl = updateList[i];
                resInfo.iType = ResType.None;
                resInfo.m_size = GetFileSize(resInfo.strUrl);
                templist[i] = resInfo;

                totalSize += resInfo.m_size;
            }

            Client.Inst().m_uiResInitDialog.OpenPanel(true);
            Client.Inst().m_uiResInitDialog.SetText("继续", "退出", "需要更新文件大小："+ MathEx.GetSize(totalSize));
            Client.Inst().m_uiResInitDialog.AddEvent((bOk, a, b) =>
            {
                if (bOk)
                {
                    for (int i = 0; i < templist.Length; i++)
                    {
                        Resource res = ResourceFactory.Inst.LoadResource(templist[i], OnNewResLoaded);
                        m_updateList.Add(res);
                    }
                }
                else
                {
                    Application.Quit();
                }
            });
        }

        private void OnNewResLoaded(Resource res)
        {
            // 保存到磁盘
            res.SaveToDisk();
            ResourceFactory.Inst.UnLoadResource(res, true);

            Debug.Log("更新资源存盘：" + res.GetResInfo().strUrl);

            ++m_curUpdateCount;
            if (m_curUpdateCount == m_updateList.Count)
            {
                Debug.Log("更新完毕，总数：" + m_updateList);
                StartGame();
            }
        }

        private void StartGame()
        {
            m_newManifest.SaveToDisk();
            ResourceFactory.Inst.UnLoadResource(m_newManifest, true);
            InitBaseConfig();
        }

        public int GetFileSize(string url)
        {
            string fullUrl = GlobleConfig.GetFileServerPath() + url;
            return (int)GetHttpFileSize(fullUrl);
        }

        // 获取远程文件大小
        public long GetHttpFileSize(string url)
        {
            long size = 0L;
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Method = "HEAD";
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                size = response.ContentLength;
                //size = System.Convert.ToInt64(response.Headers["Content-Length"]);
                response.Close();
            }
            catch
            {
                size = 0L;
            }
            return size;
        }

        public void OnUpdateUI()
        {
            if (m_curUpdateCount == m_updateList.Count)
                return;

            m_curSize = 0;
            m_maxSize = 0;
            for (int i = 0; i < m_updateList.Count; i ++)
            {
                Resource res = m_updateList[i];
                float resSize = (float)res.GetResInfo().m_size;
                m_curSize += res.GetDownLoadProcess() * resSize;
                m_maxSize += resSize;
            }
            string pct = ((m_curSize / m_maxSize) * 100).ToString("F1") + "% ";
            string info = MathEx.GetSize((int)m_curSize) + "/" + MathEx.GetSize((int)m_maxSize);
            Client.Inst().m_uiResInit.SetText("更新进度：" + pct + info);
            Client.Inst().m_uiResInit.SetProgress(m_curSize / m_maxSize);
        }


        private List<Resource> m_updateList = new List<Resource>();
        private Resource m_newManifest;   // 新的主配置，在更新完成后保存

        private int m_curUpdateCount;
        private float m_curSize;
        private float m_maxSize;
    }
}