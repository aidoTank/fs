using UnityEngine;
using System.Collections;

namespace Roma
{
    public class GameQuit
    {
        public void Init()
        {
            if (LogicSystem.Inst != null)
                LogicSystem.Inst.UnInitModule();
        }
    }
}

