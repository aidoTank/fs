using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
namespace Roma
{
    public enum CmdFspEnum
    {
        none,

        eFspMove,           // 移动
        eFspStopMove,       // 停止移动
        eFspRotation,       // 转向
        eFspSendSkill,      // 技能 逻辑，表现层公用
        eFspCancelSkill,    //取消技能


        eSkillCreate,
        eSkillHit,


        eUIHead,
        eLife,
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


        /// <summary>
    /// 设置表现层，只用于设置一次的时候
    /// </summary>
    public class CmdUIHead : IFspCmdType
    {
        // 1.名字 2等级 3血量 4跳字 5隐藏 6经验 7BUFF信息 8阵营 9复活点CD 10战斗表情 11玩家特效 12等级经验提示效果
        public int type;

        public string name;
        public int lv;

        public int curHp;
        public int maxHp;

        public eHUDType hudType;
        public string hudText;

        public bool bShow;

        public int curExp;
        public int maxExp;

        public int buffId;
        public int buffIcon;
        public int buffTime;

        public bool bTeam;

        public int effectId;
        public int effectBindPos;

        public CmdUIHead()
        {
            cmdenum = CmdFspEnum.eUIHead;
        }
    }

    public class CmdLife : IFspCmdType
    {
        public bool state;         // true为复活，false为死亡

        public CmdLife()
        {
            cmdenum = CmdFspEnum.eLife;
        }
    }

}