using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public enum eAIType
    {
        None,
        /// <summary>
        /// 本地ai 不发送指令到服务器
        /// </summary>
        Client,  
        /// <summary>
        /// 玩家ai 会发送指令到服务器,用于开发测试,
        /// 只能在发送消息时使用，其他本地逻辑不能调用，因为只有自己的客户端知道自己是模拟AI
        /// 在模拟玩家AI遇到障碍时，不会重新寻找移动位置，因为只用于内部测试，暂不处理。
        /// </summary>
        Player,   
    }

    public enum eAILevel
    {
        NEWBIE,   // 吃技能，可以闪现躲熔岩，攻击频率低，无法检测隐身
        EASY,     // 不躲技能（躲避距离为0），可以闪现躲熔岩，攻击频率低，无法检测隐身
        NORMAL,   // 躲技能（躲避距离短），可以闪现躲熔岩，攻击频率中，无法检测隐身
        HARD      // 躲技能（躲避距离远），可以闪现躲熔岩，攻击频率高，可以检测隐身
    }

    public enum eAIParam
    {
        INT_ROLE_UID,
        V3_MOVE_TO_POS,
        // buff 物品
        INT_BUFF_ID,
        // fight
        INT_TARGET_UID,
        INT_SELECT_SKILL_INDEX,

        // 技能间隔，多久攻击一次的设定
        INT_SKILL_INTERVAL,
    }

    public class AIParam
    {
        // 技能升级index
        //public const string INT_SKILL_UP_INDEX = "INT_SKILL_UP_INDEX";
        // 躲避
        //public const string V3_MOVE_TO_POS = "V3_MOVE_TO_POS";
        //// buff 物品
        //public const string INT_BUFF_ID = "INT_BUFF_ID";
        //// fight
        //public const string INT_TARGET_UID = "INT_TARGET_UID";
        //public const string INT_SELECT_SKILL_INDEX = "INT_SELECT_SKILL";

        //// 技能间隔，多久攻击一次的设定
        //public const string INT_SKILL_INTERVAL = "INT_SKILL_INTERVAL";
        // 技能释放间隔
        public const int INT_NEWBIE_CAST_SKILL_INTERVAL = 8 * 30 * FSPParam.clientFrameMsTime;
        public const int INT_EASY_CAST_SKILL_INTERVAL = 6 * 30 * FSPParam.clientFrameMsTime;
        public const int INT_NORMAL_CAST_SKILL_INTERVAL = 4 * 30 * FSPParam.clientFrameMsTime;
        //public const int INT_HARD_CAST_SKILL_INTERVAL = 1 * 30 * FSPParam.clientFrameMsTime;
        public const int INT_HARD_CAST_SKILL_INTERVAL = 0;
        // 连招状态,取消公共CD
        //public const string BOOL_COMBO_STATE = "BOOL_COMBO_STATE";



        // 躲避技能 预留
        // 检查躲避技能距离  / 2
        public const int NEWBIE_CHECK_SKILL_DIS = 6;   // 新手AI往子弹上跑
        public const int EASY_CHECK_SKILL_DIS = 0;   
        public const int NORMAL_CHECK_SKILL_DIS = 4;
        public const int HARD_CHECK_SKILL_DIS = 6;   // 当前陷阱和弹道用的同一个距离判断，如果太远，AI会检测陷阱出BUG

        /// <summary>
        /// 通用初始化
        /// </summary>
        public static void Init(BtDatabase dataBase)
        {
        }

        public static int GetCheckSkillDis(eAILevel level)
        {
            switch(level)
            {
                case eAILevel.NEWBIE:
                    return NEWBIE_CHECK_SKILL_DIS;
                case eAILevel.EASY:
                    return EASY_CHECK_SKILL_DIS;
                case eAILevel.NORMAL:
                    return NORMAL_CHECK_SKILL_DIS;
                case eAILevel.HARD:
                    return HARD_CHECK_SKILL_DIS;
            }
            return EASY_CHECK_SKILL_DIS;
        }

        /// <summary>
        /// 常规情况的CD，combo时，缩短CD间隔
        /// </summary>
        public static int GetSkillCastCd(CCreature creature, BtDatabase baseData)
        {
            // 连招状态，取最小间隔
            //if (baseData.GetData<bool>(BOOL_COMBO_STATE))
            //    return INT_HARD_CAST_SKILL_INTERVAL;
            //return INT_HARD_CAST_SKILL_INTERVAL / 2;
            int curInterval = INT_NORMAL_CAST_SKILL_INTERVAL;
            switch (creature.m_ai.m_level)
            {
                case eAILevel.NEWBIE:
                    curInterval = INT_NEWBIE_CAST_SKILL_INTERVAL;
                    break;
                case eAILevel.EASY:
                    curInterval = INT_EASY_CAST_SKILL_INTERVAL;
                    break;
                case eAILevel.NORMAL:
                    curInterval = INT_NORMAL_CAST_SKILL_INTERVAL;
                    break;
                case eAILevel.HARD:
                    curInterval = INT_HARD_CAST_SKILL_INTERVAL;
                    break;
            }
            return curInterval;
        }


        #region 获取攻击目标
        public static bool GetAtkTarget(CCreature m_creature, BtDatabase m_dataBase, int lookDis)
        {

            // 仇恨值高的优先作为目标
            int hTargetUid = m_creature.m_ai.GetHightHatred();
            CCreature hTargetCC = CCreatureMgr.Get(hTargetUid);
            if (hTargetUid != -1 && 
                hTargetCC != null && 
                !hTargetCC.IsDie())   // 有仇恨对象，并且没死
            {
                m_dataBase.SetData<int>((int)eAIParam.INT_TARGET_UID, hTargetUid);
            }

            // 如果有目标，判断是否距离足够远，远了就清空目标，继续找附近的
            int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
            CCreature targetCC =  CCreatureMgr.Get(targetUid);
            if (targetCC == null || 
                FPCollide.GetDis2(targetCC.GetPos(), m_creature.GetPos()) > new FixedPoint(20 * 20) ||
                targetCC.IsDie())  // 隐藏时，不作为目标
            {
                targetUid = 0;
            }
            else
            {
                return true;
            }

            targetUid = m_creature.GetTarget(lookDis);
            // 如果没有目标，检测玩家附近的单位，锁定目标
            //FixedPoint m_minDis2 = new FixedPoint(999999);
            //List<long> list = CCreatureMgr.GetCreatureList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    CCreature cc = CCreatureMgr.Get(list[i]);
            //    FixedPoint abDis2 = FPCollide.GetDis2(m_creature.GetPos(), cc.GetPos());
            //    if (abDis2 > new FixedPoint(lookDis * lookDis))
            //        continue;

            //    if (cc.IsDie() || cc.GetUid() == m_creature.GetUid())
            //        continue;
            //    //if (m_creature.bCamp(cc))
            //    //    continue;

                
            //    if (abDis2 < new FixedPoint(lookDis * lookDis))    // 如果目标在视线范围内
            //    {
            //        if (abDis2 < m_minDis2)
            //        {
            //            targetUid = (int)cc.GetUid();
            //            m_minDis2 = abDis2;
            //        }
            //    }
            //}

            if (targetUid != 0)
            {
                m_dataBase.SetData<int>((int)eAIParam.INT_TARGET_UID, targetUid);
                return true;
            }
            //m_creature.DestoryDownUpSkill();  // 无目标，结束机枪
            return false;
        }
        #endregion

        #region 随机选择一个可用技能
        private static List<CSkillInfo> m_canUserSkillList;
        public static bool RandomSelectSkill(CCreature m_creature, BtDatabase m_dataBase)
        {
            if (m_canUserSkillList == null)
                m_canUserSkillList = new List<CSkillInfo>();

            // 如果有技能没有被使用，就不进行新的随机
            int skillId = m_dataBase.GetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX);
            if (skillId != -1)
                return true;

            m_creature.GetCanUseSkillList(ref m_canUserSkillList);

            if (m_canUserSkillList.Count == 0)
                return false;


            int i;
            if (m_creature.m_aiType == eAIType.Client)
            {
                i = GameManager.Inst.GetRand(0, m_canUserSkillList.Count, 513);
            }
            else
            {
                i = GameManager.Inst.GetClientRand(0, m_canUserSkillList.Count);
            }
            int index = m_canUserSkillList[i].GetSkillIndex();
            m_dataBase.SetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX, index);
            return true;
        }
        #endregion

        #region 需要获取玩家目标的施法接口
        public static bool SendSkill(CCreature m_creature, BtDatabase m_dataBase, bool bDown = false)
        {
            int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
            if (targetUid == m_creature.GetUid())
            {
                return false;
            }
            CCreature targetCc =  CCreatureMgr.Get(targetUid);
            if (targetCc != null)
            {
                Vector2d dir = (targetCc.GetPos() - m_creature.GetPos());
                dir.Normalize();
                //if (dir == Vector2.zero)
                //{
                //    dir = m_creature.GetDir();
                //}
                int selectSkillUid = m_dataBase.GetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX);

                CSkillInfo skillInfo = m_creature.GetSkillByIndex(selectSkillUid);
         


                CmdFspSendSkill cmd = new CmdFspSendSkill();
                cmd.m_casterUid = (int)m_creature.GetUid();
                cmd.m_skillIndex = selectSkillUid;

                if (skillInfo.m_skillInfo.selectTargetType == (int)eSelectTargetType.Self)
                {

                }
                else if (skillInfo.m_skillInfo.selectTargetType == (int)eSelectTargetType.Dir ||
                         skillInfo.m_skillInfo.selectTargetType == (int)eSelectTargetType.SectorDir)
                {
                    cmd.m_dir = dir;
                }
                else if (skillInfo.m_skillInfo.selectTargetType == (int)eSelectTargetType.Pos)
                {
                    cmd.m_dir = dir;
                    cmd.m_endPos = targetCc.GetPos();
                }
                cmd.m_bDown = bDown;

                if (m_creature.m_aiType == eAIType.Player)
                {
                    m_creature.SendFspCmd(cmd);
                }
                else
                {
                    m_creature.PushCommand(cmd);
                }

                // 进入公共CD
                m_dataBase.SetData<int>((int)eAIParam.INT_SKILL_INTERVAL, GetSkillCastCd(m_creature, m_dataBase));
                // 清空当前发送的技能
                m_dataBase.SetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX, -1);
                return true;
            }
            return false;
        }
        #endregion

        #region 无需目标，比如对自己释放霸体
        public static void SendSkill(CCreature m_creature, BtDatabase m_dataBase)
        {
            int selectSkillUid = m_dataBase.GetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX);
            CmdFspSendSkill cmd = new CmdFspSendSkill();
            cmd.m_casterUid = (int)m_creature.GetUid();
            cmd.m_skillIndex = selectSkillUid;
            cmd.m_bDown = false;
            m_creature.PushCommand(cmd);

            // 进入公共CD
            m_dataBase.SetData<int>((int)eAIParam.INT_SKILL_INTERVAL, GetSkillCastCd(m_creature, m_dataBase));
            // 清空当前发送的技能
            m_dataBase.SetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX, 0);
        }
        #endregion
    }

}
