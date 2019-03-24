using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
	public enum eSceneBarrierCsv
	{
		mapId,
		mapName,
		shapeType,
		objName,
		pos,
		dir,
		scale,
	}

	public class SceneBarrierCsvData
	{
		/// <summary>
		/// mapId
		/// </summary>
		public int mapId;

		/// <summary>
		/// 地图名字
		/// </summary>
		public string mapName;

		/// <summary>
		/// 形状类型（1矩形 2圆形 3三角形）
		/// </summary>
		public int shapeType;

		/// <summary>
		/// 障碍物名字
		/// </summary>
		public string objName;

		/// <summary>
		/// 位置
		/// </summary>
		public string pos;
		public Vector3 vPos;

		/// <summary>
		/// 方向
		/// </summary>
		public string dir;
		public Vector3 vDir;

		/// <summary>
		/// 缩放
		/// </summary>
		public string scale;
		public Vector3 vScale;

	}

	public class SceneBarrierCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new SceneBarrierCsv();
		}

		protected override void _Load()
		{
			for (int i = 0; i < m_csv.GetRows(); i++)
			{
				SceneBarrierCsvData data = new SceneBarrierCsvData();
				data.mapId = m_csv.GetIntData(i, (int)eSceneBarrierCsv.mapId);
				data.mapName = m_csv.GetData(i, (int)eSceneBarrierCsv.mapName);
				data.shapeType = m_csv.GetIntData(i, (int)eSceneBarrierCsv.shapeType);
				data.objName = m_csv.GetData(i, (int)eSceneBarrierCsv.objName);
				data.pos = m_csv.GetData(i, (int)eSceneBarrierCsv.pos);
				data.dir = m_csv.GetData(i, (int)eSceneBarrierCsv.dir);
				data.scale = m_csv.GetData(i, (int)eSceneBarrierCsv.scale);

				data.vPos = MathEx.GetVector3(data.pos);
				data.vDir = MathEx.GetVector3(data.dir);
				data.vScale = MathEx.GetVector3(data.scale);
				m_dicData.Add(i, data);
			}
		}

		public void GetData(ref List<SceneBarrierCsvData> list, int mapId)
		{
			foreach(KeyValuePair<int, SceneBarrierCsvData> item in m_dicData)
			{
				if(mapId == item.Value.mapId)
				{
					list.Add(item.Value);
				}
			}
		}

		public Dictionary<int, SceneBarrierCsvData> m_dicData = new Dictionary<int, SceneBarrierCsvData>();
	}
}
