

using UnityEngine;

namespace Roma
{
    public class AICondi : BtPrecondition
    {
        public CCreature m_creature;

        public override void Activate(BtDatabase database)
        {
            base.Activate(database);
            m_creature = database.GetData<CCreature>((int)eAIParam.INT_ROLE_UID);
        }
    }
}



