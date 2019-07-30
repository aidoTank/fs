using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 状态类BUFF
    /// </summary>
    public class Buff10 : BuffBase
    {
        private CCreature ride;

        public Buff10(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();
            m_rec.SetState((eBuffState)m_buffData.ParamValue1, true);

            //ride = m_rec.GetRide();
            if (ride != null)
            {
                ride.SetState((eBuffState)m_buffData.ParamValue1, true);
            }

            switch (m_buffData.ParamValue1)
            {
                case (int)eBuffState.stun:
                    //Debug.Log("晕眩，添加特效");
                    SetStopMove(m_rec);
                    SetStopSkill(m_rec);
                    break;
                case (int)eBuffState.silent:
                    Debug.Log("沉默");
                    break;
                case (int)eBuffState.God:
                    //Debug.Log("无敌");
                    break;
                case (int)eBuffState.unmove:
                    SetStopMove(m_rec);
                    break;
                case (int)eBuffState.sleep:
                    SetStopMove(m_rec);
                    SetStopSkill(m_rec);
                    break;
            }

        }

        public override bool IsStateBuff()
        {
            return true;
        }

        public override void ExecuteFrame()
        {

        }

        public override void Destroy()
        {
            base.Destroy();

            m_rec.SetState((eBuffState)m_buffData.ParamValue1, false);

            //ride = m_rec.GetRide();
            if (ride != null)
            {
                ride.SetState((eBuffState)m_buffData.ParamValue1, true);
            }

            switch (m_buffData.ParamValue1)
            {
                case (int)eBuffState.stun:
                    //Debug.Log("移除 晕眩");
                    ResetMove(m_rec);
                    ResetSkill(m_rec);
                    break;
                case (int)eBuffState.silent:
                    //Debug.Log("移除 沉默");
                    break;
                case (int)eBuffState.God:
                    //Debug.Log("移除 无敌");
                    break;
                case (int)eBuffState.unmove: // 禁锢
                    ResetMove(m_rec);
                    break;
                case (int)eBuffState.sleep:
                    ResetMove(m_rec);
                    ResetSkill(m_rec);
                    break;
            }
        }

        public void SetStopMove(CCreature rec)
        {
            rec.SetLogicMoveEnabled(false);
            rec.PushCommand(CmdFspStopMove.Inst);
            //CCreature ride = rec.GetRide();
            //if (ride != null)
            //{
            //    ride.m_logicMoveEnabled = false;
            //    ride.PushCommand(CmdFspStopMove.Inst);
            //    ride.StopAutoMove();
            //}
        }

        private void SetStopSkill(CCreature rec)
        {
            rec.m_logicSkillEnabled = false;
            //CCreature ride = rec.GetRide();
            //if (ride != null)
            //{
            //    ride.m_logicSkillEnabled = false;
            //}
        }

        private void ResetMove(CCreature rec)
        {
            // 解除BUFF状态时，如果没有晕眩，禁锢，则重置遥感，让遥感继续生效
            if (!rec.bStateBuff(eBuffState.stun) && !rec.bStateBuff(eBuffState.unmove) && !rec.bStateBuff(eBuffState.sleep))
            {
                rec.SetLogicMoveEnabled(true);
                rec.UpdateUI_ResetJoyStick(true);
            }

            //rec = rec.GetRide();
            if (rec != null)
            {
                if (!rec.bStateBuff(eBuffState.stun) && !rec.bStateBuff(eBuffState.unmove) && !rec.bStateBuff(eBuffState.sleep))
                {
                    rec.SetLogicMoveEnabled(true);
                    rec.UpdateUI_ResetJoyStick(true);
                }
            }
        }

        private void ResetSkill(CCreature rec)
        {
            rec.m_logicSkillEnabled = true;
            //CCreature ride = rec.GetRide();
            //if (ride != null)
            //{
            //    ride.m_logicSkillEnabled = true;
            //}
        }
    }
}