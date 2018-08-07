//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Roma
//{
//    public class SoundManager : Singleton
//	{
//        public SoundManager()
//            : base(true)
//        {
//        }

//        public uint PlaySound(int soundCsvId, EntityInitNotify initEnd = null)
//        {
//            //SoundCsv sound = CsvManager.Inst.GetCsv<SoundCsv>((int)eAllCSV.eAC_Sound);
//            //if (sound == null)
//            //    return 0;
//            //SoundCsvData data = sound.GetData(soundCsvId);
//            //if (data != null)
//            //{
//            //    return SoundManager.Inst.Play((uint)data.ResourceID, (SoundType)data.type, initEnd);
//            //}
//            //else
//            //{
//            //    Debug.Log("音效表无此音乐：" + soundCsvId);
//            //}
//            return 0;
//        }

//        public uint Play(int resId, SoundType type, EntityInitNotify initEnd = null)
//        {
//            EntityBaseInfo bInfo = new EntityBaseInfo();
//            bInfo.m_resID = resId;
//            bInfo.m_initNotify = initEnd;
//            uint handle = EntityManager.Inst.CreateEntity(eEntityType.eSoundEntity, OnEntityLoaded, bInfo);
//            SoundEntity entity = (SoundEntity)EntityManager.Inst.GetEnity(handle, false);
//            entity.m_type = type;
//            return handle;
//        }

//        private void OnEntityLoaded(Entity ent, object obj)
//        {
//            SoundEntity sEnt = (SoundEntity)ent;
//            m_dicSoundEntity.Add(ent.m_handleID, sEnt);
//            float soundVol = 1.0f;
//            // 通过游戏设置获取声音
//            if (m_dicTypeVolumn.ContainsKey((int)sEnt.m_type))
//            {
//                soundVol = m_dicTypeVolumn[(int)sEnt.m_type];
//            }
//            sEnt.SetVolumn(soundVol);

//            // 执行回调，如果有
//            if (ent.m_entityInfo.m_initNotify != null)
//            {
//                ent.m_entityInfo.m_initNotify(ent, obj);
//            }
//            // 如果是说话，就降之前的声音变小
//            if (sEnt.m_type == SoundType.eSpeak)
//            {
//                TweenFloat startPlay = TweenFloat.Get(sEnt.GetObject());
//                startPlay.from = 0;
//                startPlay.to = soundVol;
//                startPlay.duration = 2f;
//                startPlay.FloatUpdateEvent = (val) =>
//                {
//                    sEnt.SetVolumn(val);
//                };
//                startPlay.Play(true);

//                foreach (KeyValuePair<uint, SoundEntity> item in m_dicSoundEntity)
//                {
//                    if(item.Value.m_type == SoundType.eSpeak && item.Key != sEnt.m_handleID)
//                    {
//                        TweenFloat tf = TweenFloat.Get(item.Value.GetObject());
//                        tf.from = soundVol;
//                        tf.to = 0;
//                        tf.duration = 3f;
//                        tf.FloatUpdateEvent = (val) => {
//                            item.Value.SetVolumn(val);
//                        };
//                        tf.Play(true);
//                    }
//                }
//            }
//        }

//        public void SetVolumn(SoundType type,  float fVal)
//        {
//            // 设置已经播放的声音
//            foreach(KeyValuePair<uint, SoundEntity> item in m_dicSoundEntity)
//            {
//                if(item.Value.m_type == type)
//                {
//                    item.Value.SetVolumn(fVal);
//                }
//            }
//            // 设置全局声音
//            m_dicTypeVolumn[(int)type] = fVal;
//        }

    

//        public void SetMute(SoundType type, bool mute)
//        {
//            foreach (KeyValuePair<uint, SoundEntity> item in m_dicSoundEntity)
//            {
//                if (item.Value.m_type == type)
//                {
//                    item.Value.SetMute(mute);
//                }
//            }
//        }

//        public void SetMute(bool mute)
//        {
//            foreach (KeyValuePair<uint, SoundEntity> item in m_dicSoundEntity)
//            {
//                item.Value.SetMute(mute);
//            }
//        }

//        /// <summary>
//        /// 根据handle设置静音
//        /// </summary>
//        public void SetStop(uint uEntityID, bool stop)
//        {
//            SoundEntity entity = (SoundEntity)EntityManager.Inst.GetEnity(uEntityID, false);
//            if (null != entity)
//            {
//                entity.Stop(stop);
//            }
//        }

//        /// <summary>
//        /// 暂停与恢复播放
//        /// </summary>
//        public void Stop(SoundType type, bool stop)
//        {
//            foreach (KeyValuePair<uint, SoundEntity> item in m_dicSoundEntity)
//            {
//                if (item.Value.m_type == type)
//                {
//                    item.Value.Stop(stop);
//                }
//            }
//        }

//        /// <summary>
//        /// 停止，就是销毁
//        /// </summary>
//        /// <param name="uEntityID"></param>
//        public void Remove(uint uEntityID)
//        {
//            SoundEntity entity = (SoundEntity)EntityManager.Inst.GetEnity(uEntityID, false);
//            if (null != entity)
//            {
//                if (m_dicSoundEntity.ContainsKey(uEntityID))
//                {
//                    EntityManager.Inst.RemoveEntity(entity.m_handleID, false, false);
//                    m_dicSoundEntity.Remove(uEntityID);
//                }
//            }
//        }

//        public override void Update(float fTime, float fDTime)
//        {
//            Dictionary<uint, SoundEntity>.Enumerator map = m_dicSoundEntity.GetEnumerator();
//            while (map.MoveNext())
//            {
//                m_tempListEffect.Add(map.Current.Value);
//            }

//            for (int i = 0; i < m_tempListEffect.Count(); i ++)
//            {
//                SoundEntity item = m_tempListEffect[i];
//                if (!item.IsLoop() && !item.IsPlaying())
//                {
//                    //Debug.Log("停止：" + item.Value.GetResource().GetResInfo().strName);
//                    Remove(item.m_handleID);
//                }
//            }
//            m_tempListEffect.Clear();
//        }

//        public override void Destroy()
//        {
//            base.Destroy();
//            List<SoundEntity> list = new List<SoundEntity>(m_dicSoundEntity.Values);
//            for (int i = 0; i < list.Count; i++)
//            {
//                m_dicSoundEntity.Remove(list[i].m_handleID);
//                EntityManager.Inst.RemoveEntity(list[i].m_handleID, false, false);
//                list[i] = null;
//            }
//            m_dicSoundEntity.Clear();
//        }

//        public new static SoundManager Inst;
//        private Dictionary<uint, SoundEntity> m_dicSoundEntity = new Dictionary<uint, SoundEntity>();
//        // 临时销毁列表
//        private List<SoundEntity> m_tempListEffect = new List<SoundEntity>();

//        /// <summary>
//        /// 通过游戏设置控制的音量
//        /// </summary>
//        private Dictionary<int, float> m_dicTypeVolumn = new Dictionary<int, float>();

//    }
//}
