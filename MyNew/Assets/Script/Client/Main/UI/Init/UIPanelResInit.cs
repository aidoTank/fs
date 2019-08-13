using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelResInit : UIBase
    {
        public Text m_text;
        public Slider m_slider;
        public Text m_version;
        public override void Init()
        {
            base.Init();
            m_text = m_root.FindChild("panel/dynamic/bottom/text_pct").GetComponent<Text>();
            m_slider = m_root.FindChild("panel/dynamic/bottom/slider").GetComponent<Slider>();
            m_version = m_root.FindChild("panel/dynamic/ver").GetComponent<Text>();
            SetOrder(100);
        }

        public void SetText(string value)
        {
            m_text.text = value;
        }



        public void SetProgress(float value)
        {
            m_slider.gameObject.SetActiveNew(true);
            m_slider.value = value;
        }

        public void SetVersion(string text)
        {
            m_version.text = text;
        }
    }
}


