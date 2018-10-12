

using System;
using UnityEngine;

namespace Roma
{
    public class Sphere
    {
        public Vector2 c;
        public float r;
    }

    public class OBB
    {
        public Vector2 center;
        public Vector2[] u = new Vector2[2]; // 本地x,y的轴旋转矩阵
        public Vector2 e;                    // 矩形长宽的一半

        public OBB(Vector2 pos, Vector2 size, float angle)
        {
            center = pos;
            e = size * 0.5f;
            SetAngle(angle);
            // 模拟表现层
            // GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load<Object>("Cube"));
            // obj.transform.position = new Vector3(pos.x, 0, pos.y);
            // obj.transform.eulerAngles = new Vector3(0, angle, 0);
            // obj.transform.localScale = new Vector3(size.x, 1, size.y);
        }

        public void SetAngle(float angle)
        {
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

        public static Vector2 GetVector(float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
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
        public static bool bOBBInside(Sphere s, OBB b, ref Vector2 p)
        {
            p = ClosestPointOBB(s.c, b);
            Vector2 v = p - s.c;
            return Vector2.Dot(v, v) <= s.r * s.r;
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
            Vector2 p = ClosestPointOBB(s.c, b);
            Vector2 v = p - s.c;
            return Vector2.Dot(v, v) <= s.r * s.r;
        }

        
        /// <summary>
        /// 点到OBB最近点
        /// </summary>
        private static Vector2 ClosestPointOBB(Vector2 p, OBB b)
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
            Vector2 rightVec = VecRotationMatrix(sectorDir, sectorAngleSize * 0.5f);
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

        public static Vector2 VecRotationMatrix(Vector2 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            float newX = v.x * cos + v.y * sin;
            float newY = v.x * -sin + v.y * cos;
            return new Vector2((float)newX, (float)newY);
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