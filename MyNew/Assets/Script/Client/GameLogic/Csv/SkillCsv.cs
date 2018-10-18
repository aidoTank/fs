using UnityEngine;
using System.Collections.Generic;
namespace Roma
{

	public enum eSkillCsv
	{
		id,
		name,
		icon,
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
		ad,
		pd,
		lifeTime,
	}

    // 指示器类型 1.自己 2.目标 3.方向 4.位置
    public enum eSelectTargetType
    {
       None = 0,          
       Self,           
       SectorDir,          
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
	   Jump,           
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
		/// 技能ID
		/// </summary>
		public int id;

		/// <summary>
		/// 技能名称
		/// </summary>
		public string name;
		public int icon;

		/// <summary>
		/// 技能大类型 1.主动 2.被动
		/// </summary>
		public int type;

		/// <summary>
		/// 指示器类型 1.自己 2.目标 3.方向 4.位置
		/// </summary>
		public int selectTargetType;

		/// <summary>
		/// 技能类型（不同实体类） 1.近战 2.子弹 3.远程区域 4.陷阱
		/// </summary>
		public int skillType;

		/// <summary>
		/// 检测目标方式(实体类的形状) 1扇形 2圆形 3矩形 (暂时无用)
		/// </summary>
		public int checkTargetType;

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

		public int launchTime;
		// 暂时无用，近战 AOE都用launchTime作为hit时间
		public int  hitTime;
		public int flySpeed;
		public int ad;
		public int pd;
		public int lifeTime;
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
				data.icon = m_csv.GetIntData(i, (int)eSkillCsv.icon);
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
				data.ad = m_csv.GetIntData(i, (int)eSkillCsv.ad);
				data.pd = m_csv.GetIntData(i, (int)eSkillCsv.pd);
				data.lifeTime = m_csv.GetIntData(i, (int)eSkillCsv.lifeTime);
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
