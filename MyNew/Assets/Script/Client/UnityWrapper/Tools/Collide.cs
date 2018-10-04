

using System;
using UnityEngine;

namespace Roma
{
    public static class Collide
    {

        /// <summary>
        /// 点和圆是否相交
        /// </summary>
        public static bool bSphereInside(Vector2 center, float r, Vector2 focusPos)
        {
            Vector2 d = center - focusPos;
            float dist2 = Vector2.Dot(d, d); // 点积的妙用，同一个向量时，cos0=1,结果|a||b|为长度的平方
            return dist2 <= r * r;
        }

        /// <summary>
        /// 是否在扇形内部
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

        private static Vector2 VecRotationMatrix(Vector2 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            float newX = v.x * cos + v.y * sin;
            float newY = v.x * -sin + v.y * cos;
            return new Vector2((float)newX, (float)newY);
        }
    }

}