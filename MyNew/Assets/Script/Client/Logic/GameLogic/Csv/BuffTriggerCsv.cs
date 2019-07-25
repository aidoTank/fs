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
    /// ��ͬ�����ļ����
    /// </summary>
    public enum eBuffTriggerPosType
    {
        SkillEndPos = 0,                // ����Ŀ��λ�õ�AOE��        �籬ը�֧�ֶ��
        BindCaster = 1,                 // ��ʩ���ߣ�               ������ն
        CasterStartPos_SkillDir = 2,    // ͨ�����ܷ�����еģ�       ���ӵ���֧�ֶ��
        SkillEndPos_Curve = 3,          // ����Ŀ��λ�õ����ߣ�       �����������ף�֧�ֶ��
        Laser = 4,                      // �����Կɱ䳤�������ͣ�     �缤�⣬֧�ֶ��
        Lightning = 5,                  // ˲��������                 ��������������N����λ
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
		/// ����
		/// </summary>
		public string Name;

		/// <summary>
		/// ͼ��
		/// </summary>
		public int Icon;

		/// <summary>
		/// ģ����Դid
		/// </summary>
		public int ModelResId;

		/// <summary>
		/// ͷ���߶�
		/// </summary>
		public int HeadHeight;

        public int PosType;

        /// <summary>
        /// ��ײһ����������
        /// </summary>
        public bool bBullet;
        /// <summary>
        /// �������ڽ����Զ�����
        /// </summary>
        public bool bAutoTrigger;
        public string BulletDeltaPos;
        /// <summary>
        /// �ӵ�ƫ����
        /// </summary>
        public Vector3 vBulletDeltaPos;
        /// <summary>
        /// ������ʱ��Ϊ�߶�
        /// �ӵ�ʱ��Ϊ���ƫ����
        /// </summary>
        public float iCurveHeight;
        // Ů����Ϊ��ǹ�����е�㲻һ������ȡ�µ�����
        public Vector3 vBulletDeltaPos_Woman;
        public float iCurveHeight_Woman;

        /// <summary>
        /// ���Ȼ�ȡŮ������
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
        /// ����ƫ��
        /// </summary>
        public int dirDelta;
        /// <summary>
        /// ����ƫ��
        /// </summary>
        public int disDelta;
   
        public float FlySpeed;
        public int DelayCreateTime;
        // �ӳټ��ʱ��
        public int DelayCheckTime;
        /// <summary>
        /// �ܳ���ʱ��(����)
        /// </summary>
        public int ContinuanceTime;

		/// <summary>
		/// �������
		/// </summary>
		public int IntervalTime;

        /// <summary>
        /// 1Բ 2���� 3����
        /// </summary>
        public int ShapeType;
        /// <summary>
        /// ���� ��Ϊ0���Ⱦ���Բ�뾶
        /// </summary>
        public float Length;

		/// <summary>
		/// ���
		/// </summary>
		public float Width;

		/// <summary>
		/// �ҷ�����BUFF1
		/// </summary>
		public int[] selfBuffList;

		/// <summary>
		/// �з�����BUFF2
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

                // ���ƫ�� ����
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
                // ���ƫ�� Ů��
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
                // ��չ����
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
