using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Roma
{
    public class SoundManager : Singleton
    {
        public SoundManager()
            : base(true)
        {

        }

        public int PlaySound(int soundCsvId, Action<Entity> initEnd = null, Action playEnd = null)
        {
            return PlaySound(soundCsvId, Vector3.zero, initEnd, playEnd);
        }

        public int PlaySound(int soundCsvId, Vector3 pos, Action<Entity> initEnd = null, Action playEnd = null)
        {
            if (soundCsvId == 0)
                return 0;
            SoundCsv sound = CsvManager.Inst.GetCsv<SoundCsv>((int)eAllCSV.eAC_Sound);
            if (sound == null)
                return 0;
            SoundCsvData data = sound.GetData(soundCsvId);
            if (data != null)
            {
                return SoundManager.Inst.Play(data.ResourceID, pos, data, initEnd, playEnd);
            }
            else
            {
                //Debug.Log("音效表无此音乐：" + soundCsvId);
            }
            return 0;
        }

        public int Play(int resId, Vector3 pos, SoundCsvData data, Action<Entity> initEnd = null, Action playEnd = null)
        {
            if (resId == 0)
                return 0;
            EntityBaseInfo bInfo = new EntityBaseInfo();
            bInfo.m_resID = resId;
            bInfo.m_soundMute = m_dicTypeMute[(int)data.type];
            bInfo.m_soundType = (int)data.type;
            bInfo.m_soundLoop = data.Loop;
            bInfo.m_vPos = pos;
            bInfo.m_ilayer = (int)LusuoLayer.eEL_Sound;
            int handle = EntityManager.Inst.CreateEntity(eEntityType.eSoundEntity, bInfo, initEnd);
            SoundEntity entity = (SoundEntity)EntityManager.Inst.GetEnity(handle);
            m_dicSoundEntity.Add(entity.m_hid, entity);
            entity.m_playEnd = playEnd;
            return handle;
        }

        public void SetVolumn(SoundType type, float fVal)
        {
            // 设置已经播放的声音
            foreach (KeyValuePair<int, SoundEntity> item in m_dicSoundEntity)
            {
                if (item.Value.m_entityInfo.m_soundType == (int)type)
                {
                    item.Value.SetVolumn(fVal);
                }
            }
            // 设置全局声音
            //m_dicTypeVolumn[(int)type] = fVal;
        }

        public void SetMute(SoundType type, bool mute)
        {
            foreach (KeyValuePair<int, SoundEntity> item in m_dicSoundEntity)
            {
                if (item.Value.m_entityInfo.m_soundType == (int)type)
                {
                    item.Value.SetMute(mute);
                }
            }
            m_dicTypeMute[(int)type] = mute;
        }

        public void SetMute(bool mute)
        {
            foreach (KeyValuePair<int, SoundEntity> item in m_dicSoundEntity)
            {
                item.Value.SetMute(mute);
            }
        }

        /// <summary>
        /// 根据handle设置静音
        /// </summary>
        public void SetStop(int uEntityID, bool stop)
        {
            SoundEntity entity = (SoundEntity)EntityManager.Inst.GetEnity(uEntityID);
            if (null != entity)
            {
                entity.Stop(stop);
            }
        }

        /// <summary>
        /// 暂停与恢复播放
        /// </summary>
        public void Stop(SoundType type, bool stop)
        {
            foreach (KeyValuePair<int, SoundEntity> item in m_dicSoundEntity)
            {
                if (item.Value.m_entityInfo.m_soundType == (int)type)
                {
                    item.Value.Stop(stop);
                }
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Remove(int hid)
        {
            if (m_dicSoundEntity.ContainsKey(hid))
            {
                SoundEntity entity = (SoundEntity)EntityManager.Inst.GetEnity(hid);
                if (null != entity)
                {
                    if (entity.m_entityInfo.m_soundType == (int)SoundType.eBG)
                    {
                        // 延迟2秒结束
                        entity.m_bBgmRemove = true;
                    }
                    else
                    {
                        RemoveLast(hid);
                    }
                }
            }
        }

        public void RemoveLast(int hid)
        {
            EntityManager.Inst.RemoveEntity(hid, true);
            m_dicSoundEntity.Remove(hid);
        }

        public override void Update(float fTime, float fDTime)
        {
            Dictionary<int, SoundEntity>.Enumerator map = m_dicSoundEntity.GetEnumerator();
            while (map.MoveNext())
            {
                if (map.Current.Value.IsInited())
                    m_tempListEffect.Add(map.Current.Value);
            }

            for (int i = 0; i < m_tempListEffect.Count; i++)
            {
                SoundEntity item = m_tempListEffect[i];
                if (!item.IsLoop() && !item.IsPlaying())
                {
                    if (item.m_playEnd != null)
                    {
                        item.m_playEnd();
                        item.m_playEnd = null;
                    }
                    Remove(item.m_hid);
                }
            }
            m_tempListEffect.Clear();
        }

        public override void Destroy()
        {
            base.Destroy();
            List<SoundEntity> list = new List<SoundEntity>(m_dicSoundEntity.Values);
            for (int i = 0; i < list.Count; i++)
            {
                m_dicSoundEntity.Remove(list[i].m_hid);
                EntityManager.Inst.RemoveEntity(list[i].m_hid);
                list[i] = null;
            }
            m_dicSoundEntity.Clear();
        }

        public static SoundManager Inst;
        private Dictionary<int, SoundEntity> m_dicSoundEntity = new Dictionary<int, SoundEntity>();
        // 临时销毁列表
        private List<SoundEntity> m_tempListEffect = new List<SoundEntity>();

        /// <summary>
        /// 通过游戏设置控制的音量
        /// </summary>
        //private Dictionary<int, float> m_dicTypeVolumn = new Dictionary<int, float>();

        /// <summary>
        /// 通过游戏设置控制的音量静音
        /// </summary>
        private Dictionary<int, bool> m_dicTypeMute = new Dictionary<int, bool>();

    }
}
