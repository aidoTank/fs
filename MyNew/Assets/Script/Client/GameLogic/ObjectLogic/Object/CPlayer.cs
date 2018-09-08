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
        public VCreature m_vCreature;


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
            m_vCreature = new VCreature(data.ModelResId);
            return true;
        }


    }
}

