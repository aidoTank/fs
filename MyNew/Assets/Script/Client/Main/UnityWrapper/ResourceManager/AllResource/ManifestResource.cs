using UnityEngine;
using System.Collections.Generic;

namespace Roma
{

    public class ManifestResource : Resource
    {
        public ManifestResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool SetResource(ref WWW www)
        {
            base.SetResource(ref www);
            if (GlobleConfig.m_gameState == eGameState.Update)
            {
                m_assertBundle = www.assetBundle;    // 主配置文件比较特殊， 在更新时也需要读取ab，数据用于版本对比
            }
            return true;
        }

        public override bool OnLoadedLogic()
        {
            return true;
        }

        public AssetBundleManifest GetManifest()
        {
            if(m_manifest == null)
                m_manifest = m_assertBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");    //获取总的Manifest
            return m_manifest;
        }

        public Dictionary<string, string> GetNameAndHashId()
        {
            GetManifest();
            Dictionary<string, string> nameAndHashId = new Dictionary<string, string>();
            string[] names = m_manifest.GetAllAssetBundles();
            for(int i = 0; i < names.Length; i ++)
            {
                string name = names[i];
                string hashId = m_manifest.GetAssetBundleHash(name).ToString();
                if(nameAndHashId.ContainsKey(name))
                {
                    Debug.LogError("重复资源:"+ name);
                    continue;
                }
                nameAndHashId.Add(name, hashId);
            }
            return nameAndHashId;
        }

        private AssetBundleManifest m_manifest = null;
    }

}
