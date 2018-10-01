using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Roma
{
    public static class TextureManager
    {
        /// <summary>
        /// 临时下载列表
        /// </summary>
        private static Dictionary<int, List<RawImage>> m_listLoading = new Dictionary<int, List<RawImage>>();
        private static Dictionary<int, Texture> m_dicTexure = new Dictionary<int, Texture>();
        private static List<Resource> m_listRes = new List<Resource>();

        public static void SetImage(int resId, RawImage rawImage)
        {
            Texture texture;
            if(m_dicTexure.TryGetValue(resId, out texture))
            {
                rawImage.gameObject.SetActiveNew(true);
                rawImage.enabled = true;
                rawImage.texture = texture;
            }
            else
            {
                rawImage.enabled = false;

                List<RawImage> list;
                if (m_listLoading.TryGetValue(resId, out list))
                {
                    list.Add(rawImage);
                }
                else
                {
                    list = new List<RawImage>();
                    list.Add(rawImage);
                    m_listLoading.Add(resId, list);
                    LoadResource(resId);
                }
            }
        }

        public static Texture GetImage(int resId)
        {
            Texture img;
            m_dicTexure.TryGetValue(resId, out img);
            return img;
        }

        /// <summary>
        /// 进入战斗时，清掉所有动态icon
        /// </summary>
        public static void Clear()
        {
            m_dicTexure.Clear();
            for (int i = 0; i < m_listRes.Count; i++)
            {
                ResourceFactory.Inst.UnLoadResource(m_listRes[i], true);
            }
            m_listRes.Clear();
        }

        private static void LoadResource(int resId)
        {
            Resource tempRes = ResourceFactory.Inst.LoadResource(resId, (res) =>
            {
                if (null == res)
                {
                    Debug.LogError("图片资源为空：" + resId);
                    return;
                }
                IconResource iconRes = res as IconResource;
                if (null == iconRes)
                {
                    Debug.LogError(resId + "动态图片加载失败,类型错误 !");
                    return;
                }

                Texture tex = iconRes.GetTexture();
                TextureManager.m_dicTexure.Add(resId, tex);

                List<RawImage> list = TextureManager.m_listLoading[resId];
                for (int i = 0; i < list.Count; i++)
                {
                    if (null == list[i])
                        continue;
                    list[i].enabled = true;
                    list[i].texture = tex;
                }
                list.Clear();
                list = null;
                TextureManager.m_listLoading.Remove(resId);
            });
            m_listRes.Add(tempRes);
        }
    }
}