using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Roma
{
    // 暂时先只做模型和全局灯光
    public enum eDBType
    {
        eDB_Error = -1,
        eDB_Lights,
        eDB_EntityList,
        eDB_Heights,        // 高度
        eDB_Block,          // 障碍
        eDB_MaxData,
    }
    public class SceneCfg
    {
        private Dictionary<int, DataBlock> m_mapDataDlock = new Dictionary<int, DataBlock>();
        private int m_nW = 0;
        private int m_nH = 0;

        // 进游戏时使用
        public SceneCfg()
        {
            CreateDataBlock((int)eDBType.eDB_Lights);
            CreateDataBlock((int)eDBType.eDB_EntityList);
            CreateDataBlock((int)eDBType.eDB_Heights);
            CreateDataBlock((int)eDBType.eDB_Block);
        }

        // 编辑场景时使用，此时需要知道高宽
        public SceneCfg(int nW,int nH)
        {
            m_nW = nW;
            m_nH = nH;
            // new的时候，就创建好每种数据块类型
            CreateDataBlock((int)eDBType.eDB_Lights);
            CreateDataBlock((int)eDBType.eDB_EntityList);
            CreateDataBlock((int)eDBType.eDB_Heights);
            CreateDataBlock((int)eDBType.eDB_Block);
        }

        private void CreateDataBlock(int type)
        {
            DataBlock db = null;
            switch (type)
            {
                case (int)eDBType.eDB_Lights:

                    break;
                case (int)eDBType.eDB_EntityList:
                    db = new EntityInfoList();
                    break;
                case (int)eDBType.eDB_Heights:
                    db = new TerrainHeightData(m_nW, m_nH);
                    break;
                case (int)eDBType.eDB_Block:
                    db = new TerrainBlockData(m_nW, m_nH);
                    break; 
            }
            m_mapDataDlock.Add(type, db);
        }

        public bool Write(LusuoStream lf)
        {
            for (int i = 0; i < (int)eDBType.eDB_MaxData; i++)
            {
                DataBlock db = null;
                if (m_mapDataDlock.TryGetValue(i, out db))
                {
                    if (null != db)
                        db.Write(lf);
                }
            }
            return true;
        }

        public bool Read(ref LusuoStream lf)
        {
            for (int i = 0; i < (int)eDBType.eDB_MaxData; i++)
            {
                DataBlock db = null;
                if (m_mapDataDlock.TryGetValue(i, out db))
                {
                    if (null != db)
                        db.Read(ref lf);
                }
            }
            return true;
        }

        public EntityInfoList GetEntityInfoListCfg()
        {
            return (EntityInfoList)m_mapDataDlock[(int)eDBType.eDB_EntityList];
        }

        public TerrainHeightData GetTerrainHeightData()
        {
            return (TerrainHeightData)m_mapDataDlock[(int)eDBType.eDB_Heights];
        }

        public TerrainBlockData GetBlockData()
        {
            return (TerrainBlockData)m_mapDataDlock[(int)eDBType.eDB_Block];
        }
    }
}
