using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class Dead : FSMState
    {
        public Dead(CCreature go)
            : base(go)
        {
            m_stateId = StateID.DeadState;
        }
        public override void Enter()
        {
            //LogicSystem.Inst.PlaySound(4797);  // 死亡声音

            AnimationAction anima = new AnimationAction();
            anima.crossTime = AnimationInfo.m_crossTime;
            anima.strFull = AnimationInfo.m_animaDie_1;
            anima.eMode = WrapMode.Once;

            //if (m_creature.GetRideState())   // 有坐骑时，人物不播放死亡
            //{
            //    //m_creature.Play(anima);
            //    m_creature.GetCCreture().Play(anima);
            //    anima.endEvent = AfterDeadEvent;
            //}
            //else
            //{
            //    anima.endEvent = AfterDeadEvent;
            //    if (null != m_creature.GetEntity())
            //        m_creature.GetEntity().Play(anima);

            //    m_creature.GetEntity().CancelAnimaAutoPlay();
            //}
        }

        public void AfterDeadEvent(AnimationAction animation)
        {
            //if(m_creature.IsPet() || m_creature.m_bDeadHide)  // 如果是宠物直接死亡消失
            //{
            //    m_creature.m_ent.SetDissolveShader(Color.white, () => {
            //        BtInfoModule bt = LayoutMgr.Inst.GetLogicModule<BtInfoModule>(LayoutName.S_BtInfo);
            //        bt.SetPlayerInfo();
            //        LogicSystem.Inst.RemoveCreatrue(m_creature.GetUid());
            //    });
            //}
            //else
            //{ 
            //    // 战斗场景外的死亡
            //    m_creature.SetRealDead();

            //    // 测试溶解
            //    //m_creature.m_ent.SetDissolveShader(Color.white, () =>
            //    //{
            //    //    if (!BattleControl.GetSingle().itfBattleType)
            //    //        LogicSystem.Inst.RemoveCreatrue(m_creature.GetUid());
            //    //});
            //}
        }

        public override void Update(float fTime, float fDTime)
        {
            //if(m_creature.IsPet() || m_creature.m_bDeadHide)
            //{
            //    m_creature.SetPos(new Vector3(m_creature.GetPos().x, m_creature.GetPos().y - 0.001f, m_creature.GetPos().z));
            //}
        }
        public override void Exit()
        {

        }
    }
}