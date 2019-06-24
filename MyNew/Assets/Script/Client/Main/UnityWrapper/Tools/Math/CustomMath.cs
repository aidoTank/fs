using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class CustomMath
    {
        /// <summary>
        /// 角度-sin值
        /// </summary>
        private static Dictionary<int, FixedPoint> dicSin = new Dictionary<int, FixedPoint>()
        {
            { 0,new FixedPoint(0, 1000)},
            { 1,new FixedPoint(17, 1000)},
            { 2,new FixedPoint(34, 1000)},
            { 3,new FixedPoint(52, 1000)},
            { 4,new FixedPoint(69, 1000)},
            { 5,new FixedPoint(87, 1000)},
            { 6,new FixedPoint(104, 1000)},
            { 7,new FixedPoint(121, 1000)},
            { 8,new FixedPoint(139, 1000)},
            { 9,new FixedPoint(156, 1000)},
            { 10,new FixedPoint(173, 1000)},
            { 11,new FixedPoint(190, 1000)},
            { 12,new FixedPoint(207, 1000)},
            { 13,new FixedPoint(224, 1000)},
            { 14,new FixedPoint(241, 1000)},
            { 15,new FixedPoint(258, 1000)},
            { 16,new FixedPoint(275, 1000)},
            { 17,new FixedPoint(292, 1000)},
            { 18,new FixedPoint(309, 1000)},
            { 19,new FixedPoint(325, 1000)},
            { 20,new FixedPoint(342, 1000)},
            { 21,new FixedPoint(358, 1000)},
            { 22,new FixedPoint(374, 1000)},
            { 23,new FixedPoint(390, 1000)},
            { 24,new FixedPoint(406, 1000)},
            { 25,new FixedPoint(422, 1000)},
            { 26,new FixedPoint(438, 1000)},
            { 27,new FixedPoint(453, 1000)},
            { 28,new FixedPoint(469, 1000)},
            { 29,new FixedPoint(484, 1000)},
            { 30,new FixedPoint(500, 1000)},
            { 31,new FixedPoint(515, 1000)},
            { 32,new FixedPoint(529, 1000)},
            { 33,new FixedPoint(544, 1000)},
            { 34,new FixedPoint(559, 1000)},
            { 35,new FixedPoint(573, 1000)},
            { 36,new FixedPoint(587, 1000)},
            { 37,new FixedPoint(601, 1000)},
            { 38,new FixedPoint(615, 1000)},
            { 39,new FixedPoint(629, 1000)},
            { 40,new FixedPoint(642, 1000)},
            { 41,new FixedPoint(656, 1000)},
            { 42,new FixedPoint(669, 1000)},
            { 43,new FixedPoint(681, 1000)},
            { 44,new FixedPoint(694, 1000)},
            { 45,new FixedPoint(707, 1000)},
            { 46,new FixedPoint(719, 1000)},
            { 47,new FixedPoint(731, 1000)},
            { 48,new FixedPoint(743, 1000)},
            { 49,new FixedPoint(754, 1000)},
            { 50,new FixedPoint(766, 1000)},
            { 51,new FixedPoint(777, 1000)},
            { 52,new FixedPoint(788, 1000)},
            { 53,new FixedPoint(798, 1000)},
            { 54,new FixedPoint(809, 1000)},
            { 55,new FixedPoint(819, 1000)},
            { 56,new FixedPoint(829, 1000)},
            { 57,new FixedPoint(838, 1000)},
            { 58,new FixedPoint(848, 1000)},
            { 59,new FixedPoint(857, 1000)},
            { 60,new FixedPoint(866, 1000)},
            { 61,new FixedPoint(874, 1000)},
            { 62,new FixedPoint(882, 1000)},
            { 63,new FixedPoint(891, 1000)},
            { 64,new FixedPoint(898, 1000)},
            { 65,new FixedPoint(906, 1000)},
            { 66,new FixedPoint(913, 1000)},
            { 67,new FixedPoint(920, 1000)},
            { 68,new FixedPoint(927, 1000)},
            { 69,new FixedPoint(933, 1000)},
            { 70,new FixedPoint(939, 1000)},
            { 71,new FixedPoint(945, 1000)},
            { 72,new FixedPoint(951, 1000)},
            { 73,new FixedPoint(956, 1000)},
            { 74,new FixedPoint(961, 1000)},
            { 75,new FixedPoint(965, 1000)},
            { 76,new FixedPoint(970, 1000)},
            { 77,new FixedPoint(974, 1000)},
            { 78,new FixedPoint(978, 1000)},
            { 79,new FixedPoint(981, 1000)},
            { 80,new FixedPoint(984, 1000)},
            { 81,new FixedPoint(987, 1000)},
            { 82,new FixedPoint(990, 1000)},
            { 83,new FixedPoint(992, 1000)},
            { 84,new FixedPoint(994, 1000)},
            { 85,new FixedPoint(996, 1000)},
            { 86,new FixedPoint(997, 1000)},
            { 87,new FixedPoint(998, 1000)},
            { 88,new FixedPoint(999, 1000)},
            { 89,new FixedPoint(999, 1000)},
            { 90,new FixedPoint(1000, 1000)},
        };

        
        private static Dictionary<int, FixedPoint> dicCos = new Dictionary<int, FixedPoint>()
        {
            { 0,new FixedPoint(1000, 1000)},
            { 1,new FixedPoint(999, 1000)},
            { 2,new FixedPoint(999, 1000)},
            { 3,new FixedPoint(998, 1000)},
            { 4,new FixedPoint(997, 1000)},
            { 5,new FixedPoint(996, 1000)},
            { 6,new FixedPoint(994, 1000)},
            { 7,new FixedPoint(992, 1000)},
            { 8,new FixedPoint(990, 1000)},
            { 9,new FixedPoint(987, 1000)},
            { 10,new FixedPoint(984, 1000)},
            { 11,new FixedPoint(981, 1000)},
            { 12,new FixedPoint(978, 1000)},
            { 13,new FixedPoint(974, 1000)},
            { 14,new FixedPoint(970, 1000)},
            { 15,new FixedPoint(965, 1000)},
            { 16,new FixedPoint(961, 1000)},
            { 17,new FixedPoint(956, 1000)},
            { 18,new FixedPoint(951, 1000)},
            { 19,new FixedPoint(945, 1000)},
            { 20,new FixedPoint(939, 1000)},
            { 21,new FixedPoint(933, 1000)},
            { 22,new FixedPoint(927, 1000)},
            { 23,new FixedPoint(920, 1000)},
            { 24,new FixedPoint(913, 1000)},
            { 25,new FixedPoint(906, 1000)},
            { 26,new FixedPoint(898, 1000)},
            { 27,new FixedPoint(891, 1000)},
            { 28,new FixedPoint(882, 1000)},
            { 29,new FixedPoint(874, 1000)},
            { 30,new FixedPoint(866, 1000)},
            { 31,new FixedPoint(857, 1000)},
            { 32,new FixedPoint(848, 1000)},
            { 33,new FixedPoint(838, 1000)},
            { 34,new FixedPoint(829, 1000)},
            { 35,new FixedPoint(819, 1000)},
            { 36,new FixedPoint(809, 1000)},
            { 37,new FixedPoint(798, 1000)},
            { 38,new FixedPoint(788, 1000)},
            { 39,new FixedPoint(777, 1000)},
            { 40,new FixedPoint(766, 1000)},
            { 41,new FixedPoint(754, 1000)},
            { 42,new FixedPoint(743, 1000)},
            { 43,new FixedPoint(731, 1000)},
            { 44,new FixedPoint(719, 1000)},
            { 45,new FixedPoint(707, 1000)},
            { 46,new FixedPoint(694, 1000)},
            { 47,new FixedPoint(681, 1000)},
            { 48,new FixedPoint(669, 1000)},
            { 49,new FixedPoint(656, 1000)},
            { 50,new FixedPoint(642, 1000)},
            { 51,new FixedPoint(629, 1000)},
            { 52,new FixedPoint(615, 1000)},
            { 53,new FixedPoint(601, 1000)},
            { 54,new FixedPoint(587, 1000)},
            { 55,new FixedPoint(573, 1000)},
            { 56,new FixedPoint(559, 1000)},
            { 57,new FixedPoint(544, 1000)},
            { 58,new FixedPoint(529, 1000)},
            { 59,new FixedPoint(515, 1000)},
            { 60,new FixedPoint(500, 1000)},
            { 61,new FixedPoint(484, 1000)},
            { 62,new FixedPoint(469, 1000)},
            { 63,new FixedPoint(453, 1000)},
            { 64,new FixedPoint(438, 1000)},
            { 65,new FixedPoint(422, 1000)},
            { 66,new FixedPoint(406, 1000)},
            { 67,new FixedPoint(390, 1000)},
            { 68,new FixedPoint(374, 1000)},
            { 69,new FixedPoint(358, 1000)},
            { 70,new FixedPoint(342, 1000)},
            { 71,new FixedPoint(325, 1000)},
            { 72,new FixedPoint(309, 1000)},
            { 73,new FixedPoint(292, 1000)},
            { 74,new FixedPoint(275, 1000)},
            { 75,new FixedPoint(258, 1000)},
            { 76,new FixedPoint(241, 1000)},
            { 77,new FixedPoint(224, 1000)},
            { 78,new FixedPoint(207, 1000)},
            { 79,new FixedPoint(190, 1000)},
            { 80,new FixedPoint(173, 1000)},
            { 81,new FixedPoint(156, 1000)},
            { 82,new FixedPoint(139, 1000)},
            { 83,new FixedPoint(121, 1000)},
            { 84,new FixedPoint(104, 1000)},
            { 85,new FixedPoint(87, 1000)},
            { 86,new FixedPoint(69, 1000)},
            { 87,new FixedPoint(52, 1000)},
            { 88,new FixedPoint(34, 1000)},
            { 89,new FixedPoint(17, 1000)},
            { 90,new FixedPoint(0, 1000)},
        };

        /// <summary>
        /// 曲线图
        /// </summary>
        public static FixedPoint Cos(int tAngle)
        {
            tAngle = ClampAngle(tAngle);
            // -180
            if (tAngle == -180)
            {
                return new FixedPoint(-1);
            }
            // -179和-89
            else if (tAngle < -90)
            {
                tAngle = -tAngle;
                tAngle = 180 - tAngle;
                FixedPoint cos = dicCos[tAngle];
                cos = -cos;
                return cos;
            }
            // -90到-1
            else if (tAngle < 0)
            {
                tAngle = -tAngle;
                return dicCos[tAngle];
            }
            // 0到89
            else if (tAngle < 90)
            {
                return dicCos[tAngle];
            }
            // 90到179
            else if (tAngle < 180)
            {
                tAngle = 180 - tAngle;
                FixedPoint cos = dicCos[tAngle];
                cos = -cos;
                return cos;
            }
            // 180
            else
            {
                return new FixedPoint(-1);
            }
        }

        public static FixedPoint Sin(int tAngle)
        {
            tAngle = ClampAngle(tAngle);
            // -180
            if (tAngle == -180)
            {
                return new FixedPoint(0);
            }
            // -179和-89
            else if (tAngle < -90)
            {
                tAngle = -tAngle;
                tAngle = 180 - tAngle;
                FixedPoint sin = dicSin[tAngle];
                sin = -sin;
                return sin;
            }
            // -90到-1
            else if (tAngle < 0)
            {
                tAngle = -tAngle;
                FixedPoint sin = dicSin[tAngle];
                sin = -sin;
                return sin;
            }
            // 0到89
            else if (tAngle < 90)
            {
                return dicSin[tAngle];
            }
            // 90到179
            else if (tAngle < 180)
            {
                tAngle = 180 - tAngle;
                return dicSin[tAngle];
            }
            // 180
            else
            {
                return new FixedPoint(0);
            }
        }

        /// <summary>
        /// 限定在-180，180
        /// </summary>
        public static int ClampAngle(int tAngle)
        {
            if (tAngle != 0)
            {
                tAngle %= 360;
                if (tAngle > 180)
                    tAngle -= 360;
                else if (tAngle < -180)
                    tAngle += 360;
            }
            return tAngle;
        }


        private const int LUT_NUM = 100;
        private const float PRICISION = 0.02f;
        private static float[] g_atan_tab = new float[LUT_NUM]
        {
            0f,0.01010067f,0.02019927f,0.03029376f,0.04038208f,0.05046217f,0.06053202f,0.07058959f,0.08063287f,0.09065989f,
            0.1006687f,0.1106572f,0.1206237f,0.1305661f,0.1404826f,0.1503714f,0.1602307f,0.1700586f, 0.1798535f,0.1896136f,
            0.1993373f,0.209023f,0.218669f,0.2282738f,0.2378359f,0.2473539f,0.2568265f,0.2662521f, 0.2756294f,0.2849573f,
            0.2942346f,0.3034599f,0.3126323f,0.3217506f,0.3308137f,0.3398209f,0.348771f,0.3576632f,0.3664968f,0.3752708f,
            0.3839846f,0.3926375f,0.4012288f,0.4097579f,0.4182243f,0.4266275f,0.434967f,0.4432423f, 0.4514531f,0.459599f,
            0.4676798f,0.4756952f,0.4836449f,0.4915288f,0.4993467f,0.5070985f,0.5147841f,0.5224034f, 0.5299565f,0.5374433f,
            0.5448639f,0.5522184f,0.5595067f,0.5667292f,0.5738859f,0.580977f,0.5880026f,0.594963f, 0.6018585f,0.6086893f,
            0.6154557f,0.6221579f,0.6287963f,0.6353712f,0.641883f,0.648332f,0.6547186f,0.6610432f, 0.6673061f,0.6735079f,
            0.6796489f,0.6857295f,0.6917502f,0.6977115f,0.7036138f,0.7094575f,0.7152432f,0.7209713f, 0.7266424f,0.7322568f,
            0.7378151f,0.7433177f,0.7487653f,0.7541583f,0.7594972f,0.7647825f,0.7700148f,0.7751944f,0.7803221f,0.7853982f
        };

        // 返回弧度 [-π/2, π/2]  [-90, 90]
        //public static void InitATanTab()
        //{
        //    // (0,1) 返回（0-45）度的弧度值
        //    string val = "";
        //    for (int i = 0; i < LUT_NUM; i++)
        //    {
        //        float rad = (float)i / (LUT_NUM - 1);
        //        Debug.Log(rad);
        //        float info = Mathf.Atan(rad);
        //        val += info + "f,";
        //    }
        //    Debug.Log(val);
        //}

        /// <summary>
        /// 返回角度
        /// </summary>
        public static FixedPoint Atan2(int y, int x)
        {
            return FixedPoint.Rad2Deg * Atan2_rad(y, x);
        }

        /// <summary>
        /// 返回弧度
        /// </summary>
        public static FixedPoint Atan2_rad(int y, int x)
        {
            FixedPoint Y_X, Y_X_temp;
            FixedPoint result = FixedPoint.zero;
            Y_X = (x == 0) ? 
                new FixedPoint(x ,y) :
                new FixedPoint(y, x);
            Y_X_temp = Y_X;

            if (y == 0)
            {
                if (x >= 0)
                    return FixedPoint.zero * FixedPoint.Pi;    // 在正x轴上为0
                else
                    return FixedPoint.Pi; // 在负x轴上为π，180度
            }
            else if (x > 0 && y > 0)
            {
                //第一象限
                Y_X = (Y_X <= 1) ? Y_X : 1 / Y_X;
                result = (Y_X_temp > 1) ? FixedPoint.HalfPi - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))] : 
                                            (FixedPoint)g_atan_tab[(int)(Y_X * (LUT_NUM - 1))];
            }
            else if (x < 0 && y > 0)
            {
                //第二象限
                Y_X = (Y_X < -1) ? Y_X = -1 / Y_X : -Y_X;
                result = (Y_X_temp >= -1) ? FixedPoint.Pi - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))] : //小于45°
                                        FixedPoint.HalfPi + g_atan_tab[(int)(Y_X * (LUT_NUM - 1))];
            }
            else if (x < 0 && y < 0)
            {
                //第三象限
                Y_X = (Y_X <= 1) ? Y_X : 1 / Y_X;
                result = (Y_X_temp <= 1) ? g_atan_tab[(int)(Y_X * (LUT_NUM - 1))] - FixedPoint.Pi : 
                                            -FixedPoint.HalfPi - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))];
            }
            else if (x > 0 && y < 0)
            {
                //第四象限
                Y_X = (Y_X < -1) ? Y_X = -1 / Y_X : -Y_X;
                result = (Y_X_temp >= -1) ? (FixedPoint)(-g_atan_tab[(int)(Y_X * (LUT_NUM - 1))]) : //小于45°
                                        -(FixedPoint.HalfPi - g_atan_tab[(int)(Y_X * (LUT_NUM - 1))]);
            }
            else if (x == 0 && y > 0)
            {
                result = FixedPoint.HalfPi;
            }
            else
            {
                result = -FixedPoint.HalfPi;
            }

            return result;
        }
    }
}
