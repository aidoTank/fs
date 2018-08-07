using UnityEngine;
using System.Collections;


namespace Roma
{
    public class GameObjectHelper : MonoBehaviour
    {
        public void SetCreatrue(CCreature cc)
        {
            m_creature = cc;
        }

        //public void SetMtCreatrue(MtCreature mt)
        //{
        //    m_mtCreature = mt;
        //}

        public void SetResource(Resource res)
        {
            m_resource = res;
        }

        public CCreature GetCreature()
        {
            return m_creature;
        }

        //public MtCreature GetMTCreature()
        //{
        //    return m_mtCreature;
        //}

        public Resource GetResource()
        {
            return m_resource;
        }

        static public GameObjectHelper Get(GameObject go)
        {
            GameObjectHelper helper = go.GetComponent<GameObjectHelper>();
            if (helper == null)
                helper = go.AddComponent<GameObjectHelper>();
            return helper;
        }

        private CCreature m_creature;
        //private MtCreature m_mtCreature;
        private Resource m_resource;
    }
}
