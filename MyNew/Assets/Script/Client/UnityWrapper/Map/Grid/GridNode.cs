//#define ASTAR_NoTagPenalty
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public enum eCollisionMode
    {
        eCM_NavMesh,
        eCM_DownUp,
    }

    public class RaycastHitWrapper : Util.DistanceWrapper
    {
        // 摘要:
        //     The normal of the surface the ray hit.
        public Vector3 normal { get; set; }
        //
        // 摘要:
        //     The impact point in world space where the ray hit the collider.
        public Vector3 point { get; set; }

        //
        // 摘要:
        //    layer
        public int layer { get; set; }

    }

    public class GraphCollision
    {

        /** Height of capsule or length of ray when checking for collision.
         * If #type is set to Sphere, this does not affect anything
         */
        public float collisionheight = 2F;

        /** Layer mask to use for height check. */
        public LayerMask heightMask = -1;

        /** The height to check from when checking height */
        public float checkHeight = 100;

        /** Offset to apply after each raycast to make sure we don't hit the same point again in CheckHeightAll */
        public const float RaycastErrorMargin = 0.005F;

        public static eCollisionMode collisionMode = eCollisionMode.eCM_DownUp;

        protected GameObject go = null;

        //#if !PhotonImplementation

        /** Sets up several variables using the specified matrix and scale.
          * \see GraphCollision.up
          * \see GraphCollision.upheight
          * \see GraphCollision.finalRadius
          * \see GraphCollision.finalRaycastRadius
          */
        public void Initialize()
        {
            if (collisionMode == eCollisionMode.eCM_NavMesh)
            {
                heightMask = (int)LusuoLayer.eEL_NavMesh;
            }
        }

        /** Returns the position with the correct height. If #heightCheck is false, this will return \a position.\n
          * \a walkable will be set to false if nothing was hit. The ray will check a tiny bit further than to the grids base to avoid floating point errors when the ground is exactly at the base of the grid */
        public Vector3 CheckHeight(Vector3 position, ref RaycastHit hit, out bool walkable)
        {
            walkable = true;

            if (collisionMode == eCollisionMode.eCM_DownUp)
            {
                // 从100米高的地方向下发射射线
                RaycastHit[] hits = Physics.RaycastAll(position + Vector3.up * checkHeight, -Vector3.up, checkHeight + 0.005F, heightMask);
                if (hits.Length > 1)
                {
                    //取最高
                    bool bGetUp = false;
                    float fTmp = Mathf.Infinity;
                    int imax = 0;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (fTmp > hits[i].distance)
                        {
                            fTmp = hits[i].distance;
                            imax = i;
                        }

                        if (hits[i].collider.gameObject.layer == (int)LusuoLayer.eEL_StaticCollision)
                        {
                            bGetUp = true;
                        }
                    }

                    if (bGetUp)
                    {
                        hit.point = hits[imax].point;
                        hit.normal = hits[imax].normal;
                        hit.distance = hits[imax].distance;
                        if (hits[imax].collider.gameObject.layer == (int)LusuoLayer.eEL_StaticNoWalking)
                            walkable = false;

                        return hit.point;
                    }

                    //对付有空隔的情况
                    RaycastHitWrapper[] hitswrap = new RaycastHitWrapper[hits.Length];

                    for (int i = 0; i < hits.Length; i++)
                    {
                        hitswrap[i] = new RaycastHitWrapper();
                        hitswrap[i].distance = hits[i].distance;
                        hitswrap[i].normal = hits[i].normal;
                        hitswrap[i].point = hits[i].point;
                        hitswrap[i].layer = hits[i].collider.gameObject.layer;
                    }

                    Util.QuickSort(hitswrap);
                    RaycastHitWrapper lowhit = hitswrap[0];

                    for (int i = 1; i < hitswrap.Length; i++)
                    {
                        if (lowhit.distance - hitswrap[i].distance < collisionheight)
                            lowhit = hitswrap[i];
                        else if (lowhit.distance - hitswrap[i].distance < 0.0f)
                            Debug.LogError("快速排序算法错误");
                        else
                            break;
                    }

                    hit.point = lowhit.point;
                    hit.normal = lowhit.normal;
                    hit.distance = lowhit.distance;

                    if (lowhit.layer == (int)LusuoLayer.eEL_StaticNoWalking)
                        walkable = false;

                    return hit.point;
                }
                else if (hits.Length == 1)
                {
                    hit = hits[0];

                    if (hit.collider.gameObject.layer == (int)LusuoLayer.eEL_StaticNoWalking)
                        walkable = false;

                    return hit.point;
                }
                else
                {
                    walkable = false;
                }
            }
            else if (collisionMode == eCollisionMode.eCM_NavMesh)
            {
                if (Physics.Raycast(position + Vector3.up * checkHeight, -Vector3.up, out hit, checkHeight + 0.005F, heightMask))
                {
                    return hit.point;
                }
                else
                {
                    walkable = false;
                }
            }
            else
            {
                walkable = false;
            }

            return position;
        }

    }

    public enum eGridType : byte
    {
        eGT_none = 0,       // 红色障碍
        eGT_block,          // 黑色
        eGT_walkable,       // 蓝色能走
        eGT_grass,
        eGT_dirt,
        eGT_stone,
        eGT_metal,
    }

    public class GridNode : Node
    {

        public GridNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector3 position;

        //Last 8 bytes used 
        public byte flags;

        /** Is the node walkable */
        public eGridType type = eGridType.eGT_none;

        //Flags used
        //last 8 bits - see Node class
        //First 8 bits for connectivity info inside this grid

        //Size = [Node, 28 bytes] + 4 = 32 bytes

        /** First 24 bits used for the index value of this node in the graph specified by the last 8 bits. \see #gridGraphs */
        private int x;
        private int y;

        /** Has connection to neighbour \a i.
         * The neighbours are the up to 8 grid neighbours of this node.
         * \see SetConnection
         */
        public bool GetConnection(int i)
        {
            return ((flags >> i) & 1) == 1;
        }

        /** Sets a connection without clearing the previous value.
         * Faster if you are setting all connections at once and have cleared the value before calling this function
         * \see SetConnection */
        // 左移n位就相当于乘以2的n次方
        // 0
        public void SetConnection(int i)
        {
            flags = (byte)(flags | (1 << i));
        }

        public void SetDisConnection(int i)
        {
            flags = (byte)(flags & ~(1 << i));
        }


        /** Returns the grid index in the graph */
        public int GetX()
        {
            return x;
        }

        /** Set the grid index in the graph */
        public int GetY()
        {
            return y;
        }

        public GridNode parent = null;
        //public LinkedList<GridNode> inNeighbors = new LinkedList<GridNode>();
    }
}
