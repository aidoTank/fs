using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public class CCreatureAI
    {
        //private GameObject m_aiObject;
        public BtTree m_tree;

        private CCreature m_player;
        public eAILevel m_level;
        // 连招combo
        private List<int> m_comboList = new List<int>();
        private int m_curTime;

        private int m_curHatredTime;
        public UISuperList m_list;

        public bool IsRun()
        {
            return m_tree.m_bRunning;
        }

        public void SetRun(bool bRun)
        {
            m_tree.m_bRunning = bRun;
            // 关闭AI时，清除目标
            if(!bRun && m_tree != null && m_tree.m_dataBase != null)
            {
                m_tree.m_dataBase.SetData<int>((int)eAIParam.INT_TARGET_UID, 0);
            }
        }

        public CCreatureAI(CCreature creature, eAILevel level = eAILevel.HARD, bool bPatrol = true)
        {
            //if(creature.IsMaster())
            //{
            //    m_list = GameObject.Find("ai_list").AddComponent<UISuperList>();
            //    m_list.Init(300, 60, 1, 5);
            //}
            m_player = creature;
            m_level = level;
            InitComboData();

            //m_aiObject = new GameObject("ai_" + creature.GetUid());
            m_tree = new BtTree();
            m_tree.m_root = new BtPrioritySelector();
            m_tree.Init();
            m_tree.m_dataBase.SetData<CCreature>((int)eAIParam.INT_ROLE_UID, creature);
            AIParam.Init(m_tree.m_dataBase);


            // skill level up
            //Condi_SkillUp c_skillUp = new Condi_SkillUp();
            //Action_SkillUp a_skillUp = new Action_SkillUp();
            //BtSequence skillUp = new BtSequence(c_skillUp);
            //skillUp.AddChild(a_skillUp);
            //skillUp.name = "升级中";

            //Condi_CheckTransmit c_checkTransmit = new Condi_CheckTransmit();
            //Action_Transmit a_trans = new Action_Transmit();
            //BtSequence transmit = new BtSequence(c_checkTransmit);
            //transmit.AddChild(a_trans);
            //transmit.name = "瞬间传送";

            // Elude skill
            Condi_CheckSendSkill_ c_checkSkill = new Condi_CheckSendSkill_();
            Action_EludePos_ a_eludePos = new Action_EludePos_();
            BtSequence eludeSkill = new BtSequence(c_checkSkill);
            eludeSkill.AddChild(a_eludePos);
            eludeSkill.name = "躲技能";


            // find buff
            Condi_FindMapBuff_ c_find = new Condi_FindMapBuff_();
            Action_MoveToBuff_ a_move = new Action_MoveToBuff_();
            BtSequence findBuff = new BtSequence(c_find);
            findBuff.AddChild(a_move);
            findBuff.name = "寻找中";

            // fight
            AICondi[] c_fight = new AICondi[3];
            c_fight[0] = new Condi_FindTarget();        // 先通过仇恨，位置获取目标
            c_fight[1] = new Condi_CheckState();        // 再检查自身施法条件是否满足
            c_fight[2] = new Condi_SelectSkill();
            Condi_List findTarget = new Condi_List(c_fight);
            // fight action
            BtSequence fight = new BtSequence(findTarget);
            Action_MoveToTarget a_fight_move = new Action_MoveToTarget();
            Action_SendSkill_ a_fight_send = new Action_SendSkill_();
            fight.AddChild(a_fight_move);
            fight.AddChild(a_fight_send);
            fight.name = "战斗中";
            // chase
            Condi_CheckChase c_chase = new Condi_CheckChase();
            Action_Chase a_chase = new Action_Chase();
            BtSequence chase = new BtSequence(c_chase);
            chase.AddChild(a_chase);
            chase.name = "追逐";


            //Condi_CheckFollow c_checkFollow = new Condi_CheckFollow();
            //Action_Follow a_follow = new Action_Follow();
            //BtSequence follow = new BtSequence(c_checkFollow);
            //follow.AddChild(a_follow);
            //follow.name = "跟随";

            // patrol
            Action_Patrol patrol = new Action_Patrol();
            patrol.name = "巡逻中";


            //if (creature.IsPartner())
            //{
            //    m_tree.m_root.AddChild(transmit);
            //}
            //if (creature.IsMaster())
            //{
            //    m_tree.m_root.AddChild(findBuff);
            //}
            m_tree.m_root.AddChild(fight);
            //m_tree.m_root.AddChild(chase);

            //if (creature.IsPartner())
            //{
            //    m_tree.m_root.AddChild(follow);
            //}

        
            m_tree.m_root.AddChild(patrol);
            
     
            m_tree.OnStart();
        }

        public void EnterFrame()
        {
            if (m_player.IsDie())
                return;

            m_curTime += FSPParam.clientFrameMsTime;
            if (m_curTime >= 40 * FSPParam.clientFrameMsTime)
            {
                if (m_tree != null)
                    m_tree.OnUpdate();

                // AI技能外部CD
                int curSkillInterval = m_tree.m_dataBase.GetData<int>((int)eAIParam.INT_SKILL_INTERVAL);
                if(curSkillInterval >= 0)
                {
                    curSkillInterval -= FSPParam.clientFrameMsTime;
                    m_tree.m_dataBase.SetData<int>((int)eAIParam.INT_SKILL_INTERVAL, curSkillInterval);
                }

                if (Client.Inst().m_bDebug)
                {
                    string state = GetCurState();
                    if (!string.IsNullOrEmpty(state))// 如果卡主，可以知道最后一个状态
                    {
                        // 状态显示
                        VObject mt = m_player.GetVObject();
                        if (mt == null)
                            return;
                        CmdUIHead cmd = new CmdUIHead();
                        cmd.type = 1;
                        cmd.name = m_player.GetUid() + ":" + state;
                        mt.PushCommand(cmd);
                    }
                }

                // 仇恨值
                m_curHatredTime += FSPParam.clientFrameMsTime;
                if(m_curHatredTime >= 30 * FSPParam.clientFrameMsTime)
                {
                    _UpdateHatredList();
                    //主角显示测试界面
                    if (m_player.IsMaster())
                    {
                        m_hatredList.Sort((x, y) =>
                        {
                            if (x.val == y.val)
                                return 0;
                            else if (x.val > y.val)
                                return -1;
                            else
                                return 1;
                        });
                        //m_list.Init(m_hatredList.Count, (item, index) =>
                        //{
                        //    UIItem.SetText(item, "txt", m_hatredList[index].uid + " 仇恨值：" + m_hatredList[index].val);
                        //    UIItem.SetProgress(item, "pct", m_hatredList[index].val, 100);
                        //});
                    }
                    m_curHatredTime = 0;
                }
            }
        }

        public string GetCurState()
        {
            BtPrioritySelector se = (BtPrioritySelector)m_tree.m_root;
            if (se.m_curActiveChild != null)
            {
                return se.m_curActiveChild.name;
            }
            return "";
        }

        public void Destroy()
        {
            if(m_tree != null)
                m_tree.OnDestroy();
            //if(m_aiObject != null)
            //    GameObject.Destroy(m_aiObject);
            m_hatredList.Clear();
            m_hatredList = null;
            m_comboList.Clear();
            m_comboList = null;
        }

        public List<int> GetComBoList()
        {
            return m_comboList;
        }

        public void InitComboData()
        {
            //AIComboCsv csv = CsvManager.Inst.GetCsv<AIComboCsv>((int)eAllCSV.eAC_AICombo);
            //int heroId = m_player.GetHeroID();
            //AIComboCsvData data = csv.GetData(heroId);
            //if (data != null)
            //{
            //    string[] list = data.comboList1.Split('|');
            //    for (int i = 0; i < list.Length; i++)
            //    {
            //        int id = 0;
            //        if (int.TryParse(list[i], out id))
            //        {
            //            m_comboList.Add(id);
            //        }
            //    }
            //}
        }


        #region 仇恨管理
        // 仇恨列表
        public class Hatred
        {
            public int uid;
            public int val;
        }
        private List<Hatred> m_hatredList = new List<Hatred>();
        public void AddHatred(int uid, int val)
        {
            for(int i = 0; i < m_hatredList.Count;i ++)
            {
                Hatred item = m_hatredList[i];
                if (item.uid == uid)
                {
                    item.val += val;
                    return;
                }
            }
            Hatred h = new Hatred();
            h.uid = uid;
            h.val = val;
            m_hatredList.Add(h);
        }

        public void ClearHatred()
        {
            m_hatredList.Clear();
        }

        /// <summary>
        /// 获取最高仇恨的角色id
        /// </summary>
        public int GetHightHatred()
        {
            int maxVal = 0;
            int uid = 0;
            for (int i = 0; i < m_hatredList.Count; i++)
            {
                Hatred item = m_hatredList[i];
                if (item.val > maxVal)
                {
                    maxVal = item.val;
                    uid = item.uid;
                }
            }
            if (maxVal == 0)
                uid = -1;
            return uid;
        }

        public int GetMaxHatred()
        {
            int maxVal = 0;
            for (int i = 0; i < m_hatredList.Count; i ++)
            {
                Hatred item = m_hatredList[i];
                if (item.val > maxVal)
                {
                    maxVal = item.val;
                }
            }
            return maxVal;
        }

        public void _UpdateHatredList()
        {
            for (int i = 0; i < m_hatredList.Count; i++)
            {
                Hatred item = m_hatredList[i];
                if (m_level == eAILevel.EASY || m_level == eAILevel.NEWBIE)
                {
                    item.val -= 10;
                }
                else if (m_level == eAILevel.NORMAL)
                {
                    item.val -= 10;
                }
                else if (m_level == eAILevel.HARD)
                {
                    item.val -= 5;
                }
                
                if (item.val < 0)
                    item.val = 0;
            }
        }
        #endregion
    
    }
}

