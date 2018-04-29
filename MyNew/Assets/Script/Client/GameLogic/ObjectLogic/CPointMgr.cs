using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Roma
{
    /// <summary>
    /// 鼠标管理
    /// 管理所有对象的管理类
    /// </summary>
    public class CPointMgr
    {
        private static Ray ray;
        public static RaycastHit rayhit;
        private static RaycastHit[] m_npcHitList = new RaycastHit[4];
        // 选择特效id
        private static uint m_selectEffectHandlID = 0;

        // 手机平台参数
        private static float m_pressMaxTime = 0.3f; // 如果超过0.5f就不作为移动
        private static float m_pressCurTime = 0.0f; // 如果超过0.5f就不作为移动
        private static bool m_bStartMove = false;
        private static Vector2 m_preTouchPos1;
        private static Vector2 m_preTouchPos2;
        // PC平台参数
        private static bool m_bMouseDown = false;
        private static Vector3 m_preMousePos;
        // 摄像机躲避障碍物参数
        private static float m_camMoveT = 0.0f;
        private static float m_curCamLocalZ = -20.0f;
        // 是否固定角度(5月最新需求，无自由视角设置)
        public static bool m_bCamDirFixed = true;
        // 摄像机的X旋转，用于控制最高和最低视角
        private static float m_camDirMinX = 8;
        private static float m_camDirMaxX = 40;
        // 摄像机固定角度
        private static float m_camDirFixedX = 45;

        private static float m_touchDeltaRate = 1.0f;

        /// <summary>
        /// 是否在滑动摄像机,PC和手机通用
        /// </summary>
        public static bool m_bTouchCam = false;


        public static bool isStart = true;
        public static bool isNavigation = false;
        public static bool cameraFollow = false;
        public static Vector3 startPos = new Vector3();
        public static Vector2 posListEnd = new Vector2();
        public static Vector3 endPos = new Vector3();
        public static float rotateSpeed = 3.0f;
        public static float perspectiveAngles = 30.0f;

        public static void Init()
        {
        }

        public static Vector3 GetMousePos()
        {
            if (Camera.main == null)
                return Vector3.zero;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out rayhit, 200))
            {
                return rayhit.point;
            }
            return Vector3.zero;
        }

        public static void Update(float fTime, float fDTime)
        {
            if(Application.isEditor)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    Client.Inst().m_timeScale += fDTime * 4f;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    Client.Inst().m_timeScale -= fDTime * 4f;
                }
            }

            _UpdateLowLight();

 
           // CPointMgr.OnCamAutoPos();
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                #region 新版本的手机移动
                if (Input.touchCount > 0)
                {
                    // 如果点在UI上，就不执行下面的
                    if (IsClickUI())
                    {
                        m_bStartMove = false;
                        m_bMouseDown = false;

                        m_bTouchCam = false;
                        return;
                    }
                    // 一个点，并且没有点到UI上
                    if (Input.touchCount == 1)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)// 一个点开始
                        {
                            m_bStartMove = true;
                            m_pressCurTime = 0;

                            m_bMouseDown = true;
                        }
                        else if (Input.GetTouch(0).phase == TouchPhase.Ended) // 一个点抬起为移动主角和选中目标
                        {
                            m_bMouseDown = false;
                            m_bTouchCam = false;
                            if (m_bStartMove)
                            {
                                // 如果点到模型
                                if (OnClickCreature(Input.GetTouch(0).position))
                                {
                                    return;
                                }
                                OnClickScreenMove(Input.GetTouch(0).position);
                            }
                        }
                        else if (Input.GetTouch(0).phase == TouchPhase.Moved)// 一个点移动，就移动视角y
                        {
                            m_bMouseDown = false;
                            OnOneMove(Input.GetTouch(0).deltaPosition * m_touchDeltaRate);
                        }
                        m_pressCurTime += fDTime;
                        if (m_pressCurTime > m_pressMaxTime)
                        {
                            m_bStartMove = false;
                        }
                    }
                    else if (Input.touchCount == 2)
                    {
                        OnTwoMove();
                    }
                }
                #endregion
            }
			else
            {
                // 如果点在UI上，就不执行下面的
                if (IsClickUI())
                {
                    m_bMouseDown = false;
                    m_bTouchCam = false;
                    return;
                }
                if (Input.GetMouseButtonDown(0))   // 按下
                {
                    m_preMousePos = Input.mousePosition;
                    m_pressCurTime = 0;

                    m_bMouseDown = true;
                }
                if (Input.GetMouseButtonUp(0)) // 鼠标左键选择，抬起
                {
                    OnClickCreature(Input.mousePosition);
                    m_bMouseDown = false;
                    m_bTouchCam = false;
                }
                if(m_bMouseDown)
                {
                    OnOneMove((Input.mousePosition - m_preMousePos) * 0.5f);
                    m_preMousePos = Input.mousePosition;
                }

                if (Input.GetMouseButtonUp(1) && !IsClickUI()) // 鼠标右键移动
                {
                    OnClickScreenMove(Input.mousePosition);
                }
            }

            if(m_bMouseDown)
            {
                // 长按事件
                m_pressCurTime += fDTime;
                if (m_pressCurTime > 1.2f)
                {
                    m_pressCurTime = 0;
                    m_bMouseDown = false;
                    //Debug.LogError("触发长按。。。。。。。。。。。。。");
                    OnLongClickCreature(Input.mousePosition);
                }
            }
        }


        private static void OnClickScreenMove(Vector3 screen)
        {

            ClearSelectPlayer();


            ray = Camera.main.ScreenPointToRay(screen);
            if (Physics.Raycast(ray, out rayhit, 10000, LayerMask.GetMask(LayerMask.LayerToName((int)LusuoLayer.eEL_Static))))
            {
                CCreature master = CPlayerMgr.GetMaster();
                if (master == null)
                    return;
                master.GoTo(rayhit.point.x, rayhit.point.z, eControlMode.eCM_mouse, 0);
            }
        }

        public static void OnOneMove(Vector2 deltaPos)
        {

        }

        private static void OnTwoMove()
        {
     

        }

        private static bool OnClickCreature(Vector3 screen)
        {
            // 抓宠游戏屏蔽选择角色  
            // 屏蔽掉，抓宠功能已经取消 Qiao 2017.2.4
            //if (GetPetModule.GetPetState())
            //{
            //    return false;
            //}
            if (Camera.main == null)
                return false;

            ray = Camera.main.ScreenPointToRay(screen);

            // 如果是战斗时，只通过一个射线拾取
            //if(BattleControl.GetSingle().itfBattleType)
            //{
            //    if (Physics.Raycast(ray, out rayhit, 200, LayerMask.GetMask(LayerMask.LayerToName((int)LusuoLayer.eEL_Dynamic))))
            //    {
            //        // 如果是动态对象
            //        CCreature selectCreature = GetCreature(rayhit.collider.gameObject);
            //        return OnClickCreature(selectCreature);
            //    }
            //}

            // 场景中拾取
            int hitNum = Physics.RaycastNonAlloc(ray, m_npcHitList,  200, LayerMask.GetMask(LayerMask.LayerToName((int)LusuoLayer.eEL_Dynamic)));
            if(hitNum == 0)
                return false;
            List<CCreature> npcList = new List<CCreature>();
            CCreature firstNpc = null;
            for (int i = 0; i < hitNum; i++)
            {
                CCreature hitCreature = GetCreature(m_npcHitList[i].collider.gameObject);
                if (hitCreature != null)
                {
                    if(i == 0)
                        firstNpc = hitCreature;
                    if (hitCreature.IsNpc() || hitCreature.IsMonster() || hitCreature.IsPet() || hitCreature.IsWorldBoss())
                    {
                        if (firstNpc != null && hitCreature != null)
                        {
                            if (Vector3.Distance(firstNpc.GetPos(), hitCreature.GetPos()) < 2)
                            {
                                if(!npcList.Contains(hitCreature))
                                    npcList.Add(hitCreature);
                            }
                        }
                    }
                }
            }
            if(npcList.Count == 0)      // 没有NPC就点到玩家
            {
                CCreature hitCreature = GetCreature(m_npcHitList[0].collider.gameObject);
                OnClickCreature(hitCreature);
                return true;
            }
            else if(npcList.Count == 1) // 有一个NPC的时候
            {
                OnClickCreature(npcList[0]);
                return true;
            }
            else if(npcList.Count > 1)
            {
                // 打开NPC列表
                Debug.Log("打开NPC列表");
                //NpcOverlapModule npcMoudle = LayoutMgr.Inst.GetLogicModule<NpcOverlapModule>(LayoutName.S_NpcOverlap);
                //npcMoudle.OpenEnd = () => {
                //    npcMoudle.UpdateList(npcList);
                //};
                //npcMoudle.SetVisible(true);
                return true;
            }
           
            return false;
        }

        /// <summary>
        /// 可以给外部进行模拟点击角色
        /// </summary>
        /// <param name="selectCreature"></param>
        /// <returns></returns>
        public static bool OnClickCreature(CCreature selectCreature)
        {
            if (selectCreature != null)
            {

                CPlayerMgr.GetMaster().m_targetCreature = selectCreature;
                if (selectCreature.IsNpc())
                {
                    CPlayerMgr.GetMaster().PushCommand(StateID.NearState);
                }
                else if (selectCreature.IsChunnel())   // 传送门
                {
                    CPlayerMgr.GetMaster().PushCommand(StateID.NearState);
                }
                else if (selectCreature.IsMonster() || selectCreature.IsPet())
                {
                    CPlayerMgr.GetMaster().PushCommand(StateID.NearState);
                }
                else if (selectCreature.IsPlant())
                {
  
                }
                else if (selectCreature.IsSoil())
                {
              
                }
                else if (selectCreature.IsPlayer())
                {
                    CPlayer player = (CPlayer)selectCreature;
                }
                else if (selectCreature.IsWorldBoss())
                {
                    CPlayerMgr.GetMaster().PushCommand(StateID.NearState);
                }
                
                selectCreature.SetShow(true);

                // 设置选中框框
                CEffectMgr.Destroy(m_selectEffectHandlID);
                Entity ent = selectCreature.GetEntity();
                if (ent != null || ent.GetObject() != null)
                {
                    m_selectEffectHandlID = CEffectMgr.Create(36, ent.GetObject().transform, (entity, userObj) => {
                        // 开始移动
                        entity.SetScale(selectCreature.GetScale().x, true);
                        entity.SetPos(new Vector3(0, 0.06f, 0));
                        // ((DynamicEntity)entity).SetLineMove(new Vector3(0, selectCreature.m_headHeight - 0.2f, 0), new Vector3(0, 0.1f, 0), 0.14f, UITweener.Method.EaseIn, null);
                    });
                }

                return true;
            }
            return false;
        }

        public static void ClearSelectPlayer()
        {

            CEffectMgr.Destroy(m_selectEffectHandlID);
        }

        /// <summary>
        /// 长按事件，战斗中查看属性使用
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        private static bool OnLongClickCreature(Vector3 screen)
        {
            if (Camera.main == null)
                return false;


            ray = Camera.main.ScreenPointToRay(screen);
            if (Physics.Raycast(ray, out rayhit, 200, LayerMask.GetMask(LayerMask.LayerToName((int)LusuoLayer.eEL_Dynamic))))
            {
                CCreature selectCreature = GetCreature(rayhit.collider.gameObject);
                if (selectCreature != null)
                {
                    //BattleControl.GetSingle().OnLongClickCreature(selectCreature.GetUid());
                    return true;
                }
                else
                {
                    long uid = GetUid(rayhit.collider.gameObject);
                    //BattleControl.GetSingle().OnLongClickCreature(uid);
                }
            }
            return false;
        }


        public static CCreature GetCreature(GameObject obj)
        {
            GameObjectHelper help = obj.GetComponent<GameObjectHelper>();
            if (help == null || help.GetHelpObject() == null)
            {
                return null;
            }
            Entity entity = help.GetHelpObject();
            object uidObj = entity.GetUserString(eUserData.Uid);   
            if (uidObj == null)
            {
                Debug.Log("游戏对象没有存uid：" + obj.name);
                return null;
            }
           // CCreature cc = LogicSystem.Inst.GetCreatrue((long)uidObj);
            //if (cc == null)
            //    return null;
            return null;
        }

        public static long GetUid(GameObject obj)
        {
            GameObjectHelper help = obj.GetComponent<GameObjectHelper>();
            if (help == null || help.GetHelpObject() == null)
            {
                return 0;
            }
            Entity entity = help.GetHelpObject();
            object uidObj = entity.GetUserString(eUserData.Uid);
            if (uidObj == null)
            {
                Debug.Log("游戏对象没有存uid：" + obj.name);
                return 0;
            }
            return (long)uidObj;
        }

        //函数返回真为放大，返回假为缩小
        static bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
        {
	        //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
            var leng1 =Mathf.Sqrt((oP1.x-oP2.x)*(oP1.x-oP2.x)+(oP1.y-oP2.y)*(oP1.y-oP2.y));
            var leng2 =Mathf.Sqrt((nP1.x-nP2.x)*(nP1.x-nP2.x)+(nP1.y-nP2.y)*(nP1.y-nP2.y));
            if(leng1<leng2)
            {
    	         //放大手势
                 return true;
            }
            else
            {
    	        //缩小手势
                return false;
            }
        }

        /// <summary>
        /// 通过设置获取的是否低能耗
        /// </summary>
        public static bool m_bLowLight = false;

        private static bool m_bLowLightSwtich = true;
        private static float m_lowLightTime = 120;
        private static float m_curLowLightTime = 0;

        private static void _UpdateLowLight()
        {
            if(m_bLowLight && m_bLowLightSwtich)
            {
                m_curLowLightTime += Time.deltaTime;
                if (m_curLowLightTime > m_lowLightTime)
                {
                    //DeviceInfoMgr.SetBrightness(true);
                    m_bLowLightSwtich = false;
                }
            }
        }

        /// <summary>
        /// 这个接口是接受玩家的操作入口
        /// </summary>
        public static bool IsClickUI()
        {
            if (Application.platform == RuntimePlatform.Android || 
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if(Input.touchCount > 0)
                {
                    // 手机上才需要判断
                    m_curLowLightTime = 0;
                    if (!m_bLowLightSwtich)   // 防止重复调用
                    {
                        m_bLowLightSwtich = true;
                        //DeviceInfoMgr.SetBrightness(false);
                    }

                    if (EventSystem.current.IsPointerOverGameObject() ||
                        EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
