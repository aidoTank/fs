using UnityEngine;

namespace Roma
{
    public partial class JoyStickModule : Widget
    {
        public JoyStickModule()
            : base(LogicModuleIndex.eLM_PanelJoyStick)
        {
        }

        public static Widget CreateLogicModule()
        {
            return new JoyStickModule();
        }

        public override void Init()
        {
            m_ui = GetUI<UIPanelJoyStick>();
        
            // 注册移动
            MoveJoyStick move = m_ui.m_move;
            move.OnJoySticjEvent = OnMoveEvent;
            // 注册技能事件，升级按钮
            SkillJoyStick[] skillList = m_ui.m_SkillBtn;
            for (int i = 0; i < skillList.Length; i++)
            {
                skillList[i].SetLv(1);
                skillList[i].OnJoyStickEvent = OnSkillEvent;
                //skillList[i].OnLvUpEvent = OnClickLvUpBtn;
            }

            
            // 注册取消按钮
            UIDragListener.Get(m_ui.m_cancelBtn).OnDragEvent = (ev, delta) =>
            {
                if (ev == eDragEvent.Enter)       // 红色,取消技能
                {
                    SetSkillCancel(true);
                }
                else if (ev == eDragEvent.Exit)    // 不取消技能
                {
                    SetSkillCancel(false);
                }
            };
        }

        private void SetSkillCancel(bool bCancel)
        {
            //  if (bCancel)       // 红色,取消技能
            // {
            //     m_bSkillCancel = true;
            //     NewEntity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
            //     ent.SetColor(SKLL_RED);
            //     int index = m_curSkillIndex;
            //     if (index == 5)
            //         index = 4;
            //     m_ui.SetColor(index, SKLL_RED);
            //     SetSelectColor(SKLL_RED);
            // }
            // else   // 不取消技能
            // {
            //     m_bSkillCancel = false;
            //     NewEntity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
            //     ent.SetColor(SKLL_BLUE);
            //     int index = m_curSkillIndex;
            //     if (index == 5)
            //         index = 4;
            //     m_ui.SetColor(index, SKLL_BLUE);
            //     SetSelectColor(SKLL_BLUE);
            // } 
        }

        #region 移动相关
        private void OnMoveEvent(eJoyStickEvent jsEvent, MoveJoyStick move)
        {
            if (jsEvent == eJoyStickEvent.Up)
            {
                OnMove(true, move.m_delta);
            }
            else
            {
                OnMove(false, move.m_delta);
                //Debug.Log("发送移动。。。。。。。。。。。。。。。。。。。。。" + move.m_delta);
            }
        }

        public void OnMove(bool bUp, Vector2 delta)
        {
            if (bUp)
            {
                //停止事件
                m_moveDir = Vector3.zero;
                m_curVelocity = Vector3.zero;
                m_isFirstJoyStick = true;
                CMasterPlayer master = CPlayerMgr.GetMaster();
                if (master != null)
                {
                    //发送停止消息
                    master.SendFspCmd(new CmdFspStopMove());
                    //Debug.Log("发送停止。。。。。。。。。。。。。。。。。。。。。");
                }
                return;
            }
            else
            {
                m_moveDir = delta;
                m_moveDir.Normalize();
                MasterMove();
            }
        }


        //主角移动, 如果是update心跳。避免频繁发送消息，加入15度和时间的概念。如果是fixupdate心跳，可以取消这个设置。（暂时放入update心跳中）
        public void MasterMove()
        {
            if (m_isFirstJoyStick)
            {
                //第一次直接发送移动消息
                PushMoveCommand();
            }
            else
            {
                m_time += Time.deltaTime;
                if(m_time > 0.1f)
                {
                    m_time = 0;
                    //之后大于15度发送消息
                    float angle = Vector3.Angle(m_curVelocity, m_moveDir);
                    if (angle > 15)
                    {
                        PushMoveCommand();
                    }
                }

            }
        }

        private float m_time =0;
        //直接发送移动消息
        private void PushMoveCommand()
        {
            if (m_moveDir == Vector2.zero)
            {
                return;
            }

            CMasterPlayer master = CPlayerMgr.GetMaster();

            if (master != null)
            {
                m_isFirstJoyStick = false;
                CmdFspMove cmd = new CmdFspMove(ref m_moveDir);
                master.SendFspCmd(cmd);
            }
        }
        #endregion
 

        private void OnSkillEvent(int index, eJoyStickEvent jsEvent, SkillJoyStick joyStick, bool bCancel)
        {
            m_curSkillJoyStick = jsEvent;
            CMasterPlayer master = CPlayerMgr.GetMaster();

            if(jsEvent == eJoyStickEvent.Down)
            {
                int skillId = master.m_csv.skill0;
                skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(skillId);
            }
            else if(jsEvent == eJoyStickEvent.Up)
            {
                Debug.Log("发送技能：" + skillInfo.id);
                CmdFspSendSkill cmd = new CmdFspSendSkill();
                cmd.m_skillId = skillInfo.id;
                cmd.m_dir = joyStick.m_delta;
                master.SendFspCmd(cmd);
            }
        }

        private SkillCsvData skillInfo;

        /// <summary>
        /// 是否取消施法
        /// </summary>
        private bool m_bSkillCancel;
        private int m_curSkillIndex;
        private eJoyStickEvent m_curSkillJoyStick = eJoyStickEvent.Up;

        //摇杆控制参数
        private Vector3 m_curVelocity;
        private Vector2 m_moveDir;
        private float m_sendMoveTime = 0;
        private float m_sendMoveInterval = 0;
        public bool m_isFirstJoyStick = true;

        public UIPanelJoyStick m_ui;
    }
}

