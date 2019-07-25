using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;


namespace Roma
{
    public enum ErrorCode
    {
        eErrMaxTry = 0,         // 尝试次数过多
        eErrCannotFind,         // 寻找失败
        eErrParam,              // 参数错误
        eErrMa,
    }
    public enum CostSave
    {
        CostStraight = 10,                      // 横或竖向移动一格的路径评分
        CostDiagonal = 14,                      // 斜向移动一格的路径评分
    }

    public class CBHAStar
    {
        // 格子长宽个数，现在是一格一米，这个尺寸为最大的地图尺寸即可
        public static readonly int s_MaxArrayLen = 1024;
        //public static byte[] m_GlobleByteBlock = new byte[s_MaxArrayLen * s_MaxArrayLen];
        //public static float[] m_GlobleFloatHeight = new float[s_MaxArrayLen * s_MaxArrayLen];
        public static byte[,] m_GlobleByteAStar = new byte[s_MaxArrayLen, s_MaxArrayLen];
        public static int[,] m_GlobleintAStar = new int[s_MaxArrayLen, s_MaxArrayLen];

        public CBHAStar(int iW, int iH)
        {
            InitLists(iW, iH);
        }

        public CBHAStar()
        {
            //InitLists(0, 0);
        }

        public bool LineTest2(CMap map, CCreature cc, int x1, int y1, int x2, int y2, ref Vector2 lastPos)
        {

            //Profiler.BeginSample("LineTest2");

            int dx = x2 - x1;
            int dy = y2 - y1;
            int ux = (dx > 0) ? 1 : -1;//x的增量方向，取或-1
            int uy = (dy > 0) ? 1 : -1;//y的增量方向，取或-1
            int x = x1, y = y1, eps;//eps为累加误差
            //算法问题需要向增量方向在+1
            x2 += ux; y2 += uy;
            eps = 0; dx = Mathf.Abs(dx); dy = Mathf.Abs(dy);
            if (dx > dy)
            {
                for (x = x1; x != x2; x += ux)
                {
                    if (!map.CanArrive(cc, x, y))
                    {
                        //Profiler.EndSample();
                        return false;
                    }
                    lastPos.x = x;
                    lastPos.y = y;
                    //mpaths.Add(new Vector2(x, y));
                    eps += dy;
                    if ((eps << 1) >= dx)
                    {
                        y += uy; eps -= dx;
                    }
                }
            }
            else
            {
                for (y = y1; y != y2; y += uy)
                {
                    if (!map.CanArrive(cc, x, y))
                    {
                        //Profiler.EndSample();
                        return false;
                    }
                    lastPos.x = x;
                    lastPos.y = y;
                    eps += dx;
                    if ((eps << 1) >= dy)
                    {
                        x += ux; eps -= dy;
                    }
                }
            }

            //Profiler.EndSample();

            return true;
        }

        // 开始寻路 开始点、结束点、结果路径列表、最大搜索节点数
        public bool FindPath(CMap map, CCreature cc, ref Vector2 startPt, ref Vector2 endPos, ref List<Vector2> paths, int maxRoute)
        {
            // 如果检测到是能走的直线，就取最后一个点为目的地
            // 如果检测到不是能走的直线，取靠近目标点最近的并且能走的点，作为寻路的目标点
            Vector2 vLastPos = Vector2.zero;
            if (LineTest2(map, cc, (int)startPt.x, (int)startPt.y, (int)endPos.x, (int)endPos.y, ref vLastPos))
            {
                paths.Add(vLastPos);
                return true;
            }

            // 如果目标点不能走，取检测直线时得出的，靠近目标点最近的并且能走的点
            //if (null == map || !map.CanArriveDoubleInt(ref startPt) || !map.CanArriveDoubleInt(ref endPos))
            if (null == map || !map.CanArrive(cc, (int)endPos.x, (int)endPos.y))
            {
                m_nErrorType = ErrorCode.eErrParam;
                if (vLastPos != Vector2.zero)
                {
                    paths.Add(vLastPos);
                    //直线走到最近能走到的那个点需要缩放回去
                    //Profiler.EndSample();
                    return true;
                }
                //Profiler.EndSample();
                return false;
            }
            // sPaths.Clear();

            InitLists(0, 0);
            openNote((int)startPt.x, (int)startPt.y, 0, 0, 0);

            int currTry = 0;
            int currId = 0;
            int currNoteX = 0;
            int currNoteY = 0;
            int checkingId = 0;
            int cost = 0;
            int score = 0;

            Vector2[] aroundNodes = new Vector2[8];
            int aroundCount = 0;
            while (m_nOpenCount > 0)
            {
                // 超时返回
                if (++currTry > maxRoute)
                {
                    m_nErrorType = ErrorCode.eErrMaxTry;

                    //Profiler.EndSample();
                    //Debug.Log("超过最大遍历次数");
                    return false;
                }

                // 每次取出开放列表最前面的ID
                currId = m_vOpenList[0];
                //将编码为此ID的元素列入关闭列表
                closeNote(currId);
                currNoteX = m_vXList[currId];
                currNoteY = m_vYList[currId];

                //如果终点被放入关闭列表寻路结束，返回路径
                if (currNoteX == (int)endPos.x && currNoteY == (int)endPos.y)
                {
                    bool bRef = getPath(ref startPt, ref endPos, currId, paths);

                    //Profiler.EndSample();

                    return bRef;
                }

                //获取周围节点，排除不可通过和已在关闭列表中的
                getArounds(map,cc, currNoteX, currNoteY, ref aroundNodes, ref aroundCount);
                for (int index = 0; index < aroundCount; index++)
                {
                    // 计算F和G值
                    cost = m_vMovementCostList[currId] +
                        ((aroundNodes[index].x == currNoteX || aroundNodes[index].y == currNoteY) ? (int)CostSave.CostStraight : (int)CostSave.CostDiagonal);

                    score = cost + (int)((Mathf.Abs(endPos.x - aroundNodes[index].x) + Mathf.Abs(endPos.y - aroundNodes[index].y)) * (float)CostSave.CostStraight);
                    if (isOpen(map, (int)aroundNodes[index].x, (int)aroundNodes[index].y))//如果节点已在播放列表中
                    {
                        checkingId = m_GlobleintAStar[(int)aroundNodes[index].y, (int)aroundNodes[index].x];
                        if (checkingId >= m_vMovementCostList.Count)
                        {
                            //Debug.LogWarning(string.Format("寻路出现问题场景{0}起始坐标[{1},{2}]， 目标坐标[{3}, {4}]", map.GetMapID(), startPt.x, startPt.y, endPos.x, endPos.y));
                            return false;
                        }
                        //如果新的G值比节点原来的G值小,修改F,G值，换父节点
                        if (cost < m_vMovementCostList[checkingId])
                        {
                            m_vMovementCostList[checkingId] = cost;
                            m_vPathScoreList[checkingId] = score;
                            m_vFatherList[checkingId] = currId;
                            aheadNote(getIndex(checkingId));
                        }
                    }
                    else//如果节点不在开放列表中
                    {
                        //将节点放入开放列表
                        openNote((int)aroundNodes[index].x, (int)aroundNodes[index].y, score, cost, currId);
                    }
                }
            }

            //如果找不到。那么走到可以走到的那个点上
            if (vLastPos.x > 0 && paths.Count == 0)
            {
                //直线走到最近能走到的那个点需要缩放回去

                //Profiler.EndSample();

                return true;
            }

            // 开放列表已空，找不到路径
            m_nErrorType = ErrorCode.eErrCannotFind;

            //Profiler.EndSample();

            return false;
        }

        // 取得失败原因
        ErrorCode GetErrorType() { return m_nErrorType; }

        // 初始化数组
        public void InitLists(int iW, int iH)
        {
            if (!Listinitialized)
            {
                Listinitialized = true;
                NSInvalid = 0;
                NSOpen = 1;
                NSClose = 2;
                //mNodeStates = new byte[GlobleConfig.s_MaxArrayLen, GlobleConfig.s_MaxArrayLen];
                //m_nNoteMap = new int[GlobleConfig.s_MaxArrayLen, GlobleConfig.s_MaxArrayLen];
                Array.Clear(m_GlobleByteAStar, 0, m_GlobleByteAStar.Length);
            }
            else
            {
                NSInvalid += 4;
                NSOpen += 4;
                NSClose += 4;

                if (NSInvalid == 0)
                    Array.Clear(m_GlobleByteAStar, 0, m_GlobleByteAStar.Length);
            }

            m_nOpenCount = 0;
            m_nOpenID = -1;

            m_vOpenList.Clear();
            m_vXList.Clear();
            m_vYList.Clear();
            m_vPathScoreList.Clear();
            m_vMovementCostList.Clear();
            m_vFatherList.Clear();
        }

        // 将节点加入开放列表 x坐标 y坐标 路径评分 移动成本 父节点
        void openNote(int p_x, int p_y, int p_score, int p_cost, int p_fatherId)
        {
            m_nOpenCount++;
            m_nOpenID++;

            m_GlobleByteAStar[p_y, p_x] = NSOpen;
            m_GlobleintAStar[p_y, p_x] = m_nOpenID;

            m_vXList.Add(p_x);
            m_vYList.Add(p_y);
            m_vPathScoreList.Add(p_score);
            m_vMovementCostList.Add(p_cost);
            m_vFatherList.Add(p_fatherId);

            m_vOpenList.Add(m_nOpenID);
            aheadNote(m_nOpenCount);
        }

        // 将节点加入关闭列表
        void closeNote(int p_id)
        {
            int nodeX = m_vXList[p_id];
            int nodeY = m_vYList[p_id];

            m_nOpenCount--;
            m_GlobleByteAStar[nodeY, nodeX] = NSClose;

            if (m_nOpenCount <= 0)
            {
                m_nOpenCount = 0;
                if (m_vOpenList.Count > 0)
                {
                    m_vOpenList.RemoveAt(m_vOpenList.Count - 1);
                }

                //m_vOpenList.clear();
                return;
            }

            int nSize = m_vOpenList.Count;
            m_vOpenList[0] = m_vOpenList[nSize - 1];
            m_vOpenList.RemoveAt(nSize - 1);
            backNote();
        }

        // 将(新加入开放别表或修改了路径评分的)节点向前移动
        void aheadNote(int p_index)
        {
            int father = 0;
            int change = 0;

            while (p_index > 1)
            {
                //父节点的位置
                father = (int)(p_index / 2);
                //如果该节点的F值小于父节点的F值则和父节点交换
                if (getScore(p_index) < getScore(father))
                {
                    change = m_vOpenList[p_index - 1];
                    m_vOpenList[p_index - 1] = m_vOpenList[father - 1];
                    m_vOpenList[father - 1] = change;
                    p_index = father;
                }
                else
                {
                    break;
                }
            }
        }

        // 将(取出开启列表中路径评分最低的节点后从队尾移到最前的)节点向后移动
        void backNote()
        {
            //尾部的节点被移到最前面
            int checkIndex = 1;
            int tmp = 0;
            int change = 0;

            for (; ; )
            {
                tmp = checkIndex;
                //如果有子节点
                if (2 * tmp <= m_nOpenCount)
                {
                    //如果子节点的F值更小
                    if (getScore(checkIndex) > getScore((2 * tmp)))
                    {
                        //记节点的新位置为子节点位置
                        checkIndex = 2 * tmp;
                    }
                    //如果有两个子节点
                    if (2 * tmp + 1 <= m_nOpenCount)
                    {
                        //如果第二个子节点F值更小
                        if (getScore(checkIndex) > getScore((2 * tmp + 1)))
                        {
                            //更新节点新位置为第二个子节点位置
                            checkIndex = 2 * tmp + 1;
                        }
                    }
                }

                //如果节点位置没有更新结束排序
                if (tmp == checkIndex)
                {
                    break;
                }
                else//反之和新位置交换，继续和新位置的子节点比较F值
                {
                    change = m_vOpenList[tmp - 1];
                    m_vOpenList[tmp - 1] = m_vOpenList[checkIndex - 1];
                    m_vOpenList[checkIndex - 1] = change;
                }
            }
        }

        // 判断某节点是否在开放列表
        bool isOpen(CMap map, int p_x, int p_y)
        {
            return m_GlobleByteAStar[p_y, p_x] == NSOpen;
        }

        // 判断某节点是否在关闭列表中
        bool isClosed(int p_x, int p_y)
        {
            return m_GlobleByteAStar[p_y, p_x] == NSClose;
        }

        // 获取某节点的周围节点，排除不能通过和已在关闭列表中的
        bool getArounds(CMap map,CCreature cc, int p_x, int p_y, ref Vector2[] aroundNodes, ref int aroundCount)
        {
            //Vector2 pt = Vector2.zero;
            int checkX = 0;
            int checkY = 0;
            bool canDiagonal = false;
            bool canRight = false;
            bool canDown = false;
            bool canLeft = false;
            bool canUp = false;

            aroundCount = 0;

            // 右
            checkX = p_x + 1;
            checkY = p_y;
            canRight = map.CanArrive(cc, checkX, checkY);
            if (canRight && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }

            // 下
            checkX = p_x;
            checkY = p_y + 1;
            canDown = map.CanArrive(cc, checkX, checkY);
            if (canDown && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }

            // 左
            checkX = p_x - 1;
            checkY = p_y;
            canLeft = map.CanArrive(cc, checkX, checkY);
            if (canLeft && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }

            // 上
            checkX = p_x;
            checkY = p_y - 1;
            canUp = map.CanArrive(cc, checkX, checkY);
            if (canUp && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }

            // 右下
            checkX = p_x + 1;
            checkY = p_y + 1;
            canDiagonal = map.CanArrive(cc, checkX, checkY);
            if (canDiagonal && canRight && canDown && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }

            // 左下
            checkX = p_x - 1;
            checkY = p_y + 1;
            canDiagonal = map.CanArrive(cc, checkX, checkY);
            if (canDiagonal && canLeft && canDown && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }

            // 左上
            checkX = p_x - 1;
            checkY = p_y - 1;
            canDiagonal = map.CanArrive(cc, checkX, checkY);
            if (canDiagonal && canLeft && canUp && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }

            // 右上
            checkX = p_x + 1;
            checkY = p_y - 1;
            canDiagonal = map.CanArrive(cc, checkX, checkY);
            if (canDiagonal && canRight && canUp && !isClosed(checkX, checkY))
            {
                aroundNodes[aroundCount++] = new Vector2(checkX, checkY);
            }
            return true;
        }

        // 获取路径 起始点X坐标、起始点Y坐标、终点的ID
        bool getPath(ref Vector2 startPt, ref Vector2 endPt, int p_id, List<Vector2> paths)
        {
            Vector2 spt = startPt;
            Vector2 ept = endPt;

            // 先压入终点
            paths.Clear();
            paths.Insert(0, ept);

            // 最后一个搜索到的点的 父点
            p_id = m_vFatherList[p_id];
            Vector2 pt = new Vector2(m_vXList[p_id], m_vYList[p_id]);
            Vector2 prevPt = (ept);

            // 设置上次插入点之间的斜率
            float lastAngle = Vector2.Angle(ept, pt);

            for (; ; )
            {
                float currentAngle = Vector2.Angle(prevPt, pt);
                if (currentAngle != lastAngle)
                {
                    paths.Insert(0, prevPt);
                    lastAngle = currentAngle;
                }

                if ((int)pt.x == (int)spt.x && (int)pt.y == (int)spt.y)
                    break;

                p_id = m_vFatherList[p_id];
                prevPt = pt;
                pt.x = m_vXList[p_id];
                pt.y = m_vYList[p_id];
            }

            Vector2 fpt = paths[0];
            if ((int)fpt.x == (int)spt.x && (int)fpt.y == (int)spt.y)
            {
                fpt = spt;
            }

            return true;
        }

        // 获取某ID节点在开放列表中的索引(从1开始)
        int getIndex(int p_id)
        {
            for (int id = 0; id < m_vOpenList.Count; id++)
            {
                if (m_vOpenList[id] == p_id)
                {
                    return id;
                }
            }
            return -1;
        }

        // 获取某节点的路径评分 节点在开启列表中的索引(从1开始)
        int getScore(int p_index)
        {
            int nPos = m_vOpenList[p_index - 1];
            int nScore = m_vPathScoreList[nPos];
            return nScore;
        }

        //====================================
        //    Member Variables
        //====================================
        int m_nOpenCount;                    // 开放列表长度
        int m_nOpenID;                       // 节点加入开放列表时分配的唯一ID(从0开始)

        List<int> m_vOpenList = new List<int>();            // 开放列表，存放节点ID
        List<int> m_vXList = new List<int>();               // 节点x坐标列表
        List<int> m_vYList = new List<int>();               // 节点y坐标列表

        List<int> m_vPathScoreList = new List<int>();       // 节点路径评分列表
        List<int> m_vMovementCostList = new List<int>();    // (从起点移动到)节点的移动耗费列表
        List<int> m_vFatherList = new List<int>();          // 节点的父节点(ID)列表

        //enum ENodeState {
        //    NSInvalid = 0,
        //    NSOpen = 1,
        //    NSClose = 2,
        //};
        byte NSInvalid;
        byte NSOpen;
        byte NSClose;
        bool Listinitialized = false;

        //static byte[,] mNodeStates;

        //static int[,] m_nNoteMap;           // 节点(数组)地图,根据节点坐标记录节点开启关闭状态和ID

        //====================================
        //    private used
        //====================================
        ErrorCode m_nErrorType;                          // 失败原因
    };
}