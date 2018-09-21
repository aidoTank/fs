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
        }

        public Entity GetEnt()
        {
            return EntityManager.Inst.GetEnity(m_hid);
        }

        public void SetPos(Vector2 pos)
        {
            GetEnt().SetPos(new Vector3(pos.x, 0, pos.y));
        }

    }
}

