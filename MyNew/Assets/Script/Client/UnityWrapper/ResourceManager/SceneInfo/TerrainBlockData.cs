using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 障碍信息真实的宽长放大两倍，是为了方便A*计算
    /// 在游戏中要获取一个点是否为障碍时，需要*2
    /// </summary>
    public class TerrainBlockData : DataBlock
    {

        public TerrainBlockData(int nW, int nH)
        {
            m_nW = nW;
            m_nH = nH;
            m_nDW = (int)(m_nW / nodesize); 
            m_nDH = (int)(m_nH / nodesize);
        }

        //极限压缩法每一个单位都代表阻挡(挑表算法)
        //0.5一个单位
        public const float nodesize = 1f;         // 真实世界坐标变换到寻路坐标时，除以这个系数
        public const float halfnodesize = 0.5f;

        public override bool Write(LusuoStream lf)
        {
            lf.WriteInt(m_nW);
            lf.WriteInt(m_nH);
            //lf.WriteInt(m_nNums);
            //Debug.Log("block--Save" + m_nNums.ToString());
            if (null == m_bBlock)
            {
                m_bBlock = new byte[m_nDW * m_nDH];
            }

            //由UNITY AB包压缩
            //byte[] byteCompData = new byte[m_nW * m_nH];
            //for (int i = 0; i < m_nDW * m_nDH; i += 4)
            //{
            //    int iIndex = i / 4;
            //    byteCompData[iIndex] |= m_bBlock[i] > 0 ? (byte)2 : (byte)0;
            //    byteCompData[iIndex] |= m_bBlock[i + 1] > 0 ? (byte)4 : (byte)0;
            //    byteCompData[iIndex] |= m_bBlock[i + 2] > 0 ? (byte)8 : (byte)0;
            //    byteCompData[iIndex] |= m_bBlock[i + 3] > 0 ? (byte)16 : (byte)0;
            //}
            //Debug.Log("block--Save" + m_bBlock.Length);
            lf.Write(ref m_bBlock);
            return true;
        }

        public override bool Read(ref LusuoStream lf)
        {
            lf.ReadInt(ref m_nW);
            lf.ReadInt(ref m_nH);
            //lf.ReadInt(ref m_nNums);
            //Debug.Log("block--Read" + m_nNums.ToString());
            m_nDW = (int)(m_nW / nodesize);
            m_nDH = (int)(m_nH / nodesize);
            if (null == m_bBlock)
            {
                m_bBlock = new byte[m_nDW * m_nDH];
            }

            //由UNITY AB包压缩
            //byte[] byteCompData = new byte[m_nW * m_nH];
            //lf.Read(ref byteCompData);
            //for (int i = 0; i < m_nW * m_nH; i ++ )
            //{
            //    m_bBlock[i * 4] = (byteCompData[i] & 2) != 0 ? (byte)1 : (byte)0;
            //    m_bBlock[i * 4 + 1] = (byteCompData[i] & 4) != 0 ? (byte)1 : (byte)0;
            //    m_bBlock[i * 4 + 2] = (byteCompData[i] & 8) != 0 ? (byte)1 : (byte)0;
            //    m_bBlock[i * 4 + 3] = (byteCompData[i] & 16) != 0 ? (byte)1 : (byte)0;
            //}
            lf.Read(ref m_bBlock);
            //Debug.Log("block--Read" + m_bBlock.Length);
            //GameObject root = new GameObject("root");
            //GameObject cube = GameObject.Find("Cube");
            //for (int i = 0; i < 256; i += 2)
            //{
            //    for (int j = 0; j < 256; j += 2)
            //    {
            //        if ((eGridType)m_bBlock[j * 256 + i] == eGridType.eGT_walkable)
            //        {
            //            GameObject obj = (GameObject)GameObject.Instantiate(cube);
            //            obj.transform.parent = root.transform;
            //            cube.transform.localPosition = new Vector3(i, 0, j);
            //            cube.transform.localScale = Vector3.one;
            //        }

            //    }
            //}

            return true;
        }

        //因为单位为0.5米每个单位所以传入的值必须是原始值的二倍
        public eGridType GetTerrainBlock(int nX, int nZ)
        {
            if (nX < 0 || nX >= m_nDW || nZ < 0 || nZ >= m_nDH)
            {
                return eGridType.eGT_none;
            }
            return (eGridType)m_bBlock[nZ * m_nDW + nX];
        }

        //因为单位为0.5米每个单位所以传入的值必须是原始值的二倍，0红色1黑色3蓝色可走
        public void SetTerrainBlock(int nX, int nZ, byte bBlock)
        {
            if (nX < 0 || nX >= m_nDW || nZ < 0 || nZ >= m_nDH)
            {
                return;
            }

            //看看属于哪一个小块中
            m_bBlock[nZ * m_nDW + nX] = bBlock;
        }

        public byte[] GetMapInfo()
        {
            return m_bBlock;
        }

        public int m_nW = 0;            //地图原始宽度
        public int m_nH = 0;
        public int m_nDW = 0;     //放大后的地图大小
        public int m_nDH = 0;
        public byte[] m_bBlock = null;  //这个是 w * h * 4 0.5一个格子。注意这里是逻辑单位，和实际的世界米单位不一样了。是二倍关系
    }
}
