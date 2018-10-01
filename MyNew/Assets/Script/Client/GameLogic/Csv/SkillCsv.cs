using UnityEngine;
using System.Collections.Generic;
namespace Roma
{

	public enum eSkillCsv
	{
		id,
		name,
		type,
		selectTargetType,
		skillType,
		checkTargetType,
		distance,
		length,
		width,
		launchTime,
		hitTime,
		flySpeed,
	}

    // ָʾ������ 1.�Լ� 2.Ŀ�� 3.���� 4.λ��
    public enum eSelectTargetType
    {
       None = 0,          
       Self,           
       Creature,          
       Dir,
       Pos,               
    }

	// ��������  1.��ս 2.�ӵ� 3.Զ������ 4.����
    public enum eSkillType
    {
       None = 0,         
       Near,                 
       Fly,     
	   Aoe,            
    }

	public enum eCheckTargetType
	{
		None,
		Sector,
		Circle,
		Rect,
	}

	public class SkillCsvData
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
		/// ���ܴ����� 1.���� 2.����
		/// </summary>
		public int type;

		/// <summary>
		/// ָʾ������ 1.�Լ� 2.Ŀ�� 3.���� 4.λ��
		/// </summary>
		public int selectTargetType;

		/// <summary>
		/// �������ͣ���ͬʵ���ࣩ 1.��ս 2.�ӵ� 3.Զ������ 4.����
		/// </summary>
		public int skillType;

		/// <summary>
		/// ���Ŀ�귽ʽ(ʵ�������״) 1���� 2Բ�� 3����
		/// </summary>
		public int checkTargetType;

		/// <summary>
		/// ʩ������
		/// </summary>
		public int distance;

		/// <summary>
		/// ��Χ���ȣ����� �뾶��
		/// </summary>
		public int length;

		/// <summary>
		/// ��Χ���(��� ���νǶ�)
		/// </summary>
		public int width;

		public int launchTime;
		public int hitTime;
		public int flySpeed;
	}

	public class SkillCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new SkillCsv();
		}

		protected override void _Load()
		{
			for (int i = 0; i < m_csv.GetRows(); i++)
			{
				SkillCsvData data = new SkillCsvData();
				data.id = m_csv.GetIntData(i, (int)eSkillCsv.id);
				data.name = m_csv.GetData(i, (int)eSkillCsv.name);
				data.type = m_csv.GetIntData(i, (int)eSkillCsv.type);
				data.selectTargetType = m_csv.GetIntData(i, (int)eSkillCsv.selectTargetType);
				data.skillType = m_csv.GetIntData(i, (int)eSkillCsv.skillType);
				data.checkTargetType = m_csv.GetIntData(i, (int)eSkillCsv.checkTargetType);
				data.distance = m_csv.GetIntData(i, (int)eSkillCsv.distance);
				data.length = m_csv.GetIntData(i, (int)eSkillCsv.length);
				data.width = m_csv.GetIntData(i, (int)eSkillCsv.width);
				data.launchTime = m_csv.GetIntData(i, (int)eSkillCsv.launchTime);
				data.hitTime = m_csv.GetIntData(i, (int)eSkillCsv.hitTime);
				data.flySpeed = m_csv.GetIntData(i, (int)eSkillCsv.flySpeed);
				m_dicData.Add(data.id, data);
			}
		}

		public SkillCsvData GetData(int csvId)
		{
			SkillCsvData data;
			if (m_dicData.TryGetValue(csvId, out data))
			{
				return data;
			}
			return null;
		}

		public Dictionary<int, SkillCsvData> m_dicData = new Dictionary<int, SkillCsvData>();
	}
}
