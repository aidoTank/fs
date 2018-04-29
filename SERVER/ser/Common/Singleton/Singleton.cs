

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

	public virtual void Update(long time){}
	
	public virtual void UnInit(){}
	
}
