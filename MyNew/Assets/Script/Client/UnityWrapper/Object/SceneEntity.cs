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
            //GameObject staticObj = GameObject.Find("Static");
            //if(staticObj != null)
            //{
            //    StaticBatchingUtility.Combine(staticObj);
            //}
        }


    }
}
