
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using System;
//using UnityEngine;

//namespace Roma
//{
//    public partial class CCreature
//    {
//        public CmdFspAutoMove m_cmdFspAutoMove;

//        private List<Vector2> m_movePath;
//        private List<float> m_pathTime;  
//        private int m_curMoveIndex;
//        private float m_curMoveTime;
//        public bool m_bMovePathing;
//        // 为了兼容碰撞的推开，这里改成根据方向移动（碰撞会修改curPos），但是必须保证障碍数据比碰撞体大一个角色的半径
//        private List<Vector2> m_dirList;
//        private Action m_moveEnd;

//        private const int UI_UNIT_DIS = 10;

//        public List<Vector2> GetPath()
//        {
//            return m_movePath;
//        }

//        /// <summary>
//        /// 根据单位距离给列表填充路点
//        /// </summary>
//        public void GetUIPath(ref List<Vector2> pathList)
//        {
//            pathList.Clear();
//            pathList.Add(m_movePath[0]);
//            for (int i = 0; i < m_movePath.Count; i++)
//            {
//                // 取最新的当前点
//                Vector2 curPos = pathList[pathList.Count - 1];
//                Vector2 nextPos = m_movePath[i];

//                float dis = Vector2.Distance(curPos, nextPos);
//                if (dis > UI_UNIT_DIS * 2)
//                {
//                    Vector2 dir = (nextPos - curPos).normalized;
//                    int num = (int)dis / UI_UNIT_DIS;
//                    for (int n = 0; n < num; n++)
//                    {
//                        Vector2 newPoint = curPos + dir * n * UI_UNIT_DIS;
//                        pathList.Add(newPoint);
//                    }
//                }
//                else if(dis >= UI_UNIT_DIS)
//                {
//                    pathList.Add(nextPos);
//                }
//            }
//            pathList.Add(m_movePath[m_movePath.Count - 1]);
//        }

//        private void InitAutoMoveParam()
//        {
//            if (m_movePath == null)
//                m_movePath = new List<Vector2>();
//            if (m_pathTime == null)
//                m_pathTime = new List<float>();
//            if (m_dirList == null)
//                m_dirList = new List<Vector2>();
//            if (m_cmdFspAutoMove == null)
//                m_cmdFspAutoMove = new CmdFspAutoMove();
//        }

       

//        public void EnterAutoMove()
//        {
//            InitAutoMoveParam();

//            List<Vector2> m_movePath = new List<Vector2>();
//            CMapMgr.m_map.GetPath(this, GetPos().ToVector2(), m_cmdFspAutoMove.m_pos.ToVector2(), ref m_movePath);
//            //m_moveEnd = moveEnd;
 
//            float divSpeed = 1 / GetSpeed().value;
//            StartAutoMove(ref m_movePath, divSpeed);
//        }

//        public void TickAutoMove()
//        {
//            if (IsDie())
//                return;


//            if (m_bMovePathing)
//            {
//                _UpdateMove(0.0f, FSPParam.clientFrameScTime);

//                if (Debug.logger.logEnabled)
//                {
//                    for (int i = 0; i < m_movePath.Count - 1; i++)
//                    {
//                        float x1 = m_movePath[i].x;
//                        float y1 = m_movePath[i].y;
//                        float x2 = m_movePath[i + 1].x;
//                        float y2 = m_movePath[i + 1].y;
//                        Debug.DrawLine(
//                            new Vector3(x1, 4, y1),
//                            new Vector3(x2, 4, y2),
//                            Color.green);
//                    }
//                }
//            }
//        }

//        private void InitMove()
//        {
//            m_movePath.Clear();
//            m_pathTime.Clear();
//            m_curMoveIndex = 0;
//            m_curMoveTime = 0.0f;
//            m_bMovePathing = false;

//            m_dirList.Clear();
//        }

//        public void StartAutoMove(ref List<Vector2> pathList, float divSpeed)
//        {
//            InitMove();
//            m_bMovePathing = true;
//            // 给路径赋值
//            for (int i = 0; i < pathList.Count; i++)
//            {
//                m_movePath.Add(pathList[i]);
//            }
//            // 插入玩家当前起点位置
//            m_movePath.Insert(0, GetPos().ToVector2());

//            // 开始获取移动时间
//            float runTime = 0.0f;
//            for (int i = 0; i < m_movePath.Count - 1; i++)
//            {
//                float dis = Vector2.Distance(m_movePath[i], m_movePath[i + 1]);
//                runTime += dis * divSpeed;
//                m_pathTime.Add(runTime);
//                // 方向
//                m_dirList.Add(m_movePath[i + 1] - m_movePath[i]);
//            }
//            m_pathTime.Insert(0, 0);    // 在第一个位置时间当然是0了
//        }

//        private void _UpdateMove(float fTime, float fDTime)
//        {
//            //float t = 0.0f;
//            m_curMoveTime += fDTime;
//            int begin = m_curMoveIndex + 1;
//            for (int i = begin; i < m_pathTime.Count; i++)    // 索引0,1,2,3,4 取 1-4 作为对比的目标时间
//            {
//                if (m_curMoveTime > m_pathTime[i])
//                {
//                    m_curMoveIndex = i;
//                    break;
//                }
//            }

//            if (m_curMoveIndex >= m_pathTime.Count - 1)
//            {
//                // 最后一个地点
//                StopAutoMove();
//                return;
//            }
//            else
//            {
//                float delta = FSPParam.clientFrameScTime * GetSpeed().value;
//                Vector2 nextPos = m_curPos.ToVector2() + m_dirList[m_curMoveIndex].normalized * delta;

//                SetPos(nextPos.ToVector2d());
//                SetDir(m_dirList[m_curMoveIndex].normalized.ToVector2d());
//            }
//        }

//        public void StopAutoMove()
//        {
//            if(m_bMovePathing)
//            {
//                m_bMovePathing = false;
//                PushCommand(CmdFspStopMove.Inst);
//                if (m_moveEnd != null)
//                {
//                    m_moveEnd();
//                    m_moveEnd = null;
//                }
//            }
//        }


//        //public List<Vector2> m_blockInfo = new List<Vector2>();

//        //public void SetDynamicBlock()
//        //{
//        //    for (int i = 0; i < m_blockInfo.Count; i++)
//        //    {
//        //        Vector2 info = m_blockInfo[i];
//        //        SceneManager.Inst.SetBlock((int)info.x, (int)info.y, false);
//        //    }
//        //    m_blockInfo.Clear();

//        //    Vector2 pos = GetPos();
//        //    Vector2 size = Vector3.one;
//        //    int x0 = (int)(pos.x);
//        //    int x1 = (int)(pos.x);
//        //    int z0 = (int)(pos.y);
//        //    int z1 = (int)(pos.y);
//        //    for (int x = x0; x <= x1; x++)
//        //    {
//        //        for (int z = z0; z <= z1; z++)
//        //        {
//        //            // 这个值是以0.5为单元格，所以在物体计算时，边界起点终点都要*2
//        //            if (!SceneManager.Inst.Isblock(x, z))
//        //            {
//        //                SceneManager.Inst.SetBlock(x, z, true);
//        //                // 保存障碍点信息
//        //                m_blockInfo.Add(new Vector2(x, z));
//        //            }
//        //        }
//        //    }
//        //}

//    }
//}