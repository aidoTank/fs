using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class SkillNear : SkillBase
    {
   
        public SkillNear(int uid, VSkillBase vSkill)
            : base(uid, vSkill)
        {
      
        }


        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
        }

        public override void Launch()
        {
            Debug.Log("近战击中，计算伤害");
        }

        public override void Destory()
        {

        }


    }
}