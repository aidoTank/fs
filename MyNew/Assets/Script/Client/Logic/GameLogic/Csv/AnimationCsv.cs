using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    public enum eAnimationCsv
    {
        animationID,
        animationName,
        fadeLength,
        speed,
        mode,
        length,
        priority,
        atOnce,
        switchHand,
        name,
    }

    public class AnimationCsvData
    {
        /// <summary>
        /// 动作ID
        /// </summary>
        public int animationID;

        /// <summary>
        /// 动作名字
        /// </summary>
        public string animationName;

        /// <summary>
        /// 切换速度
        /// </summary>
        public float fadeLength;

        /// <summary>
        /// 播放速度
        /// </summary>
        public float speed;

        /// <summary>
        /// 播放模式0播一次 1循环播
        /// </summary>
        public int mode;

        /// <summary>
        /// 长度 时间长度
        /// </summary>
        public float length;

        /// <summary>
        /// 优先级 die99 stand0 astand0 jump0  run0  skill 60  hit70  gosh80  victory90
        /// </summary>
        public int priority;
        public bool atOnce;
        public bool switchHand;
        /// <summary>
        /// 备注
        /// </summary>
        public string name;

    }

    public class AnimationCsv : CsvExWrapper
    {
        static public CsvExWrapper CreateCSV()
        {
            return new AnimationCsv();
        }

        protected override void _Load()
        {
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                AnimationCsvData data = new AnimationCsvData();
                data.animationID = m_csv.GetIntData(i, (int)eAnimationCsv.animationID);
                data.animationName = m_csv.GetData(i, (int)eAnimationCsv.animationName);
                data.fadeLength = m_csv.GetFloatData(i, (int)eAnimationCsv.fadeLength);
                data.speed = m_csv.GetFloatData(i, (int)eAnimationCsv.speed);
                data.mode = m_csv.GetIntData(i, (int)eAnimationCsv.mode);
                data.length = m_csv.GetFloatData(i, (int)eAnimationCsv.length);
                data.priority = m_csv.GetIntData(i, (int)eAnimationCsv.priority);
                data.atOnce = m_csv.GetBoolData(i, (int)eAnimationCsv.atOnce);
                data.switchHand = m_csv.GetBoolData(i, (int)eAnimationCsv.switchHand);
                data.name = m_csv.GetData(i, (int)eAnimationCsv.name);
                m_dicData.Add(data.animationID, data);
            }
        }

        public AnimationCsvData GetData(int csvId)
        {
            AnimationCsvData data;
            if (m_dicData.TryGetValue(csvId, out data))
            {
                return data;
            }
            return null;
        }

        public Dictionary<int, AnimationCsvData> m_dicData = new Dictionary<int, AnimationCsvData>();
    }
}
