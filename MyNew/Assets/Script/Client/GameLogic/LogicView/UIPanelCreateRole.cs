using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelCreateRole : UIBase
    {
        public InputField m_name; 

        public GameObject m_btnOcc1;
        public GameObject m_btnOcc2;
        public GameObject m_btnOk;

        public override void Init()
        {
            base.Init();
            m_name = m_root.FindChild("panel/dynamic/name").GetComponent<InputField>();
            m_btnOcc1 = m_root.FindChild("panel/dynamic/btn_occ_1").gameObject;
            m_btnOcc2 = m_root.FindChild("panel/dynamic/btn_occ_2").gameObject;
            m_btnOk = m_root.FindChild("panel/dynamic/btn_ok").gameObject;
        }
       

    }
}

