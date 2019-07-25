using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public class Condi_List : AICondi
    {
        public AICondi[] m_condiList;

        public Condi_List(AICondi[] condiList)
        {
            m_condiList = condiList;
        }

        public override void Activate(BtDatabase database)
        {
            base.Activate(database);

            for (int i = 0; i < m_condiList.Length; i++)
            {
                m_condiList[i].Activate(database);
            };
        }

        public override bool Check()
        {
            for(int i = 0; i < m_condiList.Length; i ++)
            {
                bool check = m_condiList[i].Check();
                if (!check)
                {
                    return false;
                }
            }
            return true;
        }
    }
}


