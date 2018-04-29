using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace Roma
{

    public class GridGenerate
    {
        /** All nodes this graph contains. This can be iterated to search for a specific node.
         * This should be set if the graph does contain any nodes.
         * \note Entries are permitted to be NULL, make sure you account for that when iterating a graph's nodes
         */
        public static GridNode[] nodes/**< Width of the grid in nodes */
        {
            get
            {
                return m_nodes;
            }
        }

        private static GridNode[] m_nodes = null;

        /** \name Inspector - Settings
         * \{ */

        public static int Width/**< Width of the grid in nodes */
        {
            get
            {
                return m_width;
            }
        }

        public static int Depth
        {
            get
            {
                return m_depth;
            }
        }

        private static int m_width;
        private static int m_depth;

        // private static float nodeSize = 1; /**< Size of one node in world units */

        /* Collision and stuff */

        /** Settings on how to check for walkability and height */

        private static GraphCollision collision;

        //private static float[] heightblockData = null;

        /** The max position difference between two nodes to enable a connection. Set to 0 to ignore the value.*/

        public static float maxClimb = 0.6F;

        /** The axis to use for #maxClimb. X = 0, Y = 1, Z = 2. */

        private static int maxClimbAxis = 1;

        /** The max slope in degrees for a node to be walkable. */

        public static float maxSlope = 45;

        /** Check Slope */
        public static bool bCheckSlope = false;

        /** Erosion of the graph.
         * The graph can be eroded after calculation.
         * This means a margin is put around unwalkable nodes or other unwalkable connections.
         * It is really good if your graph contains ledges where the nodes without erosion are walkable too close to the edge.
         * 
         * Below is an image showing a graph with erode iterations 0, 1 and 2
         * \shadowimage{erosion.png}
         * 
         * \note A high number of erode iterations can seriously slow down graph updates during runtime (GraphUpdateObject)
         * and should be kept as low as possible.
         * \see erosionUseTags
         */
        public static int erodeIterations = 0;

        /** Number of neighbours for each node. Either four directional or eight directional */

        public const int neighbours = 8;

        /* End collision and stuff */

        private static int[] neighbourOffsets;

        private static void SetUpOffsetsAndCosts()
        {
            neighbourOffsets = new int[neighbours] {
                -m_width, 1 , m_width , -1,
                -m_width+1, m_width+1 , m_width-1, -m_width-1
            };
        }

        public static void Scan(int width, int depth, out byte[] blocks, out float[] heights)
        {
            GameObject newRoot = GameObject.Find("MHGridBHs");
            if (newRoot != null)
            {
                GameObject.DestroyImmediate(newRoot);
            }

            m_width = (int)(width / TerrainBlockData.nodesize);
            m_depth = (int)(depth / TerrainBlockData.nodesize);

            blocks = new byte[m_width * m_depth];
            Memory.MemSet(blocks, (byte)eGridType.eGT_none);//È«×èµ²

            int hwidth = width + 1;
            int hdepth = depth + 1;
            heights = new float[hwidth * hdepth];
            for (int i = 0; i < heights.Length; i++)
            {
                heights[i] = Mathf.NegativeInfinity;
            }

            //if (TerrainBlockData.nodesize > 1)
            //{
            //    return;
            //}

            if (m_width > 1024 || m_depth > 1024)
            {
                Debug.LogError("One of the grid's sides is longer than 1024 nodes");
                return;
            }

            SetUpOffsetsAndCosts();

            m_nodes = new GridNode[m_width * m_depth];

            if (collision == null)
            {
                collision = new GraphCollision();
            }
            collision.Initialize();

            for (int z = 0; z < m_depth; z++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    int index = z * m_width + x;
                    GridNode node = new GridNode(x, z);
                    m_nodes[index] = node;
                    UpdateNodePositionCollision(node, x, z);
                }
            }

            for (int z = 0; z < m_depth; z++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    int index = z * m_width + x;
                    GridNode node = m_nodes[index];
                    CalculateConnections(m_nodes, index, node);
                }
            }

            for (int z = 0; z < m_depth; z++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    GridNode node = m_nodes[z * m_width + x];

                    if (eGridType.eGT_walkable != node.type)
                    {
                        /*int index = node.GetIndex ();
								
                        for (int i=0;i<4;i++) {
                            if (node.GetConnection (i)) {
                                nodes[index+neighbourOffsets[i]].walkable = false;
                            }
                        }*/
                    }
                    else
                    {
                        bool anyFalseConnections = false;

                        for (int i = 0; i < neighbours; i++)
                        {
                            if (!node.GetConnection(i))
                            {
                                anyFalseConnections = true;
                                break;
                            }
                        }

                        if (anyFalseConnections)
                        {
                            node.type = eGridType.eGT_block;
                        }
                    }
                }
            }

            ErodeWalkableArea();


            float[] staggeredheightData = new float[m_width * m_depth];
            for (int i = 0; i < staggeredheightData.Length; i++)
            {
                staggeredheightData[i] = Mathf.NegativeInfinity;
            }

            //heightblockData = new float[hwidth * hdepth];
            //for (int i = 0; i < heightblockData.Length; i++)
            //{
            //    heightblockData[i] = Mathf.NegativeInfinity;
            //}

            foreach (GridNode key in GridGenerate.nodes)
            {
                int i = key.GetX();
                int j = key.GetY();
                blocks[j * m_width + i] = (byte)key.type;
                staggeredheightData[j * m_width + i] = key.position.y;
            }

            // smooth staggered grid ½»´íÍø¸ñ woooo
            int divide = (int)(1 / TerrainBlockData.nodesize);
            int staggeredoffset = divide >> 1;
            //GameObject root = new GameObject("GridNodes");
            for (int i = 0; i < hwidth; i++)
            {
                for (int j = 0; j < hdepth; j++)
                {
                    float y = 0.0f;
                    int count = 0;
                    //bool blockAround = false;
                    for (int ih = -staggeredoffset; ih <= 0; ih++)
                    {
                        for (int jh = -staggeredoffset; jh <= 0; jh++)
                        {
                            int index = (j * divide + jh) * m_width + (i * divide + ih);
                            if (index < 0 || index >= staggeredheightData.Length)
                            {
                                //blockAround = true;
                                continue;
                            }

                            if (blocks[index] == (byte)eGridType.eGT_none)
                            {
                                //blockAround = true;
                                continue;
                            }

                            y += staggeredheightData[index];
                            count++;
                        }
                    }

                    if (count > 0)
                        y /= count;

                    heights[j * hwidth + i] = y;
                    //if (!blockAround)
                    //    heightblockData[j * hwidth + i] = y;
                }
            }

        }

        /** Updates position, walkability and penalty for the node.
         * Assumes that collision.Initialize (...) has been called before this function */
        protected static void UpdateNodePositionCollision(GridNode node, int x, int z, bool resetPenalty = true)
        {
            node.position = (new Vector3(x * TerrainBlockData.nodesize + TerrainBlockData.halfnodesize, 0, z * TerrainBlockData.nodesize + TerrainBlockData.halfnodesize));

            RaycastHit hit = new RaycastHit();

            bool walkable = true;

            node.position = collision.CheckHeight(node.position, ref hit, out walkable);

            //If the walkable flag has already been set to false, there is no point in checking for it again
            if (walkable)
                node.type = eGridType.eGT_walkable;

            //Check if the node is on a slope steeper than permitted
            if (walkable)
            {
                if (hit.normal != Vector3.zero && bCheckSlope)
                {
                    //Take the dot product to find out the cosinus of the angle it has (faster than Vector3.Angle)
                    float angle = Vector3.Dot(hit.normal.normalized, Vector3.up);

                    //Max slope in cosinus
                    float cosAngle = Mathf.Cos(maxSlope * Mathf.Deg2Rad);

                    //Check if the slope is flat enough to stand on
                    if (angle < cosAngle)
                    {
                        node.type = eGridType.eGT_block;
                    }
                }
            }


            //Equal to (node as GridNode).WalkableErosion = node.walkable, but this is faster

        }

        /** Erodes the walkable area.
         * 
         * xmin, zmin (inclusive)\n
         * xmax, zmax (exclusive)
         * 
         * \see #erodeIterations */
        protected static void ErodeWalkableArea()
        {
            for (int it = 0; it < erodeIterations; it++)
            {
                //Recalculate connections
                for (int z = 0; z < m_depth; z++)
                {
                    for (int x = 0; x < m_width; x++)
                    {
                        int index = z * m_width + x;
                        GridNode node = m_nodes[index];
                        CalculateOutline(m_nodes, index, node);
                    }
                }

                for (int z = 0; z < m_depth; z++)
                {
                    for (int x = 0; x < m_width; x++)
                    {
                        GridNode node = m_nodes[z * m_width + x];

                        if (node.type != eGridType.eGT_walkable)
                        {
                            /*int index = node.GetIndex ();
								
                            for (int i=0;i<4;i++) {
                                if (node.GetConnection (i)) {
                                    nodes[index+neighbourOffsets[i]].walkable = false;
                                }
                            }*/
                        }
                        else
                        {
                            bool anyFalseConnections = false;

                            for (int i = 0; i < neighbours; i++)
                            {
                                if (!node.GetConnection(i))
                                {
                                    anyFalseConnections = true;
                                    break;
                                }
                            }

                            if (anyFalseConnections)
                            {
                                node.type = eGridType.eGT_block;
                            }
                        }
                    }
                }
            }

        }

        /** Returns true if a connection between the adjacent nodes \a n1 and \a n2 is valid. Also takes into account if the nodes are walkable */
        protected static bool IsValidConnection(GridNode n1, GridNode n2)
        {
            if (!(n1.type == eGridType.eGT_walkable) || !(n2.type == eGridType.eGT_walkable))
            {
                return false;
            }

            if (maxClimb != 0 && Mathf.Abs(n1.position[maxClimbAxis] - n2.position[maxClimbAxis]) > maxClimb)
            {
                return false;
            }

            return true;
        }

        /** Returns true if a connection between the adjacent nodes \a n1 and \a n2 is valid. Also takes into account if the nodes are walkable */
        protected static bool IsValidOutline(GridNode n)
        {
            if (!(n.type == eGridType.eGT_walkable))
            {
                return true;
            }

            return false;
        }

        /** Calculates the grid connections for a single node */
        protected static void CalculateConnections(GridNode[] nodes, int index, GridNode node)
        {

            //Reset all connections
            node.flags = 0;

            //All connections are disabled if the node is not walkable
            if (!(node.type == eGridType.eGT_walkable))
            {
                return;
            }

            for (int i = 0; i < neighbours; i++)
            {
                int neighbourIndex = index + neighbourOffsets[i];

                if (neighbourIndex < 0 || neighbourIndex >= m_width * m_depth)
                {
                    continue;
                }

                GridNode other = nodes[neighbourIndex];

                if (IsValidConnection(node, other))
                {
                    node.SetConnection(i);
                }
            }
        }

        /** Calculates the grid connections for a single node */
        protected static void CalculateOutline(GridNode[] nodes, int index, GridNode node)
        {
            //All connections are disabled if the node is not walkable
            if (!(node.type == eGridType.eGT_walkable))
            {
                return;
            }

            for (int i = 0; i < neighbours; i++)
            {
                int neighbourIndex = index + neighbourOffsets[i];

                if (neighbourIndex < 0 || neighbourIndex >= m_width * m_depth)
                {
                    continue;
                }

                GridNode other = nodes[neighbourIndex];

                if (IsValidOutline(other))
                {
                    node.SetDisConnection(i);
                }
            }
        }

        [ExecuteInEditMode]
        public static void GenGizmos(Color b, Color h, Color n)
        {
            GameObject newRoot = GameObject.Find("MHGrid");
            if (newRoot != null)
            {
                GameObject.DestroyImmediate(newRoot);
            }

            newRoot = new GameObject("MHGrid");

            Material mat;

#if UNITY_EDITOR
            mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Resource/shader/scene/grid/grid_vertex.mat", typeof(Material));
#else
            mat = (Material)Resources.Load("Materials/MHVertex", typeof(Material));
#endif        



            if (mat == null)
                Debug.LogError("MHVertex.mat load failed");

            int i = 0;
            int v = 0;
            int iCount = 512;
            int istep = (m_depth * m_width) / iCount;
            if (istep == 0)
                iCount = m_depth * m_width;

            Vector3[] vertices = new Vector3[iCount * 4];
            Color[] colors = new Color[iCount * 4];
            int[] triangles = new int[iCount * 6];

            for (int z = 0; z < m_depth; z++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    int index = z * m_width + x;

                    if (i == iCount * 6)
                    {
                        i = 0;
                        v = 0;

                        vertices = new Vector3[iCount * 4];
                        colors = new Color[iCount * 4];
                        triangles = new int[iCount * 6];
                    }

                    vertices[v] = m_nodes[index].position - Vector3.one * TerrainBlockData.halfnodesize;
                    vertices[v + 1] = m_nodes[index].position + Vector3.right * TerrainBlockData.halfnodesize + Vector3.back * TerrainBlockData.halfnodesize;
                    vertices[v + 2] = m_nodes[index].position + Vector3.one * TerrainBlockData.halfnodesize;
                    vertices[v + 3] = m_nodes[index].position + Vector3.left * TerrainBlockData.halfnodesize + Vector3.forward * TerrainBlockData.halfnodesize;

                    if (m_nodes[index].type == eGridType.eGT_walkable)
                    {
                        colors[v] = b;
                        colors[v + 1] = b;
                        colors[v + 2] = b;
                        colors[v + 3] = b;
                    }
                    else if (m_nodes[index].type == eGridType.eGT_none)
                    {
                        colors[v] = n;
                        colors[v + 1] = n;
                        colors[v + 2] = n;
                        colors[v + 3] = n;
                    }
                    else
                    {
                        colors[v] = h;
                        colors[v + 1] = h;
                        colors[v + 2] = h;
                        colors[v + 3] = h;
                    }


                    triangles[i] = v;
                    triangles[i + 1] = v + 3;
                    triangles[i + 2] = v + 1;
                    triangles[i + 3] = v + 3;
                    triangles[i + 4] = v + 2;
                    triangles[i + 5] = v + 1;

                    i += 6;
                    v += 4;


                    if (i == iCount * 6)
                    {
                        GameObject newSub = new GameObject("MHSubGrid");

                        MeshFilter mf = newSub.AddComponent<MeshFilter>();
                        MeshRenderer mr = newSub.AddComponent<MeshRenderer>();

                        newSub.transform.parent = newRoot.transform;

                        mf.sharedMesh = new Mesh();

                        mf.sharedMesh.vertices = vertices;
                        mf.sharedMesh.triangles = triangles;
                        mf.sharedMesh.colors = colors;
                        mr.sharedMaterial = mat;
                    }
                }
            }
        }
    }
}