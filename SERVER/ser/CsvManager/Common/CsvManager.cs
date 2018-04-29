using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum eCSV
{
    eNone = 0,
    eMap = 1,
}

public class CsvManager : Singleton
{
    public CsvManager() : base(true) { }

    public override void Init()
    {
        AddCSVCreate((int)eCSV.eMap, "地图表.csv", MapCsv.CreateCSV);

        foreach (KeyValuePair<int, CreateCSV> item in m_mapCSVCreate)
        {
            CsvExWrapper csv = GetOrCreateGetCsv(item.Key);
            string name = GetName(item.Key);
            csv.Load("D:/Server/C#_Server/sev_c#/CLIENT_DEV/Assets/Config/"+ name, Encoding.Default);
        }
        Console.WriteLine("CSV初始化成功");
    }


    public T GetCsv<T>(eCSV key) where T : CsvExWrapper
    {
        CsvExWrapper csv;
        if (m_mapCSV.TryGetValue((int)key, out csv))
        {
            System.Object obj = csv;
            return (T)obj;
        }
        return default(T);
    }

    public CsvExWrapper GetOrCreateGetCsv(int csv)
    {
        CsvExWrapper csvWr = null;
        if (!m_mapCSV.TryGetValue(csv, out csvWr))
        {
            CreateCSV csvCreate;
            if (m_mapCSVCreate.TryGetValue(csv, out csvCreate))
            {
                csvWr = csvCreate();
                m_mapCSV.Add(csv, csvWr);
            }
        }
        return (CsvExWrapper)csvWr;
    }

    public string GetType(int eac)
    {
        string strOut = "";
        if (m_mapCSVType2String.TryGetValue(eac, out strOut))
        {
            return strOut;
        }
        return strOut;
    }

    public int GetType(string strValue)
    {
        int eac = 0;
        if (m_mapCSVString2Type.TryGetValue(strValue, out eac))
        {
            return eac;
        }
        return eac;
    }

    public string GetName(int type)
    {
        string name = "";
        if (m_mapCSVType2String.TryGetValue(type, out name))
        {
            return name;
        }
        return name;
    }

    public void AddCSVCreate(int iType, string str, CreateCSV create)
    {
        m_mapCSVCreate.Add(iType, create);
        m_mapCSVString2Type.Add(str, iType);
        m_mapCSVType2String.Add(iType, str);
    }

    public void RemoveCSV(int e)
    {
        if (m_mapCSV.ContainsKey(e))
        {
            m_mapCSV.Remove(e);
        }
    }

    public override void UnInit()
    {
        m_mapCSV.Clear();
        m_mapCSVCreate.Clear();
        m_mapCSVType2String.Clear();
        m_mapCSVString2Type.Clear();
    }

    private Dictionary<int, CsvExWrapper> m_mapCSV = new Dictionary<int, CsvExWrapper>();
    private Dictionary<int, CreateCSV> m_mapCSVCreate = new Dictionary<int, CreateCSV>();
    private Dictionary<int, string> m_mapCSVType2String = new Dictionary<int, string>();
    private Dictionary<string, int> m_mapCSVString2Type = new Dictionary<string, int>();
    public delegate CsvExWrapper CreateCSV();
    public static CsvManager Inst = null;
}
