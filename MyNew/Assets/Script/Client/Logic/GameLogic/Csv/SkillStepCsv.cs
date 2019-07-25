using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
	public enum eSkillStepCsv
	{
		id,
		name,
		animaName,
		effectId,
		bindPoint,
		startTime,
        ForwardAnimaId,
        BackAnimaId,
	}


	//public enum eSkillStepType
	//{
	//	Caster = 1,
 //       Hit = 2,
 //       Fly = 3,
 //       FlyHit = 4,
	//}

	public class SkillStepCsvData
	{
		/// <summary>
		/// 技能ID
		/// </summary>
		public int id;

		/// <summary>
		/// 技能名称
		/// </summary>
		public string name;

		/// <summary>
		/// 动作名
		/// </summary>
		public int animaName;

		/// <summary>
		/// 特效id
		/// </summary>
		public int effectId;

		/// <summary>
		/// 特效绑定点
		/// </summary>
		public string bindPoint;

		/// <summary>
		/// 特效开始时间
		/// </summary>
		public int startTime;
        public int forwardAnimaId;
        public int backAnimaId;
	}

	public class SkillStepCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new SkillStepCsv();
		}

		protected override void _Load()
		{
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
			{
				SkillStepCsvData data = new SkillStepCsvData();
				data.id = m_csv.GetIntData(i, (int)eSkillStepCsv.id);
				data.name = m_csv.GetData(i, (int)eSkillStepCsv.name);
				data.animaName = m_csv.GetIntData(i, (int)eSkillStepCsv.animaName);
				data.effectId = m_csv.GetIntData(i, (int)eSkillStepCsv.effectId);
				data.bindPoint = m_csv.GetData(i, (int)eSkillStepCsv.bindPoint);
				data.startTime = m_csv.GetIntData(i, (int)eSkillStepCsv.startTime);
                data.forwardAnimaId = m_csv.GetIntData(i, (int)eSkillStepCsv.ForwardAnimaId);
                data.backAnimaId = m_csv.GetIntData(i, (int)eSkillStepCsv.BackAnimaId);
                m_dicData.Add(data);
			}
		}


		public SkillStepCsvData GetCasterData(int skillId)
        {
            foreach (SkillStepCsvData item in m_dicData)
            {
                if (item.id == skillId)
                {
                    return item;
                }
            }
            return null;
        }

		 public List<SkillStepCsvData> m_dicData = new List<SkillStepCsvData>();
	}
}
