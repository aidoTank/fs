
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
        public Vector2 m_dir;

        public void EnterMove()
        {

        }

        public void TickMove()
        {
            if(m_cmdFspMove == null)
                return;

            Vector2 moveDir = m_cmdFspMove.m_dir.normalized;
            int speed = GetPropNum(eCreatureProp.MoveSpeed);
            float delta = FSPParam.clientFrameScTime * speed * 0.001f;
            m_tempPos += moveDir * delta;
            // 获取当前方向的角度值
            Debug.Log("moveDir:" + moveDir);
            // 静态碰撞时的写法
            if(!CMapMgr.m_map.bCanMove((int)m_tempPos.x, (int)m_tempPos.y))
            {
                m_tempPos = m_curPos;
                Vector2 vertical2 = Collide.GetVerticalVector(moveDir);
                m_tempPos += vertical2.normalized * delta;
                if(!CMapMgr.m_map.bCanMove((int)m_tempPos.x, (int)m_tempPos.y))
                {
                    m_tempPos = m_curPos;
                    vertical2 = -vertical2;
                    m_tempPos += vertical2.normalized * delta;
                }
            }

            // 动态碰撞时的写法
            //int depth = 0;
            //CheckCollide(moveDir, speed, ref depth);
            // Vector2 checkPos = m_tempPos + moveDir * GetR();
            // if(!CMapMgr.m_map.bCanMove((int)m_tempPos.x, (int)m_tempPos.y))
            // {
              
            //         m_tempPos = m_curPos;

            //        // Vector2 n = point - m_tempPos;
            //         //n.Normalize();
            //         Vector2 vertical2 = Collide.GetVerticalVector(moveDir);
            //         // float dot = Vector2.Dot(moveDir.normalized, vertical2.normalized);
            //         // if (dot == 0)
            //         //     return;
            //         // if (dot < 0) 
            //         // {
            //         //     vertical2 = -vertical2;
            //         // }
            //         m_tempPos += vertical2.normalized * FSPParam.clientFrameScTime * speed * 0.001f;
           
            //     return;
            // }
            
            SetPos(m_tempPos);
            SetDir(m_cmdFspMove.m_dir);

            m_vCreature.SetPos(m_curPos.ToVector3());
            m_vCreature.SetDir(m_cmdFspMove.m_dir.ToVector3());
        }

        private void CheckCollide(Vector2 moveDir, float speed, ref int depth)
        {
            for(int i = 0 ; i < CMapMgr.m_map.m_listBarrier.Count; i ++)
            {
                object obj = CMapMgr.m_map.m_listBarrier[i];
                
                Sphere s = new Sphere();
                s.c = m_tempPos;
                s.r = GetR();
                Vector2 point = Vector2.zero;
                if(obj is OBB && Collide.bOBBInside(s, (OBB)obj, ref point))
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
                else if(obj is Sphere && Collide.bSphereSphere(s, (Sphere)obj))
                {
                    m_tempPos = m_curPos;
                    if (depth >= 1)
                        return;

                    Vector2 n = (obj as Sphere).c - m_tempPos;
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

        
        public virtual void SetPos(Vector2 pos)
        {
            m_curPos = pos;
            m_tempPos = pos;
        }

        public Vector2 GetPos()
        {
            return m_curPos;
        }

        public virtual void SetDir(Vector2 dir)
        {
            m_dir = dir;
        }

        public virtual Vector2 GetDir()
        {
            return m_dir;
        }

        public float GetR()
        {
            return 0.5f;
        }
    }
}