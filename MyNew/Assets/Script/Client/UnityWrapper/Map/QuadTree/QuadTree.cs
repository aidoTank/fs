using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public static class UQtConfig
    {
        public static float CellSize = 20.0f;

        public static float CellSwapInDist = 20.0f;
        public static float CellSwapOutDist = 20.0f;

        public static float SwapTriggerInterval = 0.5f;
        public static float SwapProcessInterval = 0.2f;
    }

    public interface IQtUserData
    {
        Vector3 GetCenter();
        Vector3 GetExtends();
        void SwapIn();
        void SwapOut();
        bool IsSwapInCompleted();
        bool IsSwapOutCompleted();
    }

    public class QtNode
    {
        public Rect Rect;
        public QtNode[] SubNodes;
        public QtNode(Rect rect)
        {
            Rect = rect;
        }

        public virtual void SetSubNodes(QtNode[] subNodes)
        {
            SubNodes = subNodes;
        }

        public virtual void Receive(IQtUserData userData)
        {
            // 如果不相交，就不做处理
            if (!QtAlgo.Intersects(Rect, userData))
                return;

            //foreach (var sub in SubNodes)
            //{
            //    sub.Receive(userData);
            //}
            for (int i = 0; i < SubNodes.Length; i++)
                SubNodes[i].Receive(userData);
        }

        public virtual void Clear()
        {
            for (int i = 0; i < SubNodes.Length; i++)
                SubNodes[i] = null;
        }
    }
    /// <summary>
    /// 叶子节点
    /// </summary>
    public class QtLeaf : QtNode
    {
        private List<IQtUserData> _ownedObjects = new List<IQtUserData>();
        private List<IQtUserData> _affectedObjects = new List<IQtUserData>();
        public QtLeaf(Rect rect) : base(rect)
        {
        }

        public override void Receive(IQtUserData userData)
        {
            if (!QtAlgo.Intersects(Rect, userData))
                return;

            if (Rect.Contains(new Vector2(userData.GetCenter().x, userData.GetCenter().z)))
            {
                _ownedObjects.Add(userData);
            }
            else
            {
                _affectedObjects.Add(userData);
            }
        }

        public void SwapIn()
        {
            //foreach (var obj in _ownedObjects)
            //    obj.SwapIn();
            //foreach (var obj in _affectedObjects)
            //    obj.SwapIn();
            for (int i = 0; i < _ownedObjects.Count; i++)
                _ownedObjects[i].SwapIn();
            for (int i = 0; i < _affectedObjects.Count; i++)
                _affectedObjects[i].SwapIn();
        }

        public void SwapOut()
        {
            //foreach (var obj in _ownedObjects)
            //    obj.SwapOut();
            //foreach (var obj in _affectedObjects)
            //    obj.SwapOut();
            for (int i = 0; i < _ownedObjects.Count; i++)
                _ownedObjects[i].SwapOut();
            for (int i = 0; i < _affectedObjects.Count; i++)
                _affectedObjects[i].SwapOut();
        }

        public bool IsSwapInCompleted()
        {
            //foreach (var obj in _ownedObjects)
            //{
            //    if (!obj.IsSwapInCompleted())
            //        return false;
            //}
            //foreach (var obj in _affectedObjects)
            //{
            //    if (!obj.IsSwapInCompleted())
            //        return false;
            //}
            for (int i = 0; i < _ownedObjects.Count; i++)
                if (!_ownedObjects[i].IsSwapInCompleted()) return false;
            for (int i = 0; i < _affectedObjects.Count; i++)
                if (!_affectedObjects[i].IsSwapInCompleted()) return false;
            return true;
        }

        public bool IsSwapOutCompleted()
        {
            //foreach (var obj in _ownedObjects)
            //{
            //    if (!obj.IsSwapOutCompleted())
            //        return false;
            //}
            //foreach (var obj in _affectedObjects)
            //{
            //    if (!obj.IsSwapOutCompleted())
            //        return false;
            //}
            for (int i = 0; i < _ownedObjects.Count; i++)
                if (!_ownedObjects[i].IsSwapOutCompleted()) return false;
            for (int i = 0; i < _affectedObjects.Count; i++)
                if (!_affectedObjects[i].IsSwapOutCompleted()) return false;
            return true;
        }
    }

    public class QuadTree
    {
        public bool EnableDebugLines;
        private QtNode RootNode;
        private QtLeaf FocusLeaf;
        private float _lastSwapTriggeredTime = 0.0f;
        private float _lastSwapProcessedTime = 0.0f;

        /// <summary>
        /// 当前显示的列表
        /// </summary>
        private List<QtLeaf> HoldingLeaves = new List<QtLeaf>();
        private List<QtLeaf> SwapInQueue = new List<QtLeaf>();
        private List<QtLeaf> SwapOutQueue = new List<QtLeaf>();

        public QuadTree(Rect rect)
        {
            RootNode = new QtNode(rect);
            QtAlgo.InitTree(RootNode);
        }

        public void Receive(IQtUserData qud)
        {
            RootNode.Receive(qud);
        }

        /// <summary>
        /// 刚进入时
        /// 1.没有【当前显示列表】，那么焦点周围的点都为【进入列表】
        /// 2.【进入列表】中的叶子节点执行swapIn方法加入【进入队列】
        /// 刷新队列
        /// 3.如果叶子节点都执行过swapIn，并且可见，就加入到当前【当前显示列表】，并从【进入队列】中移除
        /// 
        /// 当焦点移动时
        /// 1.通过【当前显示列表】和【距离】筛选出此时的【进入列表】和【出去列表】
        /// 2.【进入列表】中的叶子节点执行swapIn方法加入【进入队列】
        /// 3.【出去列表】中的叶子节点执行swapOut方法加入【出去队列】
        /// 刷新队列
        /// 4.如果叶子节点都执行过swapIn，并且可见，就加入到当前【当前显示列表】，并从【进入队列】中移除
        /// 5.如果叶子节点都执行过swapOut，并且不可见，就从【出去队列】中移除
        /// </summary>
        /// <param name="foucs"></param>
        public void Update(Vector2 foucs)
        {
            if (EnableDebugLines)
                DrawDebugLines();
            if (Time.time - _lastSwapTriggeredTime > UQtConfig.SwapTriggerInterval)
            {
                if (UpdateFocus(foucs))
                    OnSwapInOut(FocusLeaf);
                _lastSwapTriggeredTime = Time.time;
            }

            if (Time.time - _lastSwapProcessedTime > UQtConfig.SwapProcessInterval)
            {
                ProcessSwapQueues();
                _lastSwapProcessedTime = Time.time;
            }
        }

        /// <summary>
        /// 如果最新的位置的叶子节点和之前的叶子节点一样,就不处理
        /// </summary>
        private bool UpdateFocus(Vector2 focus)
        {
            QtLeaf newlLeaf = QtAlgo.FindLeafRecursively(RootNode, focus);
            if (newlLeaf == FocusLeaf)
                return false;
            FocusLeaf = newlLeaf;
            return true;
        }

        /// <summary>
        /// 通过焦点节点
        /// </summary>
        /// <param name="activeLeaf"></param>
        private void OnSwapInOut(QtLeaf activeLeaf)
        {
            // 初始化进，出叶子节点
            List<QtLeaf> inLeaves;
            List<QtLeaf> outLeaves;
            QtAlgo.GetSwapLeaves(RootNode, activeLeaf, HoldingLeaves, out inLeaves, out outLeaves);

            // 过滤掉已经在进入队列中的叶子节点
            //inLeaves.RemoveAll((leaf) => { return SwapInQueue.Contains(leaf); });
            // 过滤掉已经在进入队列中的叶子节点
            for (int i = 0; i < inLeaves.Count; i++)
            {
                if (SwapInQueue.Contains(inLeaves[i]))
                    inLeaves.Remove(inLeaves[i]);
            }

            // 将新的进出节点加到各自队列，并执行行为
            SwapIn(inLeaves);
            SwapOut(outLeaves);
        }

        private void SwapIn(List<QtLeaf> inLeaves)
        {
            //foreach (var leaf in inLeaves)
            //{
            //    leaf.SwapIn();
            //    SwapInQueue.Add(leaf);
            //}
            for (int i = 0; i < inLeaves.Count; i++)
            {
                QtLeaf leaf = inLeaves[i];
                leaf.SwapIn();
                SwapInQueue.Add(leaf);
            }
        }

        private void SwapOut(List<QtLeaf> outLeaves)
        {
            //foreach (var leaf in outLeaves)
            //{
            //    leaf.SwapOut();
            //    HoldingLeaves.Remove(leaf);
            //    SwapOutQueue.Add(leaf);
            //}

            for (int i = 0; i < outLeaves.Count; i++)
            {
                QtLeaf leaf = outLeaves[i];
                leaf.SwapOut();
                HoldingLeaves.Remove(leaf);
                SwapOutQueue.Add(leaf);
            }
        }

        private void ProcessSwapQueues()
        {
            // 遍历进入队列，如果当前节点已经是激活中，就加入到当前显示列表
            //foreach (var leaf in SwapInQueue)
            //{
            //    if (leaf.IsSwapInCompleted())
            //    {
            //        HoldingLeaves.Add(leaf);
            //    }
            //}
            for (int i = 0; i < SwapInQueue.Count; i++)
            {
                QtLeaf leaf = SwapInQueue[i];
                if (leaf.IsSwapInCompleted())
                {
                    HoldingLeaves.Add(leaf);
                }
            }

            //// 从进入队列中移除正在显示的节点
            //SwapInQueue.RemoveAll((leaf) => { return HoldingLeaves.Contains(leaf); });
            //// 从出去队列中移除已经出去的
            //SwapOutQueue.RemoveAll((leaf) => { return leaf.IsSwapOutCompleted(); });

            // 从进入队列中移除正在显示的节点
            for (int i = 0; i < SwapInQueue.Count; i++)
            {
                QtLeaf leaf = SwapInQueue[i];
                if (HoldingLeaves.Contains(leaf))
                    SwapInQueue.Remove(leaf);
            }
            // 从出去队列中移除已经出去的
            for (int i = 0; i < SwapOutQueue.Count; i++)
            {
                QtLeaf leaf = SwapOutQueue[i];
                if (leaf.IsSwapOutCompleted())
                    SwapOutQueue.Remove(leaf);
            }
        }

        private void DrawDebugLines()
        {
            return;
            QtAlgo.TraverseAllLeaves(RootNode, (leaf) =>
            {
                Color c = Color.gray;
                if (leaf == FocusLeaf)
                {
                    c = Color.blue;
                }
                else if (SwapInQueue.Contains(leaf))
                {
                    c = Color.green;
                }
                else if (SwapOutQueue.Contains(leaf))
                {
                    c = Color.red;
                }
                else if (HoldingLeaves.Contains(leaf))
                {
                    c = Color.white;
                }
                QtAlgo.DrawRect(leaf.Rect, 0.1f, c, 0.5f);
            });
        }
    }
}
