using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UVAnimation : MonoBehaviour {

	Material mat;

	public Vector2 UvDefaultTiling = new Vector2(1, 1);
	public Vector2 UvDefaultOffset;
	public Vector2 UvOffsetSpeed;
	public Color DefaultColor = Color.white;

	Vector2 offset;
	
	// Update is called once per frame
	void Update () {
		if(mat == null){
			mat = GetComponent<Renderer> ().material;
			if(mat == null){
				Debug.LogError ("没有发现材质");
				return;
			}
			mat.mainTextureScale =  UvDefaultTiling;
			mat.mainTextureOffset =  UvDefaultOffset;
			mat.color = DefaultColor;
			offset = UvDefaultOffset;
		}

		if(mat != null){
			offset += Time.deltaTime * UvOffsetSpeed;
			mat.mainTextureOffset =  offset;
			mat.mainTextureScale =  UvDefaultTiling;
			mat.color = DefaultColor;
			mat.SetColor ("_TintColor", DefaultColor);
		}	
	}
}
