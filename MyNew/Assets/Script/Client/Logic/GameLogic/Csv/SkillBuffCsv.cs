using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    /// <summary>
    /// ״̬��BUFF
    /// </summary>
    public enum eBuffState
    {
        none,
        // BUFF�߼�״̬
        stun = 1,        // ��ѣ
        silent = 2,      // ��Ĭ
        God = 3,         // �޵�

        unmove = 4,       // ���� �޷��ƶ�
        sleep = 5,        // ˯�ߣ�������ʱ���
        SuperArmor = 6,   // ����



        // �������Ʊ��ֲ�״̬
        Hit,        // �ܻ�
        AlphaToHalf, // ��͸
        AlphaToHide, // ȫ͸
        Nihility,    // ����״̬
        Show,        // �Ƿ���ʾ
    }


    public enum eBuffType
    {
        none,
        damage = 1,      // ˲���˺�
        contDamage = 2,  // �����˺�

        pullPos = 3,     // ����λ��
        atk = 4,         // ��������  �ٷֱ�prop
        speed = 5,       // �ٶ�����  �ٷֱ�prop
        repel = 6,       // ����BUFF
        addHp = 7,       // �ظ�����  �ٷֱ�

        dp = 8,          // ��������  �ٷֱ�prop
        modelScale = 9,  // ģ������  �ٷֱ�

        state = 10,      // ״̬����

        atkCreate = 11,     // ӵ�������˺�������λʱ����BUFF
        hitCreate = 12,     // ӵ�������˺�ʱ����BUFF

        createTrigger = 20, // ����BUFF�����������紴����ը�����ն�����壬�����������ĵ�λ
        createSkill = 21,
        createCreature = 22,// ��������
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
		/// BUFF����
		/// </summary>
		public string name;

		/// <summary>
		/// �߼�ID
		/// </summary>
		public int logicId;

		/// <summary>
		/// �߼�ID˵��
		/// </summary>
		public string logicIdDesc;

		/// <summary>
		/// Ŀ������ 0���� 1�Լ� 2���
		/// </summary>
		public int targetType;

		/// <summary>
		/// �Ƿ��ǳ�����Ч��
		/// </summary>
		public bool IsCont;

		/// <summary>
		/// �Ƿ���������Ч��
		/// </summary>
		public bool IsInterval;

		/// <summary>
		/// �ܳ���ʱ��(����)
		/// </summary>
		public int ContinuanceTime;

		/// <summary>
		/// ����ʱ����(����)
		/// </summary>
		public int IntervalTime;

		/// <summary>
		/// ����˵��1
		/// </summary>
		public string ParamDesc1;

		/// <summary>
		/// ����ֵ1
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
        /// �ܻ�����
        /// </summary>
        public int animaId;
        /// <summary>
        /// BUFF����ʱ����ɫ
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
                    Debug.LogError("buff id �ظ���" + data.id);
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
