using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class CMasterPlayer : CPlayer
    {
        public CMasterPlayer(long id)
            : base(id)
        {
            m_type = EThingType.Master;
        }

        public override bool InitConfigure()
        {
            return base.InitConfigure();
        }

    }
}

