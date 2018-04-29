using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Roma
{
    public static class AStar
    {

        [DllImport("TestDll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetPath(
                                    byte[] map, int w, int h,
                                    ref PosInfo startPos,
                                    ref PosInfo endPos,
                                    int maxRoute,
                                    IntPtr pPosInfos, int maxLen,
                                    ref int realLen);

        [StructLayout(LayoutKind.Sequential)]
        public struct PosInfo
        {
            public int x;
            public int y;
            public Vector2 ToVec()
            {
                return new Vector2(x, y);
            }

            public PosInfo(Vector2 pos)
            {
                x = (int)pos.x;
                y = (int)pos.y;
            }
        }

        /// <summary>
        /// 最大遍历次数,算法上限
        /// </summary>
        private const int MAXROUTE = 128*256;
        /// <summary>
        /// 寻路最大长度，逻辑上限
        /// 如果C#这边数组分配太少，CPP会越界，要把最大长度也发送过去
        /// </summary>
        private const int MAXLEN = 1000;
        private static PosInfo[] m_posInfos = new PosInfo[MAXLEN];
        private static int m_structSize = Marshal.SizeOf(typeof(PosInfo));

        public static bool GetLine(byte[] mapInfo,int w, int h,
                                   int x1, int y1, int x2, int y2,
                                   ref Vector2 lastPos)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int ux = (dx > 0) ? 1 : -1;//x的增量方向，取或-1
            int uy = (dy > 0) ? 1 : -1;//y的增量方向，取或-1
            int x = x1, y = y1, eps;//eps为累加误差
            //算法问题需要向增量方向在+1
            x2 += ux; y2 += uy;
            eps = 0; dx = Mathf.Abs(dx); dy = Mathf.Abs(dy);
            if (dx > dy)
            {
                for (x = x1; x != x2; x += ux)
                {
                    if(mapInfo[y * w + x] != 2)
                    {
                        //Profiler.EndSample();
                        return false;
                    }
                    lastPos.x = x;
                    lastPos.y = y;
                    //mpaths.Add(new Vector2(x, y));
                    eps += dy;
                    if ((eps << 1) >= dx)
                    {
                        y += uy; eps -= dx;
                    }
                }
            }
            else
            {
                for (y = y1; y != y2; y += uy)
                {
                    if (mapInfo[y * w + x] != 2)
                    {
                        //Profiler.EndSample();
                        return false;
                    }
                    lastPos.x = x;
                    lastPos.y = y;
                    eps += dx;
                    if ((eps << 1) >= dy)
                    {
                        x += ux; eps -= dy;
                    }
                }
            }
            //Profiler.EndSample();
            return true;
        }

        public static void GetPath(byte[] mapInfo, int w, int h,
            Vector2 vStart, Vector2 vEnd, ref List<Vector2> pathList)
        {
            // 如果直线能走过去
            Vector2 vLastPos = Vector2.zero;
            if (GetLine(mapInfo, w, h, (int)vStart.x, (int)vStart.y, (int)vEnd.x, (int)vEnd.y, ref vLastPos))
            {
                pathList.Add(vLastPos);
                return;
            }

            // 如果目标点是障碍点,走到能走的直线点就行
            if (mapInfo[(int)vEnd.y * w + (int)vEnd.x] != 2)
            {
                pathList.Add(vLastPos);
                return;
            }

            PosInfo startPos = new PosInfo(vStart);
            PosInfo endPos = new PosInfo(vEnd);

            IntPtr pt = Marshal.AllocHGlobal(m_structSize * MAXLEN);
            int realLen = 0;
            GetPath(mapInfo, w, h, ref startPos, ref endPos, MAXROUTE, pt, MAXLEN, ref realLen);

            //Debug.Log("realLen:" + realLen);
            if (realLen == 0)
            {
                Debug.LogWarning("路径过长，或者找不到路径");
            }
            else
            {
                for (int i = 1; i < realLen; i++)
                {
                    // Marshal.PtrToStructure获取的object转为结构体会产生较多GC
                    // 一个个的读取就不会产生GC。
                    //m_posInfos[i] = (PosInfo)Marshal.PtrToStructure((IntPtr)(pt.ToInt32() + i * structSize), typeof(PosInfo));
                    m_posInfos[i].x = Marshal.ReadInt32(pt, 0 + i * m_structSize);
                    m_posInfos[i].y = Marshal.ReadInt32(pt, 4 + i * m_structSize);
                    pathList.Add(m_posInfos[i].ToVec());
                }
            }

            Marshal.FreeHGlobal(pt);
        }
    }
}
