using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class TerrainHeightData : DataBlock
    {
        public int m_nW = 0;
        public int m_nH = 0;
        public float[] m_fHeight = null;

        private float[] fHeightG = new float[4]; // 实时获取，优化效率，将fHeightG作为全局变量

        public TerrainHeightData(int nW, int nH)
        {
            m_nW = nW + 1;// 注意这个地方 +1 正好与block形成交错网格
            m_nH = nH + 1;// 在扫描的时候也+1了
            // 在高度数量相当于顶点数量，在取高度时是通过格子的顶点获取值
        }
        public override bool Write(LusuoStream lf)
        {
            //Debug.Log("地形高度数据类写入");
            lf.WriteInt(m_nW);
            lf.WriteInt(m_nH);
            for (int i = 0; i < m_nW * m_nH; i ++)
            {
                ushort uData = (ushort)(m_fHeight[i] * 100);
                lf.WriteUShort(uData);
            }
            return true;
        }

        public override bool Read(ref LusuoStream lf)
        {
            //Debug.Log("地形高度数据类读取");
            lf.ReadInt(ref m_nW);
            lf.ReadInt(ref m_nH);
            if (null == m_fHeight)
            {
                m_fHeight = new float[m_nW * m_nH];
            }
            ushort uData = 0;
            for (int i = 0; i < m_nW * m_nH; i ++)
            {
                lf.ReadUShort(ref uData);
                m_fHeight[i] = ((float)uData * 0.01f);
            }
            return true;
        }

        public void SetTerrainHeight(int iX, int iZ, float f)
        {
            if (iX < 0 || iX >= m_nW ||
                iZ < 0 || iZ >= m_nH)
            {
                return;
            }
            int iIndex = iZ * m_nW + iX;
            m_fHeight[iIndex] = f;
        }

        public float SmoothInterpolTerrainHeight(float fX, float fZ)
        {
            int iIndex0 = (int)fZ * m_nW + (int)fX;
            int iIndex1 = iIndex0 + m_nW;
            int iIndex2 = iIndex0 + 1;
            int iIndex3 = iIndex0 + 1 + m_nW;

            if (m_nW * m_nH <= iIndex3 || iIndex0 < 0)
            {
                Debug.LogWarning("InterpolTerrainHeight not find point");
                return 0.0f;
            }
            // 实时获取，优化效率，将fHeightG作为全局变量
            // float[] fHeightG = new float[4];
            fHeightG[0] = m_fHeight[iIndex0];
            fHeightG[1] = m_fHeight[iIndex1];
            fHeightG[2] = m_fHeight[iIndex2];
            fHeightG[3] = m_fHeight[iIndex3];

            return MathEx.GetGroundHeight(fX, fZ, fHeightG);
        }

        public float FastInterpolTerrainHeight(float fX, float fZ)
        {
            int iIndex0 = (int)fZ * m_nW + (int)fX;

            if (m_nW * m_nH <= iIndex0 || iIndex0 < 0)
            {
                Debug.LogWarning("InterpolTerrainHeight not find point");
                return 0.0f;
            }

            return m_fHeight[iIndex0];
        }
    }
}
