using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelSelectHero : UIBase
    {
        public GameObject m_hero1;
        public GameObject m_hero2;
        public GameObject m_btnReady;

        public override void Init()
        {
            base.Init();
            m_hero1 = m_root.FindChild("panel/dynamic/hero_1").gameObject;
            m_hero2 = m_root.FindChild("panel/dynamic/hero_2").gameObject;
            m_btnReady = m_root.FindChild("panel/dynamic/btn_ready").gameObject;
        }
    }
}

