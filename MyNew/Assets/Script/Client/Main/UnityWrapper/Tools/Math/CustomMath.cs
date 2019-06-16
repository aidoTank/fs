﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma
{
    public class CustomMath
    {
        /// <summary>
        /// 角度-sin值
        /// </summary>
        private static Dictionary<int, FixedPointF> dicSin = new Dictionary<int, FixedPointF>()
        {
            { 0,new FixedPointF(0, 1000)},
            { 1,new FixedPointF(17, 1000)},
            { 2,new FixedPointF(34, 1000)},
            { 3,new FixedPointF(52, 1000)},
            { 4,new FixedPointF(69, 1000)},
            { 5,new FixedPointF(87, 1000)},
            { 6,new FixedPointF(104, 1000)},
            { 7,new FixedPointF(121, 1000)},
            { 8,new FixedPointF(139, 1000)},
            { 9,new FixedPointF(156, 1000)},
            { 10,new FixedPointF(173, 1000)},
            { 11,new FixedPointF(190, 1000)},
            { 12,new FixedPointF(207, 1000)},
            { 13,new FixedPointF(224, 1000)},
            { 14,new FixedPointF(241, 1000)},
            { 15,new FixedPointF(258, 1000)},
            { 16,new FixedPointF(275, 1000)},
            { 17,new FixedPointF(292, 1000)},
            { 18,new FixedPointF(309, 1000)},
            { 19,new FixedPointF(325, 1000)},
            { 20,new FixedPointF(342, 1000)},
            { 21,new FixedPointF(358, 1000)},
            { 22,new FixedPointF(374, 1000)},
            { 23,new FixedPointF(390, 1000)},
            { 24,new FixedPointF(406, 1000)},
            { 25,new FixedPointF(422, 1000)},
            { 26,new FixedPointF(438, 1000)},
            { 27,new FixedPointF(453, 1000)},
            { 28,new FixedPointF(469, 1000)},
            { 29,new FixedPointF(484, 1000)},
            { 30,new FixedPointF(500, 1000)},
            { 31,new FixedPointF(515, 1000)},
            { 32,new FixedPointF(529, 1000)},
            { 33,new FixedPointF(544, 1000)},
            { 34,new FixedPointF(559, 1000)},
            { 35,new FixedPointF(573, 1000)},
            { 36,new FixedPointF(587, 1000)},
            { 37,new FixedPointF(601, 1000)},
            { 38,new FixedPointF(615, 1000)},
            { 39,new FixedPointF(629, 1000)},
            { 40,new FixedPointF(642, 1000)},
            { 41,new FixedPointF(656, 1000)},
            { 42,new FixedPointF(669, 1000)},
            { 43,new FixedPointF(681, 1000)},
            { 44,new FixedPointF(694, 1000)},
            { 45,new FixedPointF(707, 1000)},
            { 46,new FixedPointF(719, 1000)},
            { 47,new FixedPointF(731, 1000)},
            { 48,new FixedPointF(743, 1000)},
            { 49,new FixedPointF(754, 1000)},
            { 50,new FixedPointF(766, 1000)},
            { 51,new FixedPointF(777, 1000)},
            { 52,new FixedPointF(788, 1000)},
            { 53,new FixedPointF(798, 1000)},
            { 54,new FixedPointF(809, 1000)},
            { 55,new FixedPointF(819, 1000)},
            { 56,new FixedPointF(829, 1000)},
            { 57,new FixedPointF(838, 1000)},
            { 58,new FixedPointF(848, 1000)},
            { 59,new FixedPointF(857, 1000)},
            { 60,new FixedPointF(866, 1000)},
            { 61,new FixedPointF(874, 1000)},
            { 62,new FixedPointF(882, 1000)},
            { 63,new FixedPointF(891, 1000)},
            { 64,new FixedPointF(898, 1000)},
            { 65,new FixedPointF(906, 1000)},
            { 66,new FixedPointF(913, 1000)},
            { 67,new FixedPointF(920, 1000)},
            { 68,new FixedPointF(927, 1000)},
            { 69,new FixedPointF(933, 1000)},
            { 70,new FixedPointF(939, 1000)},
            { 71,new FixedPointF(945, 1000)},
            { 72,new FixedPointF(951, 1000)},
            { 73,new FixedPointF(956, 1000)},
            { 74,new FixedPointF(961, 1000)},
            { 75,new FixedPointF(965, 1000)},
            { 76,new FixedPointF(970, 1000)},
            { 77,new FixedPointF(974, 1000)},
            { 78,new FixedPointF(978, 1000)},
            { 79,new FixedPointF(981, 1000)},
            { 80,new FixedPointF(984, 1000)},
            { 81,new FixedPointF(987, 1000)},
            { 82,new FixedPointF(990, 1000)},
            { 83,new FixedPointF(992, 1000)},
            { 84,new FixedPointF(994, 1000)},
            { 85,new FixedPointF(996, 1000)},
            { 86,new FixedPointF(997, 1000)},
            { 87,new FixedPointF(998, 1000)},
            { 88,new FixedPointF(999, 1000)},
            { 89,new FixedPointF(999, 1000)},
            { 90,new FixedPointF(1000, 1000)},
        };

        
        private static Dictionary<int, FixedPointF> dicCos = new Dictionary<int, FixedPointF>()
        {
            { 0,new FixedPointF(1000, 1000)},
            { 1,new FixedPointF(999, 1000)},
            { 2,new FixedPointF(999, 1000)},
            { 3,new FixedPointF(998, 1000)},
            { 4,new FixedPointF(997, 1000)},
            { 5,new FixedPointF(996, 1000)},
            { 6,new FixedPointF(994, 1000)},
            { 7,new FixedPointF(992, 1000)},
            { 8,new FixedPointF(990, 1000)},
            { 9,new FixedPointF(987, 1000)},
            { 10,new FixedPointF(984, 1000)},
            { 11,new FixedPointF(981, 1000)},
            { 12,new FixedPointF(978, 1000)},
            { 13,new FixedPointF(974, 1000)},
            { 14,new FixedPointF(970, 1000)},
            { 15,new FixedPointF(965, 1000)},
            { 16,new FixedPointF(961, 1000)},
            { 17,new FixedPointF(956, 1000)},
            { 18,new FixedPointF(951, 1000)},
            { 19,new FixedPointF(945, 1000)},
            { 20,new FixedPointF(939, 1000)},
            { 21,new FixedPointF(933, 1000)},
            { 22,new FixedPointF(927, 1000)},
            { 23,new FixedPointF(920, 1000)},
            { 24,new FixedPointF(913, 1000)},
            { 25,new FixedPointF(906, 1000)},
            { 26,new FixedPointF(898, 1000)},
            { 27,new FixedPointF(891, 1000)},
            { 28,new FixedPointF(882, 1000)},
            { 29,new FixedPointF(874, 1000)},
            { 30,new FixedPointF(866, 1000)},
            { 31,new FixedPointF(857, 1000)},
            { 32,new FixedPointF(848, 1000)},
            { 33,new FixedPointF(838, 1000)},
            { 34,new FixedPointF(829, 1000)},
            { 35,new FixedPointF(819, 1000)},
            { 36,new FixedPointF(809, 1000)},
            { 37,new FixedPointF(798, 1000)},
            { 38,new FixedPointF(788, 1000)},
            { 39,new FixedPointF(777, 1000)},
            { 40,new FixedPointF(766, 1000)},
            { 41,new FixedPointF(754, 1000)},
            { 42,new FixedPointF(743, 1000)},
            { 43,new FixedPointF(731, 1000)},
            { 44,new FixedPointF(719, 1000)},
            { 45,new FixedPointF(707, 1000)},
            { 46,new FixedPointF(694, 1000)},
            { 47,new FixedPointF(681, 1000)},
            { 48,new FixedPointF(669, 1000)},
            { 49,new FixedPointF(656, 1000)},
            { 50,new FixedPointF(642, 1000)},
            { 51,new FixedPointF(629, 1000)},
            { 52,new FixedPointF(615, 1000)},
            { 53,new FixedPointF(601, 1000)},
            { 54,new FixedPointF(587, 1000)},
            { 55,new FixedPointF(573, 1000)},
            { 56,new FixedPointF(559, 1000)},
            { 57,new FixedPointF(544, 1000)},
            { 58,new FixedPointF(529, 1000)},
            { 59,new FixedPointF(515, 1000)},
            { 60,new FixedPointF(500, 1000)},
            { 61,new FixedPointF(484, 1000)},
            { 62,new FixedPointF(469, 1000)},
            { 63,new FixedPointF(453, 1000)},
            { 64,new FixedPointF(438, 1000)},
            { 65,new FixedPointF(422, 1000)},
            { 66,new FixedPointF(406, 1000)},
            { 67,new FixedPointF(390, 1000)},
            { 68,new FixedPointF(374, 1000)},
            { 69,new FixedPointF(358, 1000)},
            { 70,new FixedPointF(342, 1000)},
            { 71,new FixedPointF(325, 1000)},
            { 72,new FixedPointF(309, 1000)},
            { 73,new FixedPointF(292, 1000)},
            { 74,new FixedPointF(275, 1000)},
            { 75,new FixedPointF(258, 1000)},
            { 76,new FixedPointF(241, 1000)},
            { 77,new FixedPointF(224, 1000)},
            { 78,new FixedPointF(207, 1000)},
            { 79,new FixedPointF(190, 1000)},
            { 80,new FixedPointF(173, 1000)},
            { 81,new FixedPointF(156, 1000)},
            { 82,new FixedPointF(139, 1000)},
            { 83,new FixedPointF(121, 1000)},
            { 84,new FixedPointF(104, 1000)},
            { 85,new FixedPointF(87, 1000)},
            { 86,new FixedPointF(69, 1000)},
            { 87,new FixedPointF(52, 1000)},
            { 88,new FixedPointF(34, 1000)},
            { 89,new FixedPointF(17, 1000)},
            { 90,new FixedPointF(0, 1000)},
        };

        /// <summary>
        /// 曲线图
        /// </summary>
        public static FixedPointF Cos(int tAngle)
        {
            tAngle = ClampAngle(tAngle);
            // -180
            if (tAngle == -180)
            {
                return new FixedPointF(-1);
            }
            // -179和-89
            else if (tAngle < -90)
            {
                tAngle = -tAngle;
                tAngle = 180 - tAngle;
                FixedPointF cos = dicCos[tAngle];
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
                FixedPointF cos = dicCos[tAngle];
                cos = -cos;
                return cos;
            }
            // 180
            else
            {
                return new FixedPointF(-1);
            }
        }

        public static FixedPointF Sin(int tAngle)
        {
            tAngle = ClampAngle(tAngle);
            // -180
            if (tAngle == -180)
            {
                return new FixedPointF(0);
            }
            // -179和-89
            else if (tAngle < -90)
            {
                tAngle = -tAngle;
                tAngle = 180 - tAngle;
                FixedPointF sin = dicSin[tAngle];
                sin = -sin;
                return sin;
            }
            // -90到-1
            else if (tAngle < 0)
            {
                tAngle = -tAngle;
                FixedPointF sin = dicSin[tAngle];
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
                return new FixedPointF(0);
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
    }
}
