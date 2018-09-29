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

    // 选择目标方式 1.自己 2.目标 3.方向 4.位置
    public enum eSelectTargetType
    {
       None = 0,          
       Self,           
       Creature,          
       Dir,
       Pos,               
    }

	// 技能类型  1.近战 2.子弹 3.远程区域 4.陷阱
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
		/// 技能ID
		/// </summary>
		public int id;

		/// <summary>
		/// 技能名称
		/// </summary>
		public string name;

		/// <summary>
		/// 技能大类型 1.主动 2.被动
		/// </summary>
		public int type;

		/// <summary>
		/// 选择目标方式 1.自己 2.目标 3.方向 4.位置
		/// </summary>
		public int selectTargetType;

		/// <summary>
		/// 技能类型（不同方式） 1.近战 2.子弹 3.远程区域 4.陷阱
		/// </summary>
		public int skillType;


		/// <summary>
		/// 施法距离
		/// </summary>
		public int distance;

		/// <summary>
		/// 范围长度（长度 半径）
		/// </summary>
		public int length;

		/// <summary>
		/// 范围宽度(宽度 扇形角度)
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
