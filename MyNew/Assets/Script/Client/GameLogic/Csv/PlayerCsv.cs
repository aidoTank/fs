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
		moveSpeed,
		force,
		agility,
		brain,
		armor,
		skill0,
	}

	public class PlayerCsvData
	{
		/// <summary>
		/// id
		/// </summary>
		public int Id;

		/// <summary>
		/// 名字
		/// </summary>
		public string Name;

		/// <summary>
		/// 图标
		/// </summary>
		public int icon;

		/// <summary>
		/// 模型资源id
		/// </summary>
		public int ModelResId;

		/// <summary>
		/// 移动速度
		/// </summary>
		public int moveSpeed;

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

		/// <summary>
		/// 护甲
		/// </summary>
		public int armor;

		/// <summary>
		/// 普攻技能
		/// </summary>
		public int skill0;

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
				data.Name = m_csv.GetData(i, (int)ePlayerCsv.Name);
				data.icon = m_csv.GetIntData(i, (int)ePlayerCsv.icon);
				data.ModelResId = m_csv.GetIntData(i, (int)ePlayerCsv.ModelResId);
				data.moveSpeed = m_csv.GetIntData(i, (int)ePlayerCsv.moveSpeed);
				data.force = m_csv.GetIntData(i, (int)ePlayerCsv.force);
				data.agility = m_csv.GetIntData(i, (int)ePlayerCsv.agility);
				data.brain = m_csv.GetIntData(i, (int)ePlayerCsv.brain);
				data.armor = m_csv.GetIntData(i, (int)ePlayerCsv.armor);
				data.skill0 = m_csv.GetIntData(i, (int)ePlayerCsv.skill0);
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
