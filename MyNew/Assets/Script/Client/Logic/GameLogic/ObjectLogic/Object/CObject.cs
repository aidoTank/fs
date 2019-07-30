
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 场景对象移动基类
    /// </summary>
    public class CObject : CThing
    {
        public CObject(long id)
            : base(id)
        {

        }

        //public AOINode m_aoiNode;
        public FSMMgr m_stateMgr;

        // 逻辑状态数据
        public IFspCmdType m_logicState = CmdFspStopMove.Inst;
        public Vector2d m_curPos;
        public Vector2d m_dir;
        public float m_scale =1;
        public FixedPoint m_speed = new FixedPoint(10);


        // 用于状态BUFF控制的是否能移动
        public bool m_logicMoveEnabled = true;
        // 用于技能保护，状态BUFF控制的是否能放技能
        public bool m_logicSkillEnabled = true;
        // 技能是否能旋转
        public bool m_logicSkillRotationEnabled = false;

        public void SetLogicMoveEnabled(bool bTrue)
        {
            Debug.Log("SetLogicMoveEnabled:" + bTrue);
            m_logicMoveEnabled = bTrue;
        }

        /// <summary>
        /// 逻辑方向平滑
        /// </summary>
        //private float m_rotateCurTime;
        //private float m_rotateMaxTime = 0.5f;
        //private Vector2d m_curDir;
        //private Vector2d m_destDir;

        // 表现层
        public VBase m_vCreature;

        public VObject GetVObject()
        {
            return (VObject)m_vCreature;
        }

        public VTrigger GetVTrigger()
        {
            return (VTrigger)m_vCreature;
        }

        public virtual bool Create(int csvId, string name, Vector2 pos, Vector2 dir, float scale = 1)
        {
            return true;
        }

        public virtual void PushCommand(IFspCmdType cmd)
        {
            if (m_vCreature == null)
                return;
            // 无论是释放技能还是移动都记录移动信息
            // 比如在近战技能时，技能释放过程中，操作了移动遥感，角色方向要和遥感同步
            // 可优化为缓存指令的方式
            //if (cmd is CmdFspMove)
            //    m_cmdFspMove = cmd as CmdFspMove;

            //Debug.Log("==============" + cmd.GetCmdType());
            //if (m_logicMoveEnabled && cmd.GetCmdType() == CmdFspEnum.eFspStopMove)
            //{
            //    m_vCreature.PushCommand(cmd);
            //}
            //else if (m_logicMoveEnabled && cmd.GetCmdType() == CmdFspEnum.eFspMove)
            //{
            //    //Debug.Log("进入移动");
            //    //m_cmdFspMove = cmd as CmdFspMove;
            //    EnterMove();
            //    m_vCreature.PushCommand(cmd);
            //}
            //else if (m_logicSkillRotationEnabled && cmd.GetCmdType() == CmdFspEnum.eFspRotation)
            //{
            //    CmdFspRotation rota = cmd as CmdFspRotation;
            //    m_rotateCurTime = 0;
            //    m_curDir = GetDir();
            //    m_destDir = rota.m_rotation.ToVector2d();
            //    m_vCreature.PushCommand(cmd);
            //}
        }

        public virtual void ExecuteFrame(int frameId)
        {
            //if (m_logicMoveEnabled && m_logicState.GetCmdType() == CmdFspEnum.eFspMove)
            //{
            //    TickMove();
            //}
            //else if (m_logicSkillRotationEnabled && m_logicState.GetCmdType() == CmdFspEnum.eFspRotation)
            //{
            //    // 旋转顶点数待处理
            //    m_rotateCurTime += FSPParam.clientFrameScTime;
            //    float t = m_rotateCurTime / m_rotateMaxTime;
            //    t = t >= 1.0f ? 1.0f : t;
            //    Vector2d dir = Vector2d.Lerp(m_curDir, m_destDir, new FixedPoint(t));
            //    SetDir(dir);
            //}
            if(m_stateMgr != null)
            {
                m_stateMgr.ExecuteFrame(frameId);
            }
        }


        //public void EnterMove()
        //{

        //}

        //public void TickMove()
        //{
        //    if (m_cmdFspMove == null)
        //        return;

        //    Vector2d moveDir = m_cmdFspMove.m_dir.normalized;
        //    FixedPoint delta = new FixedPoint(FSPParam.clientFrameScTime) * GetSpeed();
        //    Vector2d nextPos = m_curPos + moveDir * delta;

        //    SetPos(nextPos);
        //    SetDir(moveDir);  // 技能前摇时，移动时，模型表现方向失效，比如机枪移动时射击
        //    SetSpeed(GetSpeed());

        //    //Debug.Log("无障碍移动");
        //    m_vCreature.SetBarrier(false);
        //}

        public override void Destory()
        {
            m_destroy = true;
            if (m_vCreature != null)
            {
                m_vCreature.Destory();
                m_vCreature = null;
            }
        }

        public virtual void SetSpeed(FixedPoint speed)
        {
            m_speed = speed;

            if (m_vCreature != null)
            {
                m_vCreature.SetSpeed(speed.value);
            }
        }

        public FixedPoint GetSpeed()
        {
            return m_speed;
        }

        public float GetScale()
        {
            return m_scale;
        }

        public virtual void SetScale(float val)
        {
            m_scale = val;
            if (m_vCreature != null)
            {
                m_vCreature.SetScale(val);
            }
        }


        public virtual void SetPos(Vector2d pos, bool tr = false)
        {
            m_curPos = pos;

            if (m_vCreature != null)
            {
                m_vCreature.SetPos(pos.ToVector3(), tr);
            }

            //if (m_aoiNode != null)
            //{
            //    //Debug.Log("move:" + GetUid() + "  " + pos.x + "  " + pos.y);
            //    AOIMgr.Inst.Move(m_aoiNode, (int)pos.x, (int)pos.y);
            //}
        }

        public Vector2d GetPos()
        {
            return m_curPos;
        }

        public virtual void SetDir(Vector2d dir)
        {
            m_dir = dir;
            if (m_vCreature != null)
            {
                m_vCreature.SetDir(dir.ToVector3());
            }
        }

        public virtual Vector2d GetDir()
        {
            return m_dir;
        }

        public FixedPoint GetR()
        {
            return new FixedPoint(1.0f);
        }


    }
}