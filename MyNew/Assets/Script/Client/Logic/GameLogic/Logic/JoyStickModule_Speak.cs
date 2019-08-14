using UnityEngine;

namespace Roma
{
    public partial class JoyStickModule
    {
        private PlayerCsvData m_skinData;

        private string[] m_moveSpeakContent;
        private int m_moveSpeakIndex;        // 走路说话的当前索引，走路按照顺序来播放
        private int m_moveSpeakSoundId;
        private float m_curMoveSpeakTime = 0;
        private int m_maxMoveSpeakTime;

        private int m_curMoveSpeakHid;
        private int m_curSkillSpeakHid;

        public void InitSpeak()
        {
            m_skinData = (PlayerCsvData)m_master.m_csvData;

            m_curMoveSpeakTime = 0;
            m_moveSpeakIndex = 0;
            string[] speakData = m_skinData.runSpeak.Split('|');
            if (speakData.Length == 2)
            {
                m_moveSpeakContent = speakData[0].Split('_');
                string sSoundid = m_moveSpeakContent[m_moveSpeakIndex];
                int.TryParse(sSoundid, out m_moveSpeakSoundId);      // 声音id
                int.TryParse(speakData[1], out m_maxMoveSpeakTime);  // 间隔时间
            }
        }

        public void _UpdateMoveSpeak(float fdTime)
        {
            if (m_curMoveJoyStick != eJoyStickEvent.Drag)
                return;
            //if (!m_master.m_beginMove)
            //    return;
            //if (m_master.CheckState((int)DState.State_Back))
            //    return;
            if (m_moveSpeakContent == null)
                return;

            m_curMoveSpeakTime += fdTime;
            if (m_curMoveSpeakTime > m_maxMoveSpeakTime)
            {
                m_curMoveSpeakTime = 0;
                m_moveSpeakIndex++;
                if (m_moveSpeakIndex >= m_moveSpeakContent.Length)
                {
                    m_moveSpeakIndex = 0;
                }
                string sSoundid = m_moveSpeakContent[m_moveSpeakIndex];
                int.TryParse(sSoundid, out m_moveSpeakSoundId);      // 声音id

                SoundManager.Inst.Remove(m_curMoveSpeakHid);
                m_curMoveSpeakHid = SoundManager.Inst.PlaySound(m_moveSpeakSoundId);
            }
        }

        public void OnSkillSpeak(int index)
        {
            int soundId = 0;
            int pct = 0;
            if (index == 0)
                GetSpeak(m_skinData.skill0Speak, ref soundId, ref pct);
            else if (index == 1)
                GetSpeak(m_skinData.skill1Speak, ref soundId, ref pct);
            else if (index == 2)
                GetSpeak(m_skinData.skill2Speak, ref soundId, ref pct);
            else if (index == 3)
                GetSpeak(m_skinData.skill3Speak, ref soundId, ref pct);
            // else if(index == 10)  // 死亡时
            // {
            //     GetSpeak(m_skinData.dieSpeak, ref soundId, ref pct);
            // }
            if (pct >= UnityEngine.Random.Range(0, 100))
            {
                SoundManager.Inst.Remove(m_curMoveSpeakHid);
                SoundManager.Inst.Remove(m_curSkillSpeakHid);
                m_curSkillSpeakHid = SoundManager.Inst.PlaySound(soundId, (e) =>
                {
                    e.SetPos(m_master.GetPos().ToVector3());
                });
            }
        }

        // 死亡播放声音，改到MT，其他人都能听到
        // public void OnDieSpeak()
        // {
        //     if (GameManager.Instance.m_speedUpInLoading)
        //         return;

        //     OnSkillSpeak(10);
        // }

        public void OnDestorySpeak()
        {
            SoundManager.Inst.Remove(m_curMoveSpeakHid);
            SoundManager.Inst.Remove(m_curSkillSpeakHid);
        }

        public static void GetSpeak(string speakInfo, ref int soundId, ref int pct)
        {
            string[] speakData = speakInfo.Split('|');
            if (speakData.Length == 2)
            {
                string[] soundID = speakData[0].Split('_');
                string sSoundid = "";
                if (soundID.Length >= 2)
                {
                    int index = UnityEngine.Random.Range(0, soundID.Length);
                    sSoundid = soundID[index];
                }
                else
                {
                    sSoundid = speakData[0];
                }

                int.TryParse(sSoundid, out soundId);
                int.TryParse(speakData[1], out pct);
            }
        }

        private void GetSpeakByIndex(string speakInfo, int index, ref int soundId, ref int pct)
        {
            string[] speakData = speakInfo.Split('|');
            if (speakData.Length == 2)
            {
                string[] soundID = speakData[0].Split('_');
                string sSoundid = "";

                int.TryParse(sSoundid, out soundId);
                int.TryParse(speakData[1], out pct);
            }
        }
    }
}

