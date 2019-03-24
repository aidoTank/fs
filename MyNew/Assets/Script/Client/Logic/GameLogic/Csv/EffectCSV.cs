using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Roma
{
    enum eEffectCSV_Enum
    {
        eEE_EffectID,       // ID
        eEE_ResID,          // 资源ID
        eEE_EffectDesc,     // 描述
        eEE_LiveTime,       // 生存时间
        eEE_SoundRes,      // SoundResID
    }

    public class EffectData
    {
        public int nEffectID;
        public int nResID;
        public string strDesc;
        public float fLiveTime = 0;
        public string uSoundID;
    }

    public class EffectCsv : CsvExWrapper
    {
        protected override void _Save()
        {
            base._Save();
            foreach (KeyValuePair<int, EffectData> key in m_mapEffectCSV)
            {
                int nRow = m_csv.AddRow();
                m_csv.SetData(nRow, (int)eEffectCSV_Enum.eEE_EffectID,  key.Value.nEffectID.ToString());
                m_csv.SetData(nRow, (int)eEffectCSV_Enum.eEE_ResID,     key.Value.nResID.ToString());
                m_csv.SetData(nRow, (int)eEffectCSV_Enum.eEE_EffectDesc,key.Value.strDesc);
                m_csv.SetData(nRow, (int)eEffectCSV_Enum.eEE_LiveTime,  key.Value.fLiveTime.ToString());
                m_csv.SetData(nRow, (int)eEffectCSV_Enum.eEE_SoundRes, key.Value.uSoundID.ToString());
            }
        }

        public override void Clear()
        {
            base.Clear();
            m_mapEffectCSV.Clear();
        }

        protected override void _Load()
        {
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                EffectData effect = new EffectData();
                effect.nEffectID = m_csv.GetIntData(i, (int)eEffectCSV_Enum.eEE_EffectID);
                effect.nResID = m_csv.GetIntData(i, (int)eEffectCSV_Enum.eEE_ResID);
                effect.strDesc = m_csv.GetData(i, (int)eEffectCSV_Enum.eEE_EffectDesc);
                effect.fLiveTime = m_csv.GetFloatData(i, (int)eEffectCSV_Enum.eEE_LiveTime);
                effect.uSoundID = m_csv.GetData(i, (int)eEffectCSV_Enum.eEE_SoundRes);

                m_mapEffectCSV.Add(effect.nEffectID, effect);
            }
        }

        static public CsvExWrapper CreateCSV()
        {
            return new EffectCsv();
        }

        public EffectData GetData(int uID)
        {
            EffectData effect = null;
            if (m_mapEffectCSV.TryGetValue(uID, out effect))
            {
                return effect;
            }
            return null;
        }


        public string[] GetNameList()
        {
            string[] nameList = new string[m_mapEffectCSV.Count];
            int i = 0;
            foreach (KeyValuePair<int, EffectData> item in m_mapEffectCSV)
            {
                nameList[i] = item.Value.strDesc;
                i++;
            }
            return nameList;
        }

        public int GetId(string npcName)
        {
            foreach (KeyValuePair<int, EffectData> item in m_mapEffectCSV)
            {
                if (item.Value.strDesc == npcName)
                {
                    return (int)item.Value.nEffectID;
                }
            }
            return 0;
        }

        public int GetIndex(int effecfId)
        {
            int index = 0;
            foreach (KeyValuePair<int, EffectData> item in m_mapEffectCSV)
            {
                if (item.Value.nEffectID == effecfId)
                    break;
                index++;
            }
            return index;
        }

        public int GetId(int index)
        {
            int curIndex = 0;
            foreach (KeyValuePair<int, EffectData> item in m_mapEffectCSV)
            {
                if (curIndex == index)
                    return (int)item.Key;
                curIndex++;
            }
            return curIndex;
        }

        public Dictionary<int, EffectData> m_mapEffectCSV = new Dictionary<int, EffectData>();
    }
}