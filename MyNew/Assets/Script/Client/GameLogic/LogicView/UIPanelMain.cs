using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelMain : UIBase
    {
        public GameObject m_btnCreateRoom;
        public InputField m_roomId;
        public GameObject m_btnJoinRoom;

        public override void Init()
        {
            base.Init();
            m_btnCreateRoom = m_root.FindChild("panel/dynamic/btn_create").gameObject;
            m_roomId = m_root.FindChild("panel/dynamic/room_id").GetComponent<InputField>();
            m_btnJoinRoom = m_root.FindChild("panel/dynamic/btn_join").gameObject;
        }
    }
}

