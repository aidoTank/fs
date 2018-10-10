using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    enum eMapCsv_Enum
    {
        eID = 0,
        eName = 1,
        eResId,
        eStaticData,
    }

    public class SceneCsvData
    {
        public int id;
        public string name;
        public int resId;
        public int staticData;
    }

    public class SceneCsv : CsvExWrapper
	{
        public override void Clear()
        {
            base.Clear();
            m_mapDataDic.Clear();
        }

        protected override void _Load()
        {
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                SceneCsvData ani = new SceneCsvData();
                ani.id = m_csv.GetIntData(i, (int)eMapCsv_Enum.eID);
                ani.name = m_csv.GetData(i, (int)eMapCsv_Enum.eName);
                ani.resId = m_csv.GetIntData(i, (int)eMapCsv_Enum.eResId);
                ani.staticData = m_csv.GetIntData(i, (int)eMapCsv_Enum.eStaticData);
                m_mapDataDic.Add(ani.id, ani);
            }
        }

        static public CsvExWrapper CreateCSV()
        {
            return new SceneCsv();
        }

        public SceneCsvData GetData(int id)
		{
            SceneCsvData ani;
            if (m_mapDataDic.TryGetValue(id, out ani))
			{
                return ani;
			}
			return null;
		}


        public Dictionary<int, SceneCsvData> m_mapDataDic = new Dictionary<int, SceneCsvData>();
	}
}