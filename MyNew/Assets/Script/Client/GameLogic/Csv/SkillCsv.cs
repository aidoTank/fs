using UnityEngine;
using System.Collections.Generic;
namespace Roma
{

	public enum eSkillCsv
	{
		id,
		name,
		type,
		skillType,
		selectTargetTpye,
		distance,
		length,
		width,
	}

    // ѡ��Ŀ�귽ʽ 1.�Լ� 2.Ŀ�� 3.���� 4.λ��
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
		/// ѡ��Ŀ�귽ʽ 1.�Լ� 2.Ŀ�� 3.���� 4.λ��
		/// </summary>
		public int selectTargetType;

		/// <summary>
		/// �������ͣ���ͬ��ʽ�� 1.��ս 2.�ӵ� 3.Զ������ 4.����
		/// </summary>
		public int skillType;


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
				data.skillType = m_csv.GetIntData(i, (int)eSkillCsv.skillType);
				data.selectTargetType = m_csv.GetIntData(i, (int)eSkillCsv.selectTargetTpye);
				data.distance = m_csv.GetIntData(i, (int)eSkillCsv.distance);
				data.length = m_csv.GetIntData(i, (int)eSkillCsv.length);
				data.width = m_csv.GetIntData(i, (int)eSkillCsv.width);
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
