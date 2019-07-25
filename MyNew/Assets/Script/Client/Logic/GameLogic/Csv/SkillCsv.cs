using UnityEngine;
using System.Collections.Generic;
namespace Roma
{

	public enum eSkillCsv
	{
		id,
        languageId,
		name,
		icon,
		type,

        needPointer,
        selectTargetType,

        bRoleDir,

        skillType,
		distance,
		length,
		width,
		launchTime,
	
		flySpeed,

        bMove,
        bRota,

        lifeTime,

        levelList,
        subSkill,
        skilldata,
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

    public enum eSkillType
    {
       None = 0,         
       Near,         // 三段近战扇形    自己内部检测受击单位
       Jump,       
       Down_Up, // 引导型技能（当前也只有这一类技能是单独存在的），按下抬起的飞行技能，做一个多子弹发射器的BUFF，连续发射触发器
    }

	public class SkillCsvData
	{
		/// <summary>
		/// 技能ID
		/// </summary>
		public int id;

        public string languageId;

		/// <summary>
		/// 技能名称
		/// </summary>
		public string name
        {
            get
            {
                // 通过多语言id,查询名称
                //string languageText = LanguageCsv.GetText(languageId);
                //string[] arrays = languageText.Split('|');
                //if (arrays.Length >= 1)
                //{
                //    return arrays[0];
                //}
                return "null";
            }
        }

        public int icon;

		/// <summary>
		/// 技能大类型 1.主动 2.被动
		/// </summary>
		public int type;

        public bool needPointer;
		/// <summary>
		/// 指示器类型 1.自己 2.目标 3.方向 4.位置
		/// </summary>
		public int selectTargetType;

        /// <summary>
        /// 技能类型（不同实体类） 1.三段近战 2.跳跃 3引导型技能(按下抬起) 4闪电链
        /// </summary>
        public int skillType;

		/// <summary>
		/// 检测目标方式(实体类的形状) 1扇形 2圆形 3矩形 (暂时无用)
		/// </summary>
		//public int checkTargetType;

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
		//public int  hitTime;
		public int flySpeed;

        //public int checkNum;
        public bool bRoleDir;
        // 能移动和能旋转，不需要都为true，能移动就能旋转
        public bool bMove;
        public bool bRota;

        public int lifeTime;
        public int[] levelList;
        public int[] subSkill;

        public string skillDes
        {
            get
            {
                // 通过多语言id,查询描述
                //string languageText = LanguageCsv.GetText(languageId);
                //string[] arrays = languageText.Split('|');
                //if (arrays.Length == 2)
                //{
                //    return arrays[1];
                //}
                return "null";
            }
        }
    }

	public class SkillCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new SkillCsv();
		}

		protected override void _Load()
		{
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
			{
				SkillCsvData data = new SkillCsvData();
				data.id = m_csv.GetIntData(i, (int)eSkillCsv.id);
                data.languageId = m_csv.GetData(i, (int)eSkillCsv.languageId);
                //data.name = m_csv.GetData(i, (int)eSkillCsv.name);
				data.icon = m_csv.GetIntData(i, (int)eSkillCsv.icon);
				data.type = m_csv.GetIntData(i, (int)eSkillCsv.type);
                data.needPointer = m_csv.GetBoolData(i, (int)eSkillCsv.needPointer);

                data.selectTargetType = m_csv.GetIntData(i, (int)eSkillCsv.selectTargetType);
                data.bRoleDir = m_csv.GetBoolData(i, (int)eSkillCsv.bRoleDir);

                data.skillType = m_csv.GetIntData(i, (int)eSkillCsv.skillType);
				//data.checkTargetType = m_csv.GetIntData(i, (int)eSkillCsv.checkTargetType);
				data.distance = m_csv.GetIntData(i, (int)eSkillCsv.distance);
				data.length = m_csv.GetIntData(i, (int)eSkillCsv.length);
				data.width = m_csv.GetIntData(i, (int)eSkillCsv.width);
				data.launchTime = m_csv.GetIntData(i, (int)eSkillCsv.launchTime);
				//data.hitTime = m_csv.GetIntData(i, (int)eSkillCsv.hitTime);
				data.flySpeed = m_csv.GetIntData(i, (int)eSkillCsv.flySpeed);
                //data.checkNum = m_csv.GetIntData(i, (int)eSkillCsv.checkNum);

                data.bMove = m_csv.GetBoolData(i, (int)eSkillCsv.bMove);
                data.bRota = m_csv.GetBoolData(i, (int)eSkillCsv.bRota);

                data.lifeTime = m_csv.GetIntData(i, (int)eSkillCsv.lifeTime);

                string lvList = m_csv.GetData(i, (int)eSkillCsv.levelList);
                data.levelList = StringHelper.GetIntList(lvList);
                //data.skillDes = m_csv.GetData(i, (int)eSkillCsv.skilldata);

                string subSkill = m_csv.GetData(i, (int)eSkillCsv.subSkill);
                data.subSkill = StringHelper.GetIntList(subSkill);
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

        public int GetSkillDataIdByLv(int skillId, int lv)
        {
            if (lv == 0)
                return 0;
            SkillCsvData data = GetData(skillId);
            if(data == null)
            {
                Debug.LogError("技能数据为空：" + skillId);
                return 0;
            }
            if(lv - 1 < data.levelList.Length)
            {
                return data.levelList[lv - 1];
            }
            return data.levelList[0];
        }

		public Dictionary<int, SkillCsvData> m_dicData = new Dictionary<int, SkillCsvData>();
	}
}
