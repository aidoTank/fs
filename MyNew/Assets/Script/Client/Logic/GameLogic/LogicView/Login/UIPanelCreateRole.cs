using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelCreateRole : UIBase
    {
        public UITab m_tabIcon;
        public Toggle m_gender0;
        public Toggle m_gender1;
        public InputField m_name;
        public GameObject m_btnOk;

        public override void Init()
        {
            base.Init();
            m_name = m_root.FindChild("panel/dynamic/name").GetComponent<InputField>();
            m_gender0 = m_root.FindChild("panel/dynamic/gender/0").GetComponent<Toggle>();
            m_gender1 = m_root.FindChild("panel/dynamic/gender/1").GetComponent<Toggle>();
            m_tabIcon = UITab.Get(m_root.FindChild("panel/dynamic/icon").gameObject);
            m_btnOk = m_root.FindChild("panel/dynamic/btn_ok").gameObject;
        }

        public void SetIocn(int index, int icon)
        {
            UIItem.SetRawImage(m_tabIcon.transform, index.ToString(), icon);
        }
       

    }
}

