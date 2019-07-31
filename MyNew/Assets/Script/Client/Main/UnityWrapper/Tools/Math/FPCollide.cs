using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    public struct FPSphere
    {
        public Vector2d c;
        public FixedPoint r;
    }

    public struct FPSector
    {
        public Vector2d pos;
        public Vector2d dir;
        public FixedPoint angle;
        public FixedPoint r;
    }

    public struct FPLine
    {
        public Vector2d startPos;
        public Vector2d dir;
        public FixedPoint length;
    }

    public struct FPObb
    {
        public Vector2d center;
        public Vector2d[] u; // 表示世界坐标中x,y的轴方向向量
        public Vector2d e;   // 矩形长宽的一半

        private int ang;

        public FPObb(Vector2d pos, Vector2d size, int angle)
        {
            center = pos;
            e = size * new FixedPoint(0.5f);
            ang = angle;
            /// <summary>
            /// 2D环境中的角度，转为世界坐标中的xy方向的向量
            /// </summary>
            u = new Vector2d[2];
            u[0] = new Vector2d(CustomMath.Cos(angle), -CustomMath.Sin(angle));
            u[1] = new Vector2d(CustomMath.Sin(angle), CustomMath.Cos(angle));
        }

        public Vector2d GetDir()
        {
            return u[0];
        }

        public Vector2d[] GetVert2d()
        {
            // 先求矩形坐标系中四个点位置
            Vector2d v1 = new Vector2d(-e.x, e.y);
            Vector2d v2 = new Vector2d(e.x, e.y);
            Vector2d v3 = new Vector2d(e.x, -e.y);
            Vector2d v4 = new Vector2d(-e.x, -e.y);

            v1 = center + FPCollide.Rotate(v1, ang);
            v2 = center + FPCollide.Rotate(v2, ang);
            v3 = center + FPCollide.Rotate(v3, ang);
            v4 = center + FPCollide.Rotate(v4, ang);

            Vector2d[] list = new Vector2d[4];
            list[0] = v1;
            list[1] = v2;
            list[2] = v3;
            list[3] = v4;
            return list;
        }
    }

    public class FPCollide
    {
        public static FixedPoint GetDis2(Vector2d v1, Vector2d v2)
        {
            Vector2d dV = v1 - v2;
            return Vector2d.Dot(dV, dV);
        }

        public static void GetDis2(ref FixedPoint dis2, ref Vector2d v1, ref Vector2d v2)
        {
            Vector2d dV = v1 - v2;
            dis2 = Vector2d.Dot(dV, dV);
        }

        public static Vector2d GetVector(int angle)
        {
            return new Vector2d(CustomMath.Sin(angle), CustomMath.Cos(angle));
        }

        public static FixedPoint GetAngle(Vector2d vec)
        {
            return CustomMath.Atan2((int)(vec.x.value * 10), (int)(vec.y.value * 10));
        }

        public static Vector2d Rotate(Vector2d vec, int angle)
        {
            Vector2d last;
            FixedPoint cos = CustomMath.Cos(angle);
            FixedPoint sin = CustomMath.Sin(angle);
            last.x = vec.x * cos + vec.y * sin;
            last.y = vec.x * -sin + vec.y * cos;
            return last;
        }

        public static bool bSphereSphere(FPSphere a, FPSphere b)
        {
            Vector2d d = a.c - b.c;
            FixedPoint dis2 = Vector2d.Dot(d, d);
            FixedPoint rSum = a.r + b.r;
            return dis2 <= rSum * rSum;
        }

        /// <summary>
        /// 圆与OBB相交
        /// </summary>
        public static bool bSphereOBB(FPSphere s, FPObb b)
        {
            // p为交点
            Vector2d p = ClosestPointOBB(s.c, b);
            Vector2d v = p - s.c;
            return Vector2d.Dot(v, v) <= s.r * s.r;
        }

        /// <summary>
        /// 点到OBB最近点
        /// </summary>
        public static Vector2d ClosestPointOBB(Vector2d p, FPObb b)
        {
            Vector2d d = p - b.center;
            Vector2d q = b.center;
            FixedPoint distX = Vector2d.Dot(d, b.u[0]);
            if (distX > b.e.x) distX = b.e.x;
            if (distX < -b.e.x) distX = -b.e.x;
            q += b.u[0] * distX;

            FixedPoint distY = Vector2d.Dot(d, b.u[1]);
            if (distY > b.e.y) distY = b.e.y;
            if (distY < -b.e.y) distY = -b.e.y;
            q += b.u[1] * distY;
            return q;
        }

        /// <summary>
        /// 点是否在扇形内部
        /// </summary>
        public static bool bSectorInside(FPSector sector, Vector2d focusPos)
        {
            // 右分量
            Vector2d rightVec = Rotate(sector.dir, (int)(sector.angle * new FixedPoint(0.5f)).value);
            // 焦点与扇形的方向
            Vector2d focusDir = focusPos - sector.pos;
            // 扇形方向与右分量点乘值
            FixedPoint rightDot = Vector2d.Dot(sector.dir.normalized, rightVec.normalized);
            // 扇形方向与焦点方向的点乘值
            FixedPoint focusDot = Vector2d.Dot(sector.dir.normalized, focusDir.normalized);
            if (focusDot >= rightDot && Vector2d.Dot(focusDir, focusDir) < sector.r * sector.r)
                return true;
            return false;
        }

    }
}

