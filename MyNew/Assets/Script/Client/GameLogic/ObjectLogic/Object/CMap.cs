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
        public List<object> m_listBarrier = new List<object>();
        private byte[] m_staticData;

        public CMap(int mapId)
        {
            m_mapId = mapId;
        }

        public void Create()
        {
            // 地图
            SceneManager.Inst.LoadScene(m_mapId, null);
            // 障碍数据
            SceneBarrierCsv csv = CsvManager.Inst.GetCsv<SceneBarrierCsv>((int)eAllCSV.eAC_SceneBarrier);
            List<SceneBarrierCsvData> list = new List<SceneBarrierCsvData>();
            csv.GetData(ref list, m_mapId);
            Debug.Log("障碍数量：" + list.Count);
            for(int i = 0; i < list.Count; i ++)
            {
                SceneBarrierCsvData data = list[i];
                if(data.shapeType == 1)
                {
                    float dir = data.vDir.y;
                    Vector2 scale = new Vector2(data.vScale.x, data.vScale.z);
                    OBB obb = new OBB(data.vPos.ToVector2(), scale, dir);
                    m_listBarrier.Add(obb);
                }
                else if(data.shapeType == 2)
                {
                    Sphere obb = new Sphere();
                    obb.c = data.vPos.ToVector2();
                    obb.r = data.vScale.x * 0.5f;
                    m_listBarrier.Add(obb);
                }
            }
        }

        public void ExecuteFrame()
        {
            //CPlayerMgr.ExecuteFrame();
        }


        // 圆形在当前地图位置是否能走
        public bool CanMove(Vector2 pos, float r)
        {
            for(int i = 0 ; i < CMapMgr.m_map.m_listBarrier.Count; i ++)
            {
                object obj = CMapMgr.m_map.m_listBarrier[i];
                
                Sphere s = new Sphere();
                s.c = pos;
                s.r = r;
                Vector2 point = Vector2.zero;
                if(obj is OBB && Collide.bOBBInside(s, (OBB)obj, ref point))
                {
                    return false;
                }
                else if(obj is Sphere && Collide.bSphereSphere(s, (Sphere)obj))
                {
                    return false;
                }
            }
            return true;
        }

        public bool bCanMove(int x, int y)
        {
            return !SceneManager.Inst.Isblock(x, y);
        }

        public void Destroy()
        {

        }
    }
}
