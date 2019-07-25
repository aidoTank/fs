using System.Collections.Generic;
using System;
namespace Roma
{

    public enum eCreatureProp
    {
        Name,
	    Occ,
        Lv,

        // 一级属性
        Force,       // 力量
		Agility,     // 敏捷
        Brain,       // 智力
        InitAtk,     // 初始攻击力
        InitArmor,   // 初始护甲
        InitMoveSpeed, // 初始移速

        // 二级属性
        Hp,      // 力量*20
        CurHp,
        Armor,      // 默认+敏捷*0.17
        MaxMp,      // 智力*12
        CurMp,
        Aspd,       // 默认100+敏捷
        Atk,        // 默认+主属性
        MoveSpeed,  // 默认+（默认*(敏捷*0.06)/100）

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
                 {(int)eCreatureProp.Armor,  CreatureProp.OnChangeArmor},
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

        public static void OnChangeArmor(CCreature obj, int newV, int oldV)
        {

        }
    }
     
}