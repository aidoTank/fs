using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public partial class VCreature
    {
        public bool m_bMaster;
        public int m_hid;
        private CmdFspEnum m_state;
        private BoneEntity m_ent;

        public VCreature(int ResId)
        {
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = ResId;
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            m_hid = EntityManager.Inst.CreateEntity(eEntityType.eBoneEntity, info, (ent)=> 
            {
                if(m_bMaster)
                {
                    CameraMgr.Inst.InitCamera(this);
                }
            });
            m_ent = (BoneEntity)EntityManager.Inst.GetEnity(m_hid);
        }

        public BoneEntity GetEnt()
        {
            return m_ent;
        }

        public void SetPos(Vector2 pos)
        {
            m_ent.SetPos(new Vector3(pos.x, 0, pos.y));
        }

        public void PushCommand(IFspCmdType cmd)
        {
            m_state = cmd.GetCmdType();
            if(cmd.GetCmdType() == CmdFspEnum.eFspStopMove)
            {
                AnimationAction animaInfo = new AnimationAction();
                animaInfo.playSpeed = 1;
                animaInfo.strFull = "stand";
                animaInfo.eMode = WrapMode.Loop;
                m_ent.Play(animaInfo);
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspMove)
            {
                AnimationAction animaInfo = new AnimationAction();
                animaInfo.playSpeed = 1;
                animaInfo.strFull = "run";
                animaInfo.eMode = WrapMode.Loop;
                m_ent.Play(animaInfo);
            }
        }

    }
}

