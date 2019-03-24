using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Roma
{
    enum eGameTextCsv
    {
        eTextID,
        eModule,
        eGameText,
    }

    public class GameTextData
    {
        public int      id;
        public string textId;
        public string    gameText;
    }

    public class GameTextCsv : CsvExWrapper
    {
        protected override void _Save()
        {
            base._Save();
        }

        public override void Clear()
        {
            base.Clear();
            m_dicGameText.Clear();
        }

        protected override void _Load()
        {
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                GameTextData text = new GameTextData();
                text.id = m_csv.GetIntData(i, (int)eGameTextCsv.eTextID);
                text.textId = m_csv.GetData(i, (int)eGameTextCsv.eModule);
                text.gameText = m_csv.GetData(i, (int)eGameTextCsv.eGameText);
                m_dicGameText.Add(text.id, text);
                if (!string.IsNullOrEmpty(text.textId))
                {
                    if (m_dicGameTextByTxtId.ContainsKey(text.textId))
                    {
                        Debug.LogError("游戏文本表有相同的中文key:" + text.textId);
                        continue;
                    }
                    m_dicGameTextByTxtId.Add(text.textId, text.id);
                }
            }
        }

        static public CsvExWrapper CreateCSV()
        {
            return new GameTextCsv();
        }

        public GameTextData GetTextData(int id)
        {
            GameTextData text;
            if (m_dicGameText.TryGetValue(id, out text))
            {
                return text;
            }
            return null;
        }

        public string GetText(int id)
        {
            GameTextData text;
            if (m_dicGameText.TryGetValue(id, out text))
            {
                return text.gameText;
            }
            return "";
        }


        public string GetTextByTxtId(string txtId)
        {
            int id = 0;
            if (m_dicGameTextByTxtId.TryGetValue(txtId, out id))
            {
                return GetText(id);
            }
            return "";
        }

        public Dictionary<int, GameTextData> m_dicGameText = new Dictionary<int, GameTextData>();
        public Dictionary<string, int> m_dicGameTextByTxtId = new Dictionary<string, int>();
    }
}