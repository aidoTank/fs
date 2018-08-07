using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{

    public partial class CPlayer : CCreature
    {
        public CPlayer(long id)
            : base(id)
        {
            m_type = EThingType.Player;
        }


        public override bool InitConfigure()
        {
            base.InitConfigure();

            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            PlayerCsvData data = playerCsv.GetData(1);
            if (null == m_ent)
            {
                EntityBaseInfo info = new EntityBaseInfo();
                info.m_resID = data.ModelResId;
                info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;

                //uint uH = EntityManager.Inst.CreateEntity(eEntityType.eBoneEntity, this.OnEntityLoaded, info);
                //m_ent = EntityManager.Inst.GetEnity(uH, false) as BoneEntity;
                //m_ent.SetUserString(eUserData.Uid, m_uId);
            }
            return true;
        }

        public override void OnEntityLoaded(Entity entity, object userObject)
        {
            base.OnEntityLoaded(entity, userObject);


        }


    }
}

