using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

namespace Roma
{
    public class ResourceFactory : Singleton
    {
        public static ResourceFactory Inst = null;

        public ResourceFactory()
            : base(true)
        {
        }

        public override void Init()
        {
        }



        public void GC()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.Collect();
            System.GC.Collect();
        }

        public Resource LoadResource(int resID, Action<Resource> load)
        {
            if (resID == 0)
                return null;
            ResInfo resInfo = ResInfosResource.GetResInfo(resID);
            if (null == resInfo || string.IsNullOrEmpty(resInfo.strUrl))
            {
                load(null);
                return null;
            }
            Resource ressource = LoadResource(resInfo, load);
            return ressource;
        }

        public Resource LoadResource(string strResName, Action<Resource> load)
        {
            //从资源总表查
            ResInfo resInfo = ResInfosResource.GetResInfo(strResName);
            if (null == resInfo || string.IsNullOrEmpty(resInfo.strUrl))
            {
                load(null);
                return null;
            }
            Resource ressource = LoadResource(resInfo, load);
            return ressource;
        }

        public Resource LoadResource(ResInfo resInfo, Action<Resource> load)
        {
            // 直接通过资源信息下载资源
            Resource ressource = ResourceManager.Inst.LoadResource(resInfo, load);
            return ressource;
        }


        /// <summary>
        /// 默认卸载资源接口都加入卸载缓存
        /// </summary>
        public void UnLoadResource(Resource res)
        {
            if (null == res)
            {
                return;
            }
            ResourceManager.Inst.UnLoadResource(res, false);
        }

        /// <summary>
        /// atOnce立即销毁该资源的所有ab资源
        /// unLoadAllAB销毁该资源的所有ab关联资源
        /// </summary>
        public void UnLoadResource(Resource res, bool atOnce)
        {
            if (null == res)
            {
                return;
            }
            ResourceManager.Inst.UnLoadResource(res, atOnce);
        }

        // 退出游戏为true
        public override void Destroy()
        {
            GC();
        }


        public string GetLoadResource()
        {
            string curInfo = "";

            string uintInfo = "";

            int num = 0;
            foreach (KeyValuePair<string, Resource> res in ResourceManager.Inst.m_mapResource)
            {

                ResInfo resInfo = res.Value.GetResInfo();
                string iRef = "[引用：" + res.Value.m_ref + "]  ";


                uintInfo += "\n" + num + "." + iRef + resInfo.strName;
                num++;
            }
            if (!string.IsNullOrEmpty(uintInfo))
            {
                curInfo += "数量：" + num + uintInfo + "\n";
            }

            return curInfo;
        }

        public string GetUnLoadResource()
        {
            string curInfo = "";

            string uintInfo = "";
            int num = 0;
            foreach (KeyValuePair<string, Resource> res in ResourceManager.Inst.m_mapUnLoad)
            {
                ResInfo resInfo = res.Value.GetResInfo();
                string iRef = "[引用：" + res.Value.m_ref + "] ";
                uintInfo += "\n" + num + "." + iRef + resInfo.strName;
                num++;
            }
            if (!string.IsNullOrEmpty(uintInfo))
            {
                curInfo += "数量：" + num + uintInfo + "\n";
            }

            return curInfo;
        }
    }
}