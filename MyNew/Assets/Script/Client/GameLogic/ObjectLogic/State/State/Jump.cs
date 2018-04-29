using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class Jump : FSMState
    {
        public Jump(CCreature go)
            : base(go)
        {
            m_stateId = StateID.JumpState;
        }
        public override void Enter()
        {
            //Debug.Log("开始Jump");
            // 初始化数据
            //m_gameObject.Play(AnimaType.Jump);
            //m_jumpDir = 1;
            //m_curAcceleration = m_maxAcceleration;
        }
       // public override void Update(float fTime, float fDTime)
       // {
            // 控制加速度
            //m_curAcceleration -= Time.deltaTime * m_accelerationDelta;
            //if (m_curAcceleration < m_minAcceleration)
            //{
            //    m_curAcceleration = m_minAcceleration;
            //}
            // 控制方向：根据设置的【跳跃高度】和【地形高度】
            //if (m_gameObject.m_ent.GetPos().y > m_jumpHeight)
            //{
            //    m_jumpDir = -1;
            //}
            //if (m_gameObject.m_ent.GetPos().y < SceneManager.Inst.GetTerrainHeight(m_gameObject.m_ent.GetPos().x, m_gameObject.m_ent.GetPos().z))
            //{
            //   // m_gameObject.m_ent.MoveGround(m_gameObject.m_ent.GetRotate(), Vector3.zero);
            //    m_jumpDir = 0;
            //    //m_creature.ChangeFSM(StateID.Idle);
            //    m_creature.SetPreState();
            //}

            // 跳跃之后根据需求，可以控制在空中是否偏移
            //float v = Input.GetAxisRaw("Vertical");
           // float h = Input.GetAxisRaw("Horizontal");
            //m_gameObject.m_ent.MoveSky(
            //                            Vector3.up * h * Time.deltaTime * m_creature.m_propPublic.rotateSpeed,
            //                            new Vector3(h, m_jumpDir, v) * Time.deltaTime * m_curAcceleration);

       // }
        public override void Exit()
        {
            //Debug.Log("结束Jump");
        }

        //private short m_jumpDir = 0;        // 跳跃方向
        //private float m_jumpHeight = 4.0f;  // 跳跃的高度
        //private float m_curAcceleration = 0.0f;  // 当前加速度
        //private float m_accelerationDelta = 8f;  // 加速度增量

        //private const float m_maxAcceleration = 14f;
        //private const float m_minAcceleration = 5f;
    }
}