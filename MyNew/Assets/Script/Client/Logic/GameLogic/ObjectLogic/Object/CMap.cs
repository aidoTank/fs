using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 用场景包含一切的方式
    /// </summary>
    public class CMap
    {
        public int m_mapId;

        private CBHAStar m_aStar = new CBHAStar();
        public Action m_mapInited;

        public CMap(int mapId)
        {
            m_mapId = mapId;
        }

        public void Create()
        {
            Destroy();
            SceneManager.Inst.LoadScene(m_mapId, null);

        }

        public void ExecuteFrame()
        {
            //CPlayerMgr.ExecuteFrame();
        }


        /// <summary>
        /// 静态 包含障碍 包含空气墙
        /// </summary>
        public bool bCanMove(int x, int y)
        {
            return !SceneManager.Inst.Isblock(x, y);
        }

        /// <summary>
        /// 用于子弹，包含障碍 不包含空气墙
        /// </summary>
        public bool IsblockNotAirWal(int x, int y)
        {
            return SceneManager.Inst.IsblockNotAirWal(x, y);
        }

        /// <summary>
        /// 动态,可以支持动态寻路
        /// </summary>
        public bool CanArrive(CCreature c, int x, int y)
        {
            return !SceneManager.Inst.Isblock(x, y);
        }

        public bool GetPath(CCreature curCreature, Vector2d startPos, Vector2d targetPos, ref List<Vector2d> path)
        {
            int maxRoute = 1024;
            //if (curCreature.IsMaster())
            //{
            //    maxRoute = 512 * 512;
            //}
            path.Clear();
            // 寻路算法，没有涉及小数的计算
            List<Vector2> list = new List<Vector2>();
            bool result = m_aStar.FindPath(this, curCreature, startPos.ToVector2(), targetPos.ToVector2(),ref list, maxRoute);
            for(int i = 0; i < list.Count; i ++)
            {
                path.Add(list[i].ToVector2d());
            }
            return result;
        }

        /// <summary>
        /// 遍历直线中的点，与障碍物检测
        /// 在做激光技能时，假如激光有30米，一条激光一帧检测30次，每次遍历一次障碍数据
        /// 测试用了5条激光，每帧GC在5.3KB, 耗时在6.1MS，这种方式消耗比较高
        /// </summary>
        public bool LineObstacle(int x1, int y1, int x2, int y2, ref Vector2 intersectionPoint)
        {
            // 起点
            int x = x1;
            int y = y1;
            // XY差值
            int dx = x2 - x1;
            int dy = y2 - y1;
            // 带方向的单位增量
            int ux = (dx > 0) ? 1 : -1;
            int uy = (dy > 0) ? 1 : -1;
            // 差值绝对值
            dx = Mathf.Abs(dx);
            dy = Mathf.Abs(dy);
            // 遍历生成直线点，以较长的轴向累加
            if (dx > dy)
            {
                //  判别式
                int p = 2 * dy - dx;
                for (int i = 0; i <= dx; i++)
                {
                    if (IsblockNotAirWal(x, y))
                    {
                        intersectionPoint = new Vector2(x, y);
                        return true;
                    }
                    x += ux;
                    if (p > 0)
                    {
                        y += uy;
                        p += 2 * (dy - dx);
                    }
                    else
                    {
                        p += 2 * dy;
                    }
                }
            }
            else
            {
                int p = 2 * dx - dy;
                for (int i = 0; i <= dy; i++)
                {
                    if (IsblockNotAirWal(x, y))
                    {
                        intersectionPoint = new Vector2(x, y);
                        return true;
                    }

                    y += uy;
                    if (p >= 0)
                    {
                        x += ux;
                        p += 2 * (dx - dy);
                    }
                    else
                    {
                        p += 2 * dx;
                    }
                }
            }
            return false;
        }

        public Vector2 GetRandomPos(float x, float y, float range, eAIType aiType = eAIType.Player)
        {
            int resultNum = 0;

            int irange = (int)(range);
            int srange = irange * irange;
            int sign = 1;
            int filter = 0;

            while (resultNum != 1)
            {
                int idivx = 0;
                int idivy = 0;
                if (aiType == eAIType.Player)
                {
                    idivx = resultNum + GameManager.Inst.GetClientRand(-irange, irange) + sign * filter;
                    idivy = resultNum + GameManager.Inst.GetClientRand(-irange, irange) - sign * filter;
                }
                else
                {
                    idivx = resultNum + GameManager.Inst.GetRand(-irange, irange) + sign * filter;
                    idivy = resultNum + GameManager.Inst.GetRand(-irange, irange) - sign * filter;
                }
                if (idivx == 0 || idivy == 0)
                {
                    filter++;
                    continue;
                }

                int ix = idivx % irange;
                int iy = idivy % irange;

                Vector2 bv = new Vector2(x + ix, y + iy);
                if (bCanMove((int)bv.x, (int)bv.y))
                {
                    filter = 0;
                    resultNum = 1;
                    return new Vector2((int)bv.x, (int)bv.y);
                }
                else if (filter > srange)
                {
                    resultNum = 1;
                    return new Vector2(x, y);
                }
                else
                {
                    filter++;
                    sign = -sign;
                }
            }
            return Vector3.zero;
        }
        public void Destroy()
        {

        }
    }
}
