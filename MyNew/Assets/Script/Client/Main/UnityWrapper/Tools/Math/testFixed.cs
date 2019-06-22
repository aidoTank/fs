using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Roma;

public class testFixed : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float f = -9f;
        FixedPointF f1 = new FixedPointF(f);
        Debug.Log(f1.m_value);
        Debug.Log(f1.value);

        int i = 5;
        FixedPointF f2 = new FixedPointF(i);
        Debug.Log(f2.m_value);
        Debug.Log(f2.value);

        FixedPointF f3 = new FixedPointF(1, 2);
        Debug.Log(f3.m_value);
        Debug.Log(f3.value);

        FixedPointF f4 = FixedPointF.Abs(f1);
        Debug.Log("f4=" + f4.m_value);
        Debug.Log("f4=" + f4.value);

        CustomMath.ClampAngle(270);
   
        Vector2d v1 = new Vector2d(0, 1);
        Vector2d v2 = new Vector2d(0, 2);
        Vector2d v3 = v1 + v2;
        FixedPointF dis = Vector2d.Distance(v1, v2);
        Debug.Log("dis:" + dis.ToString());

        Vector2d val1 =  Vector2d.Lerp(new Vector2d(0, 100), new Vector2d(100, 200), new FixedPointF(9, 10));
        Vector2 val2 = Vector2.Lerp(new Vector2(0, 100), new Vector2(100, 200), 0.9f);
        Debug.Log("v1:" + val1 + "  v2:" + val2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
