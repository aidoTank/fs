using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
   
    public partial class VObject 
    {

        private int m_speakHid;

        private float m_curMoveSpeakTime = 0;
        private int m_maxMoveSpeakTime = 5;

        //public void PlaySpeak(eRoleSpeakCsv speakType)
        //{
        //    // ��ǰ˵��û˵�꣬��������˵
        //    if (m_speakHid != 0)
        //        return;

        //    // ͳһ10%�ĸ���
        //    int pct = GameManager.Inst.m_clientRand.Next(0, 100);
        //    int maxPct = 10;
        //    if(speakType == eRoleSpeakCsv.revive)  // ����100%˵��
        //    {
        //        maxPct = 100;
        //    }
        //    if(pct > maxPct)
        //    {
        //        return;
        //    }
        //    RoleSpeakCsv speakCsv = CsvManager.Inst.GetCsv<RoleSpeakCsv>((int)eAllCSV.eAC_RoleSpeak);
        //    int soundId = speakCsv.GetSpeak(m_baseInfo.m_speakId, speakType);
        //    m_speakHid = SoundManager.Inst.PlaySound(soundId, null, ()=> {
        //        m_speakHid = 0;
        //        m_curMoveSpeakTime = 0;
        //    });
        //}

        //public void _Update_MoveSpeak(float fdTime)
        //{
        //    if(m_bMaster && m_bMoveing)
        //    {
        //        m_curMoveSpeakTime += fdTime;
        //        if(m_curMoveSpeakTime > m_maxMoveSpeakTime)
        //        {
        //            PlaySpeak(eRoleSpeakCsv.move);
        //            m_curMoveSpeakTime = 0;
        //        }
        //    }
        //}
    }
}
