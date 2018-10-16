using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Roma
{

    public class UIAnimationHelp : MonoBehaviour
    {
        public delegate void AnimationHelpEnd(object obj);
        private AnimationHelpEnd m_animationHelpEnd;
        public object m_param;
        private Animation anima;

        private CanvasRenderer cr;
        private Graphic m_txt;
        private Outline m_outLine;
        private Shadow m_shadow;
        private Color m_outLineColor;

        private bool m_bPlay;
        private float m_curTime;
        private float m_maxTime;

        public AnimationClip[] m_animaList;
        private AnimationClip m_curClip;

        public void Init()
        {
            if (anima != null)    // 只初始化一次
                return;

            cr = gameObject.GetComponent<CanvasRenderer>();
            m_txt = gameObject.GetComponent<Graphic>();
            if (m_txt == null)
            {
                m_txt = gameObject.GetComponentInChildren<Graphic>();
            }
            m_outLine = gameObject.GetComponent<Outline>();
            m_shadow = gameObject.GetComponent<Shadow>();

            anima = GetComponent<Animation>();
            if(m_animaList != null)
            {
                m_curClip = m_animaList[Random.Range(0, anima.GetClipCount())];
                m_maxTime = m_curClip.length;
            }
            else
            {
                m_curClip = anima.clip;
                m_maxTime = m_curClip.length;
            }
        }

        public void Play(AnimationHelpEnd end)
        {
            m_animationHelpEnd = end;
            m_bPlay = true;
            m_curTime = 0;
            if (anima != null)
                anima.Play(m_curClip.name);
        }


        void Update()
        {
            if (!m_bPlay)
                return;

            //cr.SetAlpha(m_txt.color.a);

            m_outLineColor = new Color(0, 0, 0, m_txt.color.a);
            if (m_outLine != null)
                m_outLine.effectColor = m_outLineColor;
            if (m_shadow != null)
                m_shadow.effectColor = m_outLineColor;

            m_curTime += Time.deltaTime;
            if(m_curTime > m_maxTime)
            {
                if (m_animationHelpEnd != null)
                {
                    m_animationHelpEnd(m_param);
                    m_bPlay = false;
                }
            }
        }

        public static UIAnimationHelp Get(GameObject go)
        {
            UIAnimationHelp load = go.GetComponent<UIAnimationHelp>();
            if (load == null) load = go.AddComponent<UIAnimationHelp>();
            return load;
        }
    }
}