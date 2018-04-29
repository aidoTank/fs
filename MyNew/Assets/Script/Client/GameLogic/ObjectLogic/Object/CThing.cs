using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roma
{
    public enum EThingType
    {
        None = 0,           // 0 = 未明确
        Master = 1,             // 主角
        Player = 2,             // 玩家
        Pet = 3,                // 宠物
        Ride = 4,               // 坐骑
        NPC = 5,                // NPC
        Monster = 6,            // 怪物
        Chunnel = 7,            // 传送门
        Barrier = 8,            // 障碍
        Item = 9,              // 地上的物品
        Buff = 10,               // 地上的BUFF
        Plant = 11,             // 家园植物
        Soil = 12,               //种植土地
        Box = 13,               //箱子
        WorldBoss = 14,         // 世界BOSS
        PetCub = 15,             // 宠物幼崽

    }
    public class CThing
    {
        protected long m_uId = 0;
        protected EThingType m_type = EThingType.None;
        //public CThingHead m_thingHead;


        public CThing(long id)
        {
            m_uId = id;
        }

        public virtual void Destory()
        {

        }

        public long GetUid()
        {
            return m_uId;
        }

        public void SetType(EThingType type)
        {
            m_type = type;
        }

        public EThingType GetThingType()
        {
            return m_type;
        }

        /// <summary>
        /// 主角
        /// </summary>
        public bool IsMaster()
        {
            return m_type == EThingType.Master;
        }

        public bool IsPlayer()
        {
            return m_type == EThingType.Player;
        }

        public bool IsPet()
        {
            return m_type == EThingType.Pet;
        }

        public bool IsMonster()
        {
            return m_type == EThingType.Monster;
        }

        public bool IsNpc()
        {
            return m_type == EThingType.NPC;
        }

        public bool IsChunnel()
        {
            return m_type == EThingType.Chunnel;
        }

        public bool IsPlant()
        {
            return m_type == EThingType.Plant;
        }

        public bool IsSoil()
        {
            return m_type == EThingType.Soil;
        }

        public bool IsRide()
        {
            return m_type == EThingType.Ride;
        }

        public bool IsWorldBoss()
        {
            return m_type == EThingType.WorldBoss;
        }
    }
}