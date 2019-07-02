
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{

    public partial class CCreature : CObject
    {
        public virtual void UpdateVO_Create(int resId, float headHeight, eVOjectType type = eVOjectType.Creature)
        {
            if (resId == 0)
            {
                Debug.LogError("严重错误，模型id为空");
                return;
            }

            m_vCreature = VObjectMgr.Create(type);
            if (type == eVOjectType.Creature)
            {
                ((VObject)m_vCreature).UpdateMaster(IsMaster());
            }
            sVOjectBaseInfo info = new sVOjectBaseInfo();
            info.m_uid = (int)GetUid();
            info.m_resId = resId;
            info.m_pos = GetPos().ToVector3();
            info.m_dir = GetDir().ToVector3();
            info.m_scale = GetScale();
            info.m_speed = GetSpeed().value;
            info.m_headHeight = headHeight;
            //if (m_csvData != null)
            //{
            //    info.m_dieSound = m_csvData.dieSound;
            //    info.m_speakId = m_csvData.speakId;
            //    info.m_dieEffect = m_csvData.dieEffect;
            //}
            //if (string.IsNullOrEmpty(m_name))
            //    info.m_showHead = false;
            //else
            //    info.m_showHead = true;
            m_vCreature.Create(info);
        }




        public void UpdateVO_ShowHeadName(string sName)
        {
            if (m_vCreature == null)
                return;

            CmdUIHead name = new CmdUIHead();
            name.type = 1;
            name.name = sName;
            m_vCreature.PushCommand(name);
        }

        public void UpdateVO_ShowHeadTaskState(int m_state)
        {
            if (m_vCreature == null)
                return;

            CmdUIHead state = new CmdUIHead();
            state.type = 13;
            state.taskstate = m_state;
            m_vCreature.PushCommand(state);
        }

        /// <summary>
        /// 设置生死
        /// </summary>
        public void UpdateVO_ShowLife(bool bTrue)
        {
            if (m_vCreature == null)
                return;

            CmdLife life = new CmdLife();
            life.state = bTrue;
            m_vCreature.PushCommand(life);
        }

        public void UpdateVO_ShowHead(bool bShow)
        {
            if (m_vCreature == null)
                return;

            CmdUIHead lv = new CmdUIHead();
            lv.type = 5;
            lv.bShow = bShow;
            m_vCreature.PushCommand(lv);
        }

        public void UpdateVO_ShowHeadLv()
        {
            if (m_vCreature == null)
                return;

            CmdUIHead lv = new CmdUIHead();
            lv.type = 2;
            lv.lv = GetPropNum(eCreatureProp.Lv);
            m_vCreature.PushCommand(lv);
        }

        public void UpdateVO_ShowHeadHp()
        {
            if (m_vCreature == null)
                return;

            // 通知头顶更新
            CmdUIHead hp = new CmdUIHead();
            hp.type = 3;
            hp.curHp = GetPropNum(eCreatureProp.CurHp);
            hp.maxHp = GetPropNum(eCreatureProp.MaxHp);
            m_vCreature.PushCommand(hp);
        }

        public void UpdateVO_ShowHeadCamp(bool bTeam)
        {
            if (m_vCreature == null)
                return;

            CmdUIHead hp = new CmdUIHead();
            hp.type = 8;
            hp.bTeam = bTeam;
            m_vCreature.PushCommand(hp);
        }

        public void UpdateVO_SetNameShowOnly(bool bShow)
        {
            if (m_vCreature == null)
                return;

            CmdUIHead name = new CmdUIHead();
            name.type = 9;
            name.bShow = bShow;
            m_vCreature.PushCommand(name);
        }

        //public void UpdateVO_ChangeModel(int resId)
        //{
        //    PushCommand(new CmdFspStopMove());
        //    CmdChangeModel cmdModel = new CmdChangeModel();
        //    cmdModel.resId = resId;
        //    m_vCreature.PushCommand(cmdModel);
        //}

        public void UpdateVO_SetVObjectMaster(bool bMaster)
        {
            if (m_vCreature == null)
                return;

            ((VObject)m_vCreature).UpdateMaster(bMaster);
        }

        public void UpdateVO_ShowRide(bool bUp, CCreature ride = null)
        {
            if (m_vCreature == null)
                return;

            CmdUIHead name = new CmdUIHead();
            name.type = 12;
            name.bRide = bUp;
            if (ride != null)
                name.rideObject = ride.GetVObject();
            m_vCreature.PushCommand(name);
        }

        /// <summary>
        /// BUFF改变角色身上颜色的接口
        /// </summary>
        public void UpdateVO_ColorByBuff()
        {
            //if (m_buffList == null || m_vCreature == null)
            //    return;

            //bool hasColor = false;
            //Color color = Color.white;
            //// 获取有颜色的BUFF，取最新的
            //for (int i = 0; i < m_buffList.Count; i++)
            //{
            //    BuffBase buff = m_buffList[i];
            //    if (buff.m_buffData.hasColor)
            //    {
            //        hasColor = true;
            //        color = buff.m_buffData.hitColor;
            //    }
            //}

            //if (hasColor)
            //{
            //    //m_vCreature.GetEnt().SetShader(eShaderType.eRim, color / 255);
            //    m_vCreature.GetEnt().SetColor(color / 255);
            //}
            //else
            //{
            //    m_vCreature.GetEnt().RestoreColor();
            //    //m_vCreature.GetEnt().RemoveShader();
            //}
        }

        /// <summary>
        /// AOI创建的时候显示（没挂时）
        /// 复活的时候显示
        /// </summary>
        public int m_footHaloEffectId;
        public void UpdateVO_ShowFootHalo()
        {
            if (m_vCreature == null || m_footHaloEffectId == 0)
                return;

            CmdUIHead lv = new CmdUIHead();
            lv.type = 14;
            lv.effectId = m_footHaloEffectId;
            m_vCreature.PushCommand(lv);
        }

    }
}