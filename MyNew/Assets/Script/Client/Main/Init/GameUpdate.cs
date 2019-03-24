using UnityEngine;
using System.Collections;

namespace Roma
{

    public class GameUpdate
    {
        public void Update(float fTime, float fDTime)
        {
            if(LogicSystem.Inst != null)
                LogicSystem.Inst.UpdateModule(fTime, fDTime);
        }

        public void LateUpdate(float fTime, float fDTime)
        {
            if (LogicSystem.Inst != null)
                LogicSystem.Inst.LateUpdateModule(fTime, fDTime);
        }
    }
}
