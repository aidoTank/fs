using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    /// <summary>
    /// 绑定施法者的触发器
    /// </summary>
    public class CBuffTrigger_BindCaster : CBuffTrigger
    {
        public CBuffTrigger_BindCaster(long id)
            : base(id)
        {

        }

        public override void InitPos(ref Vector2d startPos, ref Vector2d startDir)
        {
            startPos = m_caster.GetPos();
            startDir = m_caster.GetDir();

            SetPos(m_caster.GetPos(), true);
            SetDir(m_caster.GetDir());
            SetSpeed(m_caster.GetSpeed());
            //if (m_vCreature != null)
            //{
            //    m_vCreature.SetMove(true);
            //}


        }

        public override void _UpdatePos()
        {
            SetPos(m_caster.GetPos(), true);
            SetDir(m_caster.GetDir());
            SetSpeed(m_caster.GetSpeed());
            //if (m_vCreature != null)
            //{
            //    m_vCreature.SetMove(true);
            //}
        }


    }
}

