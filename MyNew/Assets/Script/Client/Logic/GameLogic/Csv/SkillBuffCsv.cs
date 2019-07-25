using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    /// <summary>
    /// 状态类BUFF
    /// </summary>
    public enum eBuffState
    {
        none,
        // BUFF逻辑状态
        stun = 1,        // 晕眩
        silent = 2,      // 沉默
        God = 3,         // 无敌

        unmove = 4,       // 禁锢 无法移动
        sleep = 5,        // 睡眠，被攻击时解除
        SuperArmor = 6,   // 霸体



        // 仅仅控制表现层状态
        Hit,        // 受击
        AlphaToHalf, // 半透
        AlphaToHide, // 全透
        Nihility,    // 虚无状态
        Show,        // 是否显示
    }


    public enum eBuffType
    {
        none,
        damage = 1,      // 瞬间伤害
        contDamage = 2,  // 持续伤害

        pullPos = 3,     // 拉扯位置
        atk = 4,         // 攻击增减  百分比prop
        speed = 5,       // 速度增减  百分比prop
        repel = 6,       // 击退BUFF
        addHp = 7,       // 回复生命  百分比

        dp = 8,          // 护甲增益  百分比prop
        modelScale = 9,  // 模型缩放  百分比

        state = 10,      // 状态类型

        atkCreate = 11,     // 拥有者在伤害其他单位时创建BUFF
        hitCreate = 12,     // 拥有者受伤害时创建BUFF

        createTrigger = 20, // 创建BUFF触发器，比如创建爆炸物，旋风斩，陷阱，烟雾弹，独立的单位
        createSkill = 21,
        createCreature = 22,// 创建怪物
    }

	public enum eSkillBuffCsv
	{
		id,
		name,
		logicId,
        logicIdDesc,
		targetType,
		IsCont,
		IsInterval,
		ContinuanceTime,
		IntervalTime,
		ParamDesc1,
		ParamValue1,

        icon,
        effectId,
        effectPoint,
        animaId,
        hitColor,
        des,
	}

	public class SkillBuffCsvData
	{
		/// <summary>
		/// id
		/// </summary>
		public int id;

		/// <summary>
		/// BUFF名称
		/// </summary>
		public string name;

		/// <summary>
		/// 逻辑ID
		/// </summary>
		public int logicId;

		/// <summary>
		/// 逻辑ID说明
		/// </summary>
		public string logicIdDesc;

		/// <summary>
		/// 目标类型 0敌人 1自己 2伙伴
		/// </summary>
		public int targetType;

		/// <summary>
		/// 是否是持续性效果
		/// </summary>
		public bool IsCont;

		/// <summary>
		/// 是否间隔发作性效果
		/// </summary>
		public bool IsInterval;

		/// <summary>
		/// 总持续时间(毫秒)
		/// </summary>
		public int ContinuanceTime;

		/// <summary>
		/// 发作时间间隔(毫秒)
		/// </summary>
		public int IntervalTime;

		/// <summary>
		/// 参数说明1
		/// </summary>
		public string ParamDesc1;

		/// <summary>
		/// 参数值1
		/// </summary>
		//public string ParamValue1;
        public int[] listParam;

        public int ParamValue1
        {
            get
            {
                return listParam[0];
            }
        }

        public int icon;
        public int effectId;
        public int effectPoint;
        /// <summary>
        /// 受击动作
        /// </summary>
        public int animaId;
        /// <summary>
        /// BUFF持续时的颜色
        /// </summary>
        public Color hitColor;
        public bool hasColor;

        public string des;
	}

	public class SkillBuffCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new SkillBuffCsv();
		}

		protected override void _Load()
		{
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
			{
				SkillBuffCsvData data = new SkillBuffCsvData();
				data.id = m_csv.GetIntData(i, (int)eSkillBuffCsv.id);
				data.name = m_csv.GetData(i, (int)eSkillBuffCsv.name);
				data.logicId = m_csv.GetIntData(i, (int)eSkillBuffCsv.logicId);
				data.logicIdDesc = m_csv.GetData(i, (int)eSkillBuffCsv.logicIdDesc);
				data.targetType = m_csv.GetIntData(i, (int)eSkillBuffCsv.targetType);

                data.IsCont = m_csv.GetBoolData(i, (int)eSkillBuffCsv.IsCont);
                data.IsInterval = m_csv.GetBoolData(i, (int)eSkillBuffCsv.IsInterval);

                data.ContinuanceTime = m_csv.GetIntData(i, (int)eSkillBuffCsv.ContinuanceTime);
				data.IntervalTime = m_csv.GetIntData(i, (int)eSkillBuffCsv.IntervalTime);
				data.ParamDesc1 = m_csv.GetData(i, (int)eSkillBuffCsv.ParamDesc1);
				string strParam = m_csv.GetData(i, (int)eSkillBuffCsv.ParamValue1);
                data.listParam = StringHelper.GetIntList(strParam);


                data.icon = m_csv.GetIntData(i, (int)eSkillBuffCsv.icon);
                data.effectId = m_csv.GetIntData(i, (int)eSkillBuffCsv.effectId);
                data.effectPoint = m_csv.GetIntData(i, (int)eSkillBuffCsv.effectPoint);
                data.animaId = m_csv.GetIntData(i, (int)eSkillBuffCsv.animaId);
                string color = m_csv.GetData(i, (int)eSkillBuffCsv.hitColor);
                if(!string.IsNullOrEmpty(color))
                {
                    data.hasColor = true;
                    data.hitColor = StringHelper.GetColor(color);
                }
                data.des = m_csv.GetData(i, (int)eSkillBuffCsv.des);
                if(m_dicData.ContainsKey(data.id))
                {
                    Debug.LogError("buff id 重复：" + data.id);
                    continue;
                }
                m_dicData.Add(data.id, data);
			}
		}

		public SkillBuffCsvData GetData(int csvId)
		{
			SkillBuffCsvData data;
			if (m_dicData.TryGetValue(csvId, out data))
			{
				return data;
			}
			return null;
		}

		public Dictionary<int, SkillBuffCsvData> m_dicData = new Dictionary<int, SkillBuffCsvData>();
	}
}
