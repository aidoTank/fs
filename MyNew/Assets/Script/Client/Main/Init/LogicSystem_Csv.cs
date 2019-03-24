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

        eAC_Dns,
        eAC_GameText,
        eAC_Scene,
        eAC_SceneBarrier,

        eAC_Player,
        eAC_Effect,
        eAC_Skill,
        eAC_SkillStep,
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
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_Dns, "域名表", new DnsCsv());
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_GameText, "游戏文本表", new GameTextCsv());
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_Scene, "地图表", new SceneCsv());
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_SceneBarrier, "地图障碍表", new SceneBarrierCsv());

            csvMgr.AddCSVCreate((int)eAllCSV.eAC_Player, "角色表", new PlayerCsv());
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_Effect, "特效表", new EffectCsv());
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_Skill, "技能主表", new SkillCsv());
            csvMgr.AddCSVCreate((int)eAllCSV.eAC_SkillStep, "技能子表", new SkillStepCsv());

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