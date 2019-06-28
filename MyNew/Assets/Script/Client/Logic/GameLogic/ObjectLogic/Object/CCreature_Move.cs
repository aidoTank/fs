
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using System;
//using UnityEngine;

//namespace Roma
//{
   
//    public partial class CCreature
//    {
//        public Vector2 m_curPos = Vector2.zero;
//        public Vector2 m_dir;


//        public void EnterMove()
//        {

//        }

//        public void TickMove()
//        {
//            if(m_cmdFspMove == null)
//                return;

//            Vector2 moveDir = m_cmdFspMove.m_dir.normalized;
//            int speed = GetPropNum(eCreatureProp.MoveSpeed);
//            float delta = FSPParam.clientFrameScTime * speed * 0.001f;
//            Vector2 nextPos = m_curPos + moveDir * delta;

//            // 动态碰撞时的写法
//            int depth = 0;
      
//            if(CheckCollide(m_curPos, nextPos, ref moveDir, ref depth, delta))
//            {
//                nextPos = m_curPos + moveDir * delta;

//                SetPos(nextPos);
//                SetDir(moveDir);

//                m_vCreature.SetPos(m_curPos.ToVector3());
//                m_vCreature.SetDir(moveDir.ToVector3());
//                m_vCreature.SetSpeed(speed * 0.001f);
//            }
//            else
//            {
//                PushCommand(new CmdFspStopMove());
//            }
//        }


//        /// <summary>
//        /// 如果使用nextPos去计算方向，碰撞的对象会相交进入，
//        /// 此时需要用到图形中运动的相交测试，待优化
//        /// </summary>
//        private bool CheckCollide(Vector2 curPos, Vector2 nextPos, ref Vector2 moveDir, ref int depth, float delta)
//        {
//            for(int i = 0 ; i < CMapMgr.m_map.m_listBarrier.Count; i ++)
//            {
//                object obj = CMapMgr.m_map.m_listBarrier[i];
                
//                Sphere s = new Sphere();
//                s.c = nextPos;
//                s.r = GetR();
//                Vector2 point = Vector2.zero;
//                if(obj is OBB && Collide.bSphereOBB(s, (OBB)obj, ref point))
//                {
//                    if(depth >= 1)
//                        return false;
//                    // 碰撞了，修正移动方向
//                    Vector2 n = point - curPos;
//                    n.Normalize();
//                    Vector2 vertical2 = Collide.GetVerticalVector(n);
//                    float dot = Vector2.Dot(moveDir.normalized, vertical2.normalized);
            
//                    if (dot < 0) 
//                    {
//                        vertical2 = -vertical2;
//                    }
//                    moveDir = vertical2.normalized;

//                    depth ++;
//                    nextPos = m_curPos + moveDir * delta;
//                    CheckCollide(curPos, nextPos, ref moveDir, ref depth, delta);
//                }
//                else if(obj is Sphere && Collide.bSphereSphere(s, (Sphere)obj))
//                {
//                        if(depth >= 1)
//                        return false;
                        
//                    Vector2 n = ((Sphere)obj).c - m_curPos;
//                    n.Normalize();
//                    Vector2 vertical2 = Collide.GetVerticalVector(n);
//                    float dot = Vector2.Dot(moveDir.normalized, vertical2.normalized);
                
//                    if (dot < 0) 
//                    {
//                        vertical2 = -vertical2;
//                    }
//                    moveDir = vertical2.normalized;
                
//                    depth ++;
//                    nextPos = m_curPos + moveDir * delta;
//                    CheckCollide(curPos, nextPos, ref moveDir, ref depth, delta);
//                }
//            }
//            return true;
//        }

        
//        public virtual void SetPos(Vector2 pos)
//        {
//            m_curPos = pos;
//        }

//        public Vector2 GetPos()
//        {
//            return m_curPos;
//        }

//        public virtual void SetDir(Vector2 dir)
//        {
//            m_dir = dir;
//        }

//        public virtual Vector2 GetDir()
//        {
//            return m_dir;
//        }

//        public float GetR()
//        {
//            return 0.5f;
//        }
//    }
//}