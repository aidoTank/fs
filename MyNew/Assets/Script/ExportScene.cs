using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//using UnityEditor.SceneManagement;
using Roma;

public class ExportScene : MonoBehaviour
{
    private byte[] mapData;
    public float cellSize = 1;
    public int cellCount = 64;
    public Color normalColor = Color.green;
    public Color colliderColor = Color.red;
    public Color airWalColor = Color.gray;

    private const string filePath = "Resource/scene/";
    public string fileName = "";

    void Start()
    {

    }

    [ContextMenu("创建场景数据")]
    public void CreateMapData()
    {

        string sceneNamePre = Application.loadedLevelName;

        string realPath = Application.dataPath + "/" + filePath + "/" + sceneNamePre + "/data/sd_" + sceneNamePre + "_" + fileName + ".bytes";


        if (File.Exists(realPath) == true)
        {
            File.Delete(realPath);
        }

        // 改用识别多边形点检测
        GeoPolygon[] pList = GameObject.FindObjectsOfType<GeoPolygon>();

        FileStream file = new FileStream(realPath, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(file);
        bw.Write((uint)(cellCount));
        bw.Write((uint)(cellCount));

        mapData = new byte[cellCount * cellCount];

        Vector3 pos = Vector3.zero;
        pos.y = 500;
        for (int i = 0; i < cellCount; ++i)
        {
            pos.x = cellSize * i + cellSize * 0.5f;
            for (int j = 0; j < cellCount; ++j)
            {
                pos.z = cellSize * j + cellSize * 0.5f;
                //if(Physics.Raycast (pos, castDir, 1000, mask))
                //{
                //	mapData1[i * cellCount + j] = 1;
                //}
                for (int p = 0; p < pList.Length; p++)
                {
                    GeoPolygon geo = pList[p];
                    if (IsInPolygon(pos.ToVector2d(), geo.GetVertex()))
                    {
                        // 取周围8个点为障碍点
                        byte type = 1;
                        if (geo.bAirWall)
                        {
                            type = 2;
                        }
                        mapData[i * cellCount + j] = type;
                        UpdateData(i, j, 1, type);
                        //UpdateData(i, j, 2);
                        continue;
                    }
                }
            }
        }

        // 0000 0010
        // 0000 0100
        // 0000 1000
        // 0001 0000
        // var byteData = new byte[cellCount * cellCount / 4];
        // for(int i = 0; i < cellCount * cellCount; i += 4){
        // 	int index = i / 4;
        // 	byteData [index] |= mapData1 [i] == true ? (byte)2 : (byte)0;
        // 	byteData [index] |= mapData1 [i + 1] == true ? (byte)4 : (byte)0;
        // 	byteData [index] |= mapData1 [i + 2] == true? (byte)8 : (byte)0;
        // 	byteData [index] |= mapData1 [i + 3] == true? (byte)16 : (byte)0;
        // }

        bw.Write(mapData);
        bw.Close();
        file.Close();
    }
    void UpdateData(int i, int j, int val, byte type)
    {
        int pos1 = (i + val) * cellCount + j;
        int pos2 = (i - val) * cellCount + j;
        int pos3 = i * cellCount + j + val;
        int pos4 = i * cellCount + j - val;
        int pos5 = (i + val) * cellCount + j + val;
        int pos6 = (i + val) * cellCount + j - val;
        int pos7 = (i - val) * cellCount + j + val;
        int pos8 = (i - val) * cellCount + j - val;
        if (pos1 >= mapData.Length ||
            pos2 >= mapData.Length ||
            pos3 >= mapData.Length ||
            pos4 >= mapData.Length ||
            pos5 >= mapData.Length ||
            pos6 >= mapData.Length ||
            pos7 >= mapData.Length ||
            pos8 >= mapData.Length)
            return;

        mapData[pos1] = type;
        mapData[pos2] = type;
        mapData[pos3] = type;
        mapData[pos4] = type;
        mapData[pos5] = type;
        mapData[pos6] = type;
        mapData[pos7] = type;
        mapData[pos8] = type;
    }

    [ContextMenu("显示场景数据")]
    public void ShowMapDataRun()
    {
        mapData = SceneManager.Inst.m_staticData;
        ShowMapData();
    }

    /// <summary>
    /// 显示熔岩数据
    /// </summary>
    void ShowMapData()
    {
        if (mapData == null)
        {
            return;
        }

        if (mapData.Length < cellCount * cellCount)
        {
            return;
        }

        Vector3 pos = Vector3.zero;
        pos.y = transform.position.y;
        for (int i = 0; i < cellCount; ++i)
        {
            pos.x = cellSize * i + cellSize * 0.5f;
            for (int j = 0; j < cellCount; ++j)
            {
                pos.z = cellSize * j + cellSize * 0.5f;
                byte data = mapData[i * cellCount + j];
                if (data == 1)
                {
                    Gizmos.color = colliderColor;
                }
                else if (data == 2)
                {
                    Gizmos.color = airWalColor;
                }
                else
                {
                    Gizmos.color = normalColor;
                }
                Gizmos.DrawCube(pos, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }

    void OnDrawGizmos()
    {
        ShowMapData();
    }

    /// <summary> 
    /// 判断点是否在多边形内. 
    /// ----------原理---------- 
    /// 注意到如果从P作水平向左的射线的话，如果P在多边形内部，那么这条射线与多边形的交点必为奇数， 
    /// 如果P在多边形外部，则交点个数必为偶数(0也在内)。 
    /// </summary> 
    /// <param name="checkPoint">要判断的点</param> 
    /// <param name="polygonPoints">多边形的顶点</param> 
    /// <returns></returns> 
    public static bool IsInPolygon(Vector2d checkPoint, Vector2d[] polygonPoints)
    {
        bool inside = false;
        int pointCount = polygonPoints.Length;
        Vector2d p1, p2;
        for (int i = 0, j = pointCount - 1; i < pointCount; j = i, i++)//第一个点和最后一个点作为第一条线，之后是第一个点和第二个点作为第二条线，之后是第二个点与第三个点，第三个点与第四个点... 
        {
            p1 = polygonPoints[i];
            p2 = polygonPoints[j];
            if (checkPoint.y < p2.y)
            {//p2在射线之上 
                if (p1.y <= checkPoint.y)
                {//p1正好在射线中或者射线下方 
                    if ((checkPoint.y - p1.y) * (p2.x - p1.x) > (checkPoint.x - p1.x) * (p2.y - p1.y))//斜率判断,在P1和P2之间且在P1P2右侧 
                    {
                        //射线与多边形交点为奇数时则在多边形之内，若为偶数个交点时则在多边形之外。 
                        //由于inside初始值为false，即交点数为零。所以当有第一个交点时，则必为奇数，则在内部，此时为inside=(!inside) 
                        //所以当有第二个交点时，则必为偶数，则在外部，此时为inside=(!inside) 
                        inside = (!inside);
                    }
                }
            }
            else if (checkPoint.y < p1.y)
            {
                //p2正好在射线中或者在射线下方，p1在射线上 
                if ((checkPoint.y - p1.y) * (p2.x - p1.x) < (checkPoint.x - p1.x) * (p2.y - p1.y))//斜率判断,在P1和P2之间且在P1P2右侧 
                {
                    inside = (!inside);
                }
            }
        }
        return inside;
    }
}
