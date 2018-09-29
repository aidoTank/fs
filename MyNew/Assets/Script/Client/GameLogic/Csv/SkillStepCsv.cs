using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
	public enum eSkillStepCsv
	{
		id,
		name,
		step,
		animaName,
		effectId,
		bindPoint,
		startTime,
		speed,
	}

	public class SkillStepCsvData
	{
		/// <summary>
		/// ����ID
		/// </summary>
		public int id;

		/// <summary>
		/// ��������
		/// </summary>
		public string name;

		/// <summary>
		/// ���ܽ׶� 1�ͷ� 2�ܻ� 3���� 4�����Ա�
		/// </summary>
		public int step;

		/// <summary>
		/// ������
		/// </summary>
		public int animaName;

		/// <summary>
		/// ��Чid
		/// </summary>
		public int effectId;

		/// <summary>
		/// ��Ч�󶨵�
		/// </summary>
		public int bindPoint;

		/// <summary>
		/// ��Ч��ʼʱ��
		/// </summary>
		public int startTime;

		/// <summary>
		/// �����ٶ�
		/// </summary>
		public int speed;

	}

	public class SkillStepCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new SkillStepCsv();
		}

		protected override void _Load()
		{
			for (int i = 0; i < m_csv.GetRows(); i++)
			{
				SkillStepCsvData data = new SkillStepCsvData();
				data.id = m_csv.GetIntData(i, (int)eSkillStepCsv.id);
				data.name = m_csv.GetData(i, (int)eSkillStepCsv.name);
				data.step = m_csv.GetIntData(i, (int)eSkillStepCsv.step);
				data.animaName = m_csv.GetIntData(i, (int)eSkillStepCsv.animaName);
				data.effectId = m_csv.GetIntData(i, (int)eSkillStepCsv.effectId);
				data.bindPoint = m_csv.GetIntData(i, (int)eSkillStepCsv.bindPoint);
				data.startTime = m_csv.GetIntData(i, (int)eSkillStepCsv.startTime);
				data.speed = m_csv.GetIntData(i, (int)eSkillStepCsv.speed);
				m_dicData.Add(i, data);
			}
		}

		public SkillStepCsvData GetData(int csvId)
		{
			SkillStepCsvData data;
			if (m_dicData.TryGetValue(csvId, out data))
			{
				return data;
			}
			return null;
		}

		public Dictionary<int, SkillStepCsvData> m_dicData = new Dictionary<int, SkillStepCsvData>();
	}
}
