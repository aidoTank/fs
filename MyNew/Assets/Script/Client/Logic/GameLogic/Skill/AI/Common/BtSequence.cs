using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 顺序执行，前面的执行完成，才能执行下一个
/// </summary>
public class BtSequence : BtNode
{
    private BtNode m_curActiveChild;
    private int m_curActiveIndex = -1;

    public BtSequence(BtPrecondition condi = null)
        :base(condi)
    {

    }

    /// <summary>
    /// 评估当前节点,如果没有就取第一个
    /// </summary>
    public override bool DoEvaluate()
    {
        if(m_curActiveChild != null)
        {
            // 评估不通过，清除当前节点
            bool result = m_curActiveChild.Evaluate();
            if(!result)
            {
                m_curActiveChild.Clear();
                m_curActiveChild = null;
                m_curActiveIndex = -1;
            }
            return result;
        }
        else
        {
            return m_children[0].Evaluate();
        }
    }

    /// <summary>
    /// 如果当前节点Tick结束，就继续下一个节点
    /// </summary>
    public override BtResult Tick()
    {
        if(m_curActiveChild == null)
        {
            m_curActiveChild = m_children[0];
            m_curActiveIndex = 0;
        }
        BtResult result = m_curActiveChild.Tick();
        if(result == BtResult.Ended)
        {
            m_curActiveIndex++;
            if(m_curActiveIndex >= m_children.Count)
            {
                m_curActiveChild.Clear();
                m_curActiveChild = null;
                m_curActiveIndex = -1;
            }
            else
            {
                m_curActiveChild.Clear();
                m_curActiveChild = m_children[m_curActiveIndex];
                result = BtResult.Running;
            }
        }
        return result;
    }

    public override void Clear()
    {
        if(m_curActiveChild != null)
        {
            m_curActiveChild.Clear();
            m_curActiveChild = null;
            m_curActiveIndex = -1;
        }
        foreach(BtNode child in m_children)
        {
            child.Clear();
        }
    }
}

