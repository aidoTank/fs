using UnityEngine;
using System.Collections;

/// <summary>
/// �������࣬��Ϸ���еĳ�ʼ������Ϊ�Լ����ٶ��̳����
/// �������صľ�̬�����޷�����һ�����ḻ�����࣬����ÿ�����඼��Ҫ�Լ�ȥnewһ���Լ���
/// </summary>
public class Singleton
{
	//public static Singleton Inst = null;

	public bool m_isAutoUpdate = false;
	
	public Singleton(bool isAuto)
	{
		m_isAutoUpdate = isAuto;
	}
	
	public bool IsAutoUpdate()
	{
		return m_isAutoUpdate;
	}
	
	public virtual void Init(){}

    public virtual void Update(float fTime, float fDTime) { }
    public virtual void LateUpdate(float fTime, float fDTime) { }
    
    public virtual void Destroy(){}
	
}
