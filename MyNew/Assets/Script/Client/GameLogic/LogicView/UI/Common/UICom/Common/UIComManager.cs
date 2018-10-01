//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace Roma
//{
//    public struct UIComName
//    {
//        public const string s_loading = "com_loading";
//        public const string s_panelResLoading = "com_res_loading";
//        public const string s_mianMouse = "com_mouse";
//        public const string s_hud = "com_hud";
//        public const string s_head = "com_head";
//        public const string s_popmenu = "com_popmenu";
//        public const string s_dialog = "com_dialog";
//        public const string s_hoverTips = "com_hover_tips";

//        public const string s_skillOpen = "com_skill_open";
//        public const string s_worldMsg = "com_world_msg";
//    }

//    /// <summary>
//    /// 作为单件使用的UI组件，其实是在用时初始化
//    /// </summary>
//    public class UIComManager
//    {
//        public static UIComManager Inst = new UIComManager();
//        private Dictionary<string, object> m_singletonMap = new Dictionary<string, object>();
//        public UIComManager() { }

//        public void InitGame()
//        {
//            //AddSingleton(UIComName.s_loading, UICom<UIComLoading>.Inst());
//            //AddSingleton(UIComName.s_panelResLoading, UICom<UIComResLoading>.Inst());
//        }

//        public void AddSingleton(string name, object singleton)
//        {
//            if (m_singletonMap.ContainsKey(name))
//            {
//                //Debug.Log("重复添加单例类：" + name);
//                return;
//            }
//            m_singletonMap.Add(name, singleton);
//        }

//        public void RemoveSingleton(string name)
//        {
//            if (!m_singletonMap.ContainsKey(name))
//            {
//                //Debug.Log("卸载失败，不包含单例类：" + name);
//                return;
//            }
//            m_singletonMap.Remove(name);
//        }

//        public object GetSingleton(string name)
//        {
//            if (!m_singletonMap.ContainsKey(name))
//            {
//                //Debug.Log("获取失败，不包含单例类：" + name);
//            }
//            return m_singletonMap[name];
//        }

//        public void Init()
//        {
//            foreach (KeyValuePair<string, object> item in m_singletonMap)
//            {
//                ((UICom<object>)item.Value).Init();
//            }
//        }

//        public void Update()
//        {
//            foreach (KeyValuePair<string, object> item in m_singletonMap)
//            {
//                ((UICom<object>)item.Value).Update();
//            }
//        }
         
//        public void Destroy()
//        {
//            foreach (KeyValuePair<string, object> item in m_singletonMap)
//            {
//                ((UICom<object>)item.Value).Destroy();
//            }
//        }
//    }

//    /// <summary>
//    /// 通用表现层泛型单例基类
//    /// </summary>
//    public class UICom<T> where T : new()
//    {
//        public UICom() { }

//        public static T Inst()
//        {
//            return UIComCreator.inst;
//        }
//        class UIComCreator
//        {
//            static UIComCreator() { }
//            internal static readonly T inst = new T();
//        }
//        public T GetCom<T>(string name)
//        {
//            GameObject gObject = GameObject.Find(name);
//            if (gObject == null)
//            {
//                //Debug.LogError("UI对象加载失败:" + name);
//                return default(T);
//            }
//            System.Object ui = gObject.GetComponent<MonoBehaviour>();
//            if (ui == null)
//            {
//                //Debug.LogError("Com组件加载失败:" + name);
//                return default(T);
//            }
//            return (T)ui;
//        }

//        public virtual void SetVisible(bool bShow) { }
//        public virtual bool IsShow() { return false; }
//        public virtual void Init() { }
//        public virtual void Update() { }
//        public virtual void Destroy() { }
//    }
//}
