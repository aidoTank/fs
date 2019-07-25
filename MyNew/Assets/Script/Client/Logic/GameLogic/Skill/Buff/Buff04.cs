using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 增减攻击
    /// </summary>
    public class Buff04 : BuffBase
    {
        public Buff04(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();
            if (m_rec == null)
                return;

            //m_rec.UpdateAp();
            //if (m_rec.IsMaster())
            //{
            //    DoubleHitTipsModule module = (DoubleHitTipsModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelDoubleHit);
            //    if (module.IsShow())
            //    {
            //        module.ATKAdd();
            //    }
            //}
        }

        public override void Destroy()
        {
            base.Destroy();

            if (m_rec == null)
                return;
            //m_rec.UpdateAp();
        }

    }
}