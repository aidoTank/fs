using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    /// <summary>
    /// 闪电链
    /// 起点为当前位置
    /// 目标为寻找范围内最近目标
    /// </summary>
    public class CBuffTrigger_Lightning :  CBuffTrigger
    {
        private List<int> m_listHited;

        public CBuffTrigger_Lightning(long id)
            : base(id)
        {
        }

        public override bool Create(int csvId, string name, Vector2 pos, Vector2 dir, float scale = 1)
        {
            if (m_rec == null)
            {
                Destory();
                return false;
            }
           
            base.Create(csvId, name, pos, dir, scale);

            if (m_extendParam != null)
            {
                m_listHited = (List<int>)m_extendParam;
            }
            else
            {
                // when m_extendParam is null, the current class is the first lightning
                m_listHited = new List<int>();
                m_listHited.Add((int)m_rec.GetUid());
            }

            OnHitTarget();
            return true;
        }

        public override void InitPos(ref Vector2 startPos, ref Vector2 startDir)
        {
            // modified starting point
            //startPos = m_rec.GetPos();
        }

        public void OnHitTarget()
        {
            float m_minDis2 = 999999;
            CCreature target = null;
            List<long> list = CCreatureMgr.GetCreatureList();
            for (int i = 0; i < list.Count; i++)
            {
                //CCreature creature = CCreatureMgr.Get(list[i]);
                //if (m_listHited != null && m_listHited.Contains((int)list[i]))
                //    continue;
                //if (m_caster == creature || m_caster.bCamp(creature) || creature.IsDie() || creature == m_rec)
                //    continue;
                //float abDis2 = Collide.GetDis2(m_rec.GetPos(), creature.GetPos());
                //if (abDis2 < m_triggerData.Length * m_triggerData.Length) 
                //{
                //    if (abDis2 < m_minDis2)
                //    {
                //        target = creature;
                //        m_minDis2 = abDis2;
                //    }
                //}
            }
            if(target != null)
            {
                //if(m_listHited == null)
                //    m_listHited = new List<int>();
                //m_listHited.Add((int)target.GetUid());
                //OnHitAddBuff(m_caster, target, m_listHited);

                //VTrigger vTri = GetVTrigger();
                //if(vTri !=  null && m_rec != null && m_rec.GetVObject() != null)
                //{
                //    Vector3 sh = m_rec.GetVObject().GetHitHeight();
                //    Vector3 th = target.GetVObject().GetHitHeight();
                //    vTri.SetLineStartPos(GetPos().ToVector3() + sh);
                //    vTri.SetLineTargetPos(target.GetPos().ToVector3() + th);
                //}
            }
            else
            {
                Destory();
            }
        }
    }
}

