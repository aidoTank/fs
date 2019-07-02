using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{ 
    public struct Vector2d
    {
        public FixedPoint x;
        public FixedPoint y;

        public static readonly Vector2d zero = new Vector2d(0, 0);

        public Vector2d value
        {
            get
            {
                return new Vector2d(x.m_value, y.m_value);
            }
        }

        #region 构造函数
        public Vector2d(FixedPoint fx, FixedPoint fy)
        {
            x = fx;
            y = fy;
        }

        public Vector2d(int ix, int iy)
        {
            x = new FixedPoint(ix);
            y = new FixedPoint(iy);
        }

        public Vector2d(float fx, float fy)
        {
            x = new FixedPoint(fx);
            y = new FixedPoint(fy);
        }

        public Vector2d(Vector2 vec2)
        {
            x = new FixedPoint(vec2.x);
            y = new FixedPoint(vec2.y);
        }

        public Vector2d(Vector3 vec3)
        {
            x = new FixedPoint(vec3.x);
            y = new FixedPoint(vec3.z);
        }
        #endregion


        /// <summary>
        /// 向量长度
        /// </summary>
        public FixedPoint magnitude
        {
            get
            {
                FixedPoint temp = x * x + y * y;
                return FixedPoint.Sqrt(temp);
            }
        }

        /// <summary>
        /// 归一化
        /// </summary>
        public Vector2d normalized
        {
            get
            {
                FixedPoint len = magnitude;

                if (len == FixedPoint.zero)
                    return Vector2d.zero;

                if (len == FixedPoint.one)
                    return new Vector2d(x, y);

                FixedPoint tempX = x / len;
                FixedPoint tempY = y / len;
                return new Vector2d(tempX, tempY);
            }
        }

        public void Normalize()
        {
            FixedPoint len = magnitude;
            if (len == FixedPoint.zero)
                return;

            if (len == FixedPoint.one)
                return;

            x = x / len;
            y = y / len;
        }

        public static FixedPoint Distance2(Vector2d v1, Vector2d v2)
        {
            FixedPoint x = v1.x - v2.x;
            FixedPoint y = v1.y - v2.y;
            return x * x + y * y;
        }

        public static FixedPoint Distance(Vector2d v1, Vector2d v2)
        {
            return FixedPoint.Sqrt(Distance2(v1, v2));
        }

        public static Vector2d Lerp(Vector2d a, Vector2d b, FixedPoint t)
        {
            if(t >= FixedPoint.one)
            {
                return b;
            }
            else if(t <= FixedPoint.zero)
            {
                return a;
            }
            FixedPoint x = b.x * t + a.x * (FixedPoint.one - t);
            FixedPoint y = b.y * t + a.y * (FixedPoint.one - t);
            return new Vector2d(x, y);
        }

        public static FixedPoint Dot(Vector2d v1, Vector2d v2)
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

        public static Vector2d operator *(Vector2d v1, FixedPoint mag)
        {
            return new Vector2d(v1.x * mag, v1.y * mag);
        }

        public static Vector2d operator *(Vector2d v1, int mag)
        {
            return new Vector2d(v1.x * mag, v1.y * mag);
        }

        public static Vector2d operator /(Vector2d v1, FixedPoint mag)
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

        public static Vector2d operator -(Vector2d a)
        {
            return a * -1;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2d)
            {
                return (Vector2d)obj == this;
            }
            return false;
        }

        public FixedPoint GetLongHashCode()
        {
            return x * 31 + y * 7;
        }

        public int GetStateHash()
        {
            return (int)(GetLongHashCode().value % int.MaxValue);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + x.value + ", " + y.value + ")";
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
