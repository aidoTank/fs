using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
   
    public partial class VObject 
    {
        public Entity m_moveSoundEnt;
        public void PlayMoveSound()
        {
            if (!m_bMaster)
                return;

            if(m_moveSoundEnt == null)
            {
                int soundHid = SoundManager.Inst.PlaySound(51018, m_ent.GetPos());
                m_moveSoundEnt = EntityManager.Inst.GetEnity(soundHid);
            }
            else
            {
                SoundEntity sEnt = m_moveSoundEnt as SoundEntity;
                sEnt.Stop(false);
            }
        }

        public void _UpdateMoveSound()
        {
            if (m_moveSoundEnt != null && m_moveSoundEnt.IsInited())
            {
                m_moveSoundEnt.SetPos(GetEnt().GetPos());
            }
        }

        public void StopMoveSound()
        {
            if (!m_bMaster)
                return;
            if (m_moveSoundEnt != null)
            {
                SoundEntity sEnt = m_moveSoundEnt as SoundEntity;
                sEnt.Stop(true);
            }
        }

        public void DestoryMoveSound()
        {
            if (!m_bMaster)
                return;
            if (m_moveSoundEnt != null)
            {
                SoundManager.Inst.Remove(m_moveSoundEnt.m_hid);
            }
        }
    }
}
