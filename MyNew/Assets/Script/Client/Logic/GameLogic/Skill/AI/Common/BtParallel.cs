using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ParallelFunction
{
    And = 1,    // returns Ended when all results are not running
    Or = 2,     // returns Ended when any result is not running
}

/// <summary>
/// And:当所有子节点结束时，才算结束
/// or:有一个子节点结束时，就算结束
/// </summary>
public class BtParallel : BtNode
{
    public List<BtResult> m_results;
    public ParallelFunction m_func;

    public BtParallel(ParallelFunction func) : this(func, null) { }
    
    public BtParallel(ParallelFunction func, BtPrecondition preCondi)
        :base(preCondi)
    {
        m_results = new List<BtResult>();
        m_func = func;
    }

    /// <summary>
    /// 当有一个评价不通过，就都不通过
    /// </summary>
    public override bool DoEvaluate()
    {
        foreach(BtNode child in m_children)
        {
            if(!child.Evaluate())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// And:当所有子节点停止时，才算结束
    /// or:有一个子节点停止时，就算结束
    /// </summary>
    /// <returns></returns>
    public override BtResult Tick()
    {
        int endingResultCount = 0;
        for(int i = 0; i < m_children.Count; i ++)
        {
            if(m_func == ParallelFunction.And)
            {
                if(m_results[i] == BtResult.Running)
                {
                    m_results[i] = m_children[i].Tick();
                }
                if(m_results[i] != BtResult.Running)
                {
                    endingResultCount++;
                }
            }
            else
            {
                if (m_results[i] == BtResult.Running)
                {
                    m_results[i] = m_children[i].Tick();
                }
                if (m_results[i] != BtResult.Running)
                {
                    ResetResults();  // 有一个结束时，都结束
                    return BtResult.Ended;
                }
            }
        }
        if (endingResultCount == m_children.Count) // 只用于and
        {
            ResetResults();
            return BtResult.Ended;
        }
        return BtResult.Running;
    }

    /// <summary>
    /// 重置结果
    /// </summary>
    private void ResetResults()
    {
        for(int i =0; i < m_results.Count; i ++)
        {
            m_results[i] = BtResult.Running;
        }
    }

    public override void Clear()
    {
        ResetResults();
        foreach(BtNode child in m_children)
        {
            child.Clear();
        }
    }

    public override void AddChild(BtNode node)
    {
        base.AddChild(node);
        m_results.Add(BtResult.Running);
    }

    public override void RemoveChild(BtNode node)
    {
        int index = m_children.IndexOf(node);
        m_results.RemoveAt(index);
        base.RemoveChild(node);
    }
}

