using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
namespace Roma
{
    public enum eControlMode
    {
        eCM_keyboard,
        eCM_mouse,
        eCM_keyboardCtrl,
        eCM_auto,
        eCM_noFindPath,// 不寻路
    }

    public enum StateID
    {
        IdleState = 0,
        MoveState = 1,
        JumpState = 2,
        DeadState = 3,
        AttackState = 4,
        StrikeState = 5,
        StunState = 6,      // 晕眩状态
        DuckState = 7,      //下蹲,采集，放风筝什么的
        NearState,
        RotateState,

        BtMoveState,
        BtHitBackState,
        BtHitFlyState,
        BtStrikeState,
        //BtRotateState,
        BtJumpState,
        BtFlashState,
    }

    public delegate void OnCreatureCreateEnd(CCreature creature);
    public delegate void OnCreatureDestoryEnd(CCreature creature);

    public partial class CCreature : CThing
    {

        public BoneEntity m_ent = null;        // 角色本身


        public CCreature(long id)
            : base(id)
        {
        }

        public virtual bool InitConfigure()
        {
  
            return false;
        }

        public virtual void OnEntityLoaded(Entity entity, object userObject)
        {
            PushCommand(StateID.IdleState);
        }

        public virtual void PushCommand(StateID stateId)
        {
            
        }


        public virtual bool GoTo(float x, float z, eControlMode mode, int dir)
        {
            
            //m_moveStateParam.m_movePath.Clear();
            return true;
        }



        public virtual void SetShow(bool bShow)
        {
            //Debug.Log(m_name + "=============================="+bShow);
            if (m_ent == null)
            {
                return;
            }
            if (bShow && m_ent.GetActive())
            {
                return;
            }
            if (!bShow && !m_ent.GetActive())
            {
                return;
            }

            m_ent.SetShow(bShow);
     
        }


        public virtual void SetName(string name, string color = null)
        {
            if (string.IsNullOrEmpty(name))
                return;
        }


        public virtual void SetPos(float x, float y)
        {
            if (m_ent != null)
            {
                //m_ent.SetPos(x, y);
            }
        }

        public virtual void SetPos(Vector3 pos)
        {
            if (m_ent != null)
            {
               // m_ent.SetPos(pos);
            }
        }

        public virtual void SetScale(float scale)
        {
            m_ent.SetScale(scale);
        }

        public virtual void SetScale(Vector3 scale)
        {
            m_ent.SetScale(scale);
        }

        public virtual void SetDirection(Vector3 eularAngle)
        {
            if (m_ent != null)
            {
                m_ent.SetDirection(eularAngle);
            }
        }

        public void SetQua(Quaternion vRot)
        {
            if (m_ent != null)
            {
                m_ent.SetRot(vRot);
            }
        }

        // 播放动画
        public void Play(AnimationAction action)
        {
            if (m_ent != null)
            {
                m_ent.Play(action);
            }
        }


        public BoneEntity GetEntity()
        {

            return m_ent;
        }

       
        //public CCreature GetTarget()
        //{
        //    return m_targetCreature;
        //}

        public Vector3 GetPos()
        {
            return m_ent.GetPos();
        }

        public Quaternion GetQua()
        {
            if (m_ent == null)
            {
                return Quaternion.identity;
            }
            return m_ent.GetRotateQua();
        }

        public Vector3 GetScale()
        {
            if (m_ent == null)
            {
                return Vector3.one;
            }
            return m_ent.GetScale();
        }

        /// <summary>
        /// 返回欧拉角
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDirection()
        {
            if (m_ent == null)
            {
                return Vector3.zero;
            }
            return m_ent.GetRotate();
        }



        public virtual void Update(float fTime, float fDTime)
        {
            if (null == m_ent || !m_ent.IsInited())
            {
                return;
            }


        }
    }
}