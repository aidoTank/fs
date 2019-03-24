using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;

public class PrefabLightmapData : MonoBehaviour  
{  
    [System.Serializable]
    struct RendererInfo  
    {  
        public Renderer     renderer;  
        public int          lightmapIndex;  
        public Vector4      lightmapOffsetScale;  
    }

    [UnityEngine.SerializeField]
    Texture2D[] lightmapTexs;   //当前场景的灯光贴图


    [UnityEngine.SerializeField]
    RendererInfo[] rendererList;

    public void SaveLightmap()  
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>(true);
        RendererInfo rendererInfo;
        rendererList = new RendererInfo[renders.Length];

        int index = 0;

        for(int r = 0, rLength = renders.Length; r < rLength; ++r)
        {
            if (renders[r].gameObject.isStatic == false) 
                continue;
            rendererInfo.renderer = renders[r];
            rendererInfo.lightmapIndex = renders[r].lightmapIndex;
            rendererInfo.lightmapOffsetScale = renders[r].lightmapScaleOffset;

            rendererList[index] = rendererInfo;

            ++index;
        }

        //序列化光照贴图
        LightmapData[] ldata = LightmapSettings.lightmaps;
        lightmapTexs = new Texture2D[ldata.Length];
        for(int t = 0, tLength = ldata.Length; t<tLength; ++t)
        {
            lightmapTexs[t] = ldata[t].lightmapLight;
        }
    }
    
    void Awake()
    {
        this.LoadLightmap();
    }


    public void LoadLightmap()
    {
        if(null == rendererList || rendererList.Length == 0)
        {
            Debug.Log(gameObject.name +  " 的 光照信息为空");
            return;
        }

        Renderer[] renders = GetComponentsInChildren<Renderer>(true);

        for(int r = 0, rLength = renders.Length; r<rLength; ++r)
        {
            renders[r].lightmapIndex = rendererList[r].lightmapIndex;
            renders[r].lightmapScaleOffset = rendererList[r].lightmapOffsetScale;
        }

        #if UNITY_EDITOR
        if(null == lightmapTexs || lightmapTexs.Length == 0)
        {
            return;
        }

        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        LightmapData[] ldata = new LightmapData[lightmapTexs.Length];
        LightmapSettings.lightmaps = null;

        for(int t = 0, tLength = lightmapTexs.Length; t<tLength; ++t)
        {
            ldata[t] = new LightmapData();
            ldata[t].lightmapLight = lightmapTexs[t];
        }

        LightmapSettings.lightmaps = ldata;
        #endif
    }  
}