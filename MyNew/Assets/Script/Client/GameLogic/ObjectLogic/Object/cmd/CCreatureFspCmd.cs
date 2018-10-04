using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
namespace Roma
{
    public enum CmdFspEnum
    {
        eFspMove,           // 移动
        eFspStopMove,       // 停止移动
        eFspRotation,       // 转向
        eFspSendSkill,      // 技能 逻辑，表现层公用
        eFspCancelSkill,    //取消技能


        eSkillCreate,
        eSkillHit,
    }
    public class IFspCmdType
    {
        public CmdFspEnum cmdenum;
        public IFspCmdType()
        {
            
        }
        public CmdFspEnum GetCmdType()
        {
            return cmdenum;
        }
    }

     /// <summary>
    /// 移动命令
    /// </summary>
    public class CmdFspMove : IFspCmdType
    {
        public Vector2 m_dir;
        public Vector2 m_pos;
        public CmdFspMove()
        {
            cmdenum = CmdFspEnum.eFspMove;
        }
        public CmdFspMove(ref Vector2 dir)
        {
            cmdenum = CmdFspEnum.eFspMove;
            m_dir = dir;
        }
        public CmdFspMove(ref Vector2 dir, ref Vector2 pos)
        {
            cmdenum = CmdFspEnum.eFspMove;
            m_dir = dir;
            m_pos = pos;
        }
    }

    public class CmdFspRotation : IFspCmdType
    {
        public Vector3 m_rotation;
        public CmdFspRotation()
        {
            cmdenum = CmdFspEnum.eFspRotation;
        }
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    public class CmdFspStopMove : IFspCmdType
    {
        public CmdFspStopMove()
        {
            cmdenum = CmdFspEnum.eFspStopMove;
        }
    }

    public class CmdFspSendSkill : IFspCmdType
    {
        public int m_casterUid;
        public int m_skillId;
        public long m_targetId;
        public Vector2 m_dir;
        public Vector2 m_startPos;
        public Vector2 m_endPos;
 
        //public int m_casterHid;
        public CmdFspSendSkill()
        {
            cmdenum = CmdFspEnum.eFspSendSkill;
        }
    }





    public class CmdSkillCreate : IFspCmdType
    {
        public CmdSkillCreate()
        {
            cmdenum = CmdFspEnum.eSkillCreate;
        }
    }

    public class CmdSkillHit : IFspCmdType
    {
        public bool bPlayer;
        /// <summary>
        /// 玩家时，为uid
        /// </summary>
        public int uid;
        public Vector3 pos; // 弹道爆炸的位置

        public CmdSkillHit()
        {
            cmdenum = CmdFspEnum.eSkillHit;
        }
    }

}