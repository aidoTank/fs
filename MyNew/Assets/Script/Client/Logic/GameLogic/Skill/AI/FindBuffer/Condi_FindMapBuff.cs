using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Roma
{
    /// <summary>
    /// 寻找目标
    /// 1.遍历场景
    /// 2.寻找最近的目标
    /// </summary>
    public class Condi_FindMapBuff_ : AICondi
    {
        public long m_lookDis2 = 400;

        //private int m_curTime;
        /// <summary>
        /// 优先选择最近目标
        /// </summary>
        public override bool Check()
        {

            //float minDis2 = 99999;
            //int buffUid = 0;
            //List<long> list = CCreatureMgr.GetCreatureList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    CCreature cc = CCreatureMgr.Get(list[i]);
            //    if (cc.GetThingType() != EThingType.Item)
            //        continue;

            //    Vector2 pos = cc.GetPos();
            //    float abDis2 = Collide.GetDis2(m_creature.GetPos(), pos);
            //    if (abDis2 < m_lookDis2)
            //    {
            //        if (abDis2 < minDis2)
            //        {
            //            minDis2 = abDis2;
            //            buffUid = (int)cc.GetUid();
            //        }
            //    }
            //}
            //if (buffUid != 0)
            //{
            //    m_dataBase.SetData<int>((int)eAIParam.INT_BUFF_ID, buffUid);
            //    m_creature.DestoryDownUpSkill();
            //    return true;
            //}
            return false;
        }
    }
}


