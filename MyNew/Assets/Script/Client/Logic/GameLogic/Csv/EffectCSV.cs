using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    public enum eEffectCsv
    {
        Id,
        resId,
        resName,
        liveTime,
        soundId,
        shakeCamera,
    }

    public class EffectCsvData
    {
        /// <summary>
        /// 特效编号
        /// </summary>
        public int Id;

        /// <summary>
        /// 资源编号
        /// </summary>
        public int resId;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string resName;

        /// <summary>
        /// 生命时间
        /// </summary>
        public float lifeTime;

        /// <summary>
        /// 声音编号
        /// </summary>
        public int soundId;

        public int shakeCamera;

    }

    public class EffectCsv : CsvExWrapper
    {
        static public CsvExWrapper CreateCSV()
        {
            return new EffectCsv();
        }

        protected override void _Load()
        {
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                EffectCsvData data = new EffectCsvData();
                data.Id = m_csv.GetIntData(i, (int)eEffectCsv.Id);
                data.resId = m_csv.GetIntData(i, (int)eEffectCsv.resId);
                data.resName = m_csv.GetData(i, (int)eEffectCsv.resName);
                data.lifeTime = m_csv.GetFloatData(i, (int)eEffectCsv.liveTime);
                data.soundId = m_csv.GetIntData(i, (int)eEffectCsv.soundId);
                data.shakeCamera = m_csv.GetIntData(i, (int)eEffectCsv.shakeCamera);
                if (m_dicData.ContainsKey(data.Id))
                {
                    //Debug.LogError("重复特效的ID:" + data.Id);
                    continue;
                }
                m_dicData.Add(data.Id, data);
            }
        }

        public EffectCsvData GetData(int csvId)
        {
            EffectCsvData data;
            if (m_dicData.TryGetValue(csvId, out data))
            {
                return data;
            }
            return null;
        }

        public Dictionary<int, EffectCsvData> m_dicData = new Dictionary<int, EffectCsvData>();
    }
}
