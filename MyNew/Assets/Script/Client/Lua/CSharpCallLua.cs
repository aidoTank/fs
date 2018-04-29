using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma
{
    /// <summary>
    /// 用于统一管理C#调用lua的相关接口
    /// </summary>
    public class CSharpCallLua
    {
        public static Action<int, int, Roma.LusuoStream> NetManager_OnRecv;
        public static Action<bool> OpenLoading;

        public static void Init()
        {
            XLua.LuaTable luaEnv = LogicSystem.Inst.m_luaScript;
            NetManager_OnRecv = luaEnv.Get<Action<int, int, Roma.LusuoStream>>("NetManager_OnRecv");
            OpenLoading = luaEnv.Get<Action<bool>>("OpenLoading");
        }
    }
}
