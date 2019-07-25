using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 命中时触发BUFF的BUFF
    /// </summary>
    public class Buff11 : BuffBase
    {

        public Buff11(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public void CreateBuff(CCreature foe)
        {
            // format is "buffid1_chance2_buffid2_chance2"
            int[] data = m_buffData.listParam;
            for(int i = 0; i < data.Length; i +=2)
            {
                int chance = data[i + 1];
                //int pct = GameManager.Inst.m_clientRand.Next(0, 100);
                //if(pct < chance)
                //{
                //    int buffId = data[i];
                //    SkillBase.AddBuff(m_caster, foe, buffId, m_startPos, m_skillPos, m_skillDir, -1);
                //}
            }
        }
    }
}