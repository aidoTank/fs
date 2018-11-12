using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
 
    public class CameraMgr : Singleton
    {
        public static CameraMgr Inst;

        public CameraMgr() : base(true) { }

        public Camera m_cam;
        public Transform m_camTransform;
        private Transform m_audioListener;

        private VObject m_hero;
        private Vector3 m_exViewPos;

        // 战斗摄像机设置
        private Vector3 m_direction = Vector3.back * 20f;
        private Vector3 m_curEulerAngle = new Vector3(40, 0, 0);
        private float m_offset;
        // 是否拖动中
        public bool m_bDrag = false;

        public override void Init()
        {
            m_cam = Camera.main;
            m_camTransform = m_cam.transform;
            m_audioListener = m_camTransform.FindChild("audio");

            m_cam.cullingMask = (int)LusuoLayerMask.eEL_Dynamic | (int)LusuoLayerMask.eEL_Static;
            m_cam.renderingPath = RenderingPath.Forward;
            m_cam.nearClipPlane = 6f;
            m_cam.farClipPlane = 160f;
            SetClientConfigData();

        }

        public void SetDepthMode()
        {
            m_cam.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        private void SetClientConfigData()
        {
            if (Client.Inst() != null)
            {
                m_direction = new Vector3(0, 0, -1 * Client.Inst().m_mobaCameraDis);
                m_curEulerAngle = new Vector3(Client.Inst().m_mobaCameraDir, 0, 0);
                m_cam.fieldOfView = Client.Inst().m_mobaCameraFov;
                m_offset = Client.Inst().m_mobaCameraZOffset;
            }
        }

        /// <summary>
        /// 初始化相机为跟随hero
        /// </summary>
        /// <param name="hero"></param>
        public void InitCamera(VObject hero)
        {
            m_hero = hero;
        }

        /// <summary>
        /// 进入游戏时 设置为主角的位置
        /// </summary>
        public void SetAudioListenerPos(Vector3 wPos)
        {
            m_audioListener.position = wPos;
        }

        public override void LateUpdate(float fTime, float fDTime)
        {
            if (Application.isEditor)
            {
                SetClientConfigData();
            }


            if (m_bDrag || m_hero == null || m_hero.GetEnt() == null)
            {
                return;
            }
            // 技能上下左右平滑
            _UpdateSkillMove(fDTime);


            Vector3 camPos = m_hero.GetEnt().GetPos() + 
                Quaternion.Euler(m_curEulerAngle) * (m_direction + Vector3.back * m_curHeightDelta) // 抬高距离
                + m_skillCurDelta - Vector3.forward * (m_offset + m_curHeightZOffet);
            m_camTransform.position = camPos;

            m_camTransform.rotation = Quaternion.Euler(m_curEulerAngle.x, m_curEulerAngle.y, 0);  // 摄像机绕XY旋转

            // 高度距离控制
            _UpdateFov(fDTime);
        }


        public void SetPos(Vector3 pos)
        {
            if (m_camTransform != null)
            {
                m_camTransform.transform.position = pos;
            }
        }

        public Vector3 GetPos()
        {
            if (m_camTransform != null)
            {
                return m_camTransform.transform.position;
            }
            return Vector3.zero;
        }


        #region 技能指示器摄像机偏移
        public Vector3 m_skllMaxDelta = Vector3.zero;
        private Vector3 m_skillCurDelta = Vector3.zero;
        private float m_curSkillTime;
        private float m_maxSkillTime = 1f;

        public void OnSkillMove(Vector3 targetDelta, float time)
        {
            if (Client.Inst().m_bSkillCamera)
            {
                m_skllMaxDelta = targetDelta * 0.2f;
                m_maxSkillTime = time;
                m_curSkillTime = 0;
            }
        }

        private void _UpdateSkillMove(float fDTime)
        {
            if (m_curSkillTime < m_maxSkillTime)
            {
                m_curSkillTime += fDTime;

                float t = m_curSkillTime / m_maxSkillTime;
                m_skillCurDelta = Vector3.Lerp(m_skillCurDelta, m_skllMaxDelta, t);
            }
        }
        #endregion

        #region 技能控制高度
        public float m_maxHeightDelta;
        private float m_curHeightDelta;
        private float m_curHeightTime;
        private float m_maxHeightTime = 1f;

        // 抬高摄像机时的Z偏移，保证技能选择器在中间
        private float m_heightZOffset;   // 0, 0.6f
        private float m_curHeightZOffet;

        /// <summary>
        /// 设置高低
        /// </summary>
        /// <param name="deltaFov">高度偏移</param>
        /// <param name="time">时间</param>
        /// <param name="heightZOffet">Z偏移</param>
        public void OnFov(float deltaFov, float time, float heightZOffet)
        {
            m_curHeightTime = 0;
            m_maxHeightDelta = deltaFov;
            m_maxHeightTime = time;
            m_heightZOffset = heightZOffet;
        }

        private void _UpdateFov(float fDTime)
        {
            if (m_curHeightTime < m_maxHeightTime)
            {
                m_curHeightTime += fDTime;

                float t = m_curHeightTime / m_maxHeightTime;
                m_curHeightDelta = Mathf.Lerp(m_curHeightDelta, m_maxHeightDelta, t);
                m_curHeightZOffet = Mathf.Lerp(m_curHeightZOffet, m_heightZOffset, t);
            }
        }
        #endregion


        public void OnShake(float time = 0.5f)
        {
            ShakeCamera cam = ShakeCamera.Get(m_camTransform.gameObject);
            cam.Play(time);
        }
    }
}