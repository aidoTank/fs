using UnityEngine;
using System.Collections;

namespace Roma
{
    public class UIComPetSkillInfo : UI
    {
        public Transform m_tips_parent;
        public GameObject m_btn_forget;
        public GameObject m_btn_locking;

        public override void Init()
        {
            base.Init();
            m_tips_parent = GetChild("panel/tips");
            m_btn_forget = GetChild("panel/tips/btn_forget").gameObject;
            m_btn_locking = GetChild("panel/tips/btn_locking").gameObject;
        }

        public void SetSkillInfo(int iconid, string name, string decribe, Vector3 position)
        {
            UIItem.SetImage(m_tips_parent, UIItemTitle.imgIcon, iconid);
            UIItem.SetText(m_tips_parent, UIItemTitle.txtName, name);
            UIItem.SetText(m_tips_parent, UIItemTitle.txtDescribe, decribe);

            if (position != null)
            {
                m_tips_parent.localPosition = new Vector3(position.x, position.y-150f, position.z);
            }
            else
            {
                m_tips_parent.localPosition = Vector3.zero;
            }
        }
    }
}
