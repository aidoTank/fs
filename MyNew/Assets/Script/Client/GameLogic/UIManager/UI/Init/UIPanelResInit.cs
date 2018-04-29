using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelResInit : UI
    {
        public Text m_text;
        public Slider m_slider;

        public override void Init()
        {
            base.Init();
            m_text = m_root.FindChild("panel/bottom/text_pct").GetComponent<Text>();
            m_slider = m_root.FindChild("panel/bottom/slider").GetComponent<Slider>();

            //SetOrder(1000);
        }

        public void SetText(string value)
        {
            m_text.text = value;
        }

        public void SetProgress(float value)
        {
            m_slider.value = (int)(value * 100);
        }
    }
}


