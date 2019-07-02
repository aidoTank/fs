/**************************************
** company: 辰亚科技
** auth： 木木
** date： 2019/7/2 15:53:44
** desc： 尚未编写描述
** Ver.:  V1.0.0
***************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Roma
{
    [ExecuteInEditMode]
    public class GeoPolygon : MonoBehaviour
    {
        public bool bAirWall;

#if UNITY_EDITOR
        private GeoVertex[] vertexList;
        private static Vector2d[] m_initPos = new Vector2d[4]
        {
            new Vector2d(-0.5f, -0.5f),
            new Vector2d(-0.5f, 0.5f),
            new Vector2d(0.5f, 0.5f),
            new Vector2d(0.5f, -0.5f)
        };

        [MenuItem("GameObject/创建多边形障碍", false, 10)]
        static void AddPolygon(MenuCommand cmd)
        {
            GameObject targetGO = cmd.context as GameObject;
            if (targetGO == null || targetGO.GetComponent<GeoPolygon>() != null)
                return;

            for (int i = 0; i < 4; ++i)
            {
               GameObject v = new GameObject(i.ToString(), typeof(GeoVertex));
                v.transform.SetParent(targetGO.transform);
                v.transform.localPosition = m_initPos[i].ToVector3();
            }
            targetGO.AddComponent<GeoPolygon>();
        }

        void Update()
        {
            if (Application.isPlaying)
                return;
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, 1, pos.z);

            vertexList = gameObject.GetComponentsInChildren<GeoVertex>(true);
            for (int i = 0; i < vertexList.Length; ++i)
            {
                Transform tran = vertexList[i].transform;
                Vector3 curPos = tran.position;
                tran.position = new Vector3(curPos.x, 1, curPos.z);
            };
        }

        void OnDrawGizmos()
        {
            vertexList = gameObject.GetComponentsInChildren<GeoVertex>(true);
            Gizmos.color = Color.green;
            for (int i = 0; i < vertexList.Length; ++i)
            {
                Vector3 curPos = vertexList[i].transform.position;
                int index = (i + 1) % vertexList.Length;
                Gizmos.DrawLine(curPos, vertexList[index].transform.position);
            };
        }
#endif

        // 让属性面板可见
        public Vector2d[] m_vertexPos;

        public Vector2d[] GetVertex()
        {
            GeoVertex[] vertexList = gameObject.GetComponentsInChildren<GeoVertex>(true);
            m_vertexPos = new Vector2d[vertexList.Length];
            for (int i = 0; i < vertexList.Length; ++i)
            {
                Vector3 curLoc = vertexList[i].transform.position;
                m_vertexPos[i] = curLoc.ToVector2d();
            }
            return m_vertexPos;
        }
    }
}
