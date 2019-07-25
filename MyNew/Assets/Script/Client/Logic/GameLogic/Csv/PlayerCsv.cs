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
		headHeight,

		force,
		agility,
		brain,
		atk,
		armor,
		moveSpeed,
		dieDelay,
	}

    public class CreatureCsvData
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id;

        public string languageId;
        /// <summary>
        /// ����
        /// </summary>
        public string Name
        {
           get
           {
                // ͨ��������id,��ѯ����
                //string languageText = LanguageCsv.GetText(languageId);
                //return languageText;
                return "";
           }
            
        }


        /// <summary>
        /// ͼ��
        /// </summary>
        public int Icon;

        /// <summary>
        /// ģ����Դid
        /// </summary>
        public int ModelResId;
        public float ModelScale;
        public float HeadHeight;

        /// <summary>
        /// �ƶ��ٶ�
        /// </summary>
        public float moveSpeed;
        public int dieDelay;
        public int dieSound;
        public int dieEffect;
        public int speakId;

        /// <summary>
        /// ����
        /// </summary>
        public int force;

        /// <summary>
        /// ����
        /// </summary>
        public int agility;

        /// <summary>
        /// ����
        /// </summary>
        public int brain;

        public int atk;

        /// <summary>
        /// ����
        /// </summary>
        public int armor;
    }


    public class PlayerCsvData : CreatureCsvData
    {






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
				data.HeadHeight = m_csv.GetIntData(i, (int)ePlayerCsv.headHeight);

				data.force = m_csv.GetIntData(i, (int)ePlayerCsv.force);
				data.agility = m_csv.GetIntData(i, (int)ePlayerCsv.agility);
				data.brain = m_csv.GetIntData(i, (int)ePlayerCsv.brain);
				data.atk = m_csv.GetIntData(i, (int)ePlayerCsv.atk);
				data.armor = m_csv.GetIntData(i, (int)ePlayerCsv.armor);
				data.moveSpeed = m_csv.GetIntData(i, (int)ePlayerCsv.moveSpeed);
				data.dieDelay = m_csv.GetIntData(i, (int)ePlayerCsv.dieDelay);
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
