using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Roma
{
    public class AIAutoAttackParam
    {
        public uint m_skillId;
    }

    public class AIEscapeParam
    {
        public Vector3 m_targetPos;
    }


    public class AttackStateParam  // 技能释放信息
    {
        public uint m_uSkillID = 0;
        public long m_targetID = 0;
        public Vector3 m_vPos = Vector3.zero;
        public Quaternion m_quq = Quaternion.identity;
        public List<CCreature> m_lstTarget = new List<CCreature>();
        // 单机扩展参数
        public int m_hitVal = 1;
    }

    public delegate void BattleMoveEnd(CCreature create);

    public class MoveStateParam
    {
        public float m_moveSpeed = 10;
        public List<Vector2> m_movePath = new List<Vector2>();
        public BattleMoveEnd m_moveEnd;
        public bool m_bMoveing = false; // 当前角色移动中
        /// <summary>
        /// 是否旋转
        /// </summary>
        public bool m_bRota = true;
        /// <summary>
        /// 用于旋转的四元素，一般有两种情况，
        /// 通过Euler将角色角度转为四元素
        /// 通过LookRotation将方向转为四元素
        /// </summary>
        public Quaternion m_btEndQua;

        /// <summary>
        /// 用于战斗中走直线时的目标位置,击退中用于存储阵型位置
        /// </summary>
        public Vector3 m_btMoveEndPos;
        /// <summary>
        /// 击退的延迟时间
        /// </summary>
        public float m_hitBackDelayEndTime = 0f;
    }

    public class StunStateParam
    {
        public float m_stunTime = 1.0f;
    }


    public class NearStateParam
    {
        public string m_clickActionID; // 点击之后获取的行为id
    }

    public class StrikeStateParam
    {
        public string m_hitAnimaName; // 受击动画名
    }

    /// <summary>
    /// 1.用于击飞效果
    /// 2.用于地球一周的回场效果
    /// </summary>
    public class HitFlyStateParam
    {
        /// <summary>
        /// 击飞的目标地
        /// </summary>
        public Vector3 m_btMoveEndPos;
        public float m_btFlyTime;
       // public BattleMoveEnd m_btMoveEnd;
    }

    /// <summary>
    /// 1.跳跃状态参数
    /// </summary>
    public class JumpStateParam
    {
        /// <summary>
        /// 跳跃的Y轴弧度
        /// </summary>
        public Vector2 m_jumpRadian = new Vector2(2, 6);
        public Vector3 m_jumpEndPos;
        public float m_jumpSpeed;
       // public BattleMoveEnd m_jumpEnd;
    }

    public class FindTargetStateParam
    {
        public Vector3 m_targetPos; // 目标点
        public int m_mapId;
        public float m_distance;    // 距离
        public int m_battleId;  // 战斗id
        public int m_questId;
        public bool m_autoPatrol; // 开启自动巡逻
       // public List<DragonHuntCsvData> m_dragonHuntList; // 猎龙任务的战斗列表

        public float m_petAtkintervalTime = 6.0f;
        public float m_petAtkCurTime = 0f;
    }
}