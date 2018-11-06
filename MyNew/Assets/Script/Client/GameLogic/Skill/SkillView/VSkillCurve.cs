using UnityEngine;
using System.Collections.Generic;
using System;

namespace Roma
{
    /// <summary>
    /// 技能的逻辑层
    /// </summary>
    public partial class VSkillCurve : VSkillBase
    {
        private float m_curveTime;
    

        public override void CreateEffectEnt(int index, Action<Entity> loaded)
        {
            base.CreateEffectEnt(index, (ent) =>
            {
                SetCurve(ent, m_curSkillCmd.m_startPos.ToVector3());
            });
        }

        public void OnStartCurve(float time)
        {
            m_curveTime = time;
            //SetCurve(GetEnt(),  m_curSkillCmd.m_startPos.ToVector3());
        }

        private void SetCurve(Entity ent, Vector3 fromPos)
        {
            if(ent == null)
            {
                Debug.LogError("播放曲线动画时，对象为空了");
                return;
            }
            if (ent.GetObject() == null)
            {
                Debug.LogError("播放曲线动画时，对象为空了：" + ent.GetResource().m_resInfo.strName);
                return;
            }

            TweenCurve curve = TweenCurve.Get(ent.GetObject());
            if (curve == null)
                return;

            if (m_flyData == null)
                return;

            curve.duration = m_curveTime;
            curve.from = fromPos;
            curve.to = m_curSkillCmd.m_endPos.ToVector3();

            float h = 7;

            curve.front = Vector3.up * h;
            curve.back = Vector3.up * h;
            curve.Reset();
            curve.Play(true);
        }


    }
}