using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public class QtAlgo
    {
        public static void InitTree(QtNode node)
        {
            float subWidth = node.Rect.width * 0.5f;
            float subHeight = node.Rect.height * 0.5f;
            bool isPartible = subWidth >= UQtConfig.CellSize && subHeight >= UQtConfig.CellSize;
            QtNode[] nodes = new QtNode[4];
            if (isPartible)
            {
                nodes[0] = new QtNode(new Rect(node.Rect.xMin, node.Rect.yMin, subWidth, subHeight));
                nodes[1] = new QtNode(new Rect(node.Rect.xMin + subWidth, node.Rect.yMin, subWidth, subHeight));
                nodes[2] = new QtNode(new Rect(node.Rect.xMin, node.Rect.yMin + subHeight, subWidth, subHeight));
                nodes[3] = new QtNode(new Rect(node.Rect.xMin + subWidth, node.Rect.yMin + subHeight, subWidth, subHeight));
            }
            else
            {
                nodes[0] = new QtLeaf(new Rect(node.Rect.xMin, node.Rect.yMin, subWidth, subHeight));
                nodes[1] = new QtLeaf(new Rect(node.Rect.xMin + subWidth, node.Rect.yMin, subWidth, subHeight));
                nodes[2] = new QtLeaf(new Rect(node.Rect.xMin, node.Rect.yMin + subHeight, subWidth, subHeight));
                nodes[3] = new QtLeaf(new Rect(node.Rect.xMin + subWidth, node.Rect.yMin + subHeight, subWidth, subHeight));
            }
            node.SetSubNodes(nodes);
            if (isPartible)
            {
                //foreach (var sub in node.SubNodes)
                //{
                //    InitTree(sub);
                //}
                for (int i = 0; i < node.SubNodes.Length; i++)
                    InitTree(node.SubNodes[i]);
            }
        }

        /// <summary>
        /// 通过遍历获取当前坐标所在的叶子节点
        /// </summary>
        public static QtLeaf FindLeafRecursively(QtNode node, Vector2 point)
        {
            if (!node.Rect.Contains(point))
                return null;
            // 如果是叶子节点
            if (node is QtLeaf)
                return node as QtLeaf;
            // 不是叶子节点，继续遍历
            //foreach (var sub in node.SubNodes)
            //{
            //    QtLeaf leaf = FindLeafRecursively(sub, point);
            //    if (leaf != null)
            //        return leaf;
            //}
            for (int i = 0; i < node.SubNodes.Length; i++)
            {
                QtLeaf leaf = FindLeafRecursively(node.SubNodes[i], point);
                if (leaf != null)
                    return leaf;
            }
            return null;
        }

        private static List<QtLeaf> inList = new List<QtLeaf>(100);
        private static List<QtLeaf> outList = new List<QtLeaf>(100);

        public static void GetSwapLeaves(QtNode root, QtLeaf focus,
            List<QtLeaf> holdingLeaves, out List<QtLeaf> inLeaves, out List<QtLeaf> outLeaves)
        {
            inList.Clear();
            outList.Clear();
            GetLeavesByDist(root, focus, UQtConfig.CellSwapInDist, ref inList);
            // 新的周围列表中移除当前正在处理的列表，就是新的进入节点列表
            //inList.RemoveAll((item) => holdingLeaves.Contains(item));
            for (int i = 0; i < inList.Count; i++)
            {
                if (holdingLeaves.Contains(inList[i]))
                    inList.Remove(inList[i]);
            }
            inLeaves = inList;


            GetLeavesByDist(root, focus, UQtConfig.CellSwapInDist, ref outList);
            // 遍历正在处理的列表，如果新的周围列表中不包含它，那就是出去节点列表
            List<QtLeaf> outFilteredList = new List<QtLeaf>();
            //foreach (var leaf in holdingLeaves)
            //{
            //    if (!outList.Contains(leaf))
            //    {
            //        outFilteredList.Add(leaf);
            //    }
            //}
            for (int i = 0; i < holdingLeaves.Count; i++)
            {
                if (!outList.Contains(holdingLeaves[i]))
                    outFilteredList.Add(holdingLeaves[i]);
            }
            outLeaves = outFilteredList;
        }

        /// <summary>
        /// 通过当前父节点，焦点节点，距离获取叶子节点列表
        /// </summary>
        public static void GetLeavesByDist(QtNode node, QtLeaf focus, float dist, ref List<QtLeaf> leaves)
        {
            // 焦点和当前节点如果没有相交，不处理
            if (!Intersects(node.Rect, focus.Rect.center, dist))
                return;
            if (node is QtLeaf)
                leaves.Add(node as QtLeaf);
            else
            {
                //foreach (var sub in node.SubNodes)
                //    GetLeavesByDist(sub, focus, dist, ref leaves);
                for (int i = 0; i < node.SubNodes.Length; i++)
                    GetLeavesByDist(node.SubNodes[i], focus, dist, ref leaves);
            }
        }


        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="nodeBound"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static bool Intersects(Rect nodeBound, IQtUserData userData)
        {
            Rect r = new Rect(
                userData.GetCenter().x - userData.GetExtends().x,
                userData.GetCenter().z - userData.GetExtends().z,
                userData.GetExtends().x * 2.0f,
                userData.GetExtends().z * 2.0f);
            return nodeBound.Overlaps(r);
        }

        public static bool Intersects(Rect nodeBound, Vector2 targetCenter, float targetRadius)
        {
            bool xOutside = targetCenter.x + targetRadius < nodeBound.xMin || targetCenter.x - targetRadius > nodeBound.xMax;
            bool yOutside = targetCenter.y + targetRadius < nodeBound.yMin || targetCenter.y - targetRadius > nodeBound.yMax;
            bool outside = xOutside || yOutside;
            return !outside;
        }

        public delegate void UQtForeachLeaf(QtLeaf leaf);
        /// <summary>
        /// 叶子节点，才绘制
        /// </summary>
        public static void TraverseAllLeaves(QtNode node, UQtForeachLeaf func)
        {
            if (node is QtLeaf)
                func(node as QtLeaf);
            else
            {
                //foreach (var sub in node.SubNodes)
                //    TraverseAllLeaves(sub, func);
                for (int i = 0; i < node.SubNodes.Length; i++)
                    TraverseAllLeaves(node.SubNodes[i], func);
            }
        }

        public static void DrawRect(Rect r, float y, Color c, float padding = 0.0f)
        {
            Debug.DrawLine(new Vector3(r.xMin + padding, y, r.yMin + padding), new Vector3(r.xMin + padding, y, r.yMax - padding), c);
            Debug.DrawLine(new Vector3(r.xMin + padding, y, r.yMin + padding), new Vector3(r.xMax - padding, y, r.yMin + padding), c);
            Debug.DrawLine(new Vector3(r.xMax - padding, y, r.yMax - padding), new Vector3(r.xMin + padding, y, r.yMax - padding), c);
            Debug.DrawLine(new Vector3(r.xMax - padding, y, r.yMax - padding), new Vector3(r.xMax - padding, y, r.yMin + padding), c);
        }
    }
}
