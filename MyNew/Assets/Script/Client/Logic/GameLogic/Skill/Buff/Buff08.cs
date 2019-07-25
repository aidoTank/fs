using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 护甲增益
    /// </summary>
    public class Buff08 : BuffBase
    {
        public Buff08(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();
            //m_rec.UpdateDp();
            //if(m_rec.IsMaster())
            //{                
            //    DoubleHitTipsModule module = (DoubleHitTipsModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelDoubleHit);
            //    if (module.IsShow())
            //    {
            //        module.DPAdd();
            //    }
            //}
        }

        public override void Destroy()
        {
            base.Destroy();
            //m_rec.UpdateDp();
        }
    }
}