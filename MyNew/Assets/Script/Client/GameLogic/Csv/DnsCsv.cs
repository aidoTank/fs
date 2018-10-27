using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
	public enum eDnsCsv
	{
		www,
		ip,
		bz,
	}

	public class DnsCsvData
	{
		/// <summary>
		/// 域名
		/// </summary>
		public string www;

		/// <summary>
		/// ip地址
		/// </summary>
		public string ip;

		/// <summary>
		/// 备注
		/// </summary>
		public string bz;

	}

	public class DnsCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new DnsCsv();
		}

		protected override void _Load()
		{
			for (int i = 0; i < m_csv.GetRows(); i++)
			{
				DnsCsvData data = new DnsCsvData();
				data.www = m_csv.GetData(i, (int)eDnsCsv.www);
				data.ip = m_csv.GetData(i, (int)eDnsCsv.ip);
				data.bz = m_csv.GetData(i, (int)eDnsCsv.bz);
				m_dicData.Add(data.www, data);
			}
		}

		public DnsCsvData GetData(string www)
		{
			DnsCsvData data;
			if (m_dicData.TryGetValue(www, out data))
			{
				return data;
			}
			return null;
		}

		public Dictionary<string, DnsCsvData> m_dicData = new Dictionary<string, DnsCsvData>();
	}
}
