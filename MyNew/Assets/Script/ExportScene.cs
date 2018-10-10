using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//using UnityEditor.SceneManagement;

public class ExportScene : MonoBehaviour {

	public LayerMask mask;

	private byte[] mapData1;
	public float cellSize = 1;
	public int cellCount = 64;
	public Color normalColor = Color.green;
	public Color colliderColor = Color.red;
	public bool reverse = false;

	private const string filePath = "Resource/scene/";
	public string fileName = "";

	Vector3 castDir = new Vector3 (0, -1, 0);

	void Start(){

	}

	[ContextMenu("创建熔岩场景数据")]
	public void CreateMapData()
    {
        //string sceneAllName = EditorSceneManager.GetActiveScene().path;
        //sceneAllName = sceneAllName.Replace("Assets", "");
        //int startPos = sceneAllName.LastIndexOf("/") + 1;
        //string sceneName = sceneAllName.Substring(startPos, sceneAllName.Length - startPos);
        //string sceneNamePre = sceneName.Replace(".unity", "");

        string sceneNamePre = Application.loadedLevelName;

        string realPath = Application.dataPath + "/" + filePath + "/" + sceneNamePre + "/data/sd_" + sceneNamePre + "_" + fileName + ".bytes";


		if(File.Exists(realPath) == true)
        {
			File.Delete (realPath);
		}

		FileStream file = new FileStream (realPath, FileMode.Create);
		BinaryWriter bw = new BinaryWriter (file);
		bw.Write ((uint)(cellCount));
		bw.Write ((uint)(cellCount));

		mapData1 = new byte[cellCount * cellCount];

		Vector3 pos = Vector3.zero;
		pos.y = 500;
		for(int i = 0; i < cellCount; ++i)
		{
			pos.x = cellSize * i + cellSize * 0.5f;
			for(int j = 0; j < cellCount; ++j)
			{
				pos.z = cellSize * j + cellSize * 0.5f;
				if(Physics.Raycast (pos, castDir, 1000, mask))
				{
					mapData1[i * cellCount + j] = 1;
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

		bw.Write (mapData1);
		bw.Close ();
		file.Close ();

	}

	/// <summary>
	/// 显示熔岩数据
	/// </summary>
    void ShowMapData1()
    {
        if (mapData1 == null)
        {
            return;
        }

        if (mapData1.Length < cellCount * cellCount)
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
                byte data = mapData1[i * cellCount + j];
                if (data == 1)
                {
                    Gizmos.color = colliderColor;
                }
                else
                {
                    Gizmos.color = normalColor;
                }
                Gizmos.DrawCube(pos, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }

	void OnDrawGizmos(){
        ShowMapData1();
        //ShowMapData2();
    }
}
