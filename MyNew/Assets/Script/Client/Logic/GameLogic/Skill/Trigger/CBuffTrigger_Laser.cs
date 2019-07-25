using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 可旋转发射变长矩形
    /// 绑定施法者的位置lightning
    /// </summary>
    public class CBuffTrigger_Laser :  CBuffTrigger
    {
        /// <summary>
        /// 矩形可变的长度
        /// </summary>
        private float m_length;
        private bool m_bLerp = true;

        public CBuffTrigger_Laser(long id)
            : base(id)
        {

        }

        public override bool Create(int csvId, string name, Vector2 pos, Vector2 dir, float scale = 1)
        {
            base.Create(csvId, name, pos, dir, scale);
            return true;
        }

        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);

            if(m_bLerp)
            {
                m_length += FSPParam.clientFrameScTime * 40;
                if (m_length >= m_triggerData.Length)
                {
                    m_bLerp = false;
                    m_length = m_triggerData.Length;
                }
            }

            _UpdatePos();
            _UpdateSize();
        }

        public override void _UpdatePos()
        {
            //SetPos(m_caster.GetPos(), true);

            //// 击退的过程中不跟随方向（因为击退时逻辑方向是向后的，技能方向如果跟随角色会出现技能方向错误的问题）
            //if (m_caster.bConBuffLogicType(eBuffType.repel))
            //{
            //    return;
            //}
            //Vector2 lastDir = Collide.Rotate(m_caster.GetDir(), m_triggerData.dirDelta);
            //SetDir(lastDir);
        }

        public void _UpdateSize()
        {

            float length = 0;
            if (m_bLerp)
                length = m_length;
            else
                length = m_triggerData.Length;
            

            float minDis = 999999;
            CCreature minCC = null;
            List<long> list = CCreatureMgr.GetCreatureList();
            for (int i = 0; i < list.Count; i++)
            {
                CCreature creature = CCreatureMgr.Get(list[i]);
                if (m_caster.bCamp(creature) || creature.IsDie())
                    continue;

                Sphere playerS = new Sphere();
                //playerS.c = creature.GetPos();
                //playerS.r = creature.GetR();

                //Vector2 pos = GetPos() + GetDir().normalized * length * 0.5f;
                //float angle = Collide.GetAngle(GetDir());
                //OBB obb = new OBB(pos, new Vector2(m_triggerData.Width, length), angle);
                //if (Collide.bSphereOBB(playerS, obb))
                //{
                //    float dis = Vector2.Distance(creature.GetPos(), GetPos());
                //    if (dis < minDis)
                //    {
                //        minDis = dis;
                //        minCC = creature;
                //    }
                //}
            }
            if (minCC != null)
            {
                m_length = minDis;
                //Vector2 targetPos = GetPos() + GetDir().normalized * m_length;
                //if (m_vCreature != null)
                //{
                //    Vector3 tH = minCC.GetVObject().GetHitHeight();
                //    GetVTrigger().SetLineTargetPos(targetPos.ToVector3() + tH);
                //}
                //m_bLerp = false;
            }
            else
            {
                _CheckObstacle();
            }
        }
        
        public override void Trigger()
        {
            //List<long> list = CCreatureMgr.GetCreatureList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    CCreature creature = CCreatureMgr.Get(list[i]);
            //    if (m_caster.bCamp(creature) || creature.IsDie())
            //        continue;

            //    Sphere playerS = new Sphere();
            //    playerS.c = creature.GetPos();
            //    playerS.r = creature.GetR();

            //    Vector2 pos = GetPos() + GetDir().normalized * m_length * 0.5f;
            //    float angle = Collide.GetAngle(GetDir());
            //    OBB obb = new OBB(pos, new Vector2(m_triggerData.Width, m_length), angle);
            //    if (Collide.bSphereOBB(playerS, obb))
            //    {
            //        OnHitAddBuff(m_caster, creature);
            //    }
            //}
        }

        public void _CheckObstacle()
        {
            float length = 0;
            if (m_bLerp)
                length = m_length;
            else
                length = m_triggerData.Length;

            //Vector2 intersectionPont = Vector2.zero;
            //Vector2 startPos = m_caster.GetPos();
            //Vector2 endPos = startPos + GetDir().normalized * length;
            //if(CMapMgr.GetMap().LineObstacle(
            //    (int)startPos.x, (int)startPos.y,
            //    (int)endPos.x, (int)endPos.y,
            //    ref intersectionPont))
            //{
            //    if (m_vCreature != null)
            //    {
            //        GetVTrigger().SetLineTargetPos(intersectionPont.ToVector3());
            //    }
            //    m_bLerp = false;
            //}
            //else
            //{
            //    if (m_vCreature != null)
            //    {
            //        GetVTrigger().SetLineTargetPos(endPos.ToVector3());
            //    }
            //}
        }
    }
}

