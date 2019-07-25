using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    /// <summary>
    /// BUFF触发器，共用CCreature的部分接口，比如属性接口，可以被打掉的陷阱对象等
    /// 1.资源为特效表中的资源
    /// 2.声音为特效表中的声音
    /// </summary>
    public partial class CBuffTrigger : CObject
    {
        /// <summary>
        /// 又BUFF20传入的发起者和接受者
        /// </summary>
        public CCreature m_caster;
        public CCreature m_rec; 
        public int m_skillIndex;
        public Vector2 m_skillPos;
        public object m_extendParam;

        public BuffTriggerCsvData m_triggerData;
        public int m_curIntervalTime;

        public CBuffTrigger(long id)
            : base(id)
        {
            m_type = EThingType.BuffTrigger;
        }

        public override bool Create(int csvId, string name, Vector2 pos, Vector2 dir, float scale = 1)
        {
            // 通过不同类型，读取不同配置表
            BuffTriggerCsv buffCsv = CsvManager.Inst.GetCsv<BuffTriggerCsv>((int)eAllCSV.eAc_BuffTrigger);
            m_triggerData = buffCsv.GetData(csvId);

            InitPos(ref pos, ref dir);

            if (m_triggerData.ModelResId != 0)
            {
                // 表现
                m_vCreature = VObjectMgr.Create(eVOjectType.SkillTrigger);
                sVOjectBaseInfo info = new sVOjectBaseInfo();
                info.m_resId = m_triggerData.ModelResId;
                info.m_pos = pos.ToVector3() + Vector3.up * m_triggerData.vBulletDeltaPos.y;
                info.m_dir = dir.ToVector3();
                info.m_headHeight = m_triggerData.HeadHeight;
                info.m_showHead = false;
                m_vCreature.Create(info);
            }

            SetPos(pos.ToVector2d());
            SetDir(dir.ToVector2d());
            SetScale(scale);
            SetSpeed(new FixedPoint(m_triggerData.FlySpeed));

            CFrameTimeMgr.Inst.RegisterEvent(m_triggerData.ContinuanceTime, () =>
            {
                if (!m_destroy)
                    Destory();
            });

            Trigger();
            return true;
        }

        public virtual void InitPos(ref Vector2 startPos, ref Vector2 startDir)
        {

        }

        public override void ExecuteFrame(int frameId)
        {
            if (m_destroy)
                return;

            _UpdatePos();

            if (m_triggerData.IntervalTime == 0)
                return;

            m_curIntervalTime += FSPParam.clientFrameMsTime;
            if (m_curIntervalTime >= m_triggerData.IntervalTime)
            {
                m_curIntervalTime = 0;

                Trigger();
            }
        }

        public virtual void _UpdatePos()
        {

        }

        public override void Destory()
        {
            base.Destory();
            if (m_triggerData != null && m_triggerData.bAutoTrigger)
            {
                OnHitAddBuff(m_caster, null);
            }
        }

        /// <summary>
        /// 通过BUFF区域检测触发
        /// </summary>
        public virtual void Trigger()
        {
            if (m_triggerData.ShapeType == (int)eBuffTriggerShapeType.Circle)
            {
                Sphere tSphere = new Sphere();
                tSphere.c = GetPos().ToVector2();
                tSphere.r = m_triggerData.Length;

                List<long> list = CCreatureMgr.GetCreatureList();
                for (int i = 0; i < list.Count; i++)
                {
                    CCreature creature = CCreatureMgr.Get(list[i]);
                    if (m_caster.bCamp(creature) || creature.IsDie())
                        continue;

                    Sphere playerS = new Sphere();
                    playerS.c = creature.GetPos().ToVector2();
                    playerS.r = creature.GetR().value;

                    if (Collide.bSphereSphere(tSphere, playerS))
                    {
                        OnHitAddBuff(m_caster, creature);
                    }
                }
            }
            else if (m_triggerData.ShapeType == (int)eBuffTriggerShapeType.Sector)
            {
                List<long> list = CCreatureMgr.GetCreatureList();
                for (int i = 0; i < list.Count; i++)
                {
                    CCreature creature = CCreatureMgr.Get(list[i]);
                    if (m_caster.bCamp(creature) || creature.IsDie())
                        continue;

                    Sphere playerS = new Sphere();
                    playerS.c = creature.GetPos().ToVector2();
                    playerS.r = creature.GetR().value;

                    if (Collide.bSectorInside(GetPos().ToVector2(), GetDir().ToVector2(), m_triggerData.Width, m_triggerData.Length, creature.GetPos().ToVector2()))
                    {
                        OnHitAddBuff(m_caster, creature);
                    }
                }
            }
            else if (m_triggerData.ShapeType == (int)eBuffTriggerShapeType.Rect)
            {
                List<long> list = CCreatureMgr.GetCreatureList();
                for (int i = 0; i < list.Count; i++)
                {
                    CCreature creature = CCreatureMgr.Get(list[i]);
                    if (m_caster.bCamp(creature) || creature.IsDie())
                        continue;

                    Sphere playerS = new Sphere();
                    playerS.c = creature.GetPos().ToVector2();
                    playerS.r = creature.GetR().value;

                    Vector2 pos = GetPos().ToVector2() + m_caster.GetDir().normalized.ToVector2() * m_triggerData.Length * 0.5f;
                    float angle = Collide.GetAngle(m_caster.GetDir().ToVector2());
                    OBB obb = new OBB(pos, new Vector2(m_triggerData.Width, m_triggerData.Length), angle);
                    if (Collide.bSphereOBB(playerS, obb))
                    {
                        OnHitAddBuff(m_caster, creature);
                    }
                }
            }

            // 静态碰撞
            if (m_triggerData.PosType == (int)eBuffTriggerPosType.CasterStartPos_SkillDir)
            {
                if (CMapMgr.m_map.IsblockNotAirWal((int)m_curPos.x, (int)m_curPos.y))
                {
                    Destory();
                    return;
                }
                //if (PhysicsManager.Inst.IsblockNotAirWal((int)m_curPos.x, (int)m_curPos.y))
                //{
                //    Destory();
                //    return;
                //}
            }
        }

        /// <summary>
        /// 触发器触发BUFF
        /// </summary>
        public void OnHitAddBuff(CCreature caster, CCreature receiver, object obj = null)
        {
            // 给自己加
            int[] selfBuffList = m_triggerData.selfBuffList;
            SkillBase.AddBuff(caster, receiver, selfBuffList, GetPos().ToVector2(), GetPos().ToVector2(), GetDir().ToVector2(), m_skillIndex, obj);

            // 给目标加
            int[] targetBuffList = m_triggerData.targetBuffList;
            SkillBase.AddBuff(caster, receiver, targetBuffList, GetPos().ToVector2(), GetPos().ToVector2(), GetDir().ToVector2(), m_skillIndex, obj);

            if(m_triggerData.bBullet)  // 如果是子弹，碰撞就调用父类的销毁
            {
                base.Destory();
            }
        }
    }
}

