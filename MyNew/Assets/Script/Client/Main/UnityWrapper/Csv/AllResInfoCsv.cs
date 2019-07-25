using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

namespace Roma
{
    enum eCSV_Enum
    {
        eCE_ResID,
        eCE_ResName,
        eCE_url,
        eCE_type,
        eCE_bz,
    }

    public class AllResInfoCsv : CsvExWrapper
    {
        protected override void _Save()
        {
            // 将m_lstResinfo通过ID排序
            m_lstResinfo.Sort((x, y) =>
            { 
                if(x.nResID == y.nResID)
                {
                    return 0;
                }
                else if (x.nResID > y.nResID)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            });
            foreach (ResInfo key in m_lstResinfo)
            {
                int nRow = m_csv.AddRow();
                m_csv.SetData(nRow, (int)eCSV_Enum.eCE_ResID, key.nResID.ToString());
                m_csv.SetData(nRow, (int)eCSV_Enum.eCE_ResName, key.strName);
                m_csv.SetData(nRow, (int)eCSV_Enum.eCE_url, key.strUrl);
                m_csv.SetData(nRow, (int)eCSV_Enum.eCE_type, ResInfo.Type2String(key.iType));
                m_csv.SetData(nRow, (int)eCSV_Enum.eCE_bz, key.bz);
            }
        }

        public override void Clear()
        {
            base.Clear();
            m_mapNameResInfo.Clear();
            m_lstResinfo.Clear();
        }

        protected override void _Load()
        {
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                ResInfo res = new ResInfo();
                res.nResID = m_csv.GetIntData(i, (int)eCSV_Enum.eCE_ResID);
                res.strName = m_csv.GetData(i, (int)eCSV_Enum.eCE_ResName);
                res.strUrl = m_csv.GetData(i, (int)eCSV_Enum.eCE_url);
                res.iType = ResInfo.String2Type(m_csv.GetData(i, (int)eCSV_Enum.eCE_type));
                res.bz = m_csv.GetData(i, (int)eCSV_Enum.eCE_bz);
                m_lstResinfo.Add(res);
                m_mapNameResInfo[res.strName] = res;
                m_mapIDResInfo[res.nResID] = res;
            }
        }

        public Dictionary<string, ResInfo> m_mapNameResInfo = new Dictionary<string, ResInfo>();
        public Dictionary<int, ResInfo> m_mapIDResInfo = new Dictionary<int, ResInfo>();
        public List<ResInfo> m_lstResinfo = new List<ResInfo>();
    }
}
