using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 优先选择节点，选择第一个为True的节点
/// 有一个能执行就执行
/// </summary>
public class BtPrioritySelector : BtNode
{
    public BtNode m_curActiveChild;
    public BtPrioritySelector(BtPrecondition condi = null) : base(condi)
    {

    }

    /// <summary>
    /// 一般情况作为树的父节点，开始检查条件，满足后执行DoEvaluate,遍历子节点
    /// 遍历评估能通过的节点，设置为当前节点，然后tick
    /// </summary>
    public override bool DoEvaluate()
    {
        if(m_children != null)
        {
            foreach(BtNode child in m_children)
            {
                if(child.Evaluate())
                {
                    if(m_curActiveChild != null && m_curActiveChild != child)
                    {
                        m_curActiveChild.Clear();
                    }
                    m_curActiveChild = child;
                    return true;
                }
            }
        }
        m_curActiveChild = null;
        return false;
    }

    /// <summary>
    /// 如果当前节点tick完成就清除
    /// </summary>
    public override BtResult Tick()
    {
        if(m_curActiveChild == null)
        {
            return BtResult.Ended;
        }
        BtResult result = m_curActiveChild.Tick();
        if(result == BtResult.Ended)
        {
            m_curActiveChild.Clear();
            m_curActiveChild = null;
        }
        return result;
    }

    public override void Clear()
    {
        if(m_curActiveChild != null)
        {
            m_curActiveChild.Clear();
            m_curActiveChild = null;
        }
    }
}

