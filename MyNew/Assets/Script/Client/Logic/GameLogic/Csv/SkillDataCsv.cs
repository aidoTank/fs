using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    public enum eSkillCdType
    {
        Time,
        Charge,
    }

	public enum eSkillDataCsv
	{
		id,
		name,
        cdType,
		cd,

        CasterBuffDesc,
        CasterBuffValue,

        AtkBuffDesc,
        AtkBuffValue,

        HitBuffDesc,
        HitBuffValue,
    }

	public class SkillDataCsvData
	{
		/// <summary>
		/// id
		/// </summary>
		public int id;

		/// <summary>
		/// ¼¼ÄÜÃû³Æ
		/// </summary>
		public string name;

        public int cdType;
		/// <summary>
		/// CD
		/// </summary>
		public int cd;


        public int[] casterSelfBuffList;

        public int[] atkBuffList;
        public int[] hitBuffList;

        public bool ContainAtkBuff(int buffId)
        {
            if (atkBuffList == null)
                return false;
            for(int i = 0; i < atkBuffList.Length; i ++)
            {
                if (atkBuffList[i] == buffId)
                    return true;
            }
            return false;
        }
    }

	public class SkillDataCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new SkillDataCsv();
		}

		protected override void _Load()
		{
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
			{
				SkillDataCsvData data = new SkillDataCsvData();
				data.id = m_csv.GetIntData(i, (int)eSkillDataCsv.id);
				data.name = m_csv.GetData(i, (int)eSkillDataCsv.name);
                data.cdType = m_csv.GetIntData(i, (int)eSkillDataCsv.cdType);
                data.cd = m_csv.GetIntData(i, (int)eSkillDataCsv.cd);

				string casterInfo = m_csv.GetData(i, (int)eSkillDataCsv.CasterBuffValue);
                data.casterSelfBuffList = StringHelper.GetIntList(casterInfo);

                string atkInfo = m_csv.GetData(i, (int)eSkillDataCsv.AtkBuffValue);
                data.atkBuffList = StringHelper.GetIntList(atkInfo);

                string hitInfo = m_csv.GetData(i, (int)eSkillDataCsv.HitBuffValue);
                data.hitBuffList = StringHelper.GetIntList(hitInfo);

                m_dicData.Add(data.id, data);
			}
		}

		public SkillDataCsvData GetData(int csvId)
		{
			SkillDataCsvData data;
			if (m_dicData.TryGetValue(csvId, out data))
			{
				return data;
			}
			return null;
		}

		public Dictionary<int, SkillDataCsvData> m_dicData = new Dictionary<int, SkillDataCsvData>();
	}
}
