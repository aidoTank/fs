using UnityEngine;
using System;
using System.Collections.Generic;

namespace Roma
{
    public class AnimationInfo
    {
        public const string m_animaStand = "stand";
        public const float m_crossTime = 0.2f;
    }

    //public delegate void AnimationEnd(AnimationAction animation);

    public class AnimationAction
    {
        public WrapMode eMode;
        public Action endEvent;
        public float timePct = 1.0f;    // 用于扩展，只播放到百分之。。。
        public string strFull;
        public float crossTime;  // 融合时间
        public float playSpeed = 1.0f;  // 美术做的动画时间 / 配置所填的动画持续时间=播放速度
        public bool atOnce;

        public float endTime;    // 配置所填的动画持续时间，如果这里是0，那么这里就为【美术做的动画时间】
        public float curTime;    // 当前的播放时间
    }

    /// <summary>
    /// 实体类
    /// </summary>
    public class BoneEntity : Entity
    {
        private bool m_curAnimaStart;
        private AnimationAction m_curAnimaAction;
        public Animation m_animation;
        public int m_animaPriority;

        private Transform[] m_bones; // 节点信息
        private Dictionary<string, Transform> m_dicBone;

        // the entity's binding objects 
        private List<Entity> m_bindObject;

        public BoneEntity(int handle, Action<Entity> notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {

        }

        public override void ChangeResource(Action<Entity> notity, EntityBaseInfo baseInfo)
        {
            m_animation = null;
            m_bones = null;
            if (m_dicBone != null)
            {
                m_dicBone.Clear();
                m_dicBone = null;
            }
            if (m_bindObject != null)
                m_bindObject.Clear();
            base.ChangeResource(notity, baseInfo);
        }

        public override void UpdateBaseInfo(Resource res)
        {
            base.UpdateBaseInfo(res);

            if (null == m_animation)
            {
                m_animation = m_object.GetComponent<Animation>();
                if (m_animation == null)
                {
                    m_animation = m_object.GetComponentInChildren<Animation>();
                }
            }

            if (m_animation == null)
            {
                //Debug.Log("找不到的动作对象:" + res.GetResInfo().nResID);
            }
            else
            {
                m_animation.playAutomatically = false;
            }
        }

        public bool _UpdateAnima(float fTime, float fDTime)
        {
            if (m_curAnimaStart && m_curAnimaAction != null && m_curAnimaAction.eMode == WrapMode.Once)
            {
                //Debug.LogError("播放once: " + m_curAnimaAction.curTime  +"   " + m_curAnimaAction.endTime);
                m_curAnimaAction.curTime += fDTime;
                if (m_curAnimaAction.curTime >= m_curAnimaAction.endTime)
                {
                    if (null != m_curAnimaAction && null != m_curAnimaAction.endEvent)
                    {
                        m_animaPriority = 0;   // 先重置为0
                        m_curAnimaAction.endEvent();
                        m_curAnimaAction.endEvent = null;
                        m_curAnimaAction = null;
                        m_curAnimaStart = false;
                    }
                }
            }
            return true;
        }


        public virtual bool Play(AnimationAction action)
        {
            if (action == null)
            {
                return false;
            }

            if (m_animation == null)
            {
                return false;
            }


            AnimationState anima = m_animation[action.strFull];
            if (anima == null)
            {
                m_animaPriority = 0;   // 先重置为0
                return false;
            }

            anima.wrapMode = action.eMode;
            anima.speed = action.playSpeed;
            if (action.atOnce)
            {
                m_animation.Stop(action.strFull);
                m_animation[action.strFull].time = Mathf.Epsilon;
            }
            m_animation.CrossFade(action.strFull, action.crossTime);
            Debug.LogError(m_animation.gameObject.name + "播放：" + action.strFull + " 速度:" + action.playSpeed + "  时间：" + anima.length);
            if (action.endEvent != null)
            {
                action.endTime = anima.length;
                action.curTime = 0.0f;
            }
            // 如果是播放一次的才需要接受回调
            if (action.eMode == WrapMode.Once)
            {
                m_curAnimaStart = true;
            }
            m_curAnimaAction = action;   // 用于暂停时，无论是一次，还是循环都需要存储当前动作
            return true;
        }

        /// <summary>
        /// 动作暂停
        /// </summary>
        public void Play(bool bStart)
        {
            if (m_curAnimaAction == null)
                return;

            if (m_curAnimaAction.eMode == WrapMode.Once)
            {
                m_curAnimaStart = bStart;
            }

            if (m_animation != null)
                m_animation[m_curAnimaAction.strFull].speed = bStart ? m_curAnimaAction.playSpeed : 0;
        }

        public override void Update(float fTime, float fDTime)
        {
            base.Update(fTime, fDTime);
            _UpdateAnima(fTime, fDTime);
        }


        /// <summary>
        /// 获取绑定点,已优化
        /// </summary>
        public Transform GetBone(string name)
        {
            if (m_object == null)
            {
                return null;
            }
            if (name.Equals("0") || string.IsNullOrEmpty(name))
            {
                return null;
            }

            Transform tran;
            if (m_dicBone != null && m_dicBone.TryGetValue(name, out tran))
            {
                return tran;
            }

            if (m_bones == null)
                m_bones = m_object.GetComponentsInChildren<Transform>();
            for (int i = m_bones.Length - 1; i > 0; i--)
            {
                if (name == m_bones[i].name)
                {
                    if (m_dicBone == null)
                        m_dicBone = new Dictionary<string, Transform>();
                    m_dicBone.Add(name, m_bones[i]);
                    return m_bones[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 新增特效绑定，隐藏接口
        /// </summary>
        public void AddBindObject(Entity ent)
        {
            if (m_bindObject == null)
                m_bindObject = new List<Entity>();
            m_bindObject.Add(ent);
        }

        public void RemoveBindObject(Entity ent)
        {
            if (m_bindObject == null)
                m_bindObject = new List<Entity>();
            m_bindObject.Remove(ent);
        }

        /// <summary>
        /// 隐藏玩家身上的特效时，不停止特效的行为
        /// </summary>
        /// <param name="bActive"></param>
        public override void SetShow(bool bActive, bool bStop = true)
        {
            if (m_bindObject == null)
                m_bindObject = new List<Entity>();

            base.SetShow(bActive);
            for (int i = 0; i < m_bindObject.Count; i++)
            {
                m_bindObject[i].SetShow(bActive, false);
            }
        }
    }
}