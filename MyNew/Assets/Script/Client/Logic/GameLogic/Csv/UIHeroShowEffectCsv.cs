using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    public enum eUIHeroShowEffectCsv
    {
        resId,
        enterAnimaTime,
        enterAnimaId,
        enterEffectTime,
        enterEffectId,
        enterHeroSpeakId,
        clickAnimaId,
        clickEffectId,
        idleEffectId,
        bz,
    }

    public class UIHeroShowEffectCsvData
    {
        /// <summary>
        /// 资源id
        /// </summary>
        public int resId;

        /// <summary>
        /// 入场动作时间
        /// </summary>
        public float enterAnimaTime;

        /// <summary>
        /// 入场动作
        /// </summary>
        public string enterAnimaName;

        /// <summary>
        /// 入场特效时间
        /// </summary>
        public string enterEffectTime;

        /// <summary>
        /// 入场特效id
        /// </summary>
        public string enterEffectId;

        public int enterSpeakId;
        /// <summary>
        /// 点击时播放动作
        /// </summary>
        public string clickAnimaName;

        /// <summary>
        /// 点击播放特效id
        /// </summary>
        public int clickEffectId;

        public int idleEffectId;

        /// <summary>
        /// 备注
        /// </summary>
        public string bz;

    }

    public class UIHeroShowEffectCsv : CsvExWrapper
    {
        static public CsvExWrapper CreateCSV()
        {
            return new UIHeroShowEffectCsv();
        }

        protected override void _Load()
        {
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                UIHeroShowEffectCsvData data = new UIHeroShowEffectCsvData();
                data.resId = m_csv.GetIntData(i, (int)eUIHeroShowEffectCsv.resId);
                data.enterAnimaTime = m_csv.GetFloatData(i, (int)eUIHeroShowEffectCsv.enterAnimaTime);
                data.enterAnimaName = m_csv.GetData(i, (int)eUIHeroShowEffectCsv.enterAnimaId);
                data.enterEffectTime = m_csv.GetData(i, (int)eUIHeroShowEffectCsv.enterEffectTime);
                data.enterEffectId = m_csv.GetData(i, (int)eUIHeroShowEffectCsv.enterEffectId);
                data.enterSpeakId = m_csv.GetIntData(i, (int)eUIHeroShowEffectCsv.enterHeroSpeakId);
                data.clickAnimaName = m_csv.GetData(i, (int)eUIHeroShowEffectCsv.clickAnimaId);
                data.clickEffectId = m_csv.GetIntData(i, (int)eUIHeroShowEffectCsv.clickEffectId);
                data.idleEffectId = m_csv.GetIntData(i, (int)eUIHeroShowEffectCsv.idleEffectId);
                data.bz = m_csv.GetData(i, (int)eUIHeroShowEffectCsv.bz);
                m_dicData.Add(data.resId, data);
            }
        }

        public UIHeroShowEffectCsvData GetData(int csvId)
        {
            UIHeroShowEffectCsvData data;
            if (m_dicData.TryGetValue(csvId, out data))
            {
                return data;
            }
            return null;
        }

        public Dictionary<int, UIHeroShowEffectCsvData> m_dicData = new Dictionary<int, UIHeroShowEffectCsvData>();
    }
}
