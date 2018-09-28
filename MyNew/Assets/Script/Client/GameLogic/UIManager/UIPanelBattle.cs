using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelBattle : UIBase
    {
        public Image m_icon; 

        public Transform m_propParent;

        public override void Init()
        {
            base.Init();
            
            m_icon = m_root.FindChild("panel/dynamic/left_top/icon").GetComponent<Image>();
            m_propParent = m_root.FindChild("panel/dynamic/left_top/prop");
        }

        public void SetPorp(int index, string text)
        {
            UIItem.SetText(m_propParent, index.ToString(), text);
        }
       
    }
}

