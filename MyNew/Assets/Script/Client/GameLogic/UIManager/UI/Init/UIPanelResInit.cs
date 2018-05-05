using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelResInit : UIBase
    {
        public Text m_text;
        public Image m_slider;
        public Text m_version;

        public override void Init()
        {
            base.Init();
            m_text = m_root.FindChild("panel/dynamic/bottom/text_pct").GetComponent<Text>();
            m_slider = m_root.FindChild("panel/dynamic/bottom/fill").GetComponent<Image>();

            m_version = m_root.FindChild("panel/dynamic/ver").GetComponent<Text>();
            //SetOrder(1000);
        }

        public void SetText(string value)
        {
            m_text.text = value;
        }

        public void SetProgress(float value)
        {
            m_slider.fillAmount = value;
        }

        public void SetVersion(string text)
        {
            m_version.text = text;
        }
    }
}


