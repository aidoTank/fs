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

        public void SetEnt(Entity mt)
        {
            m_ent = mt;
        }



        public CCreature GetCreature()
        {
            return m_creature;
        }

        public Entity GetEnt()
        {
            return m_ent;
        }

        public void SetUid(int uid)
        {
            m_uid = uid;
        }

        public int GetUid()
        {
            return m_uid;
        }

        static public GameObjectHelper Get(GameObject go)
        {
            GameObjectHelper helper = go.GetComponent<GameObjectHelper>();
            if (helper == null)
                helper = go.AddComponent<GameObjectHelper>();
            return helper;
        }

        private CCreature m_creature;
        private Entity m_ent;
        private int m_uid;
    }
}
