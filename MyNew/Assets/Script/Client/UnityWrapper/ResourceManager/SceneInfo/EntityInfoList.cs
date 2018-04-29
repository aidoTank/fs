using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class EntityInfoList : DataBlock
    {
        private List<EntityBaseInfo> m_allEntityList = new List<EntityBaseInfo>();

        public EntityInfoList()
        {
        }
        public override bool Write(LusuoStream lf)
        {
            //Debug.Log("实体类自身写入");
            int count = m_allEntityList.Count;
            lf.WriteInt(count);
            foreach (EntityBaseInfo item in m_allEntityList)
            {
                item.Save(ref lf);
            }
            return true;
        }

        public override bool Read(ref LusuoStream lf)
        {
            int count = 0;
            lf.ReadInt(ref count);
            for (int i = 0; i < count; i ++)
            {
                EntityBaseInfo eInfo = new EntityBaseInfo();
                eInfo.Load(lf);
                m_allEntityList.Add(eInfo);
            }
            return true;
        }

        public List<EntityBaseInfo> GetEntityInfoList()
        {
            return m_allEntityList;
        }

        public void GetNeedEntityInfoList(string[] resList, 
            ref List<EntityBaseInfo> needList,
            ref List<EntityBaseInfo> notNeedList)
        {
            if (needList == null)
                needList = new List<EntityBaseInfo>();
            if (notNeedList == null)
                notNeedList = new List<EntityBaseInfo>();

            for (int i= 0; i < m_allEntityList.Count; i ++)
            {
                EntityBaseInfo baseInfo = m_allEntityList[i];
                bool bHave = false;
                for(int resIndex = 0; resIndex < resList.Length; resIndex++)
                {
                    if(baseInfo.m_strName.Equals(resList[resIndex]))
                    {
                        needList.Add(baseInfo);
                        bHave = true;
                        continue;
                    }
                }
                if(!bHave)
                {
                    notNeedList.Add(baseInfo);
                }
            }
        }
    }
}
