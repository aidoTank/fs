using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
namespace Roma
{
   
    public partial class CCreature : CThing
    {
        public CCreature(long id)
            : base(id)
        {
        }

        public virtual void ExecuteFrame()
        {

        }

        public virtual bool InitConfigure()
        {
            return false;
        }

        public virtual void PushCommand()
        {
            
        }

        public virtual void SetName(string name)
        {
           
        }


        public virtual void SetPos(int x, int y)
        {

        }

        public virtual void SetDir(int eularAngle)
        {

        }

        public void SetQua(Quaternion vRot)
        {
    
        }


    }
}