using System.Collections.Generic;
using System;
namespace Roma
{

    public enum eCreatureProp
    {
        Name,
	    Occ,
        MoveSpeed,

        Force,   // 力量
		Agility, // 敏捷
        Brain,   // 智力
        
        Max
    }

    public class CreatureProp
    {
        public static void Init()
        {
            CCreature.m_dicPlayerData = new Dictionary<int, Action<CCreature, int, int>>()
            {
                 {(int)eCreatureProp.MoveSpeed,  CreatureProp.OnChangeMoveSpeed},
                 {(int)eCreatureProp.Force,  CreatureProp.OnChangeForce},
                 {(int)eCreatureProp.Agility,  CreatureProp.OnChangeAgility},
                 {(int)eCreatureProp.Brain,  CreatureProp.OnChangeBrain},
            };
        }

        public static void OnChangeMoveSpeed(CCreature obj, int newV, int oldV)
        {

        }

        public static void OnChangeForce(CCreature obj, int newV, int oldV)
        {

        }

        public static void OnChangeAgility(CCreature obj, int newV, int oldV)
        {

        }

        public static void OnChangeBrain(CCreature obj, int newV, int oldV)
        {

        }
    }
     
}