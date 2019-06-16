using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma
{
    public struct FixedPointF
    {
        private const int SHIFT_AMOUNT = 8;
        private const long One = 1 << SHIFT_AMOUNT;

        public static FixedPointF zero = new FixedPointF(0);
        public static FixedPointF one = new FixedPointF(1);

        public long m_value;
        public float value
        {
            get
            {
                return (float)m_value / One;
            }
        }

        public FixedPointF(int i)
        {
            m_value = i * One;
        }

        public FixedPointF(float f)
        {
            m_value = (long)(f * One);
        }

        public FixedPointF(double d)
        {
            m_value = (long)(d * One);
        }

        /// <summary>
        /// 小数
        /// </summary>
        public FixedPointF(int t1, int t2)
        {
            m_value = ((long)t1 * One) / t2;
        }

        public static FixedPointF operator +(FixedPointF t1, FixedPointF t2)
        {
            FixedPointF temp;
            temp.m_value = t1.m_value + t2.m_value;
            return temp;
        }

        public static FixedPointF operator -(FixedPointF t1, FixedPointF t2)
        {
            FixedPointF temp;
            temp.m_value = t1.m_value - t2.m_value;
            return temp;
        }

        public static FixedPointF operator -(FixedPointF t)
        {
            t.m_value = -t.m_value;
            return t;
        }

        public static FixedPointF operator *(FixedPointF t1, FixedPointF t2)
        {
            FixedPointF temp;
            temp.m_value = (t1.m_value * t2.m_value) >> SHIFT_AMOUNT;
            return temp;
        }

        public static FixedPointF operator *(FixedPointF t1, int t2)
        {
            FixedPointF temp = t1 * new FixedPointF(t2);
            return temp;
        }

        public static FixedPointF operator /(FixedPointF t1, FixedPointF t2)
        {
            FixedPointF temp;
            temp.m_value = (t1.m_value << SHIFT_AMOUNT) / t2.m_value;
            return temp;
        }

        public static FixedPointF operator /(FixedPointF t1, int t2)
        {
            FixedPointF temp = t1 / new FixedPointF(t2);
            return temp;
        }

        public static bool operator ==(FixedPointF t1, FixedPointF t2)
        {
            return t1.m_value == t2.m_value;
        }

        public static bool operator !=(FixedPointF t1, FixedPointF t2)
        {
            return t1.m_value != t2.m_value;
        }

        public override bool Equals(object obj)
        {
            if(obj is FixedPointF)
            {
                FixedPointF f = (FixedPointF)obj;
                return m_value == f.m_value;
            }
            return false;
        }

        public static bool operator >(FixedPointF t1, FixedPointF t2)
        {
            return t1.m_value > t2.m_value;
        }

        public static bool operator <(FixedPointF t1, FixedPointF t2)
        {
            return t1.m_value < t2.m_value;
        }

        public static bool operator >=(FixedPointF t1, FixedPointF t2)
        {
            return t1.m_value >= t2.m_value;
        }

        public static bool operator <=(FixedPointF t1, FixedPointF t2)
        {
            return t1.m_value <= t2.m_value;
        }


        /// <summary>
        /// 求平方根
        /// </summary>
        static long n, n1;
        public static FixedPointF Sqrt(FixedPointF t)
        {
            //double v = (double)t.m_value / One;
            //double d = Math.Sqrt(v);
            //return new FixedPointF(d);
            long f1 = t.m_value;
            if (f1 == 0)
                return new FixedPointF(0);
            n = (f1 >> 1) + 1;
            n1 = (n + (f1 / n)) >> 1;
            while (n1 < n)
            {
                n = n1;
                n1 = (n + (f1 / n)) >> 1;
            }
            FixedPointF fp;
            fp.m_value = n << (SHIFT_AMOUNT / 2);
            return fp;
        }

        public static FixedPointF Abs(FixedPointF t)
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
