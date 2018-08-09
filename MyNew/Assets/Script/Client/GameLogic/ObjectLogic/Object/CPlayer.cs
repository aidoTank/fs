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

            // 测试，需建立表现层
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = data.ModelResId;
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;

            int uH = EntityManager.Inst.CreateEntity(eEntityType.eBoneEntity, info, null);


            return true;
        }
    }
}

