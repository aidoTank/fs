using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// BUFF触发-怪物
    /// </summary>
    public class Buff22 : BuffBase
    {

        public Buff22(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();

            CreateCreature();
        }

        private void CreateCreature()
        {
            //CCreature p1 = CCreatureMgr.Create(EThingType.Monster, m_caster.GetUid() + SCreatrueUid.Trigger++);
            ////p1.m_footHaloEffectId = data.effectsID;
            //Vector2 pos = m_caster.GetPos();
            //p1.Create(GetVal1(), "", pos, m_caster.GetDir());
            //p1.m_ai = new CCreatureAI(p1, eAILevel.HARD, true);
            //p1.m_refreshTime = 0;
            //if (CMapMgr.GetMap().m_bfb)
            //{
            //    p1.SetPropNum(eCreatureProp.Lv, m_caster.GetPropNum(eCreatureProp.Lv));
            //    p1.UpdateMonsterProp();
            //}
        }

        public override void ExecuteFrame()
        {

        }

        public override void Destroy()
        {
            base.Destroy();
        }

    }
}