using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma
{
    /// <summary>
    /// 实际使用时，都用FixedPoint
    /// </summary>
    public struct FixedPoint
    {
        private const int SHIFT_AMOUNT = 8;
        private const long One = 1 << SHIFT_AMOUNT;

        public static FixedPoint Pi = new FixedPoint(3.14159265f);
        public static FixedPoint TwoPi = Pi * 2;
        public static FixedPoint HalfPi = Pi / 2;

        public static FixedPoint Deg2Rad = new FixedPoint(0.0174532924F);
        public static FixedPoint Rad2Deg = new FixedPoint(57.29578F);

        public static FixedPoint zero = new FixedPoint(0);
        public static FixedPoint one = new FixedPoint(1);

        public static FixedPoint N_0 = new FixedPoint(0);
        public static FixedPoint N_1 = new FixedPoint(1);

        public long m_value;
        public float value
        {
            get
            {
                return (float)m_value / One;
            }
        }

        public FixedPoint(int i)
        {
            m_value = i * One;
        }

        public FixedPoint(float f)
        {
            m_value = (long)(f * One);
        }

        public FixedPoint(double d)
        {
            m_value = (long)(d * One);
        }

        /// <summary>
        /// 小数
        /// </summary>
        public FixedPoint(int t1, int t2)
        {
            m_value = ((long)t1 * One) / t2;
        }

        public static FixedPoint operator +(FixedPoint t1, FixedPoint t2)
        {
            FixedPoint temp;
            temp.m_value = t1.m_value + t2.m_value;
            return temp;
        }

        public static FixedPoint operator -(FixedPoint t1, FixedPoint t2)
        {
            FixedPoint temp;
            temp.m_value = t1.m_value - t2.m_value;
            return temp;
        }

        public static FixedPoint operator -(FixedPoint t)
        {
            t.m_value = -t.m_value;
            return t;
        }

        public static FixedPoint operator *(FixedPoint t1, FixedPoint t2)
        {
            FixedPoint temp;
            temp.m_value = (t1.m_value * t2.m_value) >> SHIFT_AMOUNT;
            return temp;
        }

        public static FixedPoint operator /(FixedPoint t1, FixedPoint t2)
        {
            FixedPoint temp;
            temp.m_value = (t1.m_value << SHIFT_AMOUNT) / t2.m_value;
            return temp;
        }

        public static bool operator ==(FixedPoint t1, FixedPoint t2)
        {
            return t1.m_value == t2.m_value;
        }

        public static bool operator !=(FixedPoint t1, FixedPoint t2)
        {
            return t1.m_value != t2.m_value;
        }

        public static bool operator >(FixedPoint t1, FixedPoint t2)
        {
            return t1.m_value > t2.m_value;
        }

        public static bool operator <(FixedPoint t1, FixedPoint t2)
        {
            return t1.m_value < t2.m_value;
        }

        public static bool operator >=(FixedPoint t1, FixedPoint t2)
        {
            return t1.m_value >= t2.m_value;
        }

        public static bool operator <=(FixedPoint t1, FixedPoint t2)
        {
            return t1.m_value <= t2.m_value;
        }

        public override bool Equals(object obj)
        {
            if (obj is FixedPoint)
            {
                FixedPoint f = (FixedPoint)obj;
                return m_value == f.m_value;
            }
            return false;
        }

        public static FixedPoint operator +(FixedPoint t1, float t2)
        {
            FixedPoint temp;
            temp.m_value = (t1.m_value + (new FixedPoint(t2).m_value));
            return temp;
        }

        public static FixedPoint operator -(FixedPoint t1, float t2)
        {
            FixedPoint temp;
            temp.m_value = t1.m_value - (new FixedPoint(t2)).m_value;
            return temp;
        }

        public static FixedPoint operator -(float t1, FixedPoint t2)
        {
            FixedPoint temp;
            temp.m_value =(new FixedPoint(t1)).m_value - t2.m_value;
            return temp;
        }

        public static FixedPoint operator *(FixedPoint t1, int t2)
        {
            FixedPoint temp = t1 * new FixedPoint(t2);
            return temp;
        }

        public static FixedPoint operator *(int t1, FixedPoint t2)
        {
            FixedPoint temp = t2 * new FixedPoint(t1);
            return temp;
        }

        public static FixedPoint operator /(FixedPoint t1, int t2)
        {
            FixedPoint temp = t1 / new FixedPoint(t2);
            return temp;
        }

        public static FixedPoint operator /(int t1, FixedPoint t2)
        {
            FixedPoint temp = new FixedPoint(t1) / t2;
            return temp;
        }

        public static bool operator >(FixedPoint t1, int t2)
        {
            return t1.value > t2;
        }

        public static bool operator <(FixedPoint t1, int t2)
        {
            return t1.value < t2;
        }

        public static bool operator >=(FixedPoint t1, int t2)
        {
            return t1 >= new FixedPoint(t2);
        }

        public static bool operator <=(FixedPoint t1, int t2)
        {
            return t1 <= new FixedPoint(t2);
        }

        public static explicit operator int(FixedPoint v)
        {
            return (int)v.value;
        }

        public static explicit operator float(FixedPoint v)
        {
            return v.value;
        }

        public static explicit operator FixedPoint(float v)
        {
            return new FixedPoint(v);
        }

        /// <summary>
        /// 求平方根
        /// </summary>
        static long n, n1;
        public static FixedPoint Sqrt(FixedPoint t)
        {
            //double v = (double)t.m_value / One;
            //double d = Math.Sqrt(v);
            //return new FixedPoint(d);
            long f1 = t.m_value;
            if (f1 == 0)
                return new FixedPoint(0);
            n = (f1 >> 1) + 1;
            n1 = (n + (f1 / n)) >> 1;
            while (n1 < n)
            {
                n = n1;
                n1 = (n + (f1 / n)) >> 1;
            }
            FixedPoint fp;
            fp.m_value = n << (SHIFT_AMOUNT / 2);
            return fp;
        }

        public static FixedPoint Abs(FixedPoint t)
        {
            t.m_value = t.m_value < 0 ? -t.m_value : t.m_value;
            return t;
        }


        public override string ToString()
        {
            return value.ToString();
        }

        public override int GetHashCode()
        {
            return 0;
        }

    }
}
