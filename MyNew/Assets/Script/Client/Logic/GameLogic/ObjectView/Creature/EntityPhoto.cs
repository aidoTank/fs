using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Roma
{
    public class HeroPhotoMgr : Singleton
    {
        public static HeroPhotoMgr Inst;
        public List<HeroPhoto> m_listPohot = new List<HeroPhoto>();

        public HeroPhotoMgr() : base(true)
        {

        }

        public void Add(HeroPhoto p)
        {
            m_listPohot.Add(p);
        }

        public void Remove(HeroPhoto p)
        {
            m_listPohot.Remove(p);
        }

        public override void LateUpdate(float fTime, float fDTime)
        {
            if (Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer)
                return;

            for (int i = 0; i < m_listPohot.Count; i++)
            {
                HeroPhoto p = m_listPohot[i];
                if (p.m_bFullScreen)
                {
                    p.Update();
                }
            }
        }

        public override void Destroy()
        {
            for (int i = 0; i < m_listPohot.Count; i++)
            {
                HeroPhoto p = m_listPohot[i];
                p.DestroyPhoto();
            }
        }
    }

    public enum PhotoType
    {
        hero,
        uieffect
    }
    public class HeroPhoto
    {
        private RawImage m_photoImage;    // 当前拍照对象要映射的Texture
        private int m_resId;
        private PhotoType m_type;
        public bool m_bFullScreen;
        private int m_w;
        private int m_h;

        private static int m_photoPos = 100;
        private GameObject m_photoObject;
        private Camera m_photoCam;         // 当前相机
        private RenderTexture m_photoRT;   // RT

        private const int m_bgResId = 21000;
        private int m_bgHandleId;

        public BoneEntity m_boneEnt;
        private int m_handleId;

        private UIHeroShowEffectCsvData m_showEffectData;
        private List<int> m_eventList = new List<int>();
        private List<int> m_enterEffectHid = new List<int>();  // 进入特效hid
        private int m_idleEffectHid;                           // 待机动作特效hid

        private float m_bgXOffset;

        private int m_speakSoundHid;
        /// <summary>
        /// 设置相机偏移
        /// </summary>
        /// <param name="pos"></param>
        public void SetCamOffset(Vector3 pos)
        {
            m_photoCam.transform.localPosition += pos;
            m_bgXOffset = pos.x;
            SetBgOffset();
        }
        /// <summary>
        /// 全屏时，w,h无效
        /// </summary>
        public HeroPhoto(ref RawImage image, int resId, PhotoType type, int w = 2048, int h = 2048, bool bFullScreen = false)
        {
            if (Application.isEditor)
            {
                image.material.shader = Shader.Find(image.material.shader.name);
            }
            Debug.Log("打开玩家展示：" + resId);
            m_bFullScreen = bFullScreen;
            if (bFullScreen)
            {
                RectTransform rect = image.GetComponent<RectTransform>();
                rect.anchorMax = Vector2.one;
                rect.anchorMin = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }

            m_resId = resId;
            m_photoImage = image;
            m_type = type;
            m_w = w;
            m_h = h;

            UIHeroShowEffectCsv showEffectCsv = CsvManager.Inst.GetCsv<UIHeroShowEffectCsv>((int)eAllCSV.eAC_UIHeroShowEffect);
            m_showEffectData = showEffectCsv.GetData((int)m_resId);

            OnCreateEnv();

            if (type == PhotoType.uieffect)
            {
                OnCreateEnterPhoto("born");
            }
            else
            {
                // 先加载预加载再创建
                if (m_showEffectData == null)
                    return;
                LoadPre(m_showEffectData, () =>
                {
                    if (m_photoObject == null)
                        return;
                    OnCreateUI();
                    OnEnter();
                });
            }
            HeroPhotoMgr.Inst.Add(this);
        }

        public void Update()
        {
            if (m_bFullScreen && m_photoRT != null &&
                (m_photoRT.width != Screen.width || m_photoRT.height != Screen.height))
            {
                m_photoRT.DiscardContents();
                GameObject.Destroy(m_photoRT);

                RenderTextureFormat rtFormat = RenderTextureFormat.Default;
                if (CheckSupport(ref rtFormat))
                {
                    m_photoRT = new RenderTexture(Screen.width, Screen.height, 1, rtFormat);
                    m_photoRT.depth = 24;
                    m_photoCam.targetTexture = m_photoRT;
                    m_photoImage.texture = m_photoCam.targetTexture;
                }
            }
        }

        private void SetBgOffset()
        {
            //Entity ent = EntityManager.Inst.GetEnity(m_bgHandleId);
            //if (ent == null || !ent.IsInited())
            //    return;
            //Transform bg = ent.GetObject().transform.FindChild("bg");
            //bg.localPosition = new Vector3(m_bgXOffset, bg.localPosition.y, bg.localPosition.z);
        }

        #region 环境
        private void OnCreateEnv()
        {
            // 创建photo对象,摄像机和灯光
            m_photoObject = new GameObject("photo_" + m_resId);
            m_photoObject.transform.position = new Vector3(m_photoPos, 0, 0);
            m_photoPos += 100;

            if (m_type == PhotoType.hero)
            {
                // 背景
                //EntityBaseInfo infoBg = new EntityBaseInfo();
                //infoBg.m_resID = m_bgResId;
                //infoBg.m_ilayer = (int)LusuoLayer.eEL_Photo;
                //infoBg.m_parent = m_photoObject.transform;
                //m_bgHandleId = EntityManager.Inst.CreateEntity(eEntityType.eBoneEntity, infoBg, (ent) =>
                //{
                //    SetBgOffset();
                //});
            }

            m_photoCam = (new GameObject("cam")).AddComponent<Camera>();
            m_photoCam.transform.SetParent(m_photoObject.transform);
            m_photoCam.cullingMask = LayerMask.GetMask(LayerMask.LayerToName((int)LusuoLayer.eEL_Photo));  // 以及特效
            m_photoCam.depth = -1;
            m_photoCam.farClipPlane = 15;
            m_photoCam.backgroundColor = Color.clear;
            m_photoCam.clearFlags = CameraClearFlags.SolidColor;
            m_photoCam.useOcclusionCulling = false;

            RenderTextureFormat rtFormat = RenderTextureFormat.Default;
            if (CheckSupport(ref rtFormat))
            {
                if (m_bFullScreen)
                {
                    m_photoRT = new RenderTexture((int)(Screen.width * 1f), (int)(Screen.height * 1f), 1, rtFormat);
                }
                else
                {
                    m_photoRT = new RenderTexture((int)(m_w * 1f), (int)(m_h * 1f), 1, rtFormat);
                }
                m_photoRT.depth = 24;
                m_photoCam.targetTexture = m_photoRT;
                if (m_photoImage != null)
                {
                    m_photoImage.texture = m_photoCam.targetTexture;
                    m_photoImage.gameObject.SetActiveNew(true);
                }
            }

            // 动态读取配置
            UIHeroShowCsv csv = CsvManager.Inst.GetCsv<UIHeroShowCsv>((int)eAllCSV.eAC_UIHeroShow);
            UIHeroShowCsvData data = csv.GetData((int)m_resId);

            if (data.orthographic == 0)  // 透视
            {
                m_photoCam.orthographic = false;
                m_photoCam.fieldOfView = data.fovOrSize;
                m_photoCam.farClipPlane = 15f;
            }
            else
            {
                m_photoCam.orthographic = true;
                m_photoCam.orthographicSize = data.fovOrSize;
            }

            if (data != null)
            {
                // 相机
                m_photoCam.transform.localPosition = data.vPos;
                m_photoCam.transform.localEulerAngles = data.vRota;

                GameObject lig1 = new GameObject("l1");
                lig1.transform.SetParent(m_photoObject.transform);
                Light light = lig1.AddComponent<Light>();
                light.type = LightType.Directional;
                light.transform.localPosition = Vector3.zero;
                light.transform.localEulerAngles = data.vLightPos_1;
                light.color = data.vLightColor_1 / 255;
                light.intensity = data.lightIntensity_1;
                light.range = 10;
                light.cullingMask = LayerMask.GetMask(LayerMask.LayerToName((int)LusuoLayer.eEL_Photo));

                GameObject lig2 = new GameObject("l2");
                lig2.transform.SetParent(m_photoObject.transform);
                Light light2 = lig2.AddComponent<Light>();
                light2.type = LightType.Point;
                light2.transform.localPosition = data.vLightPos_2;
                light2.color = data.vLightColor_2 / 255;
                light2.intensity = data.lightIntensity_2;
                light2.range = 10;
                light2.cullingMask = LayerMask.GetMask(LayerMask.LayerToName((int)LusuoLayer.eEL_Photo));
            }
        }
        #endregion


        private void OnEnter()
        {
            // 先播放入场特效
            if (m_showEffectData == null)
            {
                Debug.LogError("英雄展示特效表无资源id:" + m_resId);
                OnCreateEnterPhoto("born");
                return;
            }

            string[] timeList = m_showEffectData.enterEffectTime.Split('|');
            string[] effectList = m_showEffectData.enterEffectId.Split('|');

            for (int i = 0; i < timeList.Length; i++)
            {
                float time = 0;
                float.TryParse(timeList[i], out time);
                int effectId = 0;
                int.TryParse(effectList[i], out effectId);

                int eEventId = TimeMgr.Inst.RegisterEvent(time, () =>
                 {

                     int hId = CEffectMgr.Create(effectId, m_photoObject.transform.position, Vector3.zero);
                     CEffect ef = CEffectMgr.GetEffect(hId);
                     if (ef != null)
                         ef.SetLayer(LusuoLayer.eEL_Photo);
                     m_enterEffectHid.Add(hId);
                 });
     
                m_eventList.Add(eEventId);
            }

            // 入场播放动作
            int eventId = TimeMgr.Inst.RegisterEvent(m_showEffectData.enterAnimaTime, ()=> {

                if (m_photoObject == null)
                    return;
                string animaName = m_showEffectData.enterAnimaName;

                EntityBaseInfo info = new EntityBaseInfo();
                info.m_resID = (int)m_resId;
                info.m_ilayer = (int)LusuoLayer.eEL_Photo;
                m_handleId = EntityManager.Inst.CreateEntity(eEntityType.eBoneEntity, info, (ent) =>
                {
                    if (m_photoObject == null)
                        return;
                    // 播放人物动作
                    if (m_type == PhotoType.hero)
                    {
                        m_boneEnt = ent as BoneEntity;
                        m_boneEnt.SetParent(m_photoObject.transform);

                        string idleAnima = "idle";
                        AnimationAction anima = new AnimationAction();
                        anima.strFull = animaName;
                        anima.eMode = WrapMode.Once;
                        anima.endEvent = () =>
                        {
                            AnimationAction anima2 = new AnimationAction();
                            anima2.crossTime = AnimationInfo.m_crossTime;
                            anima2.strFull = idleAnima;
                            anima2.eMode = WrapMode.Loop;
                            Play(anima2);
                            OnCreateIdleEffect();
                        };
                        Play(anima);
                    }
                    else if (m_type == PhotoType.uieffect)
                    {
                        m_boneEnt = ent as BoneEntity;
                        m_boneEnt.SetParent(m_photoObject.transform);

                        AnimationAction anima = new AnimationAction();
                        anima.strFull = "idle";
                        anima.eMode = WrapMode.Loop;
                        Play(anima);
                    }
                });
            });
            m_eventList.Add(eventId);
            // 播放英雄声音
            m_speakSoundHid = SoundManager.Inst.PlaySound(m_showEffectData.enterSpeakId, null);
        }

        private void OnCreateEnterPhoto(object obj)
        {

        }

        private void OnCreateEnterEffect(object obj)
        {
            if (m_photoObject == null)
                return;
            int effectId = (int)obj;
            int hId = CEffectMgr.Create(effectId, m_photoObject.transform.position, Vector3.zero);
            CEffect ef = CEffectMgr.GetEffect(hId);
            if (ef != null)
                ef.SetLayer(LusuoLayer.eEL_Photo);
            m_enterEffectHid.Add(hId);
        }

        private bool m_bCanClick = true;
        private void OnCreateUI()
        {
            if (m_photoImage == null)
                return;
            m_photoImage.gameObject.SetActiveNew(true);
            UIDragListener.Get(m_photoImage.gameObject).OnDragEvent = (ev, delta) =>
            {
                if (m_boneEnt == null)
                    return;
                if (m_boneEnt.m_animation != null && m_boneEnt.m_animation.IsPlaying("born"))
                    return;
                if (ev == eDragEvent.Drag)
                {
                    m_boneEnt.SetDirection(m_boneEnt.GetRotate() + new Vector3(0f, -1 * delta.x, 0f));
                }
            };
            UIEventListener.Get(m_photoImage.gameObject).onClick = (obj) =>
            {
                if (m_boneEnt == null || m_boneEnt.m_animation == null || !m_bCanClick)
                    return;
                if (m_boneEnt.m_animation.IsPlaying("idle") &&
                    !m_boneEnt.m_animation.IsPlaying(m_showEffectData.clickAnimaName)) // 在休闲时，才播放点击动作
                {
                    m_bCanClick = false;
                    AnimationAction anima = new AnimationAction();
                    anima.crossTime = 0.2f;
                    anima.strFull = m_showEffectData.clickAnimaName;
                    anima.eMode = WrapMode.Once;
                    anima.endEvent = () =>
                    {
                        m_bCanClick = true;
                        AnimationAction anima2 = new AnimationAction();
                        anima2.crossTime = 0.2f;
                        anima2.strFull = "idle";
                        anima2.eMode = WrapMode.Loop;
                        Play(anima2);
                        OnCreateIdleEffect();
                    };
                    Play(anima);
                    OnDestroyIdleEffect();
                    // 点击播放动作时，播放对应的特效，切换动作时删除
                    int hId = CEffectMgr.Create(m_showEffectData.clickEffectId, m_boneEnt, "origin");
                    CEffect ef = CEffectMgr.GetEffect(hId);
                    if (ef != null)
                        ef.SetLayer(LusuoLayer.eEL_Photo);
                }
            };
        }

        public void OnCreateIdleEffect()
        {
            OnDestroyIdleEffect();
            // 待机动作时的特效
            m_idleEffectHid = CEffectMgr.Create(m_showEffectData.idleEffectId, m_boneEnt, "origin");
            CEffect effect = CEffectMgr.GetEffect(m_idleEffectHid);
            if (effect != null)
                effect.SetLayer(LusuoLayer.eEL_Photo);
        }


        private void OnDestroyIdleEffect()
        {
            if (m_idleEffectHid != 0)
            {
                CEffectMgr.Destroy(m_idleEffectHid);
                m_idleEffectHid = 0;
            }
        }

        public void Play(AnimationAction info)
        {
            if (m_boneEnt == null)
            {
                return;
            }
            m_boneEnt.Play(info);
        }

        public void DestroyPhoto()
        {
            if (m_photoImage != null)
            {
                m_photoImage.gameObject.SetActiveNew(false);
            }
            if (m_photoRT != null)
            {
                m_photoRT.DiscardContents();
                GameObject.Destroy(m_photoRT);
                m_photoRT = null;
            }
            if (null != m_photoImage)
            {
                m_photoImage.texture = null;
                m_photoImage = null;
            }
            if (null != m_photoCam)
            {
                m_photoCam.targetTexture = null;
                m_photoCam = null;
            }
            // 销毁背景
            EntityManager.Inst.RemoveEntity(m_bgHandleId, true);

            if (null != m_photoObject)
            {
                GameObject.Destroy(m_photoObject);
            }
            // 销毁待机特效
            OnDestroyIdleEffect();
            // 销毁入场特效
            for (int i = 0; i < m_enterEffectHid.Count; i++)
            {
                CEffectMgr.Destroy(m_enterEffectHid[i]);
            }
            m_enterEffectHid.Clear();
            // 当前模型实体
            if (m_handleId != 0)
            {
                EntityManager.Inst.RemoveEntity(m_handleId);
                m_handleId = 0;
            }
            for (int i = 0; i < m_eventList.Count; i++)
            {
                TimeMgr.Inst.RemoveEvent(m_eventList[i]);
            }
            m_eventList.Clear();
            SoundManager.Inst.Remove(m_speakSoundHid);
            HeroPhotoMgr.Inst.Remove(this);
            DestoryPre();
        }

        private bool CheckSupport(ref RenderTextureFormat rtF)
        {
            if (!SystemInfo.supportsImageEffects)
            {
                //Debug.Log("not support iamgeEffect");
                return false;
            }

            rtF = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32)
                ? RenderTextureFormat.ARGB32
                : RenderTextureFormat.Default;
            return true;
        }

        #region 预加载
        private Resource[] m_preRes;

        public void LoadPre(UIHeroShowEffectCsvData data, Action loaded)
        {
            List<int> loadPre = new List<int>();
            int resId = data.resId;
            loadPre.Add(resId);

            EffectCsv effectCsv = CsvManager.Inst.GetCsv<EffectCsv>((int)eAllCSV.eAC_Effect);
            // 入场特效和声音
            string[] effectList = m_showEffectData.enterEffectId.Split('|');
            for (int i = 0; i < effectList.Length; i++)
            {
                int effectId;
                int.TryParse(effectList[i], out effectId);
                EffectCsvData enterEffect = effectCsv.GetData(effectId);
                if (enterEffect == null)
                {
                    Debug.LogWarning("英雄展示表配了特效id，但是特效表没有：" + effectId);
                }
                else
                {
                    if (enterEffect.resId != 0)
                        loadPre.Add(enterEffect.resId);
                    if (enterEffect.soundId != 0)
                        loadPre.Add(enterEffect.soundId);
                }
            }
            // 点击特效和声音
            int clickEffect = m_showEffectData.clickEffectId;
            EffectCsvData eData = effectCsv.GetData(clickEffect);
            if (eData == null)
            {
                Debug.LogWarning("英雄展示表配了特效id，但是特效表没有：" + clickEffect);
            }
            else
            {
                if (eData.resId != 0)
                    loadPre.Add(eData.resId);
                if (eData.soundId != 0)
                    loadPre.Add(eData.soundId);
            }

            m_preRes = new Resource[loadPre.Count];
            //Debug.Log("预加载数量：" + loadPre.Count);
            int curPreNum = 0;
            for (int i = 0; i < loadPre.Count; i++)
            {
                Resource tRes = ResourceFactory.Inst.LoadResource(loadPre[i], (res) =>
                {
                    curPreNum++;
                    if (curPreNum == loadPre.Count)
                    {
                        if (loaded != null)
                            loaded();
                    }
                });
                m_preRes[i] = tRes;
            }
        }

        public void DestoryPre()
        {
            if (m_preRes == null)
                return;
            for (int i = 0; i < m_preRes.Length; i++)
            {
                ResourceFactory.Inst.UnLoadResource(m_preRes[i]);
            }
            m_preRes = null;
        }
        #endregion
    }
}