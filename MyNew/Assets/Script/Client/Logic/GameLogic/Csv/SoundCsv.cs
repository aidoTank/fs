using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    public enum eSoundCsv
    {
        csvId,
        type,
        ResourceID,
        Loop,
        describe,
    }

    public class SoundCsvData
    {
        /// <summary>
        /// csvId
        /// </summary>
        public int csvId;

        /// <summary>
        /// 声音类型（0背景 1环境 2特效 3UI 4说话 
        /// </summary>
        public int type;

        /// <summary>
        /// 资源ID
        /// </summary>
        public int ResourceID;
        public bool Loop;
        /// <summary>
        /// 描述
        /// </summary>
        public string describe;

    }

    public class SoundCsv : CsvExWrapper
    {
        static public CsvExWrapper CreateCSV()
        {
            return new SoundCsv();
        }

        protected override void _Load()
        {
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                SoundCsvData data = new SoundCsvData();
                data.csvId = m_csv.GetIntData(i, (int)eSoundCsv.csvId);
                data.type = m_csv.GetIntData(i, (int)eSoundCsv.type);
                data.ResourceID = m_csv.GetIntData(i, (int)eSoundCsv.ResourceID);
                data.Loop = m_csv.GetBoolData(i, (int)eSoundCsv.Loop);
                data.describe = m_csv.GetData(i, (int)eSoundCsv.describe);
                if (m_dicData.ContainsKey(data.csvId))
                {
                    Debug.LogError("重复ID：" + data.csvId);
                    continue;
                }
                m_dicData.Add(data.csvId, data);
            }
        }

        public SoundCsvData GetData(int csvId)
        {
            SoundCsvData data;
            if (m_dicData.TryGetValue(csvId, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int, SoundCsvData> m_dicData = new Dictionary<int, SoundCsvData>();
    }
}
