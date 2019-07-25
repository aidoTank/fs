using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
	public enum ePlayerCsv
	{
		Id,
		Name,
		icon,
		ModelResId,
        ModelScale,
        headHeight,
        moveSpeed,
        dieDelay,

        dieSound,    // 暂不读
        speakId,    // 暂不读

        BaseHp,
        BaseAp,
        BaseDp,

        HpGrow,
        ApGrow,
        DpGrow,

        CritChance,
        CritDamage,
    }

    public class CreatureCsvData
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id;

        public string languageId;
        /// <summary>
        /// 名字
        /// </summary>
        public string Name
        {
           get
           {
                // 通过多语言id,查询名称
                //string languageText = LanguageCsv.GetText(languageId);
                //return languageText;
                return "";
           }
        }


        /// <summary>
        /// 图标
        /// </summary>
        public int Icon;

        /// <summary>
        /// 模型资源id
        /// </summary>
        public int ModelResId;
        public float ModelScale;
        public float HeadHeight;

        /// <summary>
        /// 移动速度
        /// </summary>
        public float moveSpeed;
        public int dieDelay;

        public int dieSound;
        public int dieEffect;
        public int speakId;

        /// <summary>
        /// 力量
        /// </summary>
        public int force;

        /// <summary>
        /// 敏捷
        /// </summary>
        public int agility;

        /// <summary>
        /// 智力
        /// </summary>
        public int brain;

        public int atk;

        /// <summary>
        /// 护甲
        /// </summary>
        public int armor;
    }


    public class PlayerCsvData : CreatureCsvData
    {

        public int BaseHp;
        /// <summary>
        /// 基础攻击
        /// </summary>
        public int BaseAp;
        /// <summary>
        /// 基础防御
        /// </summary>
        public int BaseDp;

        /// <summary>
        /// 血量成长系数
        /// </summary>
        public float HpGrow;
        /// <summary>
        /// 攻击成长系数
        /// </summary>
        public float ApGrow;
        /// <summary>
        /// 防御成长系数
        /// </summary>
        public float DpGrow;

        /// <summary>
        /// 暴击率
        /// </summary>
        public float CritChance;
        /// <summary>
        /// 暴击伤害
        /// </summary>
        public float CritDamage;


    }

	public class PlayerCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new PlayerCsv();
		}

		protected override void _Load()
		{
			for (int i = 0; i < m_csv.GetRows(); i++)
			{
				PlayerCsvData data = new PlayerCsvData();
				data.Id = m_csv.GetIntData(i, (int)ePlayerCsv.Id);
				//data.Name = m_csv.GetData(i, (int)ePlayerCsv.Name);
				data.Icon = m_csv.GetIntData(i, (int)ePlayerCsv.icon);
				data.ModelResId = m_csv.GetIntData(i, (int)ePlayerCsv.ModelResId);
                data.ModelScale = m_csv.GetFloatData(i, (int)ePlayerCsv.ModelScale);
                data.HeadHeight = m_csv.GetFloatData(i, (int)ePlayerCsv.headHeight);

                data.moveSpeed = m_csv.GetIntData(i, (int)ePlayerCsv.moveSpeed);
                data.dieDelay = m_csv.GetIntData(i, (int)ePlayerCsv.dieDelay);

                data.BaseHp = m_csv.GetIntData(i, (int)ePlayerCsv.BaseHp);
                data.BaseAp = m_csv.GetIntData(i, (int)ePlayerCsv.BaseAp);
                data.BaseDp = m_csv.GetIntData(i, (int)ePlayerCsv.BaseDp);

                data.HpGrow = m_csv.GetFloatData(i, (int)ePlayerCsv.HpGrow);
                data.ApGrow = m_csv.GetFloatData(i, (int)ePlayerCsv.ApGrow);
                data.DpGrow = m_csv.GetFloatData(i, (int)ePlayerCsv.DpGrow);

                data.CritChance = m_csv.GetFloatData(i, (int)ePlayerCsv.CritChance);
                data.CritDamage = m_csv.GetFloatData(i, (int)ePlayerCsv.CritDamage);

                m_dicData.Add(data.Id, data);
			}
		}

		public PlayerCsvData GetData(int csvId)
		{
			PlayerCsvData data;
			if (m_dicData.TryGetValue(csvId, out data))
			{
				return data;
			}
			return null;
		}

		public Dictionary<int, PlayerCsvData> m_dicData = new Dictionary<int, PlayerCsvData>();
	}
}
