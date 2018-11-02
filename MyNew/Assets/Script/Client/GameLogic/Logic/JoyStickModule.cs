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

        public override void InitData()
        {
            OnLoadSelect();

            SkillCsv skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill);
            SkillCsvData skill0 = skillInfo.GetData(0);
            SkillCsvData skill1 = skillInfo.GetData(1);
            SkillCsvData skill2 = skillInfo.GetData(2);
            SkillCsvData skill3 = skillInfo.GetData(3);
            SkillCsvData skill4 = skillInfo.GetData(4);

            m_ui.SetIcon(0, skill0.icon, false);
            m_ui.SetIcon(1, skill1.icon, false);
            m_ui.SetIcon(2, skill2.icon, false);
            m_ui.SetIcon(3, skill3.icon, false);
            m_ui.SetIcon(4, skill4.icon, false);
        }

        private void OnLoadSelect()
        {
            m_master = CPlayerMgr.GetMaster();
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = 3;
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            m_skillChoseHid = EntityManager.Inst.CreateEntity(eEntityType.eNone, info, (ent) =>
            {
                ent.SetDirection(Vector3.zero);
                //ent.SetColor(SKLL_BLUE);
                ent.SetRenderQueue(3001);
                GameObject skillChose = ent.GetObject();
                m_skillChose = skillChose.transform;
                m_skillDistance = m_skillChose.FindChild("distance");
                m_skillCenter = m_skillChose.FindChild("center");
                m_skillSectorDir = m_skillCenter.FindChild("sector_dir");
                m_skillDir = m_skillCenter.FindChild("dir");
                m_skillPos = m_skillCenter.FindChild("pos");

                CancelSkill();
            });
        }

        public override void UpdateUI(float time, float fdTime)
        {
            if(m_master != null && m_skillChose != null && m_master.m_vCreature != null)
            {
                m_skillChose.position = m_master.m_vCreature.GetEnt().GetPos() + Vector3.up * 0.01f;
            }
        }

        private void SetSkillCancel(bool bCancel)
        {
             if (bCancel)       // 红色,取消技能
            {
                m_bSkillCancel = true;
                Entity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
                ent.SetColor(SKLL_RED);
                m_ui.SetColor(m_curSkillIndex, SKLL_RED);
                //SetSelectColor(SKLL_RED);
            }
            else   // 不取消技能
            {
                m_bSkillCancel = false;
                Entity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
                ent.SetColor(SKLL_BLUE);
                m_ui.SetColor(m_curSkillIndex, SKLL_BLUE);
                //SetSelectColor(SKLL_BLUE);
            } 
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
            }
        }

        public void OnMove(bool bUp, Vector2 delta)
        {
            if (bUp)
            {
                m_preMoveDir = Vector3.zero;
                m_isFirstJoyStick = true;
                if (m_master != null)
                {
                    m_master.SendFspCmd(new CmdFspStopMove());
                    Debug.Log("停止移动。。。。。。。。。。。。。。。。。。。。。");
                }
            }
            else
            {
                delta.Normalize();
                MasterMove(delta);
            }
        }

        public void MasterMove(Vector2 dir)
        {
            if (m_isFirstJoyStick)
            {
                PushMoveCommand(dir);
            }
            else
            {
                m_curSendMoveTime += Time.deltaTime;
                if(m_curSendMoveTime > m_sendMoveTimeInterval)
                {
                    float angle = Vector3.Angle(m_preMoveDir, dir);
                    if (angle > 10)
                    {
                        PushMoveCommand(dir);
                    }
                    m_curSendMoveTime = 0;
                }
            }
        }

        //直接发送移动消息
        private void PushMoveCommand(Vector2 dir)
        {
            if (dir == Vector2.zero)
            {
                return;
            }

            if (m_master != null)
            {
                CmdFspMove cmd = new CmdFspMove(ref dir);
                m_master.SendFspCmd(cmd);
                                Debug.Log("发送移动。。。。。。。。。。。。。。。。。。。。。");
                                
                m_preMoveDir = dir;
                m_isFirstJoyStick = false;
            }
        }
        #endregion
 
        /// <summary>
        /// 摇杆的场景指示器也只能主角自己有，并且是操作完成才发送指令
        /// </summary>
        private void OnSkillEvent(int index, eJoyStickEvent jsEvent, SkillJoyStick joyStick, bool bCancel)
        {
            m_curSkillIndex = index;
            m_curSkillJoyStick = jsEvent;
            CMasterPlayer master = CPlayerMgr.GetMaster();

            if(jsEvent == eJoyStickEvent.Drag)
            {
                m_curSkillDir = new Vector3(joyStick.m_delta.x, 0 , joyStick.m_delta.y);
                m_curSkillDir.Normalize();
            }

            if(jsEvent == eJoyStickEvent.Down)
            {
                m_bSkillCancel = false;
                m_ui.m_cancelBtn.SetActiveNew(true);
                m_ui.SetColor(index, SKLL_BLUE);

                skillInfo = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill).GetData(index);

                m_skillChose.gameObject.SetActiveNew(true);
                m_skillCenter.gameObject.SetActiveNew(true);
                m_skillDistance.gameObject.SetActiveNew(true);
                m_skillSectorDir.gameObject.SetActiveNew(false);
                m_skillDir.gameObject.SetActiveNew(false);
                m_skillPos.gameObject.SetActiveNew(false);

                m_skillDistance.localScale = new Vector3(skillInfo.distance, 0.01f, skillInfo.distance) * 2;
            }

            // 扇形
            if(skillInfo.selectTargetType == (int)eSelectTargetType.SectorDir)
            {
                if(jsEvent == eJoyStickEvent.Down)
                {
                    m_curSkillDir = master.GetDir().ToVector3();
                    m_skillSectorDir.gameObject.SetActiveNew(true);
                    m_skillSectorDir.localScale = new Vector3(skillInfo.distance, 0.01f, skillInfo.distance);
                }
                else if(jsEvent == eJoyStickEvent.Drag)
                {
 
                }
                m_skillCenter.rotation = Quaternion.LookRotation(m_curSkillDir);  // 只用控制中心点的方向
            }
            // 方向
            else if(skillInfo.selectTargetType == (int)eSelectTargetType.Dir)
            {
                if(jsEvent == eJoyStickEvent.Down)
                {
                    m_curSkillDir = master.GetDir().ToVector3();
                    m_skillDir.gameObject.SetActiveNew(true);
                    m_skillDir.localScale = new Vector3(skillInfo.distance, 0.01f, skillInfo.distance);
                }
                else if(jsEvent == eJoyStickEvent.Drag)
                {

                }
                m_skillCenter.rotation = Quaternion.LookRotation(m_curSkillDir);  // 只用控制中心点的方向
            }
            // 位置
            else if(skillInfo.selectTargetType == (int)eSelectTargetType.Pos)
            {
                if(jsEvent == eJoyStickEvent.Down)
                {
                    m_curSkilPos = m_master.GetPos().ToVector3();

                    m_skillPos.gameObject.SetActiveNew(true);
                    m_skillPos.position = m_curSkilPos;
                    m_skillPos.localScale = new Vector3(skillInfo.length, 0.001f, skillInfo.length);
                }
                else if(jsEvent == eJoyStickEvent.Drag)
                {
                    Vector3 atkOffset = m_curSkillDir * joyStick.m_delta.magnitude * skillInfo.distance;
                    m_curSkilPos = m_master.m_vCreature.GetEnt().GetPos() + atkOffset;
                    m_skillPos.position = m_curSkilPos;
                }
            }
            
            if(jsEvent == eJoyStickEvent.Up)
            {
                // 发送技能
                if (!m_bSkillCancel)
                {
                    m_bSkillCancel = true;
                    //Debug.Log("发送技能：" + skillInfo.id  + "dir:" + m_curSkillDir + " pos:" + m_curSkilPos);
                    CmdFspSendSkill cmd = new CmdFspSendSkill();
                    cmd.m_casterUid = (int)m_master.GetUid();
                    cmd.m_skillId = skillInfo.id;
                    cmd.m_dir = new Vector2(m_curSkillDir.x, m_curSkillDir.z);
                    cmd.m_endPos = new Vector2(m_curSkilPos.x, m_curSkilPos.z);
                    master.SendFspCmd(cmd);
                }
                // 还原指示器
                CancelSkill();

                m_skillChose.gameObject.SetActiveNew(false);
            }

        }

        /// <summary>
        /// 本地还原指示器，取消技能
        /// </summary>
        public void CancelSkill()
        {
            if (m_skillChose != null)
                m_skillChose.gameObject.SetActiveNew(false);

            m_bSkillCancel = true;
            m_ui.m_cancelBtn.SetActiveNew(false);
            m_ui.CloseSkillFocus();
            //CameraMgr.Inst.OnFov(0, 30f, 0);
            Entity ent = EntityManager.Inst.GetEnity(m_skillChoseHid);
            ent.SetColor(SKLL_BLUE);
        }

        private CMasterPlayer m_master;
        private int m_skillChoseHid;
        private Transform m_skillChose;
        private Transform m_skillCenter;
        private Transform m_skillDistance; // 范围圈
        private Transform m_skillSectorDir;
        private Transform m_skillDir;
        private Transform m_skillPos;

        
        private SkillCsvData skillInfo;
        private Vector3 m_curSkillDir;
        private Vector3 m_curSkilPos;


        private Color SKLL_BLUE = new Color(0f, 0.145f, 0.807f, 0.5f);
        private Color SKLL_RED = new Color(0.345f, 0f, 0f, 0.5f);


        /// <summary>
        /// 是否取消施法
        /// </summary>
        private bool m_bSkillCancel;
        private int m_curSkillIndex;
        private eJoyStickEvent m_curSkillJoyStick = eJoyStickEvent.Up;

        //摇杆控制参数
        public bool m_isFirstJoyStick = true;
        private Vector2 m_preMoveDir;
        private float m_curSendMoveTime;
        private float m_sendMoveTimeInterval = 0.1f;

  

        public UIPanelJoyStick m_ui;
    }
}

