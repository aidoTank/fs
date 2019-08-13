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
        public Vector2d m_skillPos;
        public object m_extendParam;

        public BuffTriggerCsvData m_triggerData;
        public int m_curIntervalTime;

        public bool m_bStartCheck;  // 开始检测
        public Collider m_collider;   // 参与碰撞的形状

        public CBuffTrigger(long id)
            : base(id)
        {
            m_type = EThingType.BuffTrigger;
        }

        public override bool Create(int csvId, string name, Vector2d pos, Vector2d dir, float scale = 1)
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

            SetPos(pos, true);
            SetDir(dir);
            SetScale(scale);
            SetSpeed(new FixedPoint(m_triggerData.FlySpeed));

            CFrameTimeMgr.Inst.RegisterEvent(m_triggerData.ContinuanceTime, () =>
            {
                if (!m_destroy)
                    Destory();
            });

            if(m_triggerData.DelayCheckTime == 0)
            {
                m_bStartCheck = true;
                Trigger();
            }
            else
            {
                CFrameTimeMgr.Inst.RegisterEvent(m_triggerData.DelayCheckTime, () =>
                {
                    m_bStartCheck = true;
                    Trigger();
                });
            }
            InitObstacle(pos);
            return true;
        }

        private void InitObstacle(Vector2d pos)
        {
            if(!m_triggerData.Obstacle)
                return;

            if (m_triggerData.ShapeType == (int)eBuffTriggerShapeType.Circle)
            {
                Circle cir = new Circle();
                cir.c = pos;
                cir.r = new FixedPoint(m_triggerData.Length);
                cir.bAirWall = false;
                m_collider = cir;
                PhysicsManager.Inst.Add(m_collider);
            }
            else if (m_triggerData.ShapeType == (int)eBuffTriggerShapeType.Rect)
            {
                int angle = (int)FPCollide.GetAngle(GetDir()).value;
                FPObb obb = new FPObb(pos, new Vector2d(m_triggerData.Width, m_triggerData.Length), angle);
                Polygon pol = new Polygon();
                pol.c = pos;
                pol.isObstacle = true;
                pol.bAirWall = false;
                pol.Init(obb.GetVert2d());
                m_collider = pol;
                PhysicsManager.Inst.Add(m_collider);
            }
        }

        public virtual void InitPos(ref Vector2d startPos, ref Vector2d startDir)
        {

        }

        public override void ExecuteFrame(int frameId)
        {
            if (m_destroy)
                return;

            if (!m_bStartCheck)
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
            if(m_collider != null)
            {
                PhysicsManager.Inst.Remove(m_collider);
            }
        }

        /// <summary>
        /// 通过BUFF区域检测触发
        /// </summary>
        public virtual void Trigger()
        {
            if (m_triggerData.ShapeType == (int)eBuffTriggerShapeType.Circle)
            {
                FPSphere tSphere = new FPSphere();
                tSphere.c = GetPos();
                tSphere.r = new FixedPoint(m_triggerData.Length);

                List<long> list = CCreatureMgr.GetCreatureList();
                for (int i = 0; i < list.Count; i++)
                {
                    CCreature creature = CCreatureMgr.Get(list[i]);
                    if (m_caster.bCamp(creature) || creature.IsDie())
                        continue;

                    FPSphere playerS = new FPSphere();
                    playerS.c = creature.GetPos();
                    playerS.r = creature.GetR();

                    if (FPCollide.bSphereSphere(tSphere, playerS))
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

                    FPSphere playerS = new FPSphere();
                    playerS.c = creature.GetPos();
                    playerS.r = creature.GetR();

                    FPSector sec = new FPSector();
                    sec.pos = GetPos();
                    sec.dir = GetDir();
                    sec.angle = new FixedPoint(m_triggerData.Width);
                    sec.r = new FixedPoint(m_triggerData.Length);

                    if (FPCollide.bSectorInside(sec, creature.GetPos()))
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

                    FPSphere playerS = new FPSphere();
                    playerS.c = creature.GetPos();
                    playerS.r = creature.GetR();

                    Vector2d pos = GetPos();
                    int angle = (int)FPCollide.GetAngle(GetDir()).value;
                    FPObb obb = new FPObb(pos, new Vector2d(m_triggerData.Width, m_triggerData.Length), angle);
                    if (FPCollide.bSphereOBB(playerS, obb))
                    {
                        OnHitAddBuff(m_caster, creature);
                    }
                }
            }

            if (m_triggerData.PosType == (int)eBuffTriggerPosType.CasterStartPos_SkillDir)
            {
                // 障碍碰撞
                if (CMapMgr.m_map.IsblockNotAirWal((int)m_curPos.x.value, (int)m_curPos.y.value))
                {
                    Destory();
                    return;
                }
                if (PhysicsManager.Inst.IsblockNotAirWal((int)m_curPos.x, (int)m_curPos.y))
                {
                    Destory();
                    return;
                }

                // 子弹碰撞
                FPSphere cur = new FPSphere();
                cur.c = GetPos();
                cur.r = GetR();
                foreach (KeyValuePair<long, CBuffTrigger> item in CBuffTriggerMgr.m_dicSkill)
                {
                    CBuffTrigger tri = item.Value;
                    if (tri.m_triggerData.PosType == (int)eBuffTriggerPosType.CasterStartPos_SkillDir && tri != this && tri.m_caster != m_caster)
                    {
                        FPSphere triItem = new FPSphere();
                        triItem.c = tri.GetPos();
                        triItem.r = tri.GetR();

                        if (FPCollide.bSphereSphere(cur, triItem))
                        {
                            Destory();
                            tri.Destory();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 触发器触发BUFF
        /// </summary>
        public void OnHitAddBuff(CCreature caster, CCreature receiver, object obj = null)
        {
            // 给自己加
            int[] selfBuffList = m_triggerData.selfBuffList;
            SkillBase.AddBuff(caster, receiver, selfBuffList, GetPos(), GetPos(), GetDir(), m_skillIndex, obj);

            // 给目标加
            int[] targetBuffList = m_triggerData.targetBuffList;
            SkillBase.AddBuff(caster, receiver, targetBuffList, GetPos(), GetPos(), GetDir(), m_skillIndex, obj);

            if(m_triggerData.bBullet)  // 如果是子弹，碰撞就调用父类的销毁
            {
                base.Destory();
            }
        }
    }
}

