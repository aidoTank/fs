using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelSelectHero : UIBase
    {
        public Text m_textRoleJoin;
        public UISuperList m_list;
        public GameObject m_btnReady;
        public RawImage m_photo;

        public override void Init()
        {
            base.Init();
            m_textRoleJoin = m_root.FindChild("panel/dynamic/role_id").GetComponent<Text>(); ;
            m_list = m_root.FindChild("panel/dynamic/list").gameObject.AddComponent<UISuperList>();
            m_list.Init(180, 190, 2, 3);
            m_btnReady = m_root.FindChild("panel/dynamic/btn_ready").gameObject;
            m_photo = m_root.FindChild("panel/dynamic/photo").GetComponent<RawImage>();
            m_photo.enabled = false;
        }


        HeroPhoto photo;
        public void SetModel(int modelId)
        {
            m_photo.enabled = true;
            DestoryModel();
            photo = new HeroPhoto(ref m_photo, modelId, PhotoType.hero, 0, 0, true);
        }

        public void DestoryModel()
        {
            if (photo != null)
                photo.DestroyPhoto();
        }
    }
}

