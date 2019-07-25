using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public partial class VTrigger : VBase
    {
        private Entity m_soundEnt;

        public VTrigger()
        {

        }

        public override void CreateEnd(Entity ent)
        {
            base.CreateEnd(ent);
            // 触发器的特效声音
            PlayEffectSound(m_baseInfo.m_resId);
        }

      
        public override void Update(float time, float fdTime)
        {
            if (m_ent == null || !m_ent.IsInited())
                return;

            _UpdateEffectSound();

            base.Update(time, fdTime);
        }


        public override void Destory()
        {
            DestroyEffectSound();
            base.Destory();
        }

        public void PlayEffectSound(int effectId)
        {
            if (GetEnt() == null)
                return;

            // 播放声音
            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            EffectCsvData data = effectCsv.GetData(effectId);
            if (data != null)
            {
                int soundHid = SoundManager.Inst.PlaySound(data.soundId, GetEnt().GetPos());
                m_soundEnt = EntityManager.Inst.GetEnity(soundHid);
            }
        }

        public void DestroyEffectSound()
        {
            if (m_soundEnt != null)
            {
                SoundManager.Inst.Remove(m_soundEnt.m_hid);
            }
        }

        public void _UpdateEffectSound()
        {
            // 声音位置和特效位置一致
            if (m_soundEnt != null && m_soundEnt.IsInited())
            {
                m_soundEnt.SetPos(GetEnt().GetPos());
            }
        }

        #region 可变长矩形接口-连接式特效
        public void SetLineStartPos(Vector3 sPos)
        {
            if (m_ent == null)
                return;

            EffectEntity eEnt = m_ent as EffectEntity;
            if (eEnt == null)
                return;
            eEnt.SetLineStart(sPos);
        }

        public void SetLineTargetPos(Vector3 tPos)
        {
            if (m_ent == null)
                return;

            EffectEntity eEnt = m_ent as EffectEntity;
            if (eEnt == null)
                return;
            eEnt.SetLineEnd(tPos);
        }
        #endregion

    }


}

