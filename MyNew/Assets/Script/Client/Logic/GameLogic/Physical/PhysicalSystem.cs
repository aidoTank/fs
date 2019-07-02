using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    public enum eColliderType
    {
        None,
        Circle,
        Polygon,
    }

    /// <summary>
    /// 逻辑层碰撞器
    /// </summary>
    public class Collider
    {
        public eColliderType type;
        public Vector2d c;
        public bool active = true;
        public bool isObstacle = true;   // 是否是障碍
        public bool notPush = false;

        public Action<Vector2d, Vector2d> m_updatePosEvent;

        public eColliderType GetColliderType()
        {
            return type;
        }

        public void AddOffsetPos(Vector2d oPos, Vector2d dir)
        {
            c += oPos;
            if (m_updatePosEvent != null)
                m_updatePosEvent(c, dir);
        }

        /// <summary>
        /// 改为由上层逻辑心跳驱动，效率提高非常多
        /// </summary>
        public virtual void Update()
        {
        }

        public virtual void Draw()
        {

        }
    }

    public class Polygon : Collider
    {
        public bool bAirWall = true;   // 是否是空气墙障碍
        public List<Vector2d> m_edgesList = new List<Vector2d>();    // 边列表
        public List<Vector2d> m_worldPosList = new List<Vector2d>(); // 顶点列表

        public Polygon()
        {
            type = eColliderType.Polygon;
        }

        /// <summary>
        /// 传入顶点列表
        /// </summary>
        public void Init(Vector2d[] point)
        {
            for (int i = 0; i < point.Length; i++)
            {
                m_worldPosList.Add(point[i]);
            }
            InitDdgesVecList();
        }

        /// <summary>
        /// 初始化边列表
        /// </summary>
        public void InitDdgesVecList()
        {
            for (int i = 0; i < m_worldPosList.Count; i++)
            {
                int pointIndex1 = i;
                int pointIndex2 = (i + 1) % m_worldPosList.Count;
                Vector2d vec = m_worldPosList[pointIndex2] - m_worldPosList[pointIndex1];
                m_edgesList.Add(vec);
            }
        }
    }

    public class Circle : Collider
    {
        public FixedPoint r;

        public Circle()
        {
            type = eColliderType.Circle;
        }

        public override void Update()
        {
            PhysicsManager.Inst.MoveCheck(this);
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
            Gizmos.DrawSphere(c.ToVector3(), r.value);
        }
#endif
    }

    /// <summary>
    /// 场景生物总数在100左右时，如果通过碰撞器管理类的心跳用2个for循环遍历检测碰撞，CPU耗时在4MS
    /// 由于上层做了AOI，此时只需要主角附近的生物参与检测，在15左右，每个去遍历一次，耗时在0.4MS
    /// </summary>
    public class PhysicsManager : Singleton
    {
        public static PhysicsManager Inst = null;

        public List<Collider> m_list = new List<Collider>();
        public bool m_rolePush = false;

        public PhysicsManager()
            : base(true)
        {
        }

        public override void Init()
        {

        }

        public void SetRolePush(bool bPush)
        {
            m_rolePush = bPush;
        }


        /// <summary>
        /// 是否障碍，用于终止冲刺
        /// </summary>
        public bool Isblock(int x, int y)
        {
            Circle cir = new Circle();
            cir.c = new Vector2d(x, y);
            cir.r = FixedPoint.one;
            for (int j = 0; j < m_list.Count; j++)
            {
                Collider col = m_list[j];
                if (col is Polygon)
                {
                    Polygon pol = col as Polygon;
                    if (CheckPolygonAndCircle(pol, cir, false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Add(Collider col)
        {
            if (!m_list.Contains(col))
            {
                m_list.Add(col);
            }
        }

        public void Remove(Collider col)
        {
            if (m_list.Contains(col))
            {
                m_list.Remove(col);
                col = null;
            }
        }

        /// <summary>
        ///  给需要移动的生物调用
        /// </summary>
        public void MoveCheck(Circle cir)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                MoveCheck(cir, m_list[i]);
            }
        }

        public void OnDrawGizmos()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].Draw();
            }
        }

        public FixedPoint CircleCircleDis2 = new FixedPoint(4);
        public FixedPoint CirclePolygonDis2 = new FixedPoint(6400);
        private FixedPoint m_dis2;
        /// <summary>
        ///  通过距离，已优化
        /// </summary>
        public void MoveCheck(Collider a, Collider b)
        {
            if (!a.active || !b.active)
                return;
            if (a == b)
                return;
            if (a is Polygon && b is Polygon)     // 障碍之间不检测
                return;

            if (!m_rolePush)                      // 角色不碰撞时，都是圆时，不处理
            {
                if (a is Circle && b is Circle)
                {
                    return;
                }
            }
            else
            {
                if (a is Circle && b is Circle)   // 如果角色需要碰撞，并且都是圆，很远时，不处理
                {
                    FPCollide.GetDis2(ref m_dis2, ref a.c, ref b.c);
                    if (m_dis2 > CircleCircleDis2)
                    {
                        return;
                    }
                }
            }

            if (a is Circle && b is Polygon)      // 角色和障碍太远不处理
            {
                FPCollide.GetDis2(ref m_dis2, ref a.c, ref b.c);
                if (m_dis2 > CirclePolygonDis2)
                {
                    return;
                }
            }

            eColliderType type1 = a.GetColliderType();
            eColliderType type2 = b.GetColliderType();
            switch (type1)
            {
                case eColliderType.Circle:
                    switch (type2)
                    {
                        case eColliderType.Circle:
                            CheckCircleAndCircle(a as Circle, b as Circle);
                            break;
                        case eColliderType.Polygon:
                            CheckPolygonAndCircle(b as Polygon, a as Circle);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// 1.获取a到b向量的方向
        /// 2.获取a在此方向上的投影点
        /// 3.获取a左右为圆半径的距离，组成的直线，为a的投影线
        /// 4.获取b的投影线
        /// 5.对比2条投影线是否相交
        /// 6.如果相交，获取a是否在左边，以及相交的距离
        /// 7.根据ab方向，偏移量，可以知道具体偏移向量
        /// 8.根据具体偏移向量和是否a在左边，来计算推开的距离
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool CheckCircleAndCircle(Circle a, Circle b, bool bPush = true)
        {
            Vector2d axis = a.c - b.c;
            axis.Normalize();

            FixedPoint projPoint = Vector2d.Dot(a.c, axis);
            FixedPoint min = projPoint - a.r;
            FixedPoint max = projPoint + a.r;
            Vector2d projA = new Vector2d(min, max);

            projPoint = Vector2d.Dot(b.c, axis);
            min = projPoint - b.r;
            max = projPoint + b.r;
            Vector2d projB = new Vector2d(min, max);

            if (CheckLine(projA, projB))
            {
                if (bPush)
                {
                    bool bAtLeft = false;
                    FixedPoint offset = FixedPoint.zero;
                    SetPushVec(projA, projB, ref bAtLeft, ref offset);
                    Push(a, b, offset, axis, bAtLeft);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 1.获取所有点的世界坐标
        /// 2.获取每条边的向量
        /// 3.遍历每条边的向量，获取它的垂直向量，也就是投影轴
        /// 4.通过当前多边形的某一边的投影轴，计算圆在这个投影轴上的投影线段
        /// 5.通过当前多边形的某一边的投影轴，计算当前多边形在这投影轴上的线段，遍历所有点，取最小和最大
        /// 6.对比多边形投影线段，和圆投影线段是否相交，如果一条边的不相交则不碰撞，退出循环
        /// 7.如果相交，获取a是否在左边，以及相交的距离
        /// 8.继续循环下一个边向量，取最小的相交距离，因为最小的相交才是真正的相交距离
        /// </summary>
        /// <returns>是否相交</returns>
        private bool CheckPolygonAndCircle(Polygon a, Circle b, bool bPush = true)
        {
            bool bInit = false;
            bool bAtLeft = true;
            Vector2d offsetVec = Vector2d.zero;
            FixedPoint offsetLen = FixedPoint.zero;

            List<Vector2d> wPosList = a.m_worldPosList;
            List<Vector2d> edgesVecList = a.m_edgesList;

            for (int i = 0; i < edgesVecList.Count; i++)
            {
                Vector2d axis = edgesVecList[i];
                // 获取投影轴
                axis = GetN(axis);
                axis.Normalize();

                // 获取当前投影轴上，多边形的投影线
                Vector2d curPoint = wPosList[0];
                FixedPoint minProj = Vector2d.Dot(curPoint, axis);
                FixedPoint maxPorj = minProj;
                for (int p = 1; p < wPosList.Count; p++)
                {
                    curPoint = wPosList[p];
                    FixedPoint curPorj = Vector2d.Dot(curPoint, axis);
                    if (curPorj < minProj)
                        minProj = curPorj;
                    if (curPorj > maxPorj)
                        maxPorj = curPorj;
                }
                Vector2d projA = new Vector2d(minProj, maxPorj);

                // 求圆的投影点和线
                FixedPoint projPont = Vector2d.Dot(b.c, axis);
                FixedPoint min = projPont - b.r;
                FixedPoint max = projPont + b.r;
                Vector2d projB = new Vector2d(min, max);

                if (!CheckLine(projA, projB))  // 其中一边时的所有点的投影不相交，则不想交
                {
                    return false;
                }
                else
                {
                    if (bPush)
                    {
                        bool bAtLeftTemp = false;
                        FixedPoint offsetLenTemp = FixedPoint.zero;
                        SetPushVec(projA, projB, ref bAtLeftTemp, ref offsetLenTemp);

                        if (!bInit)
                        {
                            bAtLeft = bAtLeftTemp;
                            offsetLen = offsetLenTemp;
                            offsetVec = axis;
                            bInit = true;
                        }
                        else
                        {
                            // 如果不是第一次，那么就得获取最小的相交距离
                            if (offsetLenTemp < offsetLen)
                            {
                                bAtLeft = bAtLeftTemp;
                                offsetLen = offsetLenTemp;
                                offsetVec = axis;
                            }
                        }
                    }
                }
            }
            if (bPush)
            {
                Push(a, b, offsetLen, offsetVec, bAtLeft);
            }
            return true;
        }

        /// <summary>
        /// 法向量通过 x1x2+y1y2 = 0 可得：(-B,A) 或 (B,-A)。
        /// </summary>
        private Vector2d GetN(Vector2d vec)
        {
            if (vec.y == FixedPoint.zero)
            {
                return new Vector2d(0, 1);
            }
            return new Vector2d(-vec.y, vec.x);
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        private bool CheckLine(Vector2d a, Vector2d b)
        {
            if (a.y < b.x || a.x > b.y)
            {
                return false;
            }
            return true;
        }

        public void SetPushVec(Vector2d projA, Vector2d projB, ref bool bAtLeft, ref FixedPoint offsetLen)
        {
            if (projA.x == projB.x)
            {
                if (projA.y <= projB.y)
                {
                    bAtLeft = true;
                    offsetLen = projA.y - projA.x;
                }
                else
                {
                    bAtLeft = false;
                    offsetLen = projB.y - projA.x;
                }
            }
            else if (projA.x < projB.x)
            {
                bAtLeft = true;
                offsetLen = projA.y - projB.x;
            }
            else
            {
                bAtLeft = false;
                offsetLen = projB.y - projA.x;
            }
        }

        public void Push(Collider a, Collider b, FixedPoint offsetLen, Vector2d offsetVect, bool bAtLeft)
        {
            Vector2d offsetPos = offsetVect * offsetLen;
            //Debug.Log("offsetPos:" + offsetPos);
            if (m_rolePush)
            {
                if (!a.isObstacle && !b.isObstacle)
                {
                    if (a.notPush && !b.notPush)
                    {
                        if (bAtLeft)
                        {
                            b.AddOffsetPos(offsetPos, offsetVect);
                        }
                        else
                        {
                            b.AddOffsetPos(-offsetPos, offsetVect);
                        }
                    }
                    else if (b.notPush && !a.notPush)
                    {
                        if (bAtLeft)
                        {
                            a.AddOffsetPos(-offsetPos, offsetVect);
                        }
                        else
                        {
                            a.AddOffsetPos(offsetPos, offsetVect);
                        }
                    }
                    else if (!b.notPush && !a.notPush)
                    {
                        if (bAtLeft)
                        {
                            a.AddOffsetPos(-offsetPos * new FixedPoint(0.5f), -offsetVect);
                            b.AddOffsetPos(offsetPos * new FixedPoint(0.5f), offsetVect);
                        }
                        else
                        {
                            a.AddOffsetPos(offsetPos * new FixedPoint(0.5f), offsetVect);
                            b.AddOffsetPos(-offsetPos * new FixedPoint(0.5f), -offsetVect);
                        }
                    }
                }
            }

            // A是障碍
            if (a.isObstacle && !b.isObstacle)
            {
                if (bAtLeft)
                {
                    b.AddOffsetPos(offsetPos, offsetVect);
                }
                else
                {
                    b.AddOffsetPos(-offsetPos, offsetVect);
                }
            }
            else if (!a.isObstacle && b.isObstacle)
            {
                if (bAtLeft)
                {
                    a.AddOffsetPos(-offsetPos, offsetVect);
                }
                else
                {
                    a.AddOffsetPos(offsetPos, offsetVect);
                }
            }
        }
    }
}