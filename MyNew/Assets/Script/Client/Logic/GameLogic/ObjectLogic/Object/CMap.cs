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

        public bool GetPath(CCreature curCreature, Vector2 startPos, Vector2 targetPos, ref List<Vector2> path)
        {
            int maxRoute = 1024;
            if (curCreature.IsMaster())
            {
                maxRoute = 512 * 512;
            }
            path.Clear();
            bool result = m_aStar.FindPath(this, curCreature, ref startPos, ref targetPos, ref path, maxRoute);
            return result;
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
