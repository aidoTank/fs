using UnityEngine;  
using UnityEditor;

public class ExportLightmapInfo {

    [MenuItem("光照图/保存该场景预制件的烘焙信息")]
    static void SaveLightmapInfo()  
    {  
        GameObject go = Selection.activeGameObject;
        if(null == go)
		return;
        PrefabLightmapData data = go.GetComponent<PrefabLightmapData>();
        if (data == null)
        {
            data = go.AddComponent<PrefabLightmapData>();
        }
        data.SaveLightmap();
        //EditorUtility.SetDirty(go);
        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go), ReplacePrefabOptions.ConnectToPrefab);
    }
}