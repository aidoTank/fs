using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum BtResult
{
    Ended = 1,
    Running = 2,
}


/// <summary>
/// 节点基类，有初始化接口(Activate)，
/// 评估接口(Evaluate&DoEvaluate)
/// 处理接口(Tick)
/// 以及必备的增删接口和非必须的先决条件接口(BTPrecondition)等。
/// </summary>
public class BtNode
{
    public string name;
    public List<BtNode> m_children;

    public BtDatabase m_dataBase;
    public bool m_actived = false;

    public BtPrecondition m_preconditon;

    public BtNode()
    {

    }

    public BtNode(BtPrecondition precondition)
    {
        this.m_preconditon = precondition;
    }
    
    /// <summary>
    /// 传入数据给子节点，并激活
    /// </summary>
    public virtual void Activate(BtDatabase database)
    {
        if (m_actived)
            return;
        this.m_dataBase = database;
        // 激活条件
        if(m_preconditon != null)
        {
            m_preconditon.Activate(database);
        }

        // 激活子节点
        if(m_children != null)
        {
            foreach(BtNode child in m_children)
            {
                child.Activate(database);
            }
        }
        m_actived = true;
    }

    /// <summary>
    /// 评价，检查条件是否满足
    /// </summary>
    public bool Evaluate()
    {
        bool bPre = m_preconditon == null || m_preconditon.Check();
        return m_actived && bPre && DoEvaluate();
    }

    /// <summary>
    /// 子类继承用的接口,给选择器使用的
    /// </summary>
    /// <returns></returns>
    public virtual bool DoEvaluate()
    {
        return true;
    }

    public virtual BtResult Tick()
    {
        return BtResult.Ended;
    }

    public virtual void Clear() { }

    public virtual void AddChild(BtNode node)
    {
        if(m_children == null)
        {
            m_children = new List<BtNode>();
        }
        if(node != null)
        {
            m_children.Add(node);
        }
    }

    public virtual void RemoveChild(BtNode node)
    {
        if(m_children != null && node != null)
        {
            m_children.Remove(node);
        }
    }
}

