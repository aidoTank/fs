using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
	public enum eBuffTriggerCsv
	{
		Id,
		Name,
		Icon,
		ModelResId,
		HeadHeight,

        PosType,

        bBullet,
        bAutoTrigger,
        BulletDeltaPos,
        BulletDeltaPosWoman,
        DirDelta,
        FlySpeed,

        DelayTime,
        DelayCheckTime,

        ContinuanceTime,
		IntervalTime,

        ShapeType,
        Length,
		Width,
		Buff1,
		Buff2,
	}

    /// <summary>
    /// 不同特征的检测器
    /// </summary>
    public enum eBuffTriggerPosType
    {
        SkillEndPos = 0,                // 技能目标位置的AOE，        如爆炸物，支持多个
        BindCaster = 1,                 // 绑定施法者，               如旋风斩
        CasterStartPos_SkillDir = 2,    // 通过技能方向飞行的，       如子弹，支持多个
        SkillEndPos_Curve = 3,          // 技能目标位置的曲线，       如抛物线手雷，支持多个
        Laser = 4,                      // 持续性可变长激光类型，     如激光，支持多个
        Lightning = 5,                  // 瞬间性闪电                 如闪电链，链接N个单位
    }

    public enum eBuffTriggerShapeType
    {
        Circle = 1,
        Sector = 2,
        Rect = 3,
        Rect_Indefinite = 4,
    }

	public class BuffTriggerCsvData
	{
		/// <summary>
		/// id
		/// </summary>
		public int Id;

		/// <summary>
		/// 名字
		/// </summary>
		public string Name;

		/// <summary>
		/// 图标
		/// </summary>
		public int Icon;

		/// <summary>
		/// 模型资源id
		/// </summary>
		public int ModelResId;

		/// <summary>
		/// 头顶高度
		/// </summary>
		public int HeadHeight;

        public int PosType;

        /// <summary>
        /// 碰撞一个立马销毁
        /// </summary>
        public bool bBullet;
        /// <summary>
        /// 生命周期结束自动触发
        /// </summary>
        public bool bAutoTrigger;
        public string BulletDeltaPos;
        /// <summary>
        /// 子弹偏移量
        /// </summary>
        public Vector3 vBulletDeltaPos;
        /// <summary>
        /// 抛物线时，为高度
        /// 子弹时，为随机偏移量
        /// </summary>
        public float iCurveHeight;
        // 女性因为机枪动作有点点不一样，读取新的配置
        public Vector3 vBulletDeltaPos_Woman;
        public float iCurveHeight_Woman;

        /// <summary>
        /// 优先获取女性主角
        /// </summary>
        //public Vector3 GetBulletDeltaPos(eGender gender)
        //{
        //    if (gender == eGender.WoMan)
        //        return vBulletDeltaPos_Woman;
        //    else
        //        return vBulletDeltaPos;
        //}

        //public float GetBulletDeltaPos_Parm(eGender gender)
        //{
        //    if (gender == eGender.WoMan)
        //        return iCurveHeight_Woman;
        //    else
        //        return iCurveHeight;
        //}


        /// <summary>
        /// 方向偏移
        /// </summary>
        public int dirDelta;
        /// <summary>
        /// 距离偏移
        /// </summary>
        public int disDelta;
   
        public float FlySpeed;
        public int DelayCreateTime;
        // 延迟检测时间
        public int DelayCheckTime;
        /// <summary>
        /// 总持续时间(毫秒)
        /// </summary>
        public int ContinuanceTime;

		/// <summary>
		/// 触发间隔
		/// </summary>
		public int IntervalTime;

        /// <summary>
        /// 1圆 2扇形 3方形
        /// </summary>
        public int ShapeType;
        /// <summary>
        /// 长度 宽为0长度就是圆半径
        /// </summary>
        public float Length;

		/// <summary>
		/// 宽度
		/// </summary>
		public float Width;

		/// <summary>
		/// 我方触发BUFF1
		/// </summary>
		public int[] selfBuffList;

		/// <summary>
		/// 敌方触发BUFF2
		/// </summary>
		public int[] targetBuffList;
	}

	public class BuffTriggerCsv : CsvExWrapper
	{
		static public CsvExWrapper CreateCSV()
		{
			return new BuffTriggerCsv();
		}

		protected override void _Load()
		{
            m_dicData.Clear();
            for (int i = 0; i < m_csv.GetRows(); i++)
			{
				BuffTriggerCsvData data = new BuffTriggerCsvData();
				data.Id = m_csv.GetIntData(i, (int)eBuffTriggerCsv.Id);
				data.Name = m_csv.GetData(i, (int)eBuffTriggerCsv.Name);
				data.Icon = m_csv.GetIntData(i, (int)eBuffTriggerCsv.Icon);
				data.ModelResId = m_csv.GetIntData(i, (int)eBuffTriggerCsv.ModelResId);
				data.HeadHeight = m_csv.GetIntData(i, (int)eBuffTriggerCsv.HeadHeight);

                data.PosType = m_csv.GetIntData(i, (int)eBuffTriggerCsv.PosType);
                data.bBullet = m_csv.GetBoolData(i, (int)eBuffTriggerCsv.bBullet);
                data.bAutoTrigger = m_csv.GetBoolData(i, (int)eBuffTriggerCsv.bAutoTrigger);

                // 起点偏移 男性
                data.BulletDeltaPos = m_csv.GetData(i, (int)eBuffTriggerCsv.BulletDeltaPos);
                if(!string.IsNullOrEmpty(data.BulletDeltaPos))
                {
                    string[] str = data.BulletDeltaPos.Split('|');
                    if(str.Length == 1)
                    {
                        data.vBulletDeltaPos = StringHelper.GetVector3(data.BulletDeltaPos);
                    }
                    else if(str.Length == 2)
                    {
                        data.vBulletDeltaPos = StringHelper.GetVector3(str[0]);
                        float h = 0;
                        float.TryParse(str[1], out h);
                        data.iCurveHeight = h;
                    }
                }
                // 起点偏移 女性
                data.BulletDeltaPos = m_csv.GetData(i, (int)eBuffTriggerCsv.BulletDeltaPosWoman);
                if (!string.IsNullOrEmpty(data.BulletDeltaPos))
                {
                    string[] str = data.BulletDeltaPos.Split('|');
                    if (str.Length == 1)
                    {
                        data.vBulletDeltaPos_Woman = StringHelper.GetVector3(data.BulletDeltaPos);
                    }
                    else if (str.Length == 2)
                    {
                        data.vBulletDeltaPos_Woman = StringHelper.GetVector3(str[0]);
                        float h = 0;
                        float.TryParse(str[1], out h);
                        data.iCurveHeight_Woman = h;
                    }
                }
                // 扩展参数
                string posParam = m_csv.GetData(i, (int)eBuffTriggerCsv.DirDelta);
                string[] strParam = posParam.Split('|');
                if(strParam.Length == 1)
                {
                    int h = 0;
                    int.TryParse(strParam[0], out h);
                    data.dirDelta = h;
                }
                else if(strParam.Length == 2)
                {
                    int h = 0;
                    int.TryParse(strParam[0], out h);
                    data.dirDelta = h;

                    int dis = 0;
                    int.TryParse(strParam[1], out dis);
                    data.disDelta = dis;
                }

                data.FlySpeed = m_csv.GetFloatData(i, (int)eBuffTriggerCsv.FlySpeed);

                data.DelayCreateTime = m_csv.GetIntData(i, (int)eBuffTriggerCsv.DelayTime);
                data.DelayCheckTime = m_csv.GetIntData(i, (int)eBuffTriggerCsv.DelayCheckTime);
                data.ContinuanceTime = m_csv.GetIntData(i, (int)eBuffTriggerCsv.ContinuanceTime);
                data.IntervalTime = m_csv.GetIntData(i, (int)eBuffTriggerCsv.IntervalTime);

                data.ShapeType = m_csv.GetIntData(i, (int)eBuffTriggerCsv.ShapeType);
                data.Length = m_csv.GetFloatData(i, (int)eBuffTriggerCsv.Length);
				data.Width = m_csv.GetFloatData(i, (int)eBuffTriggerCsv.Width);
                string b1 = m_csv.GetData(i, (int)eBuffTriggerCsv.Buff1);
                data.selfBuffList = StringHelper.GetIntList(b1);

                string b2 = m_csv.GetData(i, (int)eBuffTriggerCsv.Buff2);
                data.targetBuffList = StringHelper.GetIntList(b2);
				m_dicData.Add(data.Id, data);
			}
		}

		public BuffTriggerCsvData GetData(int csvId)
		{
			BuffTriggerCsvData data;
			if (m_dicData.TryGetValue(csvId, out data))
			{
				return data;
			}
			return null;
		}

		public Dictionary<int, BuffTriggerCsvData> m_dicData = new Dictionary<int, BuffTriggerCsvData>();
	}
}
