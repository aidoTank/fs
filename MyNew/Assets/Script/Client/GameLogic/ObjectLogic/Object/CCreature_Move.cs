
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature
    {
   
        
        public Vector2 m_curPos = Vector2.zero;

        public void EnterMove()
        {

        }

        public void TickMove()
        {
            if(m_cmdFspMove == null)
                return;
            int speed = GetPropNum(eCreatureProp.MoveSpeed);
            m_curPos = m_curPos + m_cmdFspMove.m_dir * FSPParam.clientFrameScTime * speed * 0.001f;
            m_vCreature.SetPos(m_curPos);
            m_vCreature.SetDir(m_cmdFspMove.m_dir);
        }

    }


}