
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
            //Debug.Log("moveDir:" + moveDir);
            // 静态碰撞时的写法
            // if(!CMapMgr.m_map.bCanMove((int)m_tempPos.x, (int)m_tempPos.y))
            // {
            //     m_tempPos = m_curPos;
            //     Vector2 vertical2 = Collide.GetVerticalVector(moveDir);
            //     m_tempPos += vertical2.normalized * delta;
            //     if(!CMapMgr.m_map.bCanMove((int)m_tempPos.x, (int)m_tempPos.y))
            //     {
            //         m_tempPos = m_curPos;
            //         vertical2 = -vertical2;
            //         m_tempPos += vertical2.normalized * delta;
            //     }
            // }

            // 动态碰撞时的写法
            //int depth = 0;
            //CheckCollide(moveDir, speed, ref depth);
            //Vector2 checkPos = m_tempPos + moveDir * GetR();
                // if(!CMapMgr.m_map.bCanMove((int)m_tempPos.x, (int)m_tempPos.y))
                // {
                
                //         m_tempPos = m_curPos;

                //     // Vector2 n = point - m_tempPos;
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

            // 重构
            Vector2 nextPos = m_curPos + moveDir * delta;
            Vector2 nextDir = m_cmdFspMove.m_dir.normalized;
            bool bMove = true;
            if(!CMapMgr.m_map.bCanMove((int)nextPos.x, (int)nextPos.y))
            {
                bMove = false;
                Debug.Log("开始遇到障碍物。。。。。。。。。。。。。");

                if(nextPos.x == 0)
                {
                    nextDir.x = 1;
                    nextDir.y = 0;
                    nextPos = m_curPos + nextDir * delta;
                    if(CMapMgr.m_map.bCanMove((int)nextPos.x, (int)nextPos.y))
                    {
                        bMove = true;
                    }
                    else
                    {
                        nextDir.x = -1;
                        nextDir.y = 0;
                        nextPos = m_curPos + nextDir * delta;
                        if(CMapMgr.m_map.bCanMove((int)nextPos.x, (int)nextPos.y))
                        {
                            bMove = true;
                        }
                    }
                }
                else if(nextPos.y == 0)
                {
                    nextDir.x = 0;
                    nextDir.y = 1;
                    nextPos = m_curPos + nextDir * delta;
                    if(CMapMgr.m_map.bCanMove((int)nextPos.x, (int)nextPos.y))
                    {
                        bMove = true;
                    }
                    else
                    {
                        nextDir.x = 0;
                        nextDir.y = -1;
                        nextPos = m_curPos + nextDir * delta;
                        if(CMapMgr.m_map.bCanMove((int)nextPos.x, (int)nextPos.y))
                        {
                            bMove = true;
                        }
                    }
                }
                else
                {
                    nextDir = new Vector2(0, moveDir.y > 0 ? 1: -1);
                    nextPos = m_curPos + nextDir * delta;
                    if(CMapMgr.m_map.bCanMove((int)nextPos.x, (int)nextPos.y))
                    {
                        bMove = true;
                    }
                    else
                    {
                        nextDir = new Vector2(moveDir.x > 0 ? 1: -1, 0);
                        nextPos = m_curPos + nextDir * delta;
                        if(CMapMgr.m_map.bCanMove((int)nextPos.x, (int)nextPos.y))
                        {
                            bMove = true;
                        }
                    }
                }

            }

            if(bMove)
            {
                SetPos(nextPos);
                SetDir(nextDir);

                m_vCreature.SetPos(m_curPos.ToVector3());
                m_vCreature.SetDir(nextDir.ToVector3());
                m_vCreature.SetSpeed(speed * 0.001f);
            }
            else
            {
                PushCommand(new CmdFspStopMove());
            }
            
 
        }

    /// <summary>
    /// 向量旋转
    /// </summary>
    private static Vector2 VecRotationMatrix(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        float newX = v.x * cos + v.y * sin;
        float newY = v.x * -sin + v.y * cos;
        return new Vector2((float)newX, (float)newY);
    }

        // private void CheckCollide(Vector2 moveDir, float speed, ref int depth)
        // {
        //     for(int i = 0 ; i < CMapMgr.m_map.m_listBarrier.Count; i ++)
        //     {
        //         object obj = CMapMgr.m_map.m_listBarrier[i];
                
        //         Sphere s = new Sphere();
        //         s.c = m_tempPos;
        //         s.r = GetR();
        //         Vector2 point = Vector2.zero;
        //         if(obj is OBB && Collide.bOBBInside(s, (OBB)obj, ref point))
        //         {
        //             m_tempPos = m_curPos;
        //             if(depth >= 1)
        //                 return;
        //             // 碰撞了，修正移动方向
        //             Vector2 n = point - m_tempPos;
        //             n.Normalize();
        //             Vector2 vertical2 = Collide.GetVerticalVector(n);
        //             float dot = Vector2.Dot(moveDir.normalized, vertical2.normalized);
        //             if (dot == 0)
        //                 return;
        //             if (dot < 0) 
        //             {
        //                 vertical2 = -vertical2;
        //             }
        //             m_tempPos += vertical2.normalized * FSPParam.clientFrameScTime * speed * 0.001f;
        //             depth++;
        //             CheckCollide(moveDir, speed, ref depth);
        //         }
        //         else if(obj is Sphere && Collide.bSphereSphere(s, (Sphere)obj))
        //         {
        //             m_tempPos = m_curPos;
        //             if (depth >= 1)
        //                 return;

        //             Vector2 n = (obj as Sphere).c - m_tempPos;
        //             n.Normalize();
        //             Vector2 vertical2 = Collide.GetVerticalVector(n);
        //             float dot = Vector2.Dot(moveDir.normalized, vertical2.normalized);
        //             if (dot == 0)
        //                 return;
        //             if (dot < 0) 
        //             {
        //                 vertical2 = -vertical2;
        //             }
        //             m_tempPos += vertical2.normalized * FSPParam.clientFrameScTime * speed * 0.001f;
        //             depth++;
        //             CheckCollide(moveDir, speed, ref depth);
        //         }
        //     }
        // }

        
        public virtual void SetPos(Vector2 pos)
        {
            m_curPos = pos;
            if(m_tempPos != pos)
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