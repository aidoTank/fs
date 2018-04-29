using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roma;

namespace Roma
{
    // C#配置表
    public enum eAllCSV
    {
        eAC_None = 0,
        eAC_GameText,
        eAC_Map,
        eAC_Player,
        eAC_Effect,
    }

    public partial class LogicSystem
    {
        /// <summary>
        /// lua的配置表
        /// 如果项目上线了，这里一样可以添加打包，并且支持lua配置表的热更
        /// </summary>
        private List<KeyValuePair<int, string>> m_listLuaCsv = new List<KeyValuePair<int, string>>() {
             new KeyValuePair<int, string>(100, "读取表"),
             new KeyValuePair<int, string>(108, "物品表"),
             new KeyValuePair<int, string>(109, "宠物表"),
        };

        private const int m_luaCsvUpdateMaxNum = 100;

        /// <summary>
        /// 这里初始化所有的csv配置
        /// </summary>
        public void InitCsv(ref CsvManager csvMgr)
        {
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_GameText, "游戏文本表", new GameTextCsv());
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_Map, "地图表", new MapCsv());

            csvMgr.AddCSVCreate((int)eAllCSV.eAC_Player, "玩家表", new PlayerCsv());

            InitLuaCsv(ref csvMgr);
        }

        private void InitLuaCsv(ref CsvManager csvMgr)
        {
            for (int i = 0; i < m_listLuaCsv.Count; i++)
            {
                KeyValuePair<int, string> item = m_listLuaCsv[i];
                csvMgr.AddCSVCreate(item.Key, item.Value, new CsvExWrapper());
            }

            int lastCsvId = m_listLuaCsv[m_listLuaCsv.Count - 1].Key;
            for (int i = 0; i < m_luaCsvUpdateMaxNum; i++)
            {
                int id = lastCsvId + i + 1;
                csvMgr.AddCSVCreate(id, i.ToString(), new CsvExWrapper());
            }
        }
    }
}