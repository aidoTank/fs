using UnityEngine;

/// <summary>
/// 树的入口，它依然有init,update,uninit三大接口，
/// 在心跳中评估(Evaluate)节点以及处理(Tick)节点。
/// </summary>
public class BtTree //: MonoBehaviour
{
    public BtNode m_root;
    public BtDatabase m_dataBase;

    public bool m_bRunning = true;

    // 设置共享参数 ，可以重启树
    public const int m_reset = -1;

    public virtual void Init()
    {
        m_dataBase = new BtDatabase();
        //m_dataBase = GetComponent<BtDatabase>();
        //if(m_dataBase == null)
        //{
        //    m_dataBase = gameObject.AddComponent<BtDatabase>();
        //}
        m_dataBase.SetData<bool>(m_reset, false);
    }

    public void OnStart()
    {
        m_root.Activate(m_dataBase);
    }

    public virtual void OnUpdate()
    {
        if (!m_bRunning)
            return;
        if(m_dataBase.GetData<bool>(m_reset))
        {
            Reset();
            m_dataBase.SetData<bool>(m_reset, false);
        }
        if(m_root.Evaluate())
        {
            m_root.Tick();
        }
    }

    public void Reset()
    {
        m_root.Clear();
    }

    public void OnDestroy()
    {
        if(m_root != null)
        {
            m_root.Clear();
        }
    }
}

