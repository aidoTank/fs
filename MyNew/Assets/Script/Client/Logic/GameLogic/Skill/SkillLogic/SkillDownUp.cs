using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 引导性机枪技能
    /// 第一次按下时创建了技能类
    /// 后续持续按着和抬起通过调用SetCmd传递方向等参数
    /// 方向参数输入：
    /// 1.玩家的遥感类中查找
    /// 2.AI中查找
    /// </summary>
    public partial class SkillDownUp : SkillBase
    {

        public List<BuffBase> m_curBuff;  // 当前触发器BUFF
        private float m_lifeTime;

        private bool m_noLaunchUp;  //没发射就抬起了

        public SkillDownUp(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {
            m_lifeTime = 0;
        }

        public override void Launch()
        {
            base.Launch();
            // 获取真实BUFF
            GetBuff();
            if (m_noLaunchUp) // 未发射就抬起时，如果时间小于发射时间，就延长m_skillInfo.launchTime/2时间，达到点射效果
            {
                if (m_lifeTime < m_skillInfo.launchTime)
                {
                    OnDown();
                    CFrameTimeMgr.Inst.RegisterEvent(m_skillInfo.launchTime / 2, () =>
                    {
                        OnUp();
                    });
                }
            }
        }

        public void SetCmd(CmdFspSendSkill cmd)
        {
            if (m_noLaunchUp) 
                return;

            //Debug.Log("cmd :" + cmd.m_bDown + " launch:" +m_bLaunch);
            m_curSkillCmd = cmd;

            if (cmd.m_bDown)
            {
                OnDown();
            }
            else
            {
                if (m_bLaunching)   // 如果还在前摇状态中就终止了
                {
                    m_noLaunchUp = true;
                    return;
                }
                OnUp();
            }
        }

        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
            m_lifeTime += FSPParam.clientFrameMsTime;

            // 同步玩家位置和技能方向
            SetBuffInfo();
        }

        private void SetBuffInfo()
        {
            if (m_curBuff == null || GetCaster() == null)
                return;
            for (int i = 0; i < m_curBuff.Count; i++)
            {
                m_curBuff[i].m_skillPos = GetCaster().GetPos().ToVector2();
                m_curBuff[i].m_skillDir = m_curSkillCmd.m_dir;
            }
        }

        public void OnDown()
        {
            SetBuffInfo();
            if (m_vSkill != null)
                ((VSkillDownUp)m_vSkill).OnDown(m_curSkillCmd);
        }

        public void OnUp()
        {
            if (m_curBuff != null)
            {
                for (int i = 0; i < m_curBuff.Count; i++)
                {
                    m_curBuff[i].Destroy();
                }
            }
            if(m_vSkill != null)
                ((VSkillDownUp)m_vSkill).OnUp(m_curSkillCmd);
            Destory();
        }

        private void GetBuff()
        {
            if (m_curBuff != null)
            {
                return;
            }
            m_curBuff = new List<BuffBase>();
            int[] selfBuffList = GetSkillData().casterSelfBuffList;
            for (int i = 0; i < selfBuffList.Length; i++)
            {
                BuffBase buff = GetCaster().GetBuffByCsvId(selfBuffList[i]);
                if (buff != null /*&& buff.GetBuffType() == eBuffType.createTrigger*/)
                {
                    // 如果是触发器BUFF，则传递技能指令信息
                    m_curBuff.Add(buff);
                }
            }
        }
    }
}