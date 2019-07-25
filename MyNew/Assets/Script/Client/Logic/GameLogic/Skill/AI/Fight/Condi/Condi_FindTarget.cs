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
    public partial class Condi_FindTarget : AICondi
    {
        public int m_lookDis = 24;     // 检测距离
        public int m_shaseDis = 40;    // 追击距离

        public bool m_bCheckTarget = true;

        /// <summary>
        /// 0.超过巡逻点太远，放弃战斗
        /// 1.当前有目标，并且在10米内，转下一个条件
        /// 2.仇恨高，作为目标
        /// 3.范围最近，作为目标
        /// </summary>
        public override bool Check()
        {
       
                //if (!m_bCheckTarget)
                //    return false;
                //// 如果大于追踪距离
                //Vector2 dV = m_creature.GetPos() - m_creature.m_bornPoint;
                //float dis2 = Vector2.Dot(dV, dV);
                //if (dis2 >= m_shaseDis * m_shaseDis)
                //{
                //    m_dataBase.SetData<int>((int)eAIParam.INT_TARGET_UID, 0);
                //    Vector2 pPos = m_creature.m_bornPoint;
                //    m_creature.GoTo(pPos);

                //    m_bCheckTarget = false;
                //    CFrameTimeMgr.Inst.RegisterEvent(2000, () =>
                //    {
                //        m_bCheckTarget = true;
                //    });
                //    return false;
                //}
       
      
            bool hasTarget = AIParam.GetAtkTarget(m_creature, m_dataBase, m_lookDis);
            return hasTarget;
        }
    }
}
