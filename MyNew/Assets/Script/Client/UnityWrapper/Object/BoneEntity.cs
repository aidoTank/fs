using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Roma
{
    public delegate void AnimationEnd(AnimationAction animation);

    public struct AnimationInfo
    {
        // 最新的
        public const string m_animaStand = "stand";
        public const string m_animaStand1 = "stand_1";
        public const string m_animaStand2 = "stand02";
        public const string m_animaWalk = "walk";
        public const string m_animaRun = "run";
        public const string m_animaJump = "jump";
        public const string m_animaDodge = "dodge"; // 闪避
        public const string m_animaSpell_case = "spell_cast";// 施法
        public const string m_animaDefense = "defense"; // 防御
        public const string m_animaAttack_unarm = "attack_unarm";// 徒手攻击
        public const string m_animaAttack_1h_weapon = "attack_1h_weapon"; // 单手武器攻击
        public const string m_animaAttack_2h_weapon = "attack_2h_weapon"; // 双手武器攻击
        public const string m_animaAttack_2h_bow = "attack_2h_bow"; // 弓攻击
        public const string m_animaDefense_2h_thrown = "defense_2h_thrown"; // 投掷
        public const string m_animaHit = "hit"; // 收击
        public const string m_animaMountpet = "mountpet"; // 坐骑坐姿
        public const string m_animaDie = "die"; // 受击死亡
        public const string m_animaDie_1 = "die_1"; // 普通死亡
        public const string m_animaDeath = "death"; // 死亡最后一帧
        public const string m_animaRecovery = "recovery"; // 复活
        public const string m_animaFly = "fly"; // 打飞
        public const string m_animaFlying = "flying"; // 打飞在空中
        public const string m_animaPose = "pose"; // AVATAR很酷的造型

        public const string m_animaRideStand = "zuoqi_stand"; // 坐骑上待机
        public const string m_animaRideDefense_2h_thrown = "zuoqi_defense_2h_thrown"; // 坐骑上投掷
        public const string m_animaRideAttack_2h_bow = "zuoqi_attack_2h_bow"; // 坐骑上弓攻击

        public const string m_animaDuck = "duck";       // 蹲下
        public const string m_animaYun = "yun";       // 宠物蛋的晕眩

        public const string m_animaXuli = "xuli";       // 蓄力



        public const string m_animRun = "run";
        //public const string m_animIdle = "idle";
        public const string m_animAttack = "attack";
        public const string m_animSkill = "skill";
        public const string m_animStrike = "hit";
        public const string m_animDie = "die";

        public const string m_animVictory = "win";

        public const string m_wupin = "wupin";
        public const float m_crossTime = 0.2f;
    }

    public class AnimationAction
    {
        public object parameter = null; // 附带参数
        public WrapMode eMode = WrapMode.Loop;
        public AnimationEnd endEvent = null;
        public float timePct = 1.0f;    // 用于扩展，只播放到百分之。。。
        public string strFull = null;
        public float crossTime = 0.0f;  // 融合时间
        public float playSpeed = 1.0f;  // 美术做的动画时间 / 配置所填的动画持续时间=播放速度

        public float endTime = 0.0f;    // 配置所填的动画持续时间，如果这里是0，那么这里就为【美术做的动画时间】
        public float curTime = 0.0f;    // 当前的播放时间
    }
    /// <summary>
    /// 装备绑定点的名称与特效绑定点名称，属于底层的类型，装备,与服务器类型一致
    /// </summary>
    public enum eEquipType
    {
        EET_Head = 0,  // 头部      (已用于装备)
        EET_Over_Head = 1,  // 头顶
        EET_Body = 2,   // 身体     (已用于装备)
        EET_Chest = 3,  // 身体前胸
        EET_Back = 4,  // 背部
        EET_Hand_Left = 5,  // 左手   
        EET_Hand_Right = 6, // 右手   (已用于装备)
        EET_Mount = 7,  // 坐骑
        EET_Front = 8,  // 正前方
        EET_Origin = 9, // 脚下，原点
        EET_Orn = 10, // 首饰         (已用于装备)
        EET_Shoes=11, // 鞋子         (已用于装备)
        EET_MAX,
    }

    

    /// <summary>
    /// 骨骼实体类，播动画等等
    /// </summary>
    public partial class BoneEntity : DynamicEntity
    {
        // 默认要加载动作
        private bool m_bDefaultAnimaLoaded = false;
        private List<string> m_defaultAnima = new List<string>();
        private int m_curLoadAnimNum;
        
        // 动画组件
        private Animation m_animaton;

        // 装备绑定点的名称,特效绑定点名称
        protected Dictionary<int, string> m_mapEquip2Name = new Dictionary<int, string>() 
        { 
            {(int)eEquipType.EET_Head, "Head"},
            {(int)eEquipType.EET_Over_Head, "over_head"},
            {(int)eEquipType.EET_Body, "body"},
            {(int)eEquipType.EET_Chest, "chest"},
            {(int)eEquipType.EET_Back, "back"},
            {(int)eEquipType.EET_Hand_Left, "l_hand"},
            {(int)eEquipType.EET_Hand_Right, "r_hand"},
            {(int)eEquipType.EET_Mount, "mount"},
            {(int)eEquipType.EET_Front, "front"},
            {(int)eEquipType.EET_Origin, "origin"},
        };
       


        private bool m_curAnimaStart = false;
        private AnimationAction m_curAnimaAction;

        private Transform m_shadowObject;
        private bool m_bShowShadow = true;  // 默认开启影子

        public BoneEntity(uint handle, EntityInitNotify notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {

        }

        public override bool IsInited()
        {
            return m_object != null && m_bDefaultAnimaLoaded;
        }

        public override void OnInited()
        {
            AddBoxCollider();

            if (m_defaultAnima.Count == 0)
            {
                m_bDefaultAnimaLoaded = true;
                base.OnInited();
            }
            else
            {
                m_animaton = m_object.GetComponent<Animation>();
            }

            // 加载阴影，是gameobject,需要实例化
            //EntityBaseInfo info = new EntityBaseInfo();
            //info.m_resID = 5;
            //EntityManager.Inst.CreateEntity(eEntityType.eDynamicEntity, OnShadowLoaded, info);
        }

        /// <summary>
        /// 休闲和跑步动作(战斗时才有，为了节约资源)是默认动作，可以在下载骨骼时一起下载。
        /// </summary>
        public void OnLoadAnimaEnd(Resource res)
        {
            if (res == null || m_animaton == null)
                return;

            AnimationClip[] anima = res.m_assertBundle.LoadAllAssets<AnimationClip>();
            if (anima.Length != 0 && anima[0] is AnimationClip)
            {
                // 组装动作
                m_animaton.AddClip(anima[0], anima[0].name);
            }

            m_curLoadAnimNum++;
            if (m_curLoadAnimNum == m_defaultAnima.Count)
            {
                m_bDefaultAnimaLoaded = true;
                base.OnInited();
            }
        }

        private void OnShadowLoaded(Entity ent, object go)
        {
            ((DynamicEntity)ent).SetCollider(false);
            m_shadowObject = ent.GetObject().transform;
            if(GetObject() != null)
            {
                m_shadowObject.parent = GetObject().transform;
                m_shadowObject.localPosition = Vector3.up * 0.02f;
                m_shadowObject.localEulerAngles = Vector3.zero;
                m_shadowObject.localScale = Vector3.one;
            }


            SetShadowActive(m_bShowShadow);
        }



        public void SetShadowActive(bool bShow)
        {
            m_bShowShadow = bShow;
            if (m_shadowObject != null)
            {
                m_shadowObject.gameObject.SetActive(bShow);
            }
        }

        // 给游戏对象增加组件
        public T AddComponent<T>() where T : Component
        {
            if (!IsInited())
            {
                return null;
            }
            Object obj = null;
            T t = m_object.GetComponent<T>();
            if (null == t)
            {
                obj = m_object.AddComponent<T>();
            }
            return (T)obj;
        }

        public override bool Update(float fTime, float fDTime)
        {
            if(!base.Update(fTime, fDTime))
            {
                return false;
            }


            if (m_curAnimaStart)   // 用于动作暂停
            {
                // 播放完成触发
                if (m_curAnimaAction == null)
                {
                    return true;
                }
                m_curAnimaAction.curTime += fDTime;
                if (m_curAnimaAction.curTime >= m_curAnimaAction.endTime)
                {
                    if (null != m_curAnimaAction && null != m_curAnimaAction.endEvent)
                    {
                        m_curAnimaAction.endEvent(m_curAnimaAction);
                        m_curAnimaAction.endEvent = null;
                        m_curAnimaAction = null;
                        m_curAnimaStart = false;
                    }
                }
            }
            return true;
        }

        public virtual void CancelAnimaAutoPlay()
        {
            if (m_object == null)
            {
                return;
            }
            if (null == m_animaton)
            {
                m_animaton = m_object.GetComponent<Animation>();
                if (m_animaton == null)
                {
                    //Debug.LogError("取消自动播放动作失败" + m_animaton);
                    return;
                }
            }
            m_animaton.playAutomatically = false;  
        }

        public virtual bool Play(AnimationAction action)
        {
            if (action == null || !IsInited())
            {
                return false;
            }

            // 就算没有动画，也应该处理回调事件
            m_curAnimaStart = true;
            m_curAnimaAction = action;

            if (null == m_animaton)
            {
                m_animaton = m_object.GetComponent<Animation>();
                if (m_animaton == null)
                {
                    return false;
                }
            }

            m_animaton.enabled = true;
            if (m_animaton[action.strFull] != null)
            {
                m_animaton[action.strFull].wrapMode = action.eMode;
                m_animaton[action.strFull].speed = action.playSpeed;
                m_animaton.CrossFade(action.strFull, action.crossTime, PlayMode.StopAll);

                if (action.endEvent != null)
                {
                    if (action.endTime == 0.0f)
                    {
                        action.endTime = m_animaton[action.strFull].length * action.timePct - AnimationInfo.m_crossTime;
                    }
                    else
                    {
                        action.endTime *= action.timePct;
                    }
                    action.curTime = 0.0f;
                }
            }
            else
            {
                m_animaton.CrossFade(AnimationInfo.m_animaStand, AnimationInfo.m_crossTime, PlayMode.StopAll);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 动作暂停
        /// </summary>
        public void Play(bool bStart)
        {
            m_curAnimaStart = bStart;
            if (null == m_animaton)
            {
                m_animaton = m_object.GetComponent<Animation>();
            }
            if (m_curAnimaAction == null)
                return;
            m_animaton[m_curAnimaAction.strFull].speed = bStart ? m_curAnimaAction.playSpeed : 0;
        }

        public virtual float GetAnimaClipTime(string name)
        {
            if (!IsInited())
            {
                return 0;
            }
            if (null == m_animaton)
            {
                m_animaton = m_object.GetComponent<Animation>();
            }
            if (null != m_animaton)
            {
                if (m_object.GetComponent<Animation>()[name] != null)
                {
                    return m_object.GetComponent<Animation>()[name].length;
                }
            }
            return 0;
        }

        public void UpdateComponent(eEquipType type, ResInfo res)
        {
            if (!m_mapEquip2Name.ContainsKey((int)type))
            {
                //Debug.Log("该部位暂时没有显示模型的需求:"+type);
                return;
            }

        }

        public void RemoveComponent(eEquipType type)
        {
            if (!m_mapEquip2Name.ContainsKey((int)type))
            {
                //Debug.Log("该部位暂时没有显示模型的需求:" + type);
                return;
            }

        }

        /// <summary>
        /// 设置装备的色相值
        /// </summary>
        public void SetEquipHColor(eEquipType type, float hVal)
        {
            string name;
            if (m_mapEquip2Name.TryGetValue((int)type, out name))
            {
                Transform tans = GetBone(name);
                Renderer[] rens = tans.GetComponentsInChildren<Renderer>();
                for(int i = 0; i < rens.Length; i ++)
                {
                    Material mat = rens[i].material;
                    mat.SetFloat("_HVal", hVal);
                }
            }
        }

        // 装备特效相关
        // 部位v操作id
        private Dictionary<int, uint> m_equipEffect = new Dictionary<int, uint>();

        public void AddEquipEffect(eEquipType type, int effectId)
        {
            if (type == eEquipType.EET_Hand_Right)
            {
                RemoveEquipEffect(type);
                if(effectId != 0)
                {
                    EntityBaseInfo info = new EntityBaseInfo();
                    info.m_resID = effectId;
                    uint effectHandleId = EntityManager.Inst.CreateEntity(eEntityType.eDynamicEntity, OnLoadEquipEffect, info);
                    //Debug.LogError("添加。。。。。。。。。。。。"+ GetLayer() + "  effectHandleId: " + effectHandleId + "===资源id=" + effectId);
                    m_equipEffect.Add((int)type, effectHandleId);
                }
            }
        }

        private void OnLoadEquipEffect(Entity ent, object obj)
        {
            Transform effect = ent.GetObject().transform;
            if (GetObject() != null)
            {
                Transform bindPoint = GetBone("r_hand");
                effect.parent = bindPoint;
                effect.localPosition = Vector3.zero;
                effect.localEulerAngles = Vector3.zero;
                effect.localScale = Vector3.one;
                ent.SetLayer(GetLayer());
                //Debug.LogError("创建武器特效成功。。。。。。。。。。。。" + GetLayer());
            }
            // 异步下载，如果当前实体隐藏，就隐藏装备特效
            if(!m_entityInfo.m_active)
            {
                SetActiveEquipEffect(false);
            }
        }

        /// <summary>
        /// 要先移除特效， 再移除装备，不然找不到特效了
        /// </summary>
        public void RemoveEquipEffect(eEquipType type)
        {
            uint handleId = 0;
            if(m_equipEffect.TryGetValue((int)type, out handleId))
            {
                //Debug.LogError("开始移除特效。。。。。。。。。。。。" + GetLayer() + "  effectHandleId: " + handleId);
                EntityManager.Inst.RemoveEntity(handleId, false);
                m_equipEffect.Remove((int)type);
            }
        }

        /// <summary>
        /// 显影装备特效
        /// </summary>
        /// <param name="bActive"></param>
        public void SetActiveEquipEffect(bool bActive)
        {
            foreach (KeyValuePair<int, uint> item in m_equipEffect)
            {
                uint handleId = 0;
                if (m_equipEffect.TryGetValue(item.Key, out handleId))
                {
                    Entity ent = EntityManager.Inst.GetEnity(handleId, false);
                    ent.SetActive(bActive);
                }
            }
        }


        /// <summary>
        /// 获取绑定点
        /// </summary>
        public Transform GetBone(string name)
        {
            if (m_object == null)
            {
                return null;
            }
            Transform[] bones = m_object.GetComponentsInChildren<Transform>();
            for (int i = bones.Length - 1; i > 0; i--)
            {
                if(name == bones[i].name)
                {
                    return bones[i];
                }
            }
            return null;
        }

        public Transform GetBoneByEquip(eEquipType equip)
        {
            string sBone = "";
            if(m_mapEquip2Name.TryGetValue((int)equip, out sBone))
            {
                return GetBone(sBone);
            }
            return null;
        }

        public override void SetCollider(bool bActive)
	    {
            if (null != m_object)
            {
                m_object.GetComponent<BoxCollider>().enabled = bActive;
            }
	    }

        public override void SetActive(bool bActive)
        {
            base.SetActive(bActive);
            // 显隐装备特效
            SetActiveEquipEffect(bActive);
        }

        // 是否可见
        public bool IsVisible()
        {
            if (null != m_object)
            {
                return m_object.activeSelf;
            }
            return false;
        }

        public override void Destroy()
        {
            base.Destroy();
            ClearBoneBind();
        }

        private void ClearBoneBind()
        {
            // 销毁装备特效
            foreach (KeyValuePair<int, uint> item in m_equipEffect)
            {
                uint handleId = 0;
                if (m_equipEffect.TryGetValue(item.Key, out handleId))
                {
                    EntityManager.Inst.RemoveEntity(handleId, false);
                }
            }
            m_equipEffect.Clear();

        }
    }
}