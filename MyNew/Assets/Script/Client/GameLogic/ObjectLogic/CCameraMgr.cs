using UnityEngine;
namespace Roma
{
    public class CCameraMgr
    {
        public static Camera m_mainCam;
        private static Transform m_cam;
        private static Transform m_target;
        private static bool m_bInit = false;

        // 操作相关
        private const float m_sensitivity = 0.2f;
        private static Vector3 m_preMousePos;
        private static bool m_bMouseDown = false;

        // 角度相关
        private const float MIN_ANGLE = 10;
        private const float MAX_ANGLE = 90;
        private static Vector3 m_direction = Vector3.back * 20f;
        private static Vector3 m_curEulerAngle = new Vector3(40, 0, 0);

        public static void Init()
        {
            m_mainCam = GameObject.Find("Camera").GetComponent<Camera>();
            m_cam = m_mainCam.transform;
            //m_target = CPlayerMgr.GetMaster().GetEntity().GetObject().transform;
            //m_bInit = true;
            //// 初始化摄像机
            //m_cam.eulerAngles = m_curEulerAngle;
            //m_cam.position = m_target.position + Quaternion.Euler(m_curEulerAngle) * m_direction;
        }

        private static void OnDown()
        {
            m_curEulerAngle = m_cam.eulerAngles;
        }

        private static void OnDrag(Vector2 deltaPos)
        {
            m_curEulerAngle.y += deltaPos.x;    // 平面左右滑动x轴方向的增量用于摄像机绕y轴旋转
            m_curEulerAngle.x -= deltaPos.y;    // 平面上下滑动y轴方向的增量用于摄像机绕x轴旋转

            m_curEulerAngle.x = Mathf.Clamp(m_curEulerAngle.x, MIN_ANGLE, MAX_ANGLE);    // 锁定绕X轴的角度范围（锁定上下）
            m_cam.rotation = Quaternion.Euler(m_curEulerAngle.x, m_curEulerAngle.y, 0);  // 摄像机绕XY旋转
        }

        public static void Update()
        {
            if (!m_bInit)
                return;
            
            if (Application.platform == RuntimePlatform.Android || 
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)// 一个点开始
                    {
                        OnDown();
                        m_preMousePos = Input.mousePosition;
                        m_bMouseDown = true;
                    }
                    else if (touch.phase == TouchPhase.Ended) // 一个点抬起
                    {
                        m_bMouseDown = false;
                    }
                    else if (touch.phase == TouchPhase.Moved)// 一个点移动
                    {
                        OnDrag((touch.deltaPosition) * m_sensitivity * 10);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))   // 按下
                {
                    OnDown();
                    m_preMousePos = Input.mousePosition;
                    m_bMouseDown = true;
                }
                if (Input.GetMouseButtonUp(0))    // 抬起
                {
                    m_bMouseDown = false;
                }
                if (m_bMouseDown)
                {
                    OnDrag((Input.mousePosition - m_preMousePos) * m_sensitivity);
                    m_preMousePos = Input.mousePosition;
                }
            }
            m_cam.position = m_target.position + Quaternion.Euler(m_curEulerAngle) * m_direction;
        }
    }
}
