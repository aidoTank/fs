using System;
using UnityEngine;
using System.Collections.Generic;


namespace Roma
{
    public enum LogicModuleIndex
    {
        eLM_None = 0,

        eLM_PanelLogin = 1,   // 登录
        eLM_PanelCreate = 2,  //创建角色   之后选新手有个引导场景
        eLM_PanelMain = 3, //主界面

        eLM_PanelLoading = 4,      // 加载
    }

    public class LayoutMgr : Singleton
    {
        public static LayoutMgr Inst;


        public delegate Widget OnCreateMoudleDelegate();

        private Dictionary<int, OnCreateMoudleDelegate> m_dicGameCreateMoudle = new Dictionary<int, OnCreateMoudleDelegate>()
        {
            {(int)LogicModuleIndex.eLM_PanelLogin, LoginModule.CreateLogicModule},
            {(int)LogicModuleIndex.eLM_PanelCreate, CreateRoleModule.CreateLogicModule},
            {(int)LogicModuleIndex.eLM_PanelMain, MainModule.CreateLogicModule},
        };


        public LayoutMgr() : base(true)
        {
            foreach (KeyValuePair<int, OnCreateMoudleDelegate> key in m_dicGameCreateMoudle)
            {
                m_dicGameAllPanel.Add(key.Key, key.Value());
            }
        }

        // 逻辑初始化
        public void InitLayout()
        {
            foreach (KeyValuePair<int, Widget> item in m_dicGameAllPanel)
            {
                ResInfo resInfo = ResInfosResource.GetResInfo(10000 + (int)item.Key);
                if (resInfo != null)
                {
                    m_dicWidgetByName.Add(resInfo.strName, item.Value);
                }
            }
        }

        public override void Update(float fTime, float fDTime)
        {
            var e = m_dicGameAllPanel.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.UpdateUI(fTime, fDTime);
            }
        }

        public Widget GetLogicModule(LogicModuleIndex index)
        {
            Widget ui = null;
            if (m_dicGameAllPanel.TryGetValue((int)index, out ui))
            {
                return ui;
            }
            return null;
        }

        public Widget GetLogicModule(string name)
        {
            Widget ui = null;
            if (m_dicWidgetByName.TryGetValue(name, out ui))
            {
                return ui;
            }
            return null;
        }


        public void OnLoadFristUI(Action<int, int> loaded)
        {
            int m_curItemNum = 0;
            for (int i = 0; i < m_listLoading.Count; i++)
            {
                Widget widget = LayoutMgr.Inst.GetLogicModule((LogicModuleIndex)m_listLoading[i]);
                widget.m_openEnd = () =>
                {
                    widget.SetVisible(false);
                    m_curItemNum++;
                    loaded(m_curItemNum, m_listLoading.Count);
                };
                widget.SetVisible(true);
            }
        }

        /// <summary>
        /// 在第一次loading时，就加载的界面
        /// </summary>
        public List<int> m_listLoading = new List<int>()
        {
            //(int)LogicModuleIndex.eLM_PanelLogin,
            //(int)LogicModuleIndex.eLM_PanelLoadingGameMelee,
            //(int)LogicModuleIndex.eLM_PanelHead,
            //(int)LogicModuleIndex.eLM_PanelJoyStick,
        };
       

        private Dictionary<int, Widget> m_dicGameAllPanel = new Dictionary<int, Widget>();
        private Dictionary<string, Widget> m_dicWidgetByName = new Dictionary<string, Widget>();
        private int m_mainUIBgmHid;
    }
}

