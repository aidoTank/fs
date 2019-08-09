using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening.Core;

public class EffectRatation : MonoBehaviour {

    public float x, y=360, z;

	// Use this for initialization
	void Start () {
		
	}

    void FixedUpdate() {
        transform.Rotate(x * Time.fixedDeltaTime, y * Time.fixedDeltaTime, z * Time.fixedDeltaTime);
        
    }
}
