using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{ 
    public struct Vector2d
    {
        public FixedPointF x;
        public FixedPointF y;

        public static readonly Vector2d zero = new Vector2d(0, 0);

        public Vector2d value
        {
            get
            {
                return new Vector2d(x.m_value, y.m_value);
            }
        }

        #region 构造函数
        public Vector2d(FixedPointF fx, FixedPointF fy)
        {
            x = fx;
            y = fy;
        }

        public Vector2d(int ix, int iy)
        {
            x = new FixedPointF(ix);
            y = new FixedPointF(iy);
        }

        public Vector2d(float fx, float fy)
        {
            x = new FixedPointF(fx);
            y = new FixedPointF(fy);
        }

        public Vector2d(Vector2 vec2)
        {
            x = new FixedPointF(vec2.x);
            y = new FixedPointF(vec2.y);
        }

        public Vector2d(Vector3 vec3)
        {
            x = new FixedPointF(vec3.x);
            y = new FixedPointF(vec3.z);
        }
        #endregion


        /// <summary>
        /// 向量长度
        /// </summary>
        public FixedPointF magnitude
        {
            get
            {
                FixedPointF temp = x * x + y * y;
                return FixedPointF.Sqrt(temp);
            }
        }

        /// <summary>
        /// 归一化
        /// </summary>
        public Vector2d normalized
        {
            get
            {
                FixedPointF len = magnitude;

                if (len == FixedPointF.zero)
                    return Vector2d.zero;

                if (len == FixedPointF.one)
                    return new Vector2d(x, y);

                FixedPointF tempX = x / len;
                FixedPointF tempY = y / len;
                return new Vector2d(tempX, tempY);
            }
        }

        public void Normalize()
        {
            FixedPointF len = magnitude;
            if (len == FixedPointF.zero)
                return;

            if (len == FixedPointF.one)
                return;

            x = x / len;
            y = y / len;
        }

        public static FixedPointF Distance2(Vector2d v1, Vector2d v2)
        {
            FixedPointF x = v1.x - v2.x;
            FixedPointF y = v1.y - v2.y;
            return x * x + y * y;
        }

        public static FixedPointF Distance(Vector2d v1, Vector2d v2)
        {
            return FixedPointF.Sqrt(Distance2(v1, v2));
        }

        public static FixedPointF Dot(Vector2d v1, Vector2d v2)
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static Vector2d operator +(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2d operator -(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2d operator *(Vector2d v1, FixedPointF mag)
        {
            return new Vector2d(v1.x * mag, v1.y * mag);
        }

        public static Vector2d operator *(Vector2d v1, int mag)
        {
            return new Vector2d(v1.x * mag, v1.y * mag);
        }

        public static Vector2d operator /(Vector2d v1, FixedPointF mag)
        {
            return new Vector2d(v1.x / mag, v1.y / mag);
        }

        public static Vector2d operator /(Vector2d v1, int mag)
        {
            return new Vector2d(v1.x / mag, v1.y / mag);
        }

        public static bool operator ==(Vector2d v1, Vector2d v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(Vector2d v1, Vector2d v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2d)
            {
                return (Vector2d)obj == this;
            }
            return false;
        }

        public FixedPointF GetLongHashCode()
        {
            return x * 31 + y * 7;
        }

        public int GetStateHash()
        {
            return (int)(GetLongHashCode().value % int.MaxValue);
        }

        public override int GetHashCode()
        {
            return this.GetStateHash();
        }

        public override string ToString()
        {
            return x.value + "," + y.value;
        }

        //public static Vector3d Cross(Vector3d a, Vector3d b)
        //{
        //    long x = FixedMath.Mul(a.y, b.z) - FixedMath.Mul(a.z, b.y);
        //    long y = FixedMath.Mul(a.z, b.x) - FixedMath.Mul(a.x, b.z);
        //    long z = FixedMath.Mul(a.x, b.y) - FixedMath.Mul(a.y, b.x);
        //    return new Vector3d(x, y, z);

        //    //return new Vector3((float)((double)a.y * (double)b.z - (double)a.z * (double)b.y),
        //    //    (float)((double)a.z * (double)b.x - (double)a.x * (double)b.z),
        //    //    (float)((double)a.x * (double)b.y - (double)a.y * (double)b.x));
        //}



    }
}
