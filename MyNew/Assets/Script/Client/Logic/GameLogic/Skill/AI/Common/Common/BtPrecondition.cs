using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 条件节点基类，继承于BTNode，
/// 多了一个检测条件所用的Check接口。
/// 具体的游戏逻辑判断继承于它。
/// </summary>
public class BtPrecondition : BtNode
{
    public BtPrecondition() : base()
    {

    }

    public virtual bool Check()
    {
        return true;
    }

    /// <summary>
    /// Tick检测通过，就算结束
    /// </summary>
    public override BtResult Tick()
    {
        if(Check())
        {
            //Debug.Log("条件通过");
            return BtResult.Ended;
        }
        //Debug.Log("条件不通过");
        return BtResult.Running;
    }
}

