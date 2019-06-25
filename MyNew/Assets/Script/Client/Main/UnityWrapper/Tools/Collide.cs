

using System;
using UnityEngine;

namespace Roma
{
    public struct Sphere
    {
        public Vector2 c;
        public float r;
    }

    public struct Sector
    {
        public Vector2 pos;
        public Vector2 dir;
        public float angle;
        public float r;
    }

    public struct Line
    {
        public Vector2 startPos;
        public Vector2 dir;
        public float length;
    }

    public struct OBB
    {
        public Vector2 center;
        public Vector2[] u; // 表示世界坐标中x,y的轴方向向量
        public Vector2 e;                    // 矩形长宽的一半

        public OBB(Vector2 pos, Vector2 size, float angle)
        {
            center = pos;
            e = size * 0.5f;

            u = new Vector2[2];
            float rad = angle * Mathf.Deg2Rad;
            u[0] = new Vector2(Mathf.Cos(rad), -Mathf.Sin(rad));      // X
            u[1] = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));       // Y
        }

        public Vector2 GetDir()
        {
            return u[0];
        }
    }

    public static class Collide
    {

        public static float GetDis2(Vector2 v1, Vector2 v2)
        {
            Vector2 dV = v1 - v2;
            return Vector2.Dot(dV, dV);
        }

        public static void GetDis2(ref float dis2, ref Vector2 v1, ref Vector2 v2)
        {
            Vector2 dV = v1 - v2;
            dis2 = Vector2.Dot(dV, dV);
        }

        /// <summary>
        /// 角度转向量
        /// 直角坐标系 sin@=y/r
        /// 2D环境，y为z，且向上
        /// </summary>
        public static Vector2 GetVector(float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
        }

        /// <summary>
        /// 向量转角度
        /// 直角坐标系 tan@=y/x
        /// 2D环境，y如3D中的z轴，(0,1)表示2D中的上方，角度为0
        /// </summary>
        public static float GetAngle(Vector2 vec)
        {
            float rad = Mathf.Atan2(vec.x, vec.y);
            return rad * Mathf.Rad2Deg;
        }

        public static Vector2 Rotate(Vector2 sVec, float angle)
        {
            Vector2 last;
            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            last.x = sVec.x * cos + sVec.y * sin;
            last.y = sVec.x * -sin + sVec.y * cos;
            return last;
        }

        /// <summary>
        /// 圆圆相交
        /// </summary>
        public static bool bSphereSphere(Sphere a, Sphere b)
        {
            Vector2 d = a.c - b.c;
            float dist2 = Vector2.Dot(d, d);
            float rSum = a.r + b.r;
            return dist2 <= rSum * rSum;
        }

        /// <summary>
        /// 圆与OBB相交，并取最近点
        /// </summary>
        public static bool bSphereOBB(Sphere s, OBB b, ref Vector2 p)
        {
            p = ClosestPointOBB(s.c, b);
            Vector2 v = p - s.c;
            return Vector2.Dot(v, v) <= s.r * s.r;
        }

        /// <summary>
        /// 圆与OBB相交
        /// </summary>
        public static bool bSphereOBB(Sphere s, OBB b)
        {
            // p为交点
            Vector2 p = ClosestPointOBB(s.c, b);
            Vector2 v = p - s.c;
            return Vector2.Dot(v, v) <= s.r * s.r;
        }

        /// <summary>
        /// 点到OBB最近点
        /// </summary>
        public static Vector2 ClosestPointOBB(Vector2 p, OBB b)
        {
            Vector2 d = p - b.center;
            Vector2 q = b.center;
            float distX = Vector2.Dot(d, b.u[0]);       // dist为x轴中p到中心点的距离，由x=(P-C)·Ux得来
            if (distX > b.e.x) distX = b.e.x;           // 距离超过边界时，为OBB范围的一半
            if (distX < -b.e.x) distX = -b.e.x;
            q += distX * b.u[0];

            float distY = Vector2.Dot(d, b.u[1]);
            if (distY > b.e.y) distY = b.e.y;
            if (distY < -b.e.y) distY = -b.e.y;
            q += distY * b.u[1];
            return q;
        }

        /// <summary>
        /// 点是否在扇形内部
        /// </summary>
        /// <param name="origin">扇形位置</param>
        /// <param name="sectorDir">扇形方向</param>
        /// <param name="sectorAngleSize">扇形角度</param>
        /// <param name="focusPos">焦点</param>
        public static bool bSectorInside(Vector2 origin, Vector2 sectorDir, float sectorAngleSize, float r, Vector2 focusPos)
        {
            // 右分量
            Vector2 rightVec = Rotate(sectorDir, sectorAngleSize * 0.5f);
            // 焦点与扇形的方向
            Vector2 focusDir = focusPos - origin;
            // 扇形方向与右分量点乘值
            float rightDot = Vector2.Dot(sectorDir.normalized, rightVec.normalized);
            // 扇形方向与焦点方向的点乘值
            float focusDot = Vector2.Dot(sectorDir.normalized, focusDir.normalized);
            if (focusDot >= rightDot && Vector2.Dot(focusDir, focusDir) < r * r)
                return true;
            return false;
        }

        /// <summary>
        /// 点是否在扇形内部
        public static bool bSectorInside(Sector sector, Vector2 focusPos)
        {
            // 右分量
            Vector2 rightVec = Rotate(sector.dir, sector.angle * 0.5f);
            // 焦点与扇形的方向
            Vector2 focusDir = focusPos - sector.pos;
            // 扇形方向与右分量点乘值
            float rightDot = Vector2.Dot(sector.dir.normalized, rightVec.normalized);
            // 扇形方向与焦点方向的点乘值
            float focusDot = Vector2.Dot(sector.dir.normalized, focusDir.normalized);
            if (focusDot >= rightDot && Vector2.Dot(focusDir, focusDir) < sector.r * sector.r)
                return true;
            return false;
        }

        /// <summary>
        /// 射线与圆相交
        /// </summary>
        public static bool LineSphere(Sphere s, Line line)
        {
            Vector2 m = line.startPos - s.c;        // 圆心到射线起点的向量
            float b = Vector2.Dot(m, line.dir);     // b大于0，说明射线方向背向圆心
            float c = Vector2.Dot(m, m) - s.r * s.r;
            if (c > 0f && b > 0f)                   // 如果射线起点在圆外，并且方向与到圆方向相反，则不相交
                return false;
            float discr = b * b - c;                // 判别式小于0，无实根
            if (discr < 0f)
                return false;
            float t = -b - Mathf.Sqrt(discr);       // -b-sqrt(b*b-c)表示从射线起点比较近的相交点对于的长度

            if (t < line.length)                    // 对比相交点的长度和线段长度
            {
                return true;
            }
            return false;
        }

        public static bool SphereSector(Sphere sphere, Sector sector)
        {
            // 圆和扇形的圆不相交时
            Vector2 d = sphere.c - sector.pos;
            float dist2 = Vector2.Dot(d, d);
            float rSum = sphere.r + sector.r;
            if (dist2 > rSum * rSum)
            {
                return false;
            }
            sector.dir.Normalize();

            // 圆心在扇形两边的夹角内
            Vector2 leftVec = Rotate(sector.dir, sector.angle * -0.5f);
            Vector2 rightVec = Rotate(sector.dir, sector.angle * 0.5f);
            // 圆心点与扇形的方向
            Vector2 focusDir = sphere.c - sector.pos;
            float rightDot = Vector2.Dot(sector.dir, rightVec.normalized);
            // 扇形方向与焦点方向的点乘值
            float focusDot = Vector2.Dot(sector.dir, focusDir.normalized);
            if (rightDot <= focusDot)
            {
                Debug.Log("圆心在扇形角度内");
                return true;
            }

            // 圆心在扇形内时，相交
            if (bSectorInside(sector, sphere.c))
            {
                Debug.Log("圆心在扇形内时，相交");
                return true;
            }

            // 圆与两边相交时
            Line l1;
            l1.startPos = sector.pos;
            l1.dir = leftVec;
            l1.length = sector.r;
            if (LineSphere(sphere, l1))
            {
                Debug.Log("左边线相交");
                return true;
            }
            Line l2;
            l2.startPos = sector.pos;
            l2.dir = rightVec;
            l2.length = sector.r;
            if (LineSphere(sphere, l2))
            {
                Debug.Log("右边线相交");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取2维向量的垂直向量
        /// </summary>
        public static Vector2 GetVerticalVector(Vector3 n)
        {
            Vector3 vertical3 = Vector3.Cross(Vector3.up, new Vector3(n.x, 0, n.y)).normalized;
            return new Vector2(vertical3.x, vertical3.z);
        }
    }

}