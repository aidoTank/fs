using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace Roma
{
    public  class MathEx
	{
        public static int GetLength(string str)
        {
            if (str == null || str.Length == 0) { return 0; }

            int l = str.Length;
            int realLen = l;

            #region 计算长度
            int clen = 0;//当前长度
            while (clen < l)
            {
                //每遇到一个中文，则将实际长度加一。
                if ((int)str[clen] > 128) { realLen++; }
                clen++;
            }
            #endregion

            return realLen;
        }

        public static string GetSize(int size)
        {
            if(size / 1024.0f / 1024.0f >= 0.5f)
            {
                return (size / 1024.0f / 1024.0f).ToString("f1") + "MB";
            }
            else
            {
                return (size / 1024.0f).ToString("f1") + "KB";
            }
        }

        public static Vector3 GetVector3(string pos)
        {
            string[] posList = pos.Split('_');
            if (posList.Length != 3)
            {
                Debug.LogError("位置解析错误。"+ pos);
                return Vector3.zero;
            }
            return new Vector3(float.Parse(posList[0]), float.Parse(posList[1]), float.Parse(posList[2]));
        }

        public static string GetVector3Str(Vector3 pos)
        {
            return pos.x + "_" + pos.y + "_" + pos.z;
        }

        public static Color GetColor(string pos)
        {
            string[] colorList = pos.Split('_');
            if (colorList.Length != 3)
            {
                Debug.LogError("颜色解析错误。");
                return Color.white;
            }
            return new Color(float.Parse(colorList[0]), float.Parse(colorList[1]), float.Parse(colorList[2]));
        }

        /** Returns a hash value for a integer vector. Code got from the internet */
        public static int ComputeVertexHash(int x, int y, int z)
        {
            uint h1 = 0x8da6b343; // Large multiplicative constants;
            uint h2 = 0xd8163841; // here arbitrarily chosen primes
            uint h3 = 0xcb1ab31f;
            uint n = (uint)(h1 * x + h2 * y + h3 * z);

            return (int)(n & ((1 << 30) - 1));
        }

        /** Returns the closest point on the line. The line is treated as infinite.
         * \see NearestPointStrict
         */
        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);

            float closestPoint = Vector3.Dot((point - lineStart), lineDirection); //Vector3.Dot(lineDirection,lineDirection);
            return lineStart + (closestPoint * lineDirection);
        }

        public static float NearestPointFactor(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = lineEnd - lineStart;
            float magn = lineDirection.magnitude;
            lineDirection /= magn;

            float closestPoint = Vector3.Dot((point - lineStart), lineDirection); //Vector3.Dot(lineDirection,lineDirection);
            return closestPoint / magn;
        }

        /** Returns the closest point on the line segment. The line is NOT treated as infinite.
         * \see NearestPoint
         */
        public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 fullDirection = lineEnd - lineStart;
            Vector3 lineDirection = Vector3.Normalize(fullDirection);

            float closestPoint = Vector3.Dot((point - lineStart), lineDirection); //WASTE OF CPU POWER - This is always ONE -- Vector3.Dot(lineDirection,lineDirection);
            return lineStart + (Mathf.Clamp(closestPoint, 0.0f, fullDirection.magnitude) * lineDirection);
        }

        /** Returns the closest point on the line segment on the XZ plane. The line is NOT treated as infinite.
         * \see NearestPoint
         */
        public static Vector3 NearestPointStrictXZ(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            lineStart.y = point.y;
            lineEnd.y = point.y;
            Vector3 fullDirection = lineEnd - lineStart;
            Vector3 fullDirection2 = fullDirection;
            fullDirection2.y = 0;
            Vector3 lineDirection = Vector3.Normalize(fullDirection2);
            //lineDirection.y = 0;

            float closestPoint = Vector3.Dot((point - lineStart), lineDirection); //WASTE OF CPU POWER - This is always ONE -- Vector3.Dot(lineDirection,lineDirection);
            return lineStart + (Mathf.Clamp(closestPoint, 0.0f, fullDirection2.magnitude) * lineDirection);
        }

        /** Returns the approximate shortest distance between x,z and the line p-q. The line is considered infinite. This function is not entirely exact, but it is about twice as fast as DistancePointSegment2. */
        public static float DistancePointSegment(int x, int z, int px, int pz, int qx, int qz)
        {

            float pqx = (float)(qx - px);
            float pqz = (float)(qz - pz);
            float dx = (float)(x - px);
            float dz = (float)(z - pz);
            float d = pqx * pqx + pqz * pqz;
            float t = pqx * dx + pqz * dz;
            if (d > 0)
                t /= d;
            if (t < 0)
                t = 0;
            else if (t > 1)
                t = 1;

            dx = px + t * pqx - x;
            dz = pz + t * pqz - z;

            return dx * dx + dz * dz;
        }

        /*public static float DistancePointSegment2 (int x,int z, int px, int pz, int qx, int qz) {
	    	
            Vector3 p = new Vector3 (x,0,z);
	    	
            Vector3 p1 = new Vector3 (px,0,pz);
            Vector3 p2 = new Vector3 (qx,0,qz);
	    	
            Vector3 nearest = NearestPoint (p1,p2,p);
			
            return (nearest-p).sqrMagnitude;
        }*/

        /** Returns the distance between x,z and the line p-q. The line is considered infinite. */
        public static float DistancePointSegment2(int x, int z, int px, int pz, int qx, int qz)
        {

            Vector3 p = new Vector3(x, 0, z);

            Vector3 p1 = new Vector3(px, 0, pz);
            Vector3 p2 = new Vector3(qx, 0, qz);

            return DistancePointSegment2(p1, p2, p);
        }

        /** Returns the distance between c and the line a-b. The line is considered infinite. */
        public static float DistancePointSegment2(Vector3 a, Vector3 b, Vector3 p)
        {

            float bax = b.x - a.x;
            float baz = b.z - a.z;

            float area = Mathf.Abs(bax * (p.z - a.z) - (p.x - a.x) * baz);

            float d = bax * bax + baz * baz;

            if (d > 0)
            {
                return area / Mathf.Sqrt(d);
            }

            return (a - p).magnitude;
        }

        /** Returns the squared distance between c and the line a-b. The line is not considered infinite. */
        public static float DistancePointSegmentStrict(Vector3 a, Vector3 b, Vector3 p)
        {

            Vector3 nearest = NearestPointStrict(a, b, p);
            return (nearest - p).sqrMagnitude;
        }

        /** Maps a value between startMin and startMax to be between 0 and 1 */
        public static float MapTo(float startMin, float startMax, float value)
        {
            value -= startMin;
            value /= (startMax - startMin);
            value = Mathf.Clamp01(value);
            return value;
        }

        /** Maps a value (0...1) to be between targetMin and targetMax */
        public static float MapToRange(float targetMin, float targetMax, float value)
        {
            value *= (targetMax - targetMin);
            value += targetMin;
            return value;
        }

        /** Maps a value between startMin and startMax to be between targetMin and targetMax */
        public static float MapTo(float startMin, float startMax, float targetMin, float targetMax, float value)
        {
            value -= startMin;
            value /= (startMax - startMin);
            value = Mathf.Clamp01(value);
            value *= (targetMax - targetMin);
            value += targetMin;
            return value;
        }

        /** Returns a nicely formatted string for the number of bytes (kB, MB, GB etc). Uses decimal values, not binary ones
          * \see FormatBytesBinary */
        public static string FormatBytes(int bytes)
        {
            double sign = bytes >= 0 ? 1D : -1D;
            bytes = bytes >= 0 ? bytes : -bytes;

            if (bytes < 1000)
            {
                return (bytes * sign).ToString() + " bytes";
            }
            else if (bytes < 1000000)
            {
                return ((bytes / 1000D) * sign).ToString("0.0") + " kb";
            }
            else if (bytes < 1000000000)
            {
                return ((bytes / 1000000D) * sign).ToString("0.0") + " mb";
            }
            else
            {
                return ((bytes / 1000000000D) * sign).ToString("0.0") + " gb";
            }
        }

        /** Returns a nicely formatted string for the number of bytes (KiB, MiB, GiB etc). Uses decimal names (KB, Mb - 1000) but calculates using binary values (KiB, MiB - 1024) */
        public static string FormatBytesBinary(int bytes)
        {
            double sign = bytes >= 0 ? 1D : -1D;
            bytes = bytes >= 0 ? bytes : -bytes;

            if (bytes < 1024)
            {
                return (bytes * sign).ToString() + " bytes";
            }
            else if (bytes < 1024)
            {
                return ((bytes / 1024D) * sign).ToString("0.0") + " kb";
            }
            else if (bytes < 1000000000)
            {
                return ((bytes / (1024D * 1024D)) * sign).ToString("0.0") + " mb";
            }
            else
            {
                return ((bytes / (1024D * 1024D * 1024D)) * sign).ToString("0.0") + " gb";
            }
        }

        /** Returns bit number \a b from int \a a. The bit number is zero based. Relevant \a b values are from 0 to 31\n
          * Equals to (a >> b) & 1 */
        public static int Bit(int a, int b)
        {
            return (a >> b) & 1;
            //return (a & (1 << b)) >> b; //Original code, one extra shift operation required
        }

        /** Returns a nice color from int \a i with alpha \a a. Got code from the open-source Recast project, works really good\n
          * Seems like there are only 64 possible colors from studying the code*/
        public static Color IntToColor(int i, float a)
        {
            int r = Bit(i, 1) + Bit(i, 3) * 2 + 1;
            int g = Bit(i, 2) + Bit(i, 4) * 2 + 1;
            int b = Bit(i, 0) + Bit(i, 5) * 2 + 1;
            return new Color(r * 0.25F, g * 0.25F, b * 0.25F, a);
        }

        /** Distance between two points on the XZ plane */
        public static float MagnitudeXZ(Vector3 a, Vector3 b)
        {
            Vector3 delta = a - b;
            return (float)Mathf.Sqrt(delta.x * delta.x + delta.z * delta.z);
        }

        /** Squared distance between two points on the XZ plane */
        public static float SqrMagnitudeXZ(Vector3 a, Vector3 b)
        {
            Vector3 delta = a - b;
            return delta.x * delta.x + delta.z * delta.z;
        }

        public static int Repeat(int i, int n)
        {
            while (i >= n)
            {
                i -= n;
            }
            return i;
        }

        public static float Abs(float a)
        {
            if (a < 0)
            {
                return -a;
            }
            return a;
        }

        public static int Abs(int a)
        {
            if (a < 0)
            {
                return -a;
            }
            return a;
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static uint Min(uint a, uint b)
        {
            return a < b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static uint Max(uint a, uint b)
        {
            return a > b ? a : b;
        }

        public static ushort Max(ushort a, ushort b)
        {
            return a > b ? a : b;
        }

        public static float Sign(float a)
        {
            return a < 0 ? -1F : 1F;
        }

        public static int Sign(int a)
        {
            return a < 0 ? -1 : 1;
        }

        public static float Clamp(float a, float b, float c)
        {
            return a > c ? c : a < b ? b : a;
        }

        public static int Clamp(int a, int b, int c)
        {
            return a > c ? c : a < b ? b : a;
        }

        public static float Clamp01(float a)
        {
            return a > 1 ? 1 : a < 0 ? 0 : a;
        }

        public static int Clamp01(int a)
        {
            return a > 1 ? 1 : a < 0 ? 0 : a;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * (t > 1 ? 1 : t < 0 ? 0 : t);
        }

        public static int RoundToInt(float v)
        {
            return (int)(v + 0.5F);
        }

        public static int RoundToInt(double v)
        {
            return (int)(v + 0.5D);
        }

        public static float FastDistance(float x, float y, float z, float x1, float y1, float z1)
        {
            float fx = x - x1;
            float fy = y - y1;
            float fz = z - z1;
            return (fx * fx) + (fy * fy) + (fz * fz);
        }

		//返回平方这样不用开方块的多
    	public static float FastDistance(ref Vector3 vP1, ref Vector3 vP2)  
		{
			float fx = vP1.x - vP2.x;
			float fy = vP1.y - vP2.y;
			float fz = vP1.z - vP2.z;
			return (fx * fx) + (fy * fy) + (fz * fz);
		}

        public static float FastDistance(float x, float y, float x1 , float y1)
        {
            float fx = x - x1;
            float fy = y - y1;
            return (fx * fx) + (fy * fy);
        }
		
		public static float FastDistance(ref Vector2 vP1, ref Vector2 vP2)  
		{
			float fx = vP1.x - vP2.x;
			float fy = vP1.y - vP2.y;
			return (fx * fx) + (fy * fy);
		}

        public static void InitAllQuat()
        {
            if (ms_AllAngleQuaternion.Count == 0)
            {
                for (int i = -361; i < 361; i++)
                {
                    ms_AllAngleQuaternion.Add(i, Quaternion.AngleAxis(i, Vector3.up));
                }
            }
        }


		/** Area of a triangle  This will be negative for clockwise triangles and positive for counter-clockwise ones */
		//public static long TriangleArea2 (Int3 a, Int3 b, Int3 c) {
		//	return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z);
		//	//a.x*b.z+b.x*c.z+c.x*a.z-a.x*c.z-c.x*b.z-b.x*a.z;
		//}
		
		public static float TriangleArea2 (Vector3 a, Vector3 b, Vector3 c) {
			return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
			//return a.x*b.z+b.x*c.z+c.x*a.z-a.x*c.z-c.x*b.z-b.x*a.z;
		}
		
		//public static long TriangleArea (Int3 a, Int3 b, Int3 c) {
		//	return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z);
		//	//a.x*b.z+b.x*c.z+c.x*a.z-a.x*c.z-c.x*b.z-b.x*a.z;
		//}
		
		public static float TriangleArea (Vector3 a, Vector3 b, Vector3 c) {
			return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
			//return a.x*b.z+b.x*c.z+c.x*a.z-a.x*c.z-c.x*b.z-b.x*a.z;
		}
	
		/** Returns if the triangle \a ABC contains the point \a p in XZ space */
		public static bool ContainsPoint (Vector3 a, Vector3 b, Vector3 c, Vector3 p) {
            return IsClockwiseMargin(a, b, p) && IsClockwiseMargin(b, c, p) && IsClockwiseMargin(c, a, p);
		}
		
		/** Checks if \a p is inside the polygon.
		 * \author http://unifycommunity.com/wiki/index.php?title=PolyContainsPoint (Eric5h5)
		 */
		public static bool ContainsPoint (Vector2[] polyPoints,Vector2 p) { 
		   int j = polyPoints.Length-1; 
		   bool inside = false; 
		   
		   for (int i = 0; i < polyPoints.Length; j = i++) { 
		      if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
		         (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
		         inside = !inside; 
		   } 
		   return inside; 
		}
		
		/** Checks if \a p is inside the polygon (XZ space)
		 * \author http://unifycommunity.com/wiki/index.php?title=PolyContainsPoint (Eric5h5)
		 */
		public static bool ContainsPoint (Vector3[] polyPoints,Vector3 p) { 
		   int j = polyPoints.Length-1; 
		   bool inside = false; 
		   
		   for (int i = 0; i < polyPoints.Length; j = i++) { 
		      if ( ((polyPoints[i].z <= p.z && p.z < polyPoints[j].z) || (polyPoints[j].z <= p.z && p.z < polyPoints[i].z)) && 
		         (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.z - polyPoints[i].z) / (polyPoints[j].z - polyPoints[i].z) + polyPoints[i].x)) 
		         inside = !inside; 
		   } 
		   return inside; 
		}
		
		/** Returns if \a p lies on the left side of the line \a a - \a b. Uses XZ space. Also returns true if the points are colinear */
		public static bool Left (Vector3 a, Vector3 b, Vector3 p) {
			return (b.x - a.x) * (p.z - a.z) - (p.x - a.x) * (b.z - a.z) <= 0;
		}
		
		/** Returns if \a p lies on the left side of the line \a a - \a b. Uses XZ space. Also returns true if the points are colinear */
		//public static bool Left (Int3 a, Int3 b, Int3 c) {
		//	return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) <= 0;
		//}
		
		/** Returns if the points a in a clockwise order.
		 * Will return true even if the points are colinear or very slightly counter-clockwise
		 * (if the signed area of the triangle formed by the points has an area less than or equals to float.Epsilon) */
		public static bool IsClockwiseMargin (Vector3 a, Vector3 b, Vector3 c) {
			return (b.x-a.x)*(c.z-a.z)-(c.x-a.x)*(b.z-a.z) <= float.Epsilon;
		}
		
		/** Returns if the points a in a clockwise order */
		public static bool IsClockwise (Vector3 a, Vector3 b, Vector3 c) {
			return (b.x-a.x)*(c.z-a.z)-(c.x-a.x)*(b.z-a.z) < 0;
		}
		
		/** Returns if the points a in a clockwise order */
		//public static bool IsClockwise (Int3 a, Int3 b, Int3 c) {
		//	return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) < 0;
		//}
		
		///** Returns if the points are colinear (lie on a straight line) */
		//public static bool IsColinear (Int3 a, Int3 b, Int3 c) {
		//	return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) == 0;
		//}
		
		/** Returns if the points are colinear (lie on a straight line) */
		public static bool IsColinear (Vector3 a, Vector3 b, Vector3 c) {
			return Mathf.Approximately ((b.x-a.x)*(c.z-a.z)-(c.x-a.x)*(b.z-a.z), 0);
		}
		
		/** Returns if the line segment \a a2 - \a b2 intersects the infinite line \a a - \a b. a-b is infinite, a2-b2 is not infinite */
		public static bool IntersectsUnclamped (Vector3 a, Vector3 b, Vector3 a2, Vector3 b2) {
			return Left (a,b,a2) != Left (a,b,b2);
		}
		
		/** Returns if the two line segments intersects. The lines are NOT treated as infinite (just for clarification)
		  * \see IntersectionPoint */
		public static bool Intersects (Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2) {
			
			Vector3 dir1 = end1-start1;
			Vector3 dir2 = end2-start2;
			
			float den = dir2.z*dir1.x - dir2.x * dir1.z;
			
			if (den == 0) {
				return false;
			}
			
			float nom = dir2.x*(start1.z-start2.z)- dir2.z*(start1.x-start2.x);
			float nom2 = dir1.x*(start1.z-start2.z) - dir1.z * (start1.x - start2.x);
			float u = nom/den;
			float u2 = nom2/den;
		
			if (u < 0F || u > 1F || u2 < 0F || u2 > 1F) {
				return false;
			}
			//Debug.DrawLine (start2,end2,Color.magenta);
			//Debug.DrawRay (start1,dir1*5,Color.green);
			return true;
			//Vector2 intersection = start1 + dir1*u;
			//return intersection;
		}
		
		/** Intersection point between two infinite lines.
		 * Lines are treated as infinite. If the lines are parallel 'start1' will be returned. Intersections are calculated on the XZ plane.
		 */
		public static Vector3 IntersectionPointOptimized (Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2) {
			
			float den = dir2.z*dir1.x - dir2.x * dir1.z;
			
			if (den == 0) {
				return start1;
			}
			
			float nom = dir2.x*(start1.z-start2.z)- dir2.z*(start1.x-start2.x);
			
			float u = nom/den;
			
			return start1 + dir1*u;
		}
		
		/** Intersection point between two infinite lines.
		 * Lines are treated as infinite. If the lines are parallel 'start1' will be returned. Intersections are calculated on the XZ plane.
		 */
		public static Vector3 IntersectionPointOptimized (Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2, out bool intersects) {
			
			float den = dir2.z*dir1.x - dir2.x * dir1.z;
			
			if (den == 0) {
				intersects = false;
				return start1;
			}
			
			float nom = dir2.x*(start1.z-start2.z)- dir2.z*(start1.x-start2.x);
			
			float u = nom/den;
			
			intersects = true;
			return start1 + dir1*u;
		}
	
		/** Returns the intersection factors for line 1 and line 2. The intersection factors is a distance along the line \a start - \a end where the other line intersects it.\n
		 * \code intersectionPoint = start1 + factor1 * (end1-start1) \endcode
		 * \code intersectionPoint2 = start2 + factor2 * (end2-start2) \endcode
		 * Lines are treated as infinite.\n
		 * false is returned if the lines are parallel and true if they are not */
		public static bool IntersectionFactor (Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out float factor1, out float factor2) {
			
			Vector3 dir1 = end1-start1;
			Vector3 dir2 = end2-start2;
			
			//Color rnd = new Color (Random.value,Random.value,Random.value);
			//Debug.DrawRay (start1,dir1,rnd);
			//Debug.DrawRay (start2,dir2,rnd);
			
			float den = dir2.z*dir1.x - dir2.x * dir1.z;
			
			if (den == 0) {
				factor1 = 0;
				factor2 = 0;
				return false;
			}
			
			float nom = dir2.x*(start1.z-start2.z)- dir2.z*(start1.x-start2.x);
			float nom2 = dir1.x*(start1.z-start2.z) - dir1.z * (start1.x - start2.x);
			
			float u = nom/den;
			float u2 = nom2/den;
			
			factor1 = u;
			factor2 = u2;
			
			//Debug.DrawLine (start2,end2,Color.magenta);
			//Debug.DrawRay (start1,dir1*5,Color.green);
			return true;
		}
		
		/** Returns the intersection factor for line 1 with line 2.
		 * The intersection factor is a distance along the line \a start1 - \a end1 where the line \a start2 - \a end2 intersects it.\n
		 * \code intersectionPoint = start1 + intersectionFactor * (end1-start1) \endcode.
		 * Lines are treated as infinite.\n
		 * -1 is returned if the lines are parallel (note that this is a valid return value if they are not parallel too) */
		public static float IntersectionFactor (Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2) {
			
			Vector3 dir1 = end1-start1;
			Vector3 dir2 = end2-start2;
			
			//Color rnd = new Color (Random.value,Random.value,Random.value);
			//Debug.DrawRay (start1,dir1,rnd);
			//Debug.DrawRay (start2,dir2,rnd);
			
			float den = dir2.z*dir1.x - dir2.x * dir1.z;
			
			if (den == 0) {
				return -1;
			}
			
			float nom = dir2.x*(start1.z-start2.z)- dir2.z*(start1.x-start2.x);
			
			float u = nom/den;
			
			//Debug.DrawLine (start2,end2,Color.magenta);
			//Debug.DrawRay (start1,dir1*5,Color.green);
			return u;
		}
		
		/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
		public static Vector3 IntersectionPoint (Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2) {
			bool s;
			return IntersectionPoint (start1,end1,start2,end2, out s);
		}
		
		/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
		public static Vector3 IntersectionPoint (Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects) {
			
			Vector3 dir1 = end1-start1;
			Vector3 dir2 = end2-start2;
			
			//Color rnd = new Color (Random.value,Random.value,Random.value);
			//Debug.DrawRay (start1,dir1,rnd);
			//Debug.DrawRay (start2,dir2,rnd);
			
			float den = dir2.z*dir1.x - dir2.x * dir1.z;
			
			if (den == 0) {
				intersects = false;
				return start1;
			}
			
			float nom = dir2.x*(start1.z-start2.z)- dir2.z*(start1.x-start2.x);
			
			float u = nom/den;
			
			//Debug.DrawLine (start2,end2,Color.magenta);
			//Debug.DrawRay (start1,dir1*5,Color.green);
			intersects = true;
			return start1 + dir1*u;
		}
		
		/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
		public static Vector2 IntersectionPoint (Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2) {
			bool s;
			return IntersectionPoint (start1,end1,start2,end2, out s);
		}
		
		/** Returns the intersection point between the two lines. Lines are treated as infinite. \a start1 is returned if the lines are parallel */
		public static Vector2 IntersectionPoint (Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out bool intersects) {
			
			Vector2 dir1 = end1-start1;
			Vector2 dir2 = end2-start2;
			
			//Color rnd = new Color (Random.value,Random.value,Random.value);
			//Debug.DrawRay (start1,dir1,rnd);
			//Debug.DrawRay (start2,dir2,rnd);
			
			float den = dir2.y*dir1.x - dir2.x * dir1.y;
			
			if (den == 0) {
				intersects = false;
				return start1;
			}
			
			float nom = dir2.x*(start1.y-start2.y)- dir2.y*(start1.x-start2.x);
			
			float u = nom/den;
			
			//Debug.DrawLine (start2,end2,Color.magenta);
			//Debug.DrawRay (start1,dir1*5,Color.green);
			intersects = true;
			return start1 + dir1*u;
		}
		
		/** Returns the intersection point between the two line segments.
		 * Lines are NOT treated as infinite. \a start1 is returned if the line segments do not intersect */
		public static Vector3 SegmentIntersectionPoint (Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects) {
			
			Vector3 dir1 = end1-start1;
			Vector3 dir2 = end2-start2;
			
			//Color rnd = new Color (Random.value,Random.value,Random.value);
			//Debug.DrawRay (start1,dir1,rnd);
			//Debug.DrawRay (start2,dir2,rnd);
			
			float den = dir2.z*dir1.x - dir2.x * dir1.z;
			
			if (den == 0) {
				intersects = false;
				return start1;
			}
			
			float nom = dir2.x*(start1.z-start2.z)- dir2.z*(start1.x-start2.x);
			float nom2 = dir1.x*(start1.z-start2.z) - dir1.z * (start1.x - start2.x);
			float u = nom/den;
			float u2 = nom2/den;
		
			if (u < 0F || u > 1F || u2 < 0F || u2 > 1F) {
				intersects = false;
				return start1;
			}
		
			//Debug.Log ("U1 "+u.ToString ("0.00")+" U2 "+u2.ToString ("0.00")+"\nP1: "+(start1 + dir1*u)+"\nP2: "+(start2 + dir2*u2)+"\nStart1: "+start1+"  End1: "+end1);
			//Debug.DrawLine (start2,end2,Color.magenta);
			//Debug.DrawRay (start1,dir1*5,Color.green);
			intersects = true;
			return start1 + dir1*u;
		}

        public static bool IsPointInsideTriangle(ref Vector3 point, ref Vector3 triangle0, ref Vector3 triangle1, ref Vector3 triangle2, ref Vector3 triangleNormal)
        {
            // Discard zero-size triangles; slower but more logical than considering the triangle edges as outside
            if (Vector3.Cross(triangle1 - triangle0, triangle2 - triangle0) == Vector3.zero)
            {
                return false;
            }

            Vector3 pointTo0 = triangle0 - point;
            Vector3 pointTo1 = triangle1 - point;
            Vector3 pointTo2 = triangle2 - point;

            if (Vector3.Dot(Vector3.Cross(pointTo0, pointTo1), triangleNormal) < 0.0f ||
                Vector3.Dot(Vector3.Cross(pointTo1, pointTo2), triangleNormal) < 0.0f ||
                Vector3.Dot(Vector3.Cross(pointTo2, pointTo0), triangleNormal) < 0.0f)
            {
                return false;
            }

            return true;
        }

		public static List<Vector3> hullCache = new List<Vector3>();
		
		/** Calculates convex hull in XZ space for the points.
		  * Implemented using the very simple Gift Wrapping Algorithm
		  * which has a complexity of O(nh) where \a n is the number of points and \a h is the number of points on the hull,
		  * so it is in the worst case quadratic.
		  */
		public static Vector3[] ConvexHull (Vector3[] points) {
			
			if (points.Length == 0) return new Vector3[0]; 
			
			lock (hullCache) {
				List<Vector3> hull = hullCache;
				hull.Clear ();
				
				int pointOnHull = 0;
				for (int i=1;i<points.Length;i++) if (points[i].x < points[pointOnHull].x) pointOnHull = i;
				
				int startpoint = pointOnHull;
				int counter = 0;
				
				do {
					hull.Add (points[pointOnHull]);
					int endpoint = 0;
					for (int i=0;i<points.Length;i++) if (endpoint == pointOnHull || !Left (points[pointOnHull],points[endpoint],points[i])) endpoint = i;
					
					pointOnHull = endpoint;
					
					counter++;
					if (counter > 10000) {
						Debug.LogWarning ("Infinite Loop in Convex Hull Calculation");
						break;
					}
				} while (pointOnHull != startpoint);
				
				return hull.ToArray ();
			}
		}
		
		/** Does the line segment intersect the bounding box.
		 * The line is NOT treated as infinite.
		 * \author Slightly modified code from http://www.3dkingdoms.com/weekly/weekly.php?a=21
		 */
		public static bool LineIntersectsBounds (Bounds bounds, Vector3 a, Vector3 b) {	
			// Put line in box space
			//CMatrix MInv = m_M.InvertSimple();
			//CVec3 LB1 = MInv * L1;
			//CVec3 LB2 = MInv * L2;
			a -= bounds.center;
			b -= bounds.center;
			
			// Get line midpoint and extent
			Vector3 LMid = (a + b) * 0.5F; 
			Vector3 L = (a - LMid);
            Vector3 LExt = new Vector3(MathEx.Abs(L.x), MathEx.Abs(L.y), MathEx.Abs(L.z));
			
			Vector3 extent = bounds.extents;
			
			// Use Separating Axis Test
			// Separation vector from box center to line center is LMid, since the line is in box space
            if (MathEx.Abs(LMid.x) > extent.x + LExt.x) return false;
            if (MathEx.Abs(LMid.y) > extent.y + LExt.y) return false;
            if (MathEx.Abs(LMid.z) > extent.z + LExt.z) return false;
			// Crossproducts of line and each axis
            if (MathEx.Abs(LMid.y * L.z - LMid.z * L.y) > (extent.y * LExt.z + extent.z * LExt.y)) return false;
            if (MathEx.Abs(LMid.x * L.z - LMid.z * L.x) > (extent.x * LExt.z + extent.z * LExt.x)) return false;
            if (MathEx.Abs(LMid.x * L.y - LMid.y * L.x) > (extent.x * LExt.y + extent.y * LExt.x)) return false;
			// No separating axis, the line intersects
			return true;
		}
		
		/** Dot product of two vectors. \todo Why is this function defined here? */
		public static float Dot (Vector3 lhs, Vector3 rhs)
		{
			return
					lhs.x * rhs.x +
					lhs.y * rhs.y +
					lhs.z * rhs.z;
		}
		
	/*
		public static Vector3[] WeightedSubdivide (Vector3[] path, int subdivisions) {
			
			Vector3[] path2 = new Vector3[(path.Length-1)*(int)Mathf.Pow (2,subdivisions)+1];
			
			int c = 0;
			for (int p=0;p<path.Length-1;p++) {
				float step = 1.0F/Mathf.Pow (2,subdivisions);
				
				for (float i=0;i<1.0F;i+=step) {
					path2[c] = Vector3.Lerp (path[p],path[p+1],MathEx.Hermite (0F,1F,i));//Mathf.SmoothStep (0,1, i));
					c++;
				}
			}
			
			//Debug.Log (path2.Length +" "+c);
			
			path2[c] = path[path.Length-1];
			return path2;
		}*/
	
		/** Subdivides \a path and returns the new array with interpolated values. The returned array is \a path subdivided \a subdivisions times, the resulting points are interpolated using Mathf.SmoothStep.\n
		 * If \a subdivisions is less or equal to 0 (zero), the original array will be returned */
		public static Vector3[] Subdivide (Vector3[] path, int subdivisions) {
			
			subdivisions = subdivisions < 0 ? 0 : subdivisions;
			
			if (subdivisions == 0) {
				return path;
			}
			
			Vector3[] path2 = new Vector3[(path.Length-1)*(int)Mathf.Pow (2,subdivisions)+1];
			
			int c = 0;
			for (int p=0;p<path.Length-1;p++) {
				float step = 1.0F/Mathf.Pow (2,subdivisions);
				
				for (float i=0;i<1.0F;i+=step) {
					path2[c] = Vector3.Lerp (path[p],path[p+1],Mathf.SmoothStep (0,1, i));
					c++;
				}
			}
			
			//Debug.Log (path2.Length +" "+c);
			
			path2[c] = path[path.Length-1];
			return path2;
		}
		
		/** Returns the closest point on the triangle. The \a triangle array must have a length of at least 3.
		 * \see ClosesPointOnTriangle(Vector3,Vector3,Vector3,Vector3);
		 */
		public static Vector3 ClosestPointOnTriangle ( Vector3[] triangle, Vector3 point ) {
			return ClosestPointOnTriangle (triangle[0],triangle[1],triangle[2],point);
		}
		
		/** Returns the closest point on the triangle. \note Got code from the internet, changed a bit to work with the Unity API
		  * \todo Uses Dot product to get the sqrMagnitude of a vector, should change to sqrMagnitude for readability and possibly for speed (unlikely though) */
		public static Vector3 ClosestPointOnTriangle (Vector3 tr0, Vector3 tr1, Vector3 tr2, Vector3 point ) {
		    Vector3 edge0 = tr1 - tr0;
		    Vector3 edge1 = tr2 - tr0;
		    Vector3 v0 = tr0 - point;
		
		    float a = Vector3.Dot (edge0,edge0);//edge0.dot( edge0 ); //Equals to sqrMagnitude
		    float b = Vector3.Dot (edge0, edge1 );
		    float c = Vector3.Dot (edge1, edge1 ); //Equals to sqrMagnitude
		    float d = Vector3.Dot (edge0, v0 );
		    float e = Vector3.Dot (edge1, v0 );
		
		    float det = a*c - b*b;
		    float s = b*e - c*d;
		    float t = b*d - a*e;
		
		    if ( s + t < det )
		    {
		        if ( s < 0.0F )
		        {
		            if ( t < 0.0F )
		            {
		                if ( d < 0.0F )
		                {
		                    s = MathEx.Clamp01 ( -d/a);
		                    t = 0.0F;
		                }
		                else
		                {
		                    s = 0.0F;
		                    t = MathEx.Clamp01 ( -e/c);
		                }
		            }
		            else
		            {
		                s = 0.0F;
		                t = MathEx.Clamp01 ( -e/c);
		            }
		        }
		        else if ( t < 0.0F )
		        {
		            s = MathEx.Clamp01 ( -d/a);
		            t = 0.0F;
		        }
		        else
		        {
		            float invDet = 1.0F / det;
		            s *= invDet;
		            t *= invDet;
		        }
		    }
		    else
		    {
		        if ( s < 0.0F )
		        {
		            float tmp0 = b+d;
		            float tmp1 = c+e;
		            if ( tmp1 > tmp0 )
		            {
		                float numer = tmp1 - tmp0;
		                float denom = a-2*b+c;
		                s = MathEx.Clamp01 ( numer/denom);
		                t = 1-s;
		            }
		            else
		            {
		                t = MathEx.Clamp01 ( -e/c);
		                s = 0.0F;
		            }
		        }
		        else if ( t < 0.0F )
		        {
		            if ( a+d > b+e )
		            {
		                float numer = c+e-b-d;
		                float denom = a-2*b+c;
		                s = MathEx.Clamp01 ( numer/denom);
		                t = 1-s;
		            }
		            else
		            {
		                s = MathEx.Clamp01 ( -e/c);
		                t = 0.0F;
		            }
		        }
		        else
		        {
		            float numer = c+e-b-d;
		            float denom = a-2*b+c;
		            s = MathEx.Clamp01 ( numer/denom);
		            t = 1.0F - s;
		        }
		    }
			
			return tr0 + s * edge0 + t * edge1;
		}

        //4点插值获得高度
	    public static float GetGroundHeight( float x, float y, float[] pHeight)
	    {
		    int nX = (int)x;
		    int nY = (int)y;

		    float u = x - nX;
		    float v = y - nY;

		    if( u > v )
		    {
			    // 右边
			    return pHeight[0] + u * ( pHeight[2] - pHeight[0] ) + v * ( pHeight[3] - pHeight[2] ); 
		    }
		    else
		    {
			    // 左边
			    return pHeight[0] + u * ( pHeight[3] - pHeight[1] ) + v * ( pHeight[1] - pHeight[0] );
		    }
	    }


        static Dictionary<int, Quaternion> ms_AllAngleQuaternion = new Dictionary<int, Quaternion>();


        public static bool GetQuaternion(int iAngle, out Quaternion quat)
        {
            return ms_AllAngleQuaternion.TryGetValue(iAngle, out quat);
        }
        //x z 是目标位置， x1 z1是原始位置
        public static Quaternion CalDirection(float x2, float z2, float x1, float z1)
        {
            Vector3 vDir = new Vector3(x2 - x1, 0, z2 - z1);
            //vDir.Normalize();
            return Quaternion.LookRotation(vDir);
        }

        // 根据圆心、半径获取指定角度的点
        // 角度使用角度制 
        public static Vector2 GetPointInCircle(float centreX, float centreY, float fRadius, float fDegree)
        {
            
		    float fXOffset = fRadius * Mathf.Sin(fDegree * Mathf.Deg2Rad);
		    float fYOffset = fRadius * Mathf.Cos(fDegree * Mathf.Deg2Rad);

            return new Vector2(centreX + fXOffset, centreY + fYOffset);
        }

        /// <summary>
        ///  判断物体是否在扇形内
        /// </summary>
        /// <param name="t">检测目标</param>
        /// <param name="v">圆点</param>
        /// <param name="beginAngle">起始角度(相对与世界坐标系)</param>
        /// <param name="endAngle">结束角度(相对与世界坐标系)</param>
        /// <param name="r">半径</param>
        /// <returns></returns>
        public static bool GetPointInSector(Vector3 t, Vector3 v, float beginAngle, float endAngle, float r)
        {
            if (Mathf.Pow(t.x - v.x, 2) + Mathf.Pow(t.z - v.z, 2) > Mathf.Pow(r, 2))
                return false;

            float angle = (float)Mathf.Atan2(t.x - v.x, t.z - v.z);
            if (endAngle > beginAngle)
            {
                if (angle > endAngle || angle < beginAngle)
                    return false;
            }
            else if (angle < beginAngle || angle > endAngle)
            {
                return false;
            }

            return true;
        }



        //VECTOR 3 Calculations

        //Calculate the Bezier spline position
        //t - the time (0-1) of the curve to sample
        //p - the start point of the curve
        //a - control point from p
        //b - control point from q
        //q - the end point of the curve
        public static Vector3 CalculateBezier(float t, Vector3 p, Vector3 a, Vector3 b, Vector3 q)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            float u = 1.0f - t;
            float u2 = u * u;
            float u3 = u2 * u;

            Vector3 output = u3 * p + 3 * u2 * t * a + 3 * u * t2 * b + t3 * q;

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <param name="t"></param>
        /// <param name="tension">1 is high, 0 normal, -1 is low</param>
        /// <param name="bias">0 is even, positive is towards first segment, negative towards the other</param>
        /// <returns></returns>
        public static Vector3 CalculateHermite(Vector3 p, Vector3 a, Vector3 b, Vector3 q, float t, float tension, float bias)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            Vector3 m0 = (a - p) * (1 + bias) * (1 - tension) / 2;
            m0 += (b - a) * (1 - bias) * (1 - tension) / 2;
            Vector3 m1 = (b - a) * (1 + bias) * (1 - tension) / 2;
            m1 += (q - b) * (1 - bias) * (1 - tension) / 2;
            float a0 = 2 * t3 - 3 * t2 + 1;
            float a1 = t3 - 2 * t2 + t;
            float a2 = t3 - t2;
            float a3 = -2 * t3 + 3 * t2;

            return (a0 * a + a1 * m0 + a2 * m1 + a3 * b);
        }

        public static Vector3 CalculateCatmullRom(Vector3 p, Vector3 a, Vector3 b, Vector3 q, float t)
        {
            var t2 = t * t;

            var a0 = -0.5f * p + 1.5f * a - 1.5f * b + 0.5f * q;
            var a1 = p - 2.5f * a + 2f * b - 0.5f * q;
            var a2 = -0.5f * p + 0.5f * b;
            var a3 = a;

            return (a0 * t * t2) + (a1 * t2) + (a2 * t) + a3;
        }

        //VECTOR 2 Calculations

        public static Vector2 CalculateBezier(float t, Vector2 p, Vector2 a, Vector2 b, Vector2 q)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            float u = 1.0f - t;
            float u2 = u * u;
            float u3 = u2 * u;

            Vector2 output = u3 * p + 3 * u2 * t * a + 3 * u * t2 * b + t3 * q;

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <param name="t"></param>
        /// <param name="tension">1 is high, 0 normal, -1 is low</param>
        /// <param name="bias">0 is even, positive is towards first segment, negative towards the other</param>
        /// <returns></returns>
        public static Vector2 CalculateHermite(Vector2 p, Vector2 a, Vector2 b, Vector2 q, float t, float tension, float bias)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            Vector2 m0 = (a - p) * (1 + bias) * (1 - tension) / 2;
            m0 += (b - a) * (1 - bias) * (1 - tension) / 2;
            Vector2 m1 = (b - a) * (1 + bias) * (1 - tension) / 2;
            m1 += (q - b) * (1 - bias) * (1 - tension) / 2;
            float a0 = 2 * t3 - 3 * t2 + 1;
            float a1 = t3 - 2 * t2 + t;
            float a2 = t3 - t2;
            float a3 = -2 * t3 + 3 * t2;

            return (a0 * a + a1 * m0 + a2 * m1 + a3 * b);
        }

        public static Vector2 CalculateCatmullRom(Vector2 p, Vector2 a, Vector2 b, Vector2 q, float t)
        {
            var t2 = t * t;

            var a0 = -0.5f * p + 1.5f * a - 1.5f * b + 0.5f * q;
            var a1 = p - 2.5f * a + 2f * b - 0.5f * q;
            var a2 = -0.5f * p + 0.5f * b;
            var a3 = a;

            return (a0 * t * t2) + (a1 * t2) + (a2 * t) + a3;
        }

        //Calculate Cubic Rotation
        //p - point we start with
        //q - next point
        //nextNormIndex - the point immediately before p
        //prevNormIndex - the point immediately after q
        //t - time (0-1) of the curve pq to sample
        public static Quaternion CalculateCubic(Quaternion p, Quaternion a, Quaternion b, Quaternion q, float t)
        {
            // Ensure all the quaternions are proper for interpolation - thanks Jeff!
            if (Quaternion.Dot(p, q) < 0.0f)
                q = new Quaternion(-q.x, -q.y, -q.z, -q.w);

            if (Quaternion.Dot(p, a) < 0.0f)
                a = new Quaternion(-a.x, -a.y, -a.z, -a.w);

            if (Quaternion.Dot(p, b) < 0.0f)
                b = new Quaternion(-b.x, -b.y, -b.z, -b.w);

            Quaternion a1 = SquadTangent(a, p, q);
            Quaternion b1 = SquadTangent(p, q, b);
            float slerpT = 2.0f * t * (1.0f - t);
            Quaternion sl = Quaternion.Slerp(Quaternion.Slerp(p, q, t), Quaternion.Slerp(a1, b1, t), slerpT);
            return sl;
        }

        public static float CalculateCubic(float p, float a, float b, float q, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            float u = 1.0f - t;
            float u2 = u * u;
            float u3 = u2 * u;

            return (u3 * p + 3 * u2 * t * q + 3 * u * t2 * a + t3 * b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <param name="t"></param>
        /// <param name="tension">1 is high, 0 normal, -1 is low</param>
        /// <param name="bias">0 is even, positive is towards first segment, negative towards the other</param>
        /// <returns></returns>
        public static float CalculateHermite(float p, float a, float b, float q, float t, float tension, float bias)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            float m0 = (a - p) * (1 + bias) * (1 - tension) / 2;
            m0 += (b - a) * (1 - bias) * (1 - tension) / 2;
            float m1 = (b - a) * (1 + bias) * (1 - tension) / 2;
            m1 += (q - b) * (1 - bias) * (1 - tension) / 2;
            float a0 = 2 * t3 - 3 * t2 + 1;
            float a1 = t3 - 2 * t2 + t;
            float a2 = t3 - t2;
            float a3 = -2 * t3 + 3 * t2;

            return (a0 * a + a1 * m0 + a2 * m1 + a3 * b);
        }

        public static float CalculateCatmullRom(float p, float a, float b, float q, float t)
        {
            var t2 = t * t;

            var a0 = -0.5f * p + 1.5f * a - 1.5f * b + 0.5f * q;
            var a1 = p - 2.5f * a + 2f * b - 0.5f * q;
            var a2 = -0.5f * p + 0.5f * b;
            var a3 = a;

            return (a0 * t * t2) + (a1 * t2) + (a2 * t) + a3;
        }

        public static DateTime CalculateTimeStamp(double stampTime)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            time = startTime.AddSeconds(stampTime);
            return time;
        }

        //时间转换时分秒
        public static Vector3 TransTime(int time)        //time为总秒数
        {
            int hours = time / 3600;
            int minutes = (time - hours * 3600) / 60;
            int seconds = time - hours * 3600 - minutes * 60;
            return new Vector3(hours, minutes, seconds);
        }

        public static float SmoothStep(float val)
        {
            return val * val * (3.0f - 2.0f * val);
        }

        //calculate the Squad tangent for use in Cubic Rotation Interpolation
        public static Quaternion SquadTangent(Quaternion before, Quaternion center, Quaternion after)
        {
            Quaternion l1 = LnDif(center, before);
            Quaternion l2 = LnDif(center, after);
            Quaternion e = Quaternion.identity;
            for (int i = 0; i < 4; ++i)
            {
                e[i] = -0.25f * (l1[i] + l2[i]);
            }
            return center * (Exp(e));
        }

        public static Quaternion LnDif(Quaternion a, Quaternion b)
        {
            Quaternion dif = Quaternion.Inverse(a) * b;
            Normalize(dif);
            return Log(dif);
        }

        public static Quaternion Normalize(Quaternion q)
        {
            float norm = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (norm > 0.0f)
            {
                q.x /= norm;
                q.y /= norm;
                q.z /= norm;
                q.w /= norm;
            }
            else
            {
                q.x = 0.0f;
                q.y = 0.0f;
                q.z = 0.0f;
                q.w = 1.0f;
            }
            return q;
        }

        public static Quaternion Exp(Quaternion q)
        {
            float theta = Mathf.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2]);

            if (theta < 1E-6)
            {
                return new Quaternion(q[0], q[1], q[2], Mathf.Cos(theta));
            }
            //else
            float coef = Mathf.Sin(theta) / theta;
            return new Quaternion(q[0] * coef, q[1] * coef, q[2] * coef, Mathf.Cos(theta));
        }

        public static Quaternion Log(Quaternion q)
        {
            float len = Mathf.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2]);

            if (len < 1E-6)
            {
                return new Quaternion(q[0], q[1], q[2], 0.0f);
            }
            //else
            float coef = Mathf.Acos(q[3]) / len;
            return new Quaternion(q[0] * coef, q[1] * coef, q[2] * coef, 0.0f);
        }


        public static Quaternion GetQuatConjugate(Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, q.w);
        }

        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 up)
        {
            Vector3 direction = (to - from).normalized;
            Vector3 cross = Vector3.Cross(up, direction);
            float dot = Vector3.Dot(from, cross);
            return Vector3.Angle(from, to) * Mathf.Sign(dot);
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;

            if (angle > 360)
                angle -= 360;

            return Mathf.Clamp(angle, -max, -min);
        }
	}
}
