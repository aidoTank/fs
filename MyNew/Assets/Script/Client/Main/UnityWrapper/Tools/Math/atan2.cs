//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//namespace Roma
//{

//public class atan2 : MonoBehaviour
//{
//    private static float M_PI = 3.1415926535897932384626433832795f;
//    private static float M_PI_2 = (3.1415926535897932384626433832795f / 2);

//    private const int LUT_NUM = 100;
//    private const float PRICISION = 0.02f;
//    private static float[] g_atan_tab = new float[LUT_NUM];
	
//	void Start ()
//    {
//        // 返回弧度 [-π/2, π/2]  [-90, 90]
//        for (int i = 0; i < LUT_NUM; i ++)
//        {
//            float rad = (float)i / (LUT_NUM - 1);
//            g_atan_tab[i] = Mathf.Atan(rad);
//        }

//            for (int i = 0; i < LUT_NUM; i++)
//            {
//                int x = i;
//                int y = LUT_NUM - 1;
//                float lib = Mathf.Atan2(x, y);
//                float tab = Atan2Tab(x, y);
//                float pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第一象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(y, x);
//                tab = Atan2Tab(y, x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第一象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//                lib = Mathf.Atan2(x, -y);
//                tab = Atan2Tab(x, -y);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第二象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(y, -x);
//                tab = Atan2Tab(y, -x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第二象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//                lib = Mathf.Atan2(-x, -y);
//                tab = Atan2Tab(-x, -y);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第三象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(-y, -x);
//                tab = Atan2Tab(-y, -x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第三象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//                lib = Mathf.Atan2(-x, y);
//                tab = Atan2Tab(-x, y);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第四象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(-y, x);
//                tab = Atan2Tab(-y, x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第四象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//            }


//            for (int i = 0; i < LUT_NUM; i++)
//            {
//                int x = i;
//                int y = LUT_NUM - 1;
//                float lib = Mathf.Atan2(x, y);
//                float tab = CustomMath.Atan2(x, y);
//                float pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第一象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(y, x);
//                tab = CustomMath.Atan2(y, x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第一象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//                lib = Mathf.Atan2(x, -y);
//                tab = CustomMath.Atan2(x, -y);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第二象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(y, -x);
//                tab = CustomMath.Atan2(y, -x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第二象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//                lib = Mathf.Atan2(-x, -y);
//                tab = CustomMath.Atan2(-x, -y);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第三象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(-y, -x);
//                tab = CustomMath.Atan2(-y, -x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第三象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//                lib = Mathf.Atan2(-x, y);
//                tab = CustomMath.Atan2(-x, y);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第四象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);
//                lib = Mathf.Atan2(-y, x);
//                tab = CustomMath.Atan2(-y, x);
//                pri = Mathf.Abs(lib - tab);
//                if (pri > PRICISION)
//                    Debug.Log("第四象限误差大于0.001：" + "x:" + x + " y:" + y + " " + pri);

//            }

//        }

//    public static float Atan2Tab(int y, int x)
//    {
//        float Y_X, Y_X_temp;
//        float result = 0;
//        Y_X = (x == 0) ? (float)x / y : (float)y / x;
//        Y_X_temp = Y_X;

//        if (y == 0)
//        {
//            if (x >= 0)
//                return 0;    // 在正x轴上为0
//            else
//                return M_PI; // 在负x轴上为π，180度
//        }
//        else if(x > 0 && y > 0)
//        {
//            //第一象限
//            Y_X = (Y_X <= 1) ? Y_X : 1 / Y_X;
//            result = (Y_X_temp > 1) ? M_PI_2 - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))] : 
//                                    g_atan_tab[(int)(Y_X * (LUT_NUM - 1))];
//        }
//        else if (x < 0 && y > 0)
//        {
//            //第二象限
//            Y_X = (Y_X < -1) ? Y_X = -1 / Y_X : -Y_X;
//            result = (Y_X_temp >= -1) ? M_PI - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))] : //小于45°
//                                    M_PI_2 + g_atan_tab[(int)(Y_X * (LUT_NUM - 1))];
//        }
//        else if (x < 0 && y < 0)
//        {
//            //第三象限
//            Y_X = (Y_X <= 1) ? Y_X : 1 / Y_X;
//            result = (Y_X_temp <= 1) ? g_atan_tab[(int)(Y_X * (LUT_NUM - 1))] - M_PI : 
//                                -M_PI_2 - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))];
//        }
//        else if (x > 0 && y < 0)
//        {
//            //第四象限
//            Y_X = (Y_X < -1) ? Y_X = -1 / Y_X : -Y_X;
//            result = (Y_X_temp >= -1) ? -g_atan_tab[(int)(Y_X * (LUT_NUM - 1))] : //小于45°
//                                    -(M_PI_2 - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))]);
//        }
//        else if (x == 0 && y > 0)
//        {
//            result = M_PI_2;
//        }
//        else
//        {
//            result = -M_PI_2;
//        }

//        return result;
//    }
	
	
//	void Update ()
//    {
		
//	}
//}


//}