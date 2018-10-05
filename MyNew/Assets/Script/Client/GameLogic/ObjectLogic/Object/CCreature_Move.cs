
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

namespace Roma
{
   
    public partial class CCreature
    {
   
        
        public Vector2 m_tempPos = Vector2.zero;
        public Vector2 m_curPos = Vector2.zero;
        public void EnterMove()
        {

        }

        public void TickMove()
        {
            if(m_cmdFspMove == null)
                return;
            Vector2 moveDir = m_cmdFspMove.m_dir.normalized;
            int speed = GetPropNum(eCreatureProp.MoveSpeed);
            m_tempPos = m_tempPos + moveDir * FSPParam.clientFrameScTime * speed * 0.001f;

            int depth = 0;
            CheckCollide(moveDir, speed, ref depth);
  
            m_curPos = m_tempPos;
            m_vCreature.SetPos(m_curPos.ToVector3());
            m_vCreature.SetDir(m_cmdFspMove.m_dir);
        }

        private void CheckCollide(Vector2 moveDir, float speed, ref int depth)
        {
            for(int i = 0 ; i < CMapMgr.m_map.m_listBarrier.Count; i ++)
            {
                OBB obb = CMapMgr.m_map.m_listBarrier[i];
                Sphere s = new Sphere();
                s.c = m_tempPos;
                s.r = GetR();
                Vector2 point = Vector2.zero;
                if(Collide.bOBBInside(s, obb, ref point))
                {
                    m_tempPos = m_curPos;
                    if(depth >= 1)
                        return;
                    // 碰撞了，修正移动方向
                    Vector2 n = point - m_tempPos;
                    n.Normalize();
                    Vector2 vertical2 = Collide.GetVerticalVector(n);
                    float dot = Vector2.Dot(moveDir.normalized, vertical2.normalized);
                    if (dot == 0)
                        return;
                    if (dot < 0) 
                    {
                        vertical2 = -vertical2;
                    }
                    m_tempPos += vertical2.normalized * FSPParam.clientFrameScTime * speed * 0.001f;
                    depth++;
                    CheckCollide(moveDir, speed, ref depth);
                }
            }
        }

        public Vector2 GetPos()
        {
            return m_curPos;
        }
    }


}