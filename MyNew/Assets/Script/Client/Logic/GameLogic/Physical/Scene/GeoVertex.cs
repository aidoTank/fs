/**************************************
** company: 辰亚科技
** auth： 木木
** date： 2019/7/2 15:52:42
** desc： 尚未编写描述
** Ver.:  V1.0.0
***************************************/
using UnityEngine;
namespace Roma
{
    public class GeoVertex : MonoBehaviour
    {
#if UNITY_EDITOR
        private static Vector3 _cuseSize = new Vector3(0.2f, 0.2f, 0.2f);
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, _cuseSize);
        }
#endif
    }
}