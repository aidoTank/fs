using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Roma;

public class testFixed : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //TestFixedPoint();
        TestCollide();
    }

    void TestFixedPoint()
    {
        float f = -9f;
        FixedPoint f1 = new FixedPoint(f);
        Debug.Log(f1.m_value);
        Debug.Log(f1.value);

        int i = 5;
        FixedPoint f2 = new FixedPoint(i);
        Debug.Log(f2.m_value);
        Debug.Log(f2.value);

        FixedPoint f3 = new FixedPoint(1, 2);
        Debug.Log(f3.m_value);
        Debug.Log(f3.value);

        FixedPoint f4 = FixedPoint.Abs(f1);
        Debug.Log("f4=" + f4.m_value);
        Debug.Log("f4=" + f4.value);

        CustomMath.ClampAngle(270);

        Vector2d v1 = new Vector2d(0, 1);
        Vector2d v2 = new Vector2d(0, 2);
        Vector2d v3 = v1 + v2;
        FixedPoint dis = Vector2d.Distance(v1, v2);
        Debug.Log("dis:" + dis.ToString());

        Vector2d val1 = Vector2d.Lerp(new Vector2d(0, 100), new Vector2d(100, 200), new FixedPoint(9, 10));
        Vector2 val2 = Vector2.Lerp(new Vector2(0, 100), new Vector2(100, 200), 0.9f);
        Debug.Log("v1:" + val1 + "  v2:" + val2);
    }

    void TestCollide()
    {
        Vector2 v1 = Collide.GetVector(55);
        Vector2d v2 = FPCollide.GetVector(55);
        Debug.Log("GetVector V1:" + v1 + "   V2:" + v2.ToString());

        float f1 = Collide.GetAngle(new Vector2(0.4f, 0.6f));
        FixedPoint f2 = FPCollide.GetAngle(new Vector2d(0.4f * 10, 0.6f * 10));
        Debug.Log("GetAngle f1:" + f1 + "   f2:" + f2.value);

        Vector2 rv1 = Collide.Rotate(new Vector2(0.1f, 0.8f), 60);
        Vector2d rv2 = FPCollide.Rotate(new Vector2d(0.1f, 0.8f), 60);
        Debug.Log("Rotate rv1:" + rv1 + "   rv2:" + rv2);

        Sphere s1;
        s1.r = 1;
        s1.c = new Vector2(0, 0);
        Sphere s2;
        s2.r = 1;
        s2.c = new Vector2(1.9f, 0);
        bool bTrue = Collide.bSphereSphere(s1, s2);
        Debug.Log("bSphereSphere : " +bTrue);

        FPSphere s11;
        s11.r = new FixedPoint(1);
        s11.c = new Vector2d(0, 0);
        FPSphere s22;
        s22.r = new FixedPoint(1);
        s22.c = new Vector2d(1.9f, 0);
        bTrue = FPCollide.bSphereSphere(s11, s22);
        Debug.Log("FP bSphereSphere : " + bTrue);

        OBB obb = new OBB(new Vector2(3, 2.2f), new Vector2(2.4f, 1), 50);
        Vector2 vec1 = Collide.ClosestPointOBB(new Vector2(1, 2.4f), obb);

        FPObb fpObb = new FPObb(new Vector2d(3, 2.2f), new Vector2d(2.4f, 1), 50);
        Vector2d vec2 = FPCollide.ClosestPointOBB(new Vector2d(1, 2.4f), fpObb);

        Debug.Log("ClosestPointOBB vec1" + vec1 + "   vec2:" + vec2);

        Sector sec;
        sec.pos = new Vector2(0, 0);
        sec.angle = 90;
        sec.dir = new Vector2(0, 1);
        sec.r = 1;
        bool bInside = Collide.bSectorInside(sec, new Vector2(0, 0.1f));

        FPSector sec1;
        sec1.pos = new Vector2d(0, 0);
        sec1.angle = new FixedPoint(90);
        sec1.dir = new Vector2d(0, 1);
        sec1.r = new FixedPoint(1);
        bool bInside1 = FPCollide.bSectorInside(sec1, new Vector2d(0, 0.1f));


        Debug.Log("bSectorInside 1:" + bInside + "   2:" + bInside1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
