using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 可旋转发射变长矩形
    /// 绑定施法者的位置lightning
    /// 
    /// 后面将【两点激光】和【可旋转发射变长矩形】分离
    /// 因为他们逻辑处理其实是不一样的
    /// </summary>
    public class CBuffTrigger_Laser :  CBuffTrigger
    {
        public CBuffTrigger_Laser(long id)
            : base(id)
        {

        }

        public override bool Create(int csvId, string name, Vector2d pos, Vector2d dir, float scale = 1)
        {
            base.Create(csvId, name, pos, dir, scale);

            VTrigger vTri = GetVTrigger();
            if (vTri != null)
            {
                // 设置起始点为当前闪电原点
                vTri.SetLineStartPos(vTri.m_baseInfo.m_pos);
            }
            return true;
        }

        public override void InitPos(ref Vector2d startPos, ref Vector2d startDir)
        {
            // 修正起始位置XZ
            Vector3 deltaPos = m_triggerData.vBulletDeltaPos;
            Vector2d vL = FPCollide.Rotate(startDir.normalized, 90) * new FixedPoint(deltaPos.x);   // 左右偏移
            Vector2d vF = startDir.normalized * new FixedPoint(deltaPos.z);                         // 前后偏移
            startPos = m_caster.GetPos() + vL + vF;
        }

        // 矩形检测，获取最近的单位，进行激光链接
        public override void Trigger()
        {
            FixedPoint minDis = new FixedPoint(999999);
            CCreature minCC = null;
            List<long> list = CCreatureMgr.GetCreatureList();
            for (int i = 0; i < list.Count; i++)
            {
                CCreature creature = CCreatureMgr.Get(list[i]);
                if (m_caster.bCamp(creature) || creature.IsDie())
                    continue;

                FPSphere playerS = new FPSphere();
                playerS.c = creature.GetPos();
                playerS.r = creature.GetR();

                Vector2d pos = m_caster.GetPos() + GetDir().normalized * new FixedPoint((m_triggerData.Length + m_triggerData.vBulletDeltaPos.z) * 0.5f);
                int angle = (int)FPCollide.GetAngle(GetDir()).value;
                FPObb obb = new FPObb(pos, new Vector2d(m_triggerData.Width, m_triggerData.Length), angle);
                if (FPCollide.bSphereOBB(playerS, obb))
                {
                    FixedPoint dis = Vector2d.Distance(creature.GetPos(), GetPos());
                    if (dis < minDis)
                    {
                        minDis = dis;
                        minCC = creature;
                    }
                }
            }
            if (minCC != null)
            {
                OnHitAddBuff(m_caster, minCC);
                Vector2d targetPos = GetPos() + GetDir().normalized * minDis;

                if (m_vCreature != null)
                {
                    Vector3 tH = minCC.GetVObject().GetHitHeight();
                    GetVTrigger().SetLineTargetPos(targetPos.ToVector3() + tH);
                }
            }
            else
            {
                _CheckObstacle();
            }
        }

       public void _CheckObstacle()
       {
            //FixedPoint length = FixedPoint.N_0;
            //if (m_bLerp)
            //    length = m_length;
            //else
            //    length = new FixedPoint(m_triggerData.Length);

            Vector2d startPos = m_caster.GetPos();
            Vector2 intersectionPont = Vector2.zero;
            // 实际表现效果要短一点
            Vector2d endPos = startPos + GetDir().normalized * new FixedPoint(m_triggerData.Length);
            if(CMapMgr.GetMap().LineObstacle(
                (int)startPos.x.value, (int)startPos.y.value,
                (int)endPos.x.value, (int)endPos.y.value,
                ref intersectionPont))
            {
                if (m_vCreature != null)
                {
                    GetVTrigger().SetLineTargetPos(intersectionPont.ToVector3());
                }
            }
            else if (m_vCreature != null)
            {
                GetVTrigger().SetLineTargetPos(endPos.ToVector3());
            }
            //}
        }
    }
}

