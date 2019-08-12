using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 闪现
    /// </summary>
    public class Buff101 : BuffBase
    {

        public Buff101(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            base.Init();
            CCreature cast = m_caster;
            if(cast != null)
            {
                int len = GetVal1();
                Vector2d target = cast.GetPos() + m_skillDir * len;
                cast.SetPos(target, true);
            }
        }


        public override void Destroy()
        {
            base.Destroy();
        }

    }
}