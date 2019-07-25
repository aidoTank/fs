using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
	public enum ePlayerExpCsv
	{
		Level,
		Exp,
	}

	public class PlayerExpCsvData
    {
		/// <summary>
		/// 等级
		/// </summary>
		public int Level;

		/// <summary>
		/// 经验
		/// </summary>
		public int Exp;

	
	}

	public class PlayerExpCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new PlayerExpCsv();
		}

		protected override void _Load()
		{
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
			{
                PlayerExpCsvData data = new PlayerExpCsvData();
				data.Level = m_csv.GetIntData(i, (int)ePlayerExpCsv.Level);
				data.Exp = m_csv.GetIntData(i, (int)ePlayerExpCsv.Exp);

                if(i == m_csv.GetRows() - 1)
                {
                    m_maxLv = data;
                }
                m_dicData.Add(data.Level, data);
			}
		}

        public int GetLevelByExp(int exp)
        {
            int curVal = 0;
            foreach(KeyValuePair<int, PlayerExpCsvData> item in m_dicData)
            {
                curVal += item.Value.Exp;
                if(curVal > exp)
                {
                    return item.Value.Level - 1;
                }
            }
            return m_maxLv.Level;
        }

        /// <summary>
        /// 当前等级对应的总经验
        /// </summary>
        public int GetMaxExpByLv(int lv)
        {
            int maxExp = 0;
            foreach (KeyValuePair<int, PlayerExpCsvData> item in m_dicData)
            {
                maxExp += item.Value.Exp;
                if (lv == item.Value.Level)
                {
                    return maxExp;
                }
            }
            return 0;
        }

		public PlayerExpCsvData GetData(int csvId)
		{
            if(csvId >= m_maxLv.Level)
            {
                return m_maxLv;
            }

            PlayerExpCsvData data;
			if (m_dicData.TryGetValue(csvId, out data))
			{
				return data;
			}
			return null;
		}

        public PlayerExpCsvData GetMaxLv()
        {
            int curLv = 0;
            foreach (KeyValuePair<int, PlayerExpCsvData> item in m_dicData)
            {
                if(item.Key > curLv)
                {
                    curLv = item.Key;
                }
            }
            return GetData(curLv);
        }

        public PlayerExpCsvData m_maxLv;
		public Dictionary<int, PlayerExpCsvData> m_dicData = new Dictionary<int, PlayerExpCsvData>();
	}
}
