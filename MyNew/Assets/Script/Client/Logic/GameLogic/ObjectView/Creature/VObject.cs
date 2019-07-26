using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public enum eVObjectState // 表现状态,控制shader等
    {
        None,
        stun,
        Silence,    // 被沉默
        God,        // 天神下凡，无敌

        unmove ,    // 禁锢 无法移动
        sleep,     // 睡眠，被攻击时解除
        SuperArmor,   // 霸体

        // 仅仅控制表现层状态
        Hit,         // 受击
        AlphaToHalf, // 半透
        AlphaToHide, // 全透
        Nihility,    // 虚无状态
        Show,
    }

    public struct SMtCreatureAnima
    {
        public const int ANIMA_IDLE = 1;     // 近战待机
        public const int ANIMA_IDLE2 = 2;    // 远程待机
        public const int ANIMA_WAlk = 3;
        public const int ANIMA_WAlk2 = 4;
        public const int ANIMA_DIE = 6;
        public const int ANIMA_RESET = 8;
    }

    public struct SBindPont
    {
        public enum eBindType
        {
            Head = 0,
            Chest = 1,
            Origin = 2,
            LRHand = 3,
            CreaturePos = 4,
            CreatureHeadPos = 5, // 角色头顶跟随
            Muzzle, // 武器枪口
            //RHand = 5,
            //LHand = 6,
        }

        public const string HEAD = "over_head";
        public const string CHEST = "hit";
        public const string ORIGIN = "origin";
        public const string R_HAND = "r_hand";
        public const string L_HAND = "l_hand";

        public static string GetBindPont(int type)
        {
            switch (type)
            {
                case 0:
                    return HEAD;
                case 1:
                    return CHEST;
                case 2:
                    return ORIGIN;
                case 6:
                    return "qk";
            }
            return null;
        }
    }



    public partial class VObject : VBase
    {
        public bool m_bMaster;
        public CmdFspEnum m_state;
        public CThingHead m_head;

        // key=特效id,val=特效hid 多个，支持左右手
        private Dictionary<int, object> m_dicBuff;
        // key=特效id,val=特效数量,用于特效引用计数
        private Dictionary<int, int> m_dicBuffCount;

        public int m_iShowState;   // 表现状态 0000
        public bool m_bDead;
        // 需要异步回调的数据
        public CmdFspUpdateEquip m_cmdUpdateEquip;
        public Vector3 m_curveEndPos;
        private bool m_ride;

        //private int m_masterHaloHid;

        public VObject()
        {

        }

        public override void Create(sVOjectBaseInfo baseInfo)
        {
            base.Create(baseInfo);

            if (m_baseInfo.m_showHead)
            {
                m_head = new CThingHead("1111");
            }
        }

        public override void CreateEnd(Entity ent)
        {
            base.CreateEnd(ent);
            // 角色的处理
            if(ent is BattleEntity)
            {
                ((BattleEntity)ent).SetPriority(0);
                ResetState();
                // 异步加载完成，换装
                if (m_cmdUpdateEquip != null)
                {
                    UpdateEquip();
                }
                if(ent.GetObject() == null)
                {
                    Debug.Log("表现层对象为空：" + m_baseInfo.m_resId);
                    return;
                }
                GameObjectHelper.Get(ent.GetObject()).SetUid(m_baseInfo.m_uid);
            }
            else
            {
                Debug.LogError("ent:" + ent);
            }
            //ShowFootHalo(m_footHaloEffect);
            // 主角光圈
            //ShowFootHalo_Master();
        }

        /// <summary>
        /// type=2  比如修改状态2
        /// 1<<type = 0001<<2 = 0100  
        /// m_iShowState = 0000 | 0100 = 0100 如果之前无状态，再设置2
        /// m_iShowState = 0100 & 1011 = 0000 如果之前有状态2，再取消2
        /// m_iShowState = 0110 & 1011 = 0010 如果之前有状态2和1，再取消2
        /// </summary>
        public void SetShowState(eVObjectState type, bool bSet)
        {
            int iType = (int)type;
            if (bSet)
            {
                m_iShowState = m_iShowState | (1 << iType);   // 一个为1都为1
            }
            else
            {
                m_iShowState = m_iShowState & ~(1 << iType);  // 两个都为1才是1
            }
        }

        // tpye=2  0100>>2 = 0001 与 0001做&(位与)运算  为0001 > 0 为存在 
        public bool CheckState(eVObjectState type)
        {
            int iType = (int)type;
            return ((m_iShowState >> iType) & 1) > 0;
        }

        public void UpdateMaster(bool bMaster)
        {
            m_bMaster = bMaster;
            if(bMaster)
            {
                CameraMgr.Inst.InitCamera(this);
            }
        }


        public override void PushCommand(IFspCmdType cmd)
        {
            BattleEntity ent = GetEnt() as BattleEntity;
            //Debug.Log("切换：" + cmd.GetCmdType());
            switch (cmd.GetCmdType())
            {
                #region 常态
                case CmdFspEnum.eFspStopMove:

                    m_state = cmd.GetCmdType();
                    SetMove(false);
                    //Debug.Log("设置停止");
                    ((BattleEntity)GetEnt()).SetPriority(0);
                    ResetState();
                    break;
                case CmdFspEnum.eFspMove:
                case CmdFspEnum.eFspAutoMove:
                    m_state = cmd.GetCmdType();
                    SetMove(true);
                    //Debug.Log("设置移动");
                    ResetState();
                    break;
                case CmdFspEnum.eUIHead:
                    CmdUIHead head = cmd as CmdUIHead;
                    switch (head.type)
                    {
                        case 1:
                            if(m_head != null)
                                m_head.SetName(head.name);
                            break;
                        case 2:
                            if (m_head != null)
                                m_head.SetLevel(head.lv);
                            break;
                        case 3:
                            if (m_head != null)
                                m_head.SetHp(head.curHp, head.maxHp);
                            break;
                        case 4:
                            if (m_head != null)
                                m_head.SetHud(head.hudText, head.hudType);
                            break;
                        case 5:
                            if (m_head != null)
                                m_head.SetHeadShow(head.bShow);
                            break;
                        case 8:
                            if (m_head != null)
                                m_head.SetTeam(head.bTeam);
                            break;
                        //case 9:
                        //    if (m_head != null)
                        //        m_head.ShowNameOnly(head.bShow);
                        //    break;
                        case 10:
                            // The state of dizziness does not play the hit action
                            if (CheckState(eVObjectState.stun))
                                return;
                            ent.PlayAnima(head.animaId, ()=> {
                                ResetState();
                            });
                            break;
                        case 11:
                            if (head.effectBindPos == (int)SBindPont.eBindType.CreatureHeadPos)
                            {
                                CEffectMgr.CreateByCreaturePos(head.effectId, ent, 2);
                            }
                            else if(head.effectBindPos == (int)SBindPont.eBindType.Muzzle)  // 枪口
                            {
                                CEffectMgr.Create(head.effectId, ((BattleEntity)GetEnt()).
                                    GetRightPoint());
                            }
                            else
                            {
                                CEffectMgr.Create(head.effectId, GetEnt(), SBindPont.GetBindPont(head.effectBindPos));
                            }
                            break;
                        case 12:
                            if(head.bRide)
                            {
                                m_ride = true;
                                VObject obj = head.rideObject;
                                GetEnt().SetParent(((BattleEntity)obj.GetEnt()).GetBone("ride"));
                            }
                            else
                            {
                                m_ride = false;
                                GetEnt().ClearBind();
                                Quaternion dest = Quaternion.LookRotation(m_moveInfo.m_dir);
                                GetEnt().SetRot(dest);
                                GetEnt().SetScale(Vector3.one * m_baseInfo.m_scale);
                            }
                            break;
                        //case 13:
                        //    if (m_head != null)
                        //        m_head.SetTaskState(head.taskstate);
                        //    break;
                        case 14:
                               ShowFootHalo(head.effectId);
                            break;
                    }
                    break;
                case CmdFspEnum.eLife:
                    if (ent == null)
                        return;
                    CmdLife life = cmd as CmdLife;
                    m_bDead = !life.state;

                    if (life.state)
                    {
                        ent.SetPriority(0);
                        ent.PlayAnima(SMtCreatureAnima.ANIMA_RESET, ()=>{
                            ResetState();
                            // 复活说话
                            //PlaySpeak(eRoleSpeakCsv.revive);
                        });
                        // 播放复活特效
                        if (m_bMaster)
                        {
                            CEffectMgr.Create(21002, GetEnt().GetPos(), GetEnt().GetRotate());
                        }
                    }
                    else
                    {
                        SetMove(false);
                        ent.PlayAnima(SMtCreatureAnima.ANIMA_DIE);
                        SoundManager.Inst.PlaySound(m_baseInfo.m_dieSound, ent.GetPos());
                        CEffectMgr.Create(m_baseInfo.m_dieEffect, GetEnt(), SBindPont.ORIGIN);

                        // 死亡说话
                        //PlaySpeak(eRoleSpeakCsv.die);
                    }
                    break;
                #endregion
                case CmdFspEnum.eFspUpdateEquip:
                    m_cmdUpdateEquip = cmd as CmdFspUpdateEquip;
                    UpdateEquip();
                    break;

                #region BUFF特效
                case CmdFspEnum.eBuff:                        // 特效挂点设置
                    CmdFspBuff buff = cmd as CmdFspBuff;
                    int effectId = buff.effectId;
                    if (effectId == 0)
                        return;

                    //Debug.Log(buff.effectId + " add:" + buff.bAdd + "   " + buff.bindType);
                    if (m_dicBuff == null)
                        m_dicBuff = new Dictionary<int, object>();
                    if (m_dicBuffCount == null)
                        m_dicBuffCount = new Dictionary<int, int>();
                    if (buff.bAdd)
                    {
                        if (!m_dicBuff.ContainsKey(effectId))
                        {
                            if (buff.bindType == (int)SBindPont.eBindType.LRHand)  // 左右手
                            {
                                List<int> hid = new List<int>();
                                int l = CEffectMgr.Create(effectId, m_ent, SBindPont.L_HAND);
                                int r = CEffectMgr.Create(effectId, m_ent, SBindPont.R_HAND);
                                hid.Add(l);
                                hid.Add(r);
                                m_dicBuff[effectId] = hid;

                                if (!IsVisible())
                                {
                                    CEffectMgr.GetEffect(l).SetShow(false);
                                    CEffectMgr.GetEffect(r).SetShow(false);
                                }
                            }
                            else if (buff.bindType == (int)SBindPont.eBindType.CreaturePos)
                            {
                                int hid = CEffectMgr.CreateByCreaturePos(effectId, m_ent, 1);
                                m_dicBuff[effectId] = hid;

                                if (!IsVisible())
                                {
                                    CEffect c = CEffectMgr.GetEffect(hid);
                                    if (c != null)
                                        c.SetShow(false);
                                }
                            }
                            else if (buff.bindType == (int)SBindPont.eBindType.CreatureHeadPos)
                            {
                                int hid = CEffectMgr.CreateByCreaturePos(effectId, m_ent, 2);
                                m_dicBuff[effectId] = hid;

                                if (!IsVisible())
                                {
                                    CEffect c = CEffectMgr.GetEffect(hid);
                                    if (c != null)
                                        c.SetShow(false);
                                }
                            }
                            else
                            {
                                int hid = CEffectMgr.Create(effectId, m_ent, SBindPont.GetBindPont(buff.bindType), null);
                                m_dicBuff[effectId] = hid;

                                if (!IsVisible())
                                {
                                    CEffect c = CEffectMgr.GetEffect(hid);
                                    if (c != null)
                                        c.SetShow(false);
                                }
                            }
                        }
                        if (!m_dicBuffCount.ContainsKey(effectId))
                        {
                            m_dicBuffCount[effectId] = 1;
                        }
                        else
                        {
                            m_dicBuffCount[effectId]++;
                        }
                    }
                    else
                    {
                        if (m_dicBuffCount.ContainsKey(effectId))
                        {
                            m_dicBuffCount[effectId]--;
                            if (m_dicBuffCount[effectId] <= 0)
                            {
                                m_dicBuffCount.Remove(effectId);
                            }
                        }

                        if (m_dicBuff.ContainsKey(effectId) && !m_dicBuffCount.ContainsKey(effectId))   // 如果包含这个BUFF，并且计数=0，才销毁
                        {
                            if (buff.bindType == (int)SBindPont.eBindType.LRHand)
                            {
                                List<int> hid = (List<int>)m_dicBuff[effectId];
                                for (int i = 0; i < hid.Count; i++)
                                {
                                    CEffectMgr.Destroy(hid[i]);
                                }
                            }
                            else
                            {
                                CEffectMgr.Destroy((int)m_dicBuff[effectId]);
                            }
                            m_dicBuff.Remove(effectId);
                        }
                    }
     
                    break;
                #endregion

                #region 状态表现
                case CmdFspEnum.eState:                      // 外观状态设置
                    CmdFspState state = cmd as CmdFspState;
                    eVObjectState type = state.type;
                    //Debug.Log("状态 add:" + state.bAdd + "   " + (eVObjectState)type);
                    if (state.bAdd)
                    {
                        SetShowState(type, true);
                        switch (type)
                        {
                            case eVObjectState.AlphaToHide:   // 全透 all alpha need hide effect
                                ent.SetShader(eShaderType.eAlphaToHide, Color.white, 0.3f, false, () =>
                                {
                                    ent.SetShow(false);
                                });
                                break;
                            case eVObjectState.AlphaToHalf: // 半透
                                GetEnt().SetShow(true);
                                if (CheckState(eVObjectState.AlphaToHide)) // 半透，全透状态应该是互斥的
                                {
                                    SetShowState(eVObjectState.AlphaToHide, false);
                                    ent.SetShader(eShaderType.eAlphaToHalf, Color.white, 0.0f, false);
                                }
                                else
                                {
                                    ent.SetShader(eShaderType.eAlphaToHalf, Color.white, 0.3f, false);
                                }
                                break;
                            case eVObjectState.Nihility:    // 虚无
                                //if (m_head != null)
                                //{
                                //    m_head.SetHeadAlpha(0.5f);
                                //}
                                //ent.SetShader(eShaderType.eNihility, new Color(1, 1, 1, 0.7f));
                                break;
                            case eVObjectState.God:    // 无敌
                                //ent.SetShader(eShaderType.eRim, Color.yellow);
                                break;
                            case eVObjectState.Hit:    // 受击
                                //ent.SetShader(eShaderType.eRim, new Color(0.8f, 0.8f, 0.8f, 1.0f));
                                break;
                            case eVObjectState.Silence:   // 被沉默
                                if (m_bMaster)
                                {
                                    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                                    //js.SetLock(true);
                                }
                                break;
                            case eVObjectState.Show:
                                GetEnt().SetShow(true);
                                break;
                            case eVObjectState.stun:    // 晕眩
                                ((BattleEntity)GetEnt()).Play(false);
                                if (m_bMaster)
                                {
                                    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                                    //js.SetLock(true);
                                }
                                break;
                            case eVObjectState.unmove:    // 禁锢
                    
                                break;
                            case eVObjectState.sleep:    // 睡眠
                       
                                if (m_bMaster)
                                {
                                    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                                    //js.SetLock(true);
                                }
                                break;
                        }
                    }
                    else
                    {
                        SetShowState(type, false);
                        switch (type)
                        {
                            case eVObjectState.AlphaToHide:   // 全透
                                ent.RemoveShader();
                                ent.SetShow(true);
                                break;
                            case eVObjectState.AlphaToHalf: // 半透,暂时都只用了移除全透
                                if (m_head != null)
                                {
                                    m_head.SetHeadAlpha(1.0f);
                                }
                                ent.RemoveShader();
                                break;
                            case eVObjectState.Nihility:
                            case eVObjectState.God:
                            case eVObjectState.Hit:
                                //if (m_head != null)
                                //{
                                //    m_head.SetHeadAlpha(1.0f);
                                //}
                                //ent.RemoveShader();
                                break;
                            case eVObjectState.Silence:   // 取消沉默
                                if (m_bMaster)
                                {
                                    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                                    //js.SetLock(false);
                                }
                                break;
                            case eVObjectState.Show:
                                GetEnt().SetShow(false);
                                break;
                            case eVObjectState.stun:    // 晕眩
                                ((BattleEntity)GetEnt()).Play(true);
                                ResetState();
                                if (m_bMaster)
                                {
                                    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                                    //js.SetLock(false);
                                }
                                break;
                            case eVObjectState.unmove:    // 禁锢
                                ResetState();
                                break;
                            case eVObjectState.sleep:    // 睡眠
                                ResetState();
                                if (m_bMaster)
                                {
                                    JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                                    //js.SetLock(false);
                                }
                                break;
                        }
                    }
                    break;
                #endregion

                case CmdFspEnum.eSkillAnimaPriority:   // 设置技能动作优先级
                    CmdSkillAnimaPriority cmdPri = cmd as CmdSkillAnimaPriority;
                    ((BattleEntity)GetEnt()).SetPriority(cmdPri.priority);
                    break;
            }
        }

        /// <summary>
        /// 重置动作状态
        /// 1.施法动作结束时
        /// 2.移除某状态时
        /// </summary>
        public void ResetState()
        {
            if (m_bDead || CheckState(eVObjectState.stun))
                return;

            if (m_state == CmdFspEnum.eFspStopMove || m_state == CmdFspEnum.none)
            {
                PlayIdle();
            }
            else
            {
                PlayMove();
            }
        }

        private void PlayIdle()
        {
            if (m_ent == null)
                return;
            //Debug.Log("play idle");
            BattleEntity bEnt = (BattleEntity)m_ent;

            if (m_cmdUpdateEquip == null)
            {
                bEnt.PlayAnima(SMtCreatureAnima.ANIMA_IDLE);
            }
            else
            {
                bEnt.PlayAnima(SMtCreatureAnima.ANIMA_IDLE2);
            }
            StopMoveSound();
        }

        private void PlayMove()
        {
            //Debug.Log("play move");
            if (m_ent == null)
                return;
            BattleEntity bEnt = (BattleEntity)m_ent;
            if(m_cmdUpdateEquip == null)
            {
                bEnt.PlayAnima(SMtCreatureAnima.ANIMA_WAlk);
            }
            else
            {
                bEnt.PlayAnima(SMtCreatureAnima.ANIMA_WAlk2);
            }
            PlayMoveSound();
        }

        private void UpdateEquip()
        {
            if (m_ent is BattleEntity)
            {
                ShowVipEffect(0);
                DestoryFootHalo_Master();
                (m_ent as BattleEntity).UpdateEquip(m_cmdUpdateEquip.m_dicEquip, ()=> {
                    ResetState();
                    ShowVipEffect(m_cmdUpdateEquip.m_vipLv);
                    ShowFootHalo_Master();
                });
            }
        }



        public Vector3 GetHeadPos()
        {
            return m_ent.GetPos() + Vector3.up * m_baseInfo.m_headHeight;
        }

        public Vector3 GetHitHeight()
        {
            return Vector3.up * m_baseInfo.m_headHeight * 0.7f;
        }

        public override void Update(float time, float fdTime)
        {
            if (m_ent == null || !m_ent.IsInited())
                return;


            // 主角声音监听
            if (m_bMaster && !CameraMgr.Inst.m_bDrag && !m_bDead)
            {
                CameraMgr.Inst.SetAudioListenerPos(m_ent.GetPos());
            }
            _UpdateMoveSound();
            //_Update_MoveSpeak(fdTime);
            if (m_head != null)
            {
                m_head.UpdatePos(GetHeadPos());
            }

            if (m_ride)
                return;

            base.Update(time, fdTime);
        }

        /// <summary>
        /// 是否可见，全局的，用于添加特效时
        /// </summary>
        public bool IsVisible()
        {
            return true;
        }

        private void DestoryBuffEffect()
        {
            // 销毁特效
            if (m_dicBuff != null)
            {
                foreach (KeyValuePair<int, object> item in m_dicBuff)
                {
                    if (item.Value is List<int>)
                    {
                        List<int> hid = (List<int>)item.Value;
                        for (int i = 0; i < hid.Count; i++)
                        {
                            CEffectMgr.Destroy(hid[i]);
                        }
                    }
                    else
                    {
                        CEffectMgr.Destroy((int)item.Value);
                    }
                }
                m_dicBuff.Clear();
                m_dicBuff = null;
            }
            if (m_dicBuffCount != null)
            {
                m_dicBuffCount.Clear();
                m_dicBuffCount = null;
            }
        }

        public override void Destory()
        {
            DestoryBuffEffect();
            DestoryFootHalo_Master();
            DestoryFootHalo();
            DestoryMoveSound();
            m_destroy = true;
            if(m_head != null)
            {
                m_head.RemoveHead();
                m_head = null;
            }
            base.Destory();
        }
    }

    
}

