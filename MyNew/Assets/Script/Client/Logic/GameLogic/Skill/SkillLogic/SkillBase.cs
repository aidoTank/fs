using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 技能继承CObject只是用到了位置属性，重写了命令接口，只是为了保持结构的一致
    /// 
    /// 技能的创建由管理器创建
    /// 销毁由自身死亡状态标识决定
    /// </summary>
    public partial class SkillBase : CThing
    {
        public int m_skillLevel;                // 当前技能等级

        public CmdFspSendSkill m_curSkillCmd;   // 初次设置是玩家输入的指令，后续会修改给到BUFF
        public SkillCsvData m_skillInfo;

        public bool m_bLaunching;
        private int m_curLaunchTime;
        private int m_skillLifeEventHid;

        public VSkillBase m_vSkill;

        public SkillBase(long id, VSkillBase vSkill) :
            base(id)
        {
            m_vSkill = vSkill;
        }

        public void PushCommand(IFspCmdType cmd)
        {
            switch (cmd.GetCmdType())
            {
                case CmdFspEnum.eFspSendSkill:
                    m_curSkillCmd = cmd as CmdFspSendSkill;
                    Start();

                    m_vSkill.PushCommand(cmd);
                    break;
            }
        }

        public virtual void Start()
        {
            if(m_curSkillCmd.m_skillId == 0)
            {
                m_skillInfo = GetCaster().GetSkillByIndex(m_curSkillCmd.m_skillIndex).m_skillInfo;
            }
            else
            {
                // 组合技能
                SkillCsv skillCsv = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill);
                m_skillInfo = skillCsv.GetData(m_curSkillCmd.m_skillId);
            }

            m_bLaunching = true;
            // 同步施法者方向
            GetCaster().SetDir(m_curSkillCmd.m_dir);

            // 注册最长生命周期
            if(m_skillInfo.lifeTime != 0)
            {
                m_skillLifeEventHid = CFrameTimeMgr.Inst.RegisterEvent(m_skillInfo.lifeTime, () =>
                {
                    Destory();
                });
            }

            // 释放技能，前摇时不可再操作技能
            // 至于能不能移动看技能配置
            SetLogicEnable(false);
        }

        public virtual void ExecuteFrame(int frameId)
        {
            if (m_bLaunching && !m_destroy)
            {
                m_curLaunchTime += FSPParam.clientFrameMsTime;
                if (m_curLaunchTime > m_skillInfo.launchTime)
                {
                    m_bLaunching = false;
                    Launch();
                }
            }
        }

        public CCreature GetCaster()
        {
            return CCreatureMgr.Get(m_curSkillCmd.m_casterUid);
        }

        /// <summary>
        /// 弹道起飞,近战，AOE受击
        /// </summary>
        public virtual void Launch()
        {
            // 施法时的目标只有时自己
            OnCasterAddBuff(GetCaster());
        }

        /// <summary>
        /// m_logicSkillEnabled用于控制技能的保护状态
        /// 也用于BUFF去设置技能是否可用
        /// </summary>
        public void SetLogicEnable(bool bTrue)
        {
            CCreature cc = GetCaster();
            if (cc == null)
                return;
            cc.m_logicSkillEnabled = bTrue;

            // 不能移动的技能，才需要设置逻辑是否移动，常规的MOBA技能释放
            // 比如机枪，就可以边走边释放，就不设置逻辑是否移动
            if(m_skillInfo.bRota)
            {
                cc.m_logicSkillRotationEnabled = !bTrue;
            }
            if (!m_skillInfo.bMove)
            {
                cc.PushCommand(CmdFspStopMove.Inst);
                cc.SetLogicMoveEnabled(bTrue);
                // 释放技能不能移动时
                VBase v = cc.m_vCreature;
                if(v != null)
                {
                    v.SetMove(bTrue);
                }
            }
            // 重置遥感，一般在按下移动，并技能释放完之后，还需继续移动
            if (bTrue)
            {
                cc.UpdateUI_ResetJoyStick(true);
            }
        }

        public override void Destory()
        {
            m_destroy = true;

            CFrameTimeMgr.Inst.RemoveEvent(m_skillLifeEventHid);
            //Debug.Log("destory skill:" + m_skillInfo.name);
            SetLogicEnable(true);

            CCreature cc = GetCaster();
            if (cc != null)
            {
                cc.PushCommand(CmdFspStopMove.Inst);
                // 如果在释放技能时，操作了移动，技能释放结束修改方向
                //if (cc.m_cmdFspMove != null)
                //    cc.SetDir(GetCaster().m_cmdFspMove.m_dir);
            }

            // 销毁表现层
            if (m_vSkill != null)
            {
                m_vSkill.Destory();
                m_vSkill = null;
            }
        }

        public SkillDataCsvData GetSkillData()
        {
            SkillCsv skill = CsvManager.Inst.GetCsv<SkillCsv>((int)eAllCSV.eAC_Skill);
            int skillDataId = skill.GetSkillDataIdByLv(m_skillInfo.id, 1);
            SkillDataCsv dataCsv = CsvManager.Inst.GetCsv<SkillDataCsv>((int)eAllCSV.eAC_SkillData);
            return dataCsv.GetData(skillDataId);
        }

        public void OnCasterAddBuff(CCreature caster, CCreature target = null)
        {
            if (caster == null)
            {
                return;
            }
            SkillDataCsvData dataInfo = GetSkillData();
            if (dataInfo == null)
            {
                Debug.LogError("技能数据为空，请检测配置：技能id:" + m_skillInfo.id);
                return;
            }

            int[] selfBuffList = dataInfo.casterSelfBuffList;
            if (target == null)
                target = caster;
            AddBuff(caster, target, selfBuffList, caster.GetPos(), m_curSkillCmd.m_endPos, m_curSkillCmd.m_dir, m_curSkillCmd.m_skillIndex);
        }

        /// <summary>
        /// 1.支持玩家身上的BUFF
        /// 2.支持创建基于位置的触发器BUFF
        /// </summary>
        public static void AddBuff(CCreature caster, CCreature receiver, int[] buffList, Vector2d startPos, Vector2d skillPos, Vector2d dir, int skillIndex = 0, object obj = null)
        {
            for (int i = 0; i < buffList.Length; i++)
            {
                if (buffList[i] == 0)
                    continue;
                AddBuff(caster, receiver, buffList[i], startPos, skillPos, dir, skillIndex, obj);
            }
        }

        public static BuffBase AddBuff(CCreature caster, CCreature receiver, int buffId, Vector2d startPos, Vector2d skillPos, Vector2d dir, int skillIndex = 0, object obj = null)
        {
            // 接受者无敌时
            if (receiver != null)
            {
                //if (receiver.IsDie())
                //    return null;
                if (caster != receiver && receiver.CheckState(eBuffState.God))
                    return null;
            }

            // 非瞬间伤害的其他所有BUFF，如果是持续的，并且buffid相同时，重置生命时间
            SkillBuffCsvData buffData = CsvManager.Inst.GetCsv<SkillBuffCsv>((int)eAllCSV.eAC_SkillBuff).GetData(buffId);
            if (buffData == null)
            {
                Debug.LogError("无buffid,策划检测检查配置表:" + buffId);
                return null;
            }

            // 状态BUFF的处理
            // 接受者霸体时，不再添加其他状态BUFF
            if(receiver != null && receiver.CheckState(eBuffState.SuperArmor))
            {
                if (buffData.logicId == (int)eBuffType.state ||
                    buffData.logicId == (int)eBuffType.repel ||
                    buffData.logicId == (int)eBuffType.pullPos
                    )
                    return null;
            }

            // 相同BUFF的叠加处理
            if(buffData.IsCont &&
                receiver != null &&
                buffData.logicId != (int)eBuffType.damage)
            {
                BuffBase stateBuff = receiver.GetBuffByCsvId(buffId);
                // 1.相同BUFFID
                // 2.相同施法者
                // 3.相同施法者,相同触发器（暂无）
                if (stateBuff != null && stateBuff.m_caster == caster)
                {
                    //Debug.Log("有相同buff。。。" + buffId);
                    // 相同施法者的持续性BUFF，内部可以计算叠加次数和伤害
                    stateBuff.ResetTime();
                    return stateBuff;
                }
            }

            BuffBase buff = CBuffMgr.Create(buffId);
            //Debug.Log("技能流程：创建BUFF:" + buffId + " " + buff.m_buffData.name + " 类型：" + (eBuffType)buff.m_buffData.logicId);
            if (buff == null)
            {
                Debug.LogError("添加BUFF为空：" + buffId);
                return null;
            }

            // 最终的接收者，由配表决定
            if(buff.m_buffData.targetType == 0)
            {
            }
            else if(buff.m_buffData.targetType == 1)
            {
                receiver = caster;
            }

            buff.m_caster = caster;
            buff.m_rec = receiver;

            buff.m_startPos = startPos;
            buff.m_skillIndex = skillIndex;
            buff.m_skillPos = skillPos;
            buff.m_skillDir = dir;
            buff.m_extendParam = obj;
            // 持续性的才加到人物身上,当角色挂掉时，要清除身上的BUFF
            if (buff.IsCont() && receiver != null)
            {
                // 因为BUFF改变的速度，攻击力等都是在初始化时，遍历BUFF增减数值，所以要先add
                receiver.AddBuff(buff);   
                buff.Init();
            }
            else
            {
                buff.Init();
                buff.Destroy();
            }
            return buff;
        }
    }
}