//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace Roma
//{
//    public class UIComHead : UICom<object>
//    {
//        public UIPanelHead m_ui;

//        public UIComHead()
//        {
//            m_ui = GetCom<UIPanelHead>(UIComName.s_head);
//            SetVisible(true);
//        }

//        public override void SetVisible(bool bShow)
//        {
//            m_ui.OpenPanel(bShow);
//        }

//        public Transform Create()
//        {
//            return m_ui.Create();
//        }
//        public void UpdatePos(Transform head, Vector3 pos)
//        {
//            m_ui.UpdatePos(head, pos);
//        }
//        public void Remove(Transform head)
//        {
//            m_ui.Remove(head);
//        }
//        // 名称
//        public void CreateName(Transform headTrans, string name, int vip, Vector3 offsetPos)
//        {
//            m_ui.CreateName(headTrans, name, vip, offsetPos);
//        }
//        public void UpdateName(Transform trans, string name)
//        {
//            m_ui.UpdateName(trans, name);
//        }

//        public void CreateHp(Transform head, float cur, float max, Vector3 offsetPos)
//        {
//            m_ui.CreateHp(head, cur, max, offsetPos);
//        }
//        public void UpdateHp(Transform head, float cur, float max)
//        {
//            m_ui.UpdateHp(head, cur, max);
//        }

//        public void ShowHp(Transform head, bool bShow)
//        {
//            m_ui.ShowHp(head, bShow);
//        }

//        public void ShowChatHp(Transform head, string txt, Vector2 offset, int type, float time)
//        {
//            m_ui.SetChat(head, txt, offset,type, time);
//        }

//        /// <summary>
//        /// 头顶符号(如任务提示？！)
//        /// <para>type: 0问号 1叹号</para>
//        /// </summary>
//        public void CreateHeadSymbol(Transform head, Vector3 offsetPos, int type)
//        {
//            m_ui.CreateHeadSymbol(head, offsetPos, type);
//        }
//        public void SetHeadSymbolType(Transform head, int type)
//        {
//            m_ui.SetHeadSymbolType(head, type);
//        }
//        public void RemoveHeadSymbol(Transform head)
//        {
//            m_ui.RemoveHeadSymbol(head);
//        }
//    }
//}
