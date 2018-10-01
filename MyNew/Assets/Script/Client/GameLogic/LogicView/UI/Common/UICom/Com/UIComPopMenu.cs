//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//namespace Roma
//{
//    public class UIComPopMenu : UICom<object>
//    {
//        public UIPanelPopMenu m_ui;
//        //private List<PopMenuData> m_listData = new List<PopMenuData>();

//        public UIComPopMenu()
//        {
//            m_ui = GetCom<UIPanelPopMenu>(UIComName.s_popmenu);
//            base.SetVisible(false);
//        }

//        public override void SetVisible(bool bShow)
//        {
//            m_ui.OpenPanel(bShow);
//        }

//        //public void SetPopMenuData(List<PopMenuData> menuList)
//        //{
//        //    m_listData = menuList;
//        //}

//        //public void PopMenu(GameObject go, UIPopMenu.OnClickPopMenu onClickEvent)
//        //{
//        //    PopMenu(go, m_listData, onClickEvent);
//        //}

//        public void PopMenu(GameObject go, List<PopMenuData> menuList, UIPanelPopMenu.OnClickPopMenu onClickEvent)
//        {
//            m_ui.PopMenu(go, menuList, onClickEvent);
//        }


//        public override bool IsShow()
//        {
//            return m_ui.IsShow();
//        }

//        // 此菜单是否被单击过
//        public bool IsClick(GameObject go)
//        {
//            return m_ui.GetItemList().Contains(go);
//        }
//    }
//}
