using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AOINode
{
    public AOINode xPrev;
    public AOINode xNext;
    public AOINode yPrev;
    public AOINode yNext;

    /// <summary>
    /// 当前的自己
    /// </summary>
    public object obj;
    public int x;
    public int y;
    public int range = 10;

    /// <summary>
    /// 对于当前玩家来说，下面的进入，离开，更新的其他玩家
    /// </summary>
    public List<AOINode> listEnter = new List<AOINode>();
    public List<AOINode> listMove = new List<AOINode>();
    public List<AOINode> listLeave = new List<AOINode>();

    public AOINode(object key)
    {
        this.obj = key;
    }

    public AOINode(object key, int x, int y)
    {
        this.obj = key;
        this.x = x;
        this.y = y;
    }

    public bool IsInRange(AOINode node, int range)
    {
        if (node == null)
            return false;
        if (node.x - x < range && node.y - y < range)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 同步当前玩家的行为
    /// </summary>
    public void Update()
    {
        for (int i = 0; i < listEnter.Count; i++)
        {
            //Debug.Log("通知他们我进来：" + listEnter[i].key);
            Console.WriteLine("通知他们我进来：" + listEnter[i].obj);
            //listEnter[i].m_gameObject.SetActive(true);

            // 把我同步给他们
            Player otherPlayer = listEnter[i].obj as Player;
            Player myPlayer = this.obj as Player;
            MsgMapCreatureEnter myEnter = (MsgMapCreatureEnter)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureEnter);
            myEnter.playerData = myPlayer.publicData;
            otherPlayer.Send(myEnter);

            // 其他人同步给我
            MsgMapCreatureEnter otherEnter = (MsgMapCreatureEnter)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureEnter);
            otherEnter.playerData = otherPlayer.publicData;
            myPlayer.Send(otherEnter);
        }
        for (int i = 0; i < listMove.Count; i++)
        {
            //Debug.Log("通知他们我移动：" + listMove[i].key);
            Console.WriteLine("通知他们我移动：" + listMove[i].obj);
            Player otherPlayer = listMove[i].obj as Player;
            Player myPlayer = obj as Player;

            GC_MapCreatureMove moveInfo;
            moveInfo.uid = myPlayer.publicData.userName;
            moveInfo.x = myPlayer.publicData.x;
            moveInfo.y = myPlayer.publicData.y;
            moveInfo.dir = myPlayer.publicData.dir;

            MsgMapCreatureMove move = (MsgMapCreatureMove)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureMove);
            move.moveInfo = moveInfo;
            otherPlayer.Send(move);
        }
        for (int i = 0; i < listLeave.Count; i++)
        {
            //Debug.Log("通知他们我离开：" + listLeave[i].key);
            Console.WriteLine("通知他们我离开：" + listLeave[i].obj);
            //listLeave[i].m_gameObject.SetActive(false);

            // 把我的离开同步给他们
            Player otherPlayer = listLeave[i].obj as Player;
            Player myPlayer = obj as Player;
            MsgMapCreatureLeave leave = (MsgMapCreatureLeave)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureLeave);
            leave.uid = myPlayer.publicData.userName;
            otherPlayer.Send(leave);

            // 其他人的离开同步给我
            MsgMapCreatureLeave otherLeave = (MsgMapCreatureLeave)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureLeave);
            otherLeave.uid = otherPlayer.publicData.userName;
            myPlayer.Send(otherLeave);
        }

        listEnter.Clear();
        listMove.Clear();
        listLeave.Clear();
    }
}

public class AOIScene
{
    private AOINode head;
    private AOINode tail;


    public AOIScene()
    {
        head = new AOINode("head", 0, 0);
        tail = new AOINode("tail", 0, 0);
        head.xNext = tail;
        head.yNext = tail;
        tail.xPrev = head;
        tail.yPrev = head;
    }

    public void Add(AOINode node, int x, int y)
    {
        node.x = x;
        node.y = y;
        _add(node);

        node.listEnter = GetList(node);
        node.Update();
    }

    public void Leave(AOINode node)
    {
        node.listLeave = GetList(node);  // 获取列表，再改变状态
        node.Update();

        _leave(node);
    }

    public void Move(AOINode node, int x, int y)
    {
        // 在更新位置之前,得到要通知的结点集合
        List<AOINode> oldList = GetList(node);

        _leave(node);
        node.x = x;
        node.y = y;
        _add(node);

        // 更新位置后，得到要通知的结点集合
        List<AOINode> newList = GetList(node);

        // 交集是移动前后都要通知的 oldList & newList
        for (int i = 0; i < oldList.Count; i++)
        {
            if (newList.Contains(oldList[i]))
            {
                node.listMove.Add(oldList[i]);
            }
        }
        // 要去通知离开的节点，oldList - listMove
        for (int i = 0; i < oldList.Count; i++)
        {
            if (!node.listMove.Contains(oldList[i]))
            {
                node.listLeave.Add(oldList[i]);
            }
        }
        // 要去通知加入的节点 newList - listMove
        for (int i = 0; i < newList.Count; i++)
        {
            if (!node.listMove.Contains(newList[i]))
            {
                node.listEnter.Add(newList[i]);
            }
        }
        node.Update();
    }

    /// <summary>
    /// 获取当前可以通知的节点列表，前后查找
    /// </summary>
    /// <param name="node"></param>
    public List<AOINode> GetList(AOINode node)
    {
        List<AOINode> lookList = new List<AOINode>();
        //Debug.Log("node："+ node.key + "("+ node.x + "_" + node.y +")");
        // 往后找
        AOINode cur = node.xNext;
        while (cur != tail)  // 如果不等于最后一个
        {                   // 1  node  3  4
            if (cur.x - node.x > node.range) // 当前范围大于可视范围就不处理
            {
                break;
            }
            else
            {
                int inteval = 0;   //Y轴间隔
                inteval = node.y - cur.y;
                if (inteval >= -node.range && inteval <= node.range)
                {
                    //Debug.Log("可见：" + cur.key + "(" + cur.x + "_" + cur.y + ")");
                    lookList.Add(cur);
                }
            }
            cur = cur.xNext;
        }
        // 往前找
        cur = node.xPrev;
        while (cur != head)
        {
            if (node.x - cur.x > node.range)
            {
                break;
            }
            else
            {
                int inteval = 0;
                inteval = node.y - cur.y;
                if (inteval >= -node.range && inteval <= node.range)
                {
                    //Debug.Log("可见：" + cur.key + "(" + cur.x + "_" + cur.y + ")");
                    lookList.Add(cur);
                }
            }
            cur = cur.xPrev;
        }
        return lookList;
    }

    public void DeBug()
    {
        AOINode cur = head.xNext;
        Debug.Log("X链表:");
        while (cur != tail)
        {
            Debug.Log(cur.obj + "(" + cur.x + "_" + cur.y + ")");
            cur = cur.xNext;
        }

        cur = head.yNext;
        Debug.Log("Y链表:");
        while (cur != tail)
        {
            Debug.Log(cur.obj + "(" + cur.x + "_" + cur.y + ")");
            cur = cur.yNext;
        }
    }

    private void _add(AOINode node)
    {
        AOINode cur = head.xNext;
        while (cur != null)   // 从第一个开始遍历到最后，只到【当前的那个点】大于【插入的】
        {   //   cur 1 2 3...node  遍历  1 2 3...node cur
            if (cur.x > node.x || cur == tail)
            {
                node.xNext = cur;
                node.xPrev = cur.xPrev;
                cur.xPrev.xNext = node;
                cur.xPrev = node;
                break;
            }
            // cur < node
            cur = cur.xNext;
        }

        cur = head.yNext;
        while (cur != null)
        {
            // node < cur
            if (cur.y > node.y || cur == tail)
            {
                node.yNext = cur;
                node.yPrev = cur.yPrev;
                cur.yPrev.yNext = node;
                cur.yPrev = node;
                break;
            }
            cur = cur.yNext;
        }
    }

    private void _leave(AOINode node)
    {
        node.xPrev.xNext = node.xNext;
        node.xNext.xPrev = node.xPrev;
        node.yPrev.yNext = node.yNext;
        node.yNext.yPrev = node.yPrev;
    }
}

