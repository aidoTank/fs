using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    public enum eUIHeroShowCsv
    {
        resId,
        pos,
        rota,
        orthographic,
        FovOrSize,

        lightPos_1,
        lightColor_1,
        lightIntensity_1,
        lightPos_2,
        lightColor_2,
        lightIntensity_2,
    }

    public class UIHeroShowCsvData
    {
        /// <summary>
        /// 资源id
        /// </summary>
        public int resId;

        /// <summary>
        /// 摄像机位置
        /// </summary>
        public string pos;
        public Vector3 vPos;
        /// <summary>
        /// 摄像机旋转
        /// </summary>
        public string rota;
        public Vector3 vRota;

        public int orthographic;
        public float fovOrSize;

        /// <summary>
        /// 灯光1位置
        /// </summary>
        public string lightPos_1;
        public Vector3 vLightPos_1;
        /// <summary>
        /// 灯光1颜色
        /// </summary>
        public string lightColor_1;
        public Color vLightColor_1;

        /// <summary>
        /// 灯光1强度
        /// </summary>
        public float lightIntensity_1;

        /// <summary>
        /// 灯光2位置
        /// </summary>
        public string lightPos_2;
        public Vector3 vLightPos_2;
        /// <summary>
        /// 灯光2颜色
        /// </summary>
        public string lightColor_2;
        public Color vLightColor_2;
        /// <summary>
        /// 灯光2强度
        /// </summary>
        public float lightIntensity_2;

    }

    public class UIHeroShowCsv : CsvExWrapper
    {
        static public CsvExWrapper CreateCSV()
        {
            return new UIHeroShowCsv();
        }

        protected override void _Load()
        {
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                UIHeroShowCsvData data = new UIHeroShowCsvData();
                data.resId = m_csv.GetIntData(i, (int)eUIHeroShowCsv.resId);
                data.pos = m_csv.GetData(i, (int)eUIHeroShowCsv.pos);
                data.rota = m_csv.GetData(i, (int)eUIHeroShowCsv.rota);
                data.vPos = StringHelper.GetVector3(data.pos);
                data.vRota = StringHelper.GetVector3(data.rota);

                data.orthographic = m_csv.GetIntData(i, (int)eUIHeroShowCsv.orthographic);
                data.fovOrSize = m_csv.GetFloatData(i, (int)eUIHeroShowCsv.FovOrSize);

                data.lightPos_1 = m_csv.GetData(i, (int)eUIHeroShowCsv.lightPos_1);
                data.vLightPos_1 = StringHelper.GetVector3(data.lightPos_1);
                data.lightColor_1 = m_csv.GetData(i, (int)eUIHeroShowCsv.lightColor_1);
                data.vLightColor_1 = StringHelper.GetColor(data.lightColor_1);
                data.lightIntensity_1 = m_csv.GetFloatData(i, (int)eUIHeroShowCsv.lightIntensity_1);

                data.lightPos_2 = m_csv.GetData(i, (int)eUIHeroShowCsv.lightPos_2);
                data.vLightPos_2 = StringHelper.GetVector3(data.lightPos_2);
                data.lightColor_2 = m_csv.GetData(i, (int)eUIHeroShowCsv.lightColor_2);
                data.vLightColor_2 = StringHelper.GetColor(data.lightColor_2);
                data.lightIntensity_2 = m_csv.GetFloatData(i, (int)eUIHeroShowCsv.lightIntensity_2);
                m_dicData.Add(data.resId, data);
            }
        }

        public UIHeroShowCsvData GetData(int csvId)
        {
            UIHeroShowCsvData data;
            if (m_dicData.TryGetValue(csvId, out data))
            {
                return data;
            }
            return null;
        }

        public Dictionary<int, UIHeroShowCsvData> m_dicData = new Dictionary<int, UIHeroShowCsvData>();
    }
}
