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
        // 消息指令层
        eFspStopMove,       // 停止移动    (可用于状态机)
        eFspMove,           // 移动        (可用于状态机)
        eFspAutoMove,       // 自动移动    (可用于状态机)
        eFspRotation,       // 转向        (可用于状态机)

        eFspSendSkill,      // 技能 逻辑，表现层公用
        eFspCancelSkill,    // 取消技能
        eFspUpdateEquip,    // 更新装备

        //eSkillCreate,
        //eSkillHit,

        // 表现层
        eUIHead,
        eLife,
        eBuff,
        eState,
        eSkillAnimaPriority,
        eChangeModel,
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
        public Vector2d m_dir;
        public Vector2d m_pos;
        public CmdFspMove()
        {
            cmdenum = CmdFspEnum.eFspMove;
        }
        public CmdFspMove(ref Vector2d dir)
        {
            cmdenum = CmdFspEnum.eFspMove;
            m_dir = dir;
        }
        public CmdFspMove(ref Vector2d dir, ref Vector2d pos)
        {
            cmdenum = CmdFspEnum.eFspMove;
            m_dir = dir;
            m_pos = pos;
        }
    }

    public class CmdFspAutoMove : IFspCmdType
    {
        public Vector2d m_pos;
        public CmdFspAutoMove()
        {
            cmdenum = CmdFspEnum.eFspAutoMove;
        }
        public CmdFspAutoMove(ref Vector2d pos)
        {
            cmdenum = CmdFspEnum.eFspAutoMove;
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
        public static CmdFspStopMove Inst = new CmdFspStopMove();
        public CmdFspStopMove()
        {
            cmdenum = CmdFspEnum.eFspStopMove;
        }
    }

    public class CmdFspSendSkill : IFspCmdType
    {
        public int m_casterUid;

        public int m_skillIndex; // 一般技能id, 连接技的起始id
        public bool m_bDown;

        public int m_skillId;    // 作为连接技的id
        //public int m_skillLv = 1;

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

    /// <summary>
    /// 更新装备，外观等
    /// </summary>
    public class CmdFspUpdateEquip : IFspCmdType
    {
        // 资源id
        public Dictionary<eEquipType, int> m_dicEquip;
        // 主武器的类型，用于控制动画
        public int m_armsType;
        public int m_vipLv;
        public CmdFspUpdateEquip()
        {
            cmdenum = CmdFspEnum.eFspUpdateEquip;
        }
    }



    //public class CmdSkillCreate : IFspCmdType
    //{
    //    public CmdSkillCreate()
    //    {
    //        cmdenum = CmdFspEnum.eSkillCreate;
    //    }
    //}

    //public class CmdSkillHit : IFspCmdType
    //{
    //    public bool bPlayer;
    //    /// <summary>
    //    /// 玩家时，为uid
    //    /// </summary>
    //    public int uid;
    //    public Vector3 pos; // 弹道爆炸的位置

    //    public CmdSkillHit()
    //    {
    //        cmdenum = CmdFspEnum.eSkillHit;
    //    }
    //}


    /// <summary>
    /// 设置表现层，只用于设置一次的时候
    /// </summary>
    public class CmdUIHead : IFspCmdType
    {
        // 1.名字 2等级 3血量 4跳字 5隐藏头顶 6经验 7BUFF信息 
        // 8阵营 9是否显示血条 10播放动作 11玩家特效 12上坐骑 13任务状态 14脚底光环特效
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

        public bool bTeam;  // 阵营

        public int animaId; // 动作

        public int effectId;
        public int effectBindPos;

        public bool bRide;
        public VObject rideObject;

        public int taskstate;
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

    public class CmdFspBuff : IFspCmdType
    {
        public bool bAdd;
        public int effectId;
        public int bindType;
        public float effectScale;
        public Color color; // buff改变人的颜色

        public static CmdFspBuff Inst = new CmdFspBuff();

        public CmdFspBuff()
        {
            cmdenum = CmdFspEnum.eBuff;
        }
    }
    public class CmdFspState : IFspCmdType
    {
        public bool bAdd;
        public eVObjectState type;
        public List<int> param;

        public CmdFspState()
        {
            cmdenum = CmdFspEnum.eState;
        }
    }

    /// <summary>
    /// 技能动作优先级
    /// </summary>
    public class CmdSkillAnimaPriority : IFspCmdType
    {
        public int priority;

        public CmdSkillAnimaPriority()
        {
            cmdenum = CmdFspEnum.eSkillAnimaPriority;
        }
    }

    //public class CmdChangeModel : IFspCmdType
    //{
    //    public int resId;

    //    public CmdChangeModel()
    //    {
    //        cmdenum = CmdFspEnum.eChangeModel;
    //    }
    //}

}