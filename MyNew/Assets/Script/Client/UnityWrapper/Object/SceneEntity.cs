using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    public class SceneEntity : Entity
    {

        public SceneEntity(int handle, Action<Entity> notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {

        }

        public override void InstantiateGameObject(Resource res)
        {
            base.InstantiateGameObject(res);
            PrefabLightmapData lm = m_object.GetComponent<PrefabLightmapData>();
            if(lm != null)
            {
                lm.LoadLightmap();
            }

            //GameObject staticObj = GameObject.Find("Static");
            GameObject staticObj = m_object;
            if(staticObj != null)
            {
               //StaticBatchingUtility.Combine(staticObj);

               StaticBatching(staticObj);
            }
        }

        private void StaticBatching(GameObject root)
        {
            MeshRenderer[] meshList = GameObject.FindObjectsOfType<MeshRenderer>();
            Dictionary<Material, List<GameObject>> combieDic = new Dictionary<Material, List<GameObject>>();
            for (int i = 0; i < meshList.Length; i++)
            {
                GetObjectList(meshList[i], ref combieDic);
            }

            foreach (KeyValuePair<Material, List<GameObject>> item in combieDic)
            {
                List<GameObject> list = item.Value;
                GameObject[] goList = new GameObject[list.Count];
                for(int i = 0; i < list.Count; i++)
                {
                    goList[i] = list[i];
                }
                StaticBatchingUtility.Combine(goList, root);
            }
        }


        private void GetObjectList(MeshRenderer mr, ref Dictionary<Material, List<GameObject>> dic)
        {
            List<GameObject> list;
            if(dic.TryGetValue(mr.sharedMaterial, out list))
            {
                list.Add(mr.gameObject);
            }
            else
            {
                dic.Add(mr.sharedMaterial, new List<GameObject>());
            }
        }
    }
}
