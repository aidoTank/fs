using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum BtActionState
{
    Ready = 1,
    Running = 2,
}

/// <summary>
/// 行为节点，继承于BTNode，类似于状态机的一个状态，
/// 有Enter,Exit,Execute等接口。
/// 具体游戏逻辑继承于它。
/// </summary>
public class BtAction : BtNode
{
    private BtActionState m_state = BtActionState.Ready;

    public BtAction(BtPrecondition pre = null) : base(pre)
    {

    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public override BtResult Tick()
    {
        BtResult result = BtResult.Ended;
        if(m_state == BtActionState.Ready)
        {
            Enter();
            m_state = BtActionState.Running;
        }

        if(m_state == BtActionState.Running)
        {
            result = Execute();
            if(result != BtResult.Running)
            {
                Exit();
                m_state = BtActionState.Ready;
            }
        }
        return result;
    }

    /// <summary>
    /// 子类自定义的逻辑接口，心跳
    /// </summary>
    public virtual BtResult Execute()
    {
        return BtResult.Running;
    }

    public override void Clear()
    {
        if(m_state == BtActionState.Running)
        {
            Exit();
            m_state = BtActionState.Ready;
        }
    }

    public override void AddChild(BtNode node)
    {
        
    }

    public override void RemoveChild(BtNode node)
    {

    }

}

