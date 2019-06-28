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

    }
}

