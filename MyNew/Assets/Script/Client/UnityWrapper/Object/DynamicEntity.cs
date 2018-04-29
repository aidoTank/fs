
using System.Collections.Generic;

using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 动态实体类，播动画等等
    /// </summary>
    public class DynamicEntity : Entity
    {
        private Transform m_parentTransfom;

        public DynamicEntity(uint handle, EntityInitNotify notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {

        }

        public override void UpdateBaseInfo(Resource res)
        {
            base.UpdateBaseInfo(res);
            SetLayer(LusuoLayer.eEL_Dynamic);
            Bind(m_parentTransfom);

            for (int i = 0; i < m_listRenderer.Count; i++)
            {
                Renderer re = m_listRenderer[i];
                if (re == null)
                {
                    continue;
                }
                for (int j = 0; j < re.materials.Length; j++)
                {
                    // 动态对象产生阴影
                    re.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    // 不接受阴影
                    re.receiveShadows = false;
                }
                // tips:解决在编辑器下，shader无法正常显示的问题
                if (Application.isEditor)
                {
                    for (int j = 0; j < re.materials.Length; j++)
                    {
                        re.materials[j].shader = Shader.Find(re.materials[j].shader.name);
                    }
                }
            }

            for (int i = 0; i < m_listRendererEffect.Count; i++)
            {
                Renderer re = m_listRendererEffect[i];
                if (re == null)
                {
                    continue;
                }
                for (int j = 0; j < re.materials.Length; j++)
                {
                    // 动态对象产生阴影
                    re.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    // 不接受阴影
                    re.receiveShadows = false;
                }
                // tips:解决在编辑器下，shader无法正常显示的问题
                if (Application.isEditor)
                {
                    for (int j = 0; j < re.materials.Length; j++)
                    {
                        re.materials[j].shader = Shader.Find(re.materials[j].shader.name);
                    }
                }
            }
        }

        /// <summary>
        /// 将这个对象绑定到parent上
        /// </summary>
        public override void Bind(Transform parent)
        {
            m_parentTransfom = parent;
            if (!IsInited())
            {
                return;
            }
            if (parent != null)
            {
                //Debug.Log(m_object + "绑定到：" + parent.name);
                m_transform.SetParent(parent);
                m_transform.localPosition = Vector3.zero;
                m_transform.localScale = Vector3.one;
                m_transform.localRotation = Quaternion.identity;
            }
            else
            {
                if(m_transform != null)
                    m_transform.SetParent(parent);
            }
        }

        public override void SetLayer(LusuoLayer layer)
        {
            base.SetLayer(layer);
        }

        public virtual void SetCollider(bool bActive)
	    {
            if (null != m_object)
            {
                BoxCollider bc = m_object.GetComponent<BoxCollider>();
                if(null != bc)
                {
                    bc.enabled = bActive;
                }   
            }
	    }

        //public void RemoveCollider()
        //{
        //    if (null != m_object)
        //    {
        //        BoxCollider bc = m_object.GetComponent<BoxCollider>();
        //        if (null != bc)
        //        {
        //            bc.enabled = bActive;
        //        }
        //    }
        //}

        public override void SetOcc(bool bActive)
        {
            if (null != m_object)
            {
                if(bActive)
                {
                    // 增加OCC
                    OccEntiy occ = m_object.AddComponent<OccEntiy>();
                    occ.Init();
                }
                else
                {
                    OccEntiy occ = m_object.GetComponent<OccEntiy>();
                    if (null != occ)
                    {
                        GameObject.Destroy(occ);
                        occ = null;
                    }
                }
            }
        }


        public void SetLineMove(Vector3 startPos, Vector3 endPos,
            float time, UITweener.Method style, UITweener.OnFinished moveEnd)
        {
            TweenPosition curve = TweenPosition.Get(GetObject());
            curve.duration = time;
            curve.method = style;
            curve.from = startPos;
            curve.to = endPos;
            curve.onFinished = moveEnd;
            curve.Reset();
            curve.Play(true);
        }

        /// <summary>
        /// 曲线接口
        /// </summary>
        /// <param name="startPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <param name="frontVal">前段Y偏移</param>
        /// <param name="backVal">后段Y偏移</param>
        /// <param name="time">持续事件</param>
        /// <param name="style">运动风格 Ease In 慢到快； Ease Out 快到慢; Ease In Out 中间快</param>
        /// <param name="moveEnd">结束回调</param>
        public void SetCurveMove(Vector3 startPos, Vector3 endPos, float frontVal, float backVal,
            float time, UITweener.Method style, UITweener.OnFinished moveEnd, bool bAutoDir = false)
        {
            GameObject obj = GetObject();
            if (obj == null)
            {
                Debug.Log("========此对象为空？" + startPos);
                return;
            }
            TweenCurve curve = TweenCurve.Get(obj);
            curve.duration = time;
            curve.method = style;
            curve.from = startPos;
            curve.front = new Vector3(0, frontVal, 0);
            curve.back = new Vector3(0, backVal, 0);
            curve.to = endPos;
            curve.m_bAutoDir = bAutoDir;
            curve.onFinished = moveEnd;
            curve.Play(true);
        }
        public void SetCurveMove(Vector3 startPos, Vector3 endPos, float frontVal, float backVal, UITweener.OnFinished moveEnd)
        {
            TweenCurve curve = TweenCurve.Get(GetObject());
            curve.method = UITweener.Method.Linear;
            curve.from = startPos;
            curve.front = new Vector3(0, frontVal, 0);
            curve.back = new Vector3(0, backVal, 0);
            curve.to = endPos;
            curve.onFinished = moveEnd;
            curve.Play(true);
        }


        public void SetCurveMove(Vector3 startPos, Vector3 endPos, Vector3 frontVal, Vector3 backVal, 
            float time, UITweener.Method style, UITweener.OnFinished moveEnd)
        {
            TweenCurve curve = TweenCurve.Get(GetObject());
            curve.duration = time;
            curve.method = style;
            curve.from = startPos;
            curve.front = frontVal;
            curve.back = backVal;
            curve.to = endPos;
            curve.onFinished = moveEnd;
            curve.Play(true);
        }
    }
}