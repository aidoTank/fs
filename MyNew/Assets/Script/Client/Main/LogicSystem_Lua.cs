using UnityEngine;
using System.Collections.Generic;
using XLua;
using System;
using System.Text;

namespace Roma
{
    public partial class LogicSystem
    {
        private Dictionary<string, string> m_luaList = new Dictionary<string, string>();
        public List<byte[]> m_luaPb = new List<byte[]>();        public int GetLuaPbCount()        {            return m_luaPb.Count;        }        public byte[] GetLuaPb(int index)        {            return m_luaPb[index];        }

        public void LuaInit(TextAsset[] luaList)
        {
            InitLuaLib();

            string mainLua = "";
            for (int i = 0; i < luaList.Length; i++)
            {
                TextAsset textAsset = luaList[i];
                if(textAsset.name.Equals("Main.lua"))
                {
                    mainLua = textAsset.text;
                }
                else
                {
                    if (textAsset.name.Contains(".pb"))                    {                        m_luaPb.Add(textAsset.bytes);                    }                    else                    {
                        if(m_luaList.ContainsKey(textAsset.name))
                        {
                            Debug.LogError("重复：" + textAsset.name);
                            continue;
                        }
                        m_luaList.Add(textAsset.name, textAsset.text);                    }
                }
            }

            m_luaScript = m_luaEnv.NewTable();

            LuaTable meta = m_luaEnv.NewTable();
            meta.Set("__index", m_luaEnv.Global);
            m_luaScript.SetMetaTable(meta);
            meta.Dispose();

            m_luaScript.Set("self", this);
            //if (m_injections != null)
            //{
            //    foreach (var injection in m_injections)
            //    {
            //        m_luaScript.Set(injection.name, injection.value);
            //    }
            //}

            // lua代码里头调用require时，参数将会透传给自定义回调函数和原生回调函数
            m_luaEnv.AddLoader((ref string path) =>
            {
                string lastName = path + ".lua";
                if (m_luaList.ContainsKey(lastName))
                {
                    return Encoding.Default.GetBytes(m_luaList[lastName]);
                }
                else
                {
                    Debug.LogError("Lua加载错误，找不到文件:"+ lastName);
                }
                return null;
            });

            m_luaEnv.DoString(mainLua, "LuaManager", m_luaScript);
     
            m_luaInit = m_luaScript.Get<Action>("Init");
            m_luaScript.Get("Update", out m_luaUpdate);
            m_luaScript.Get("LateUpdate", out m_luaLateUpdate);
            m_luaScript.Get("Destroy", out m_luaDestroy);

            Lua_Init();
            CSharpCallLua.Init();
        }

        private void InitLuaLib()
        {
            m_luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
            m_luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
            m_luaEnv.AddBuildin("protobuf.c", XLua.LuaDLL.Lua.LoadProtobufC);
        }

        public void Lua_Init()
        {
            if (m_luaInit != null)
                m_luaInit();
        }

        public void Lua_Update(float fTime, float fDTime)
        {
            if (m_luaUpdate != null)
                m_luaUpdate(fTime, fDTime);
        }

        public void Lua_LateUpdate(float fTime, float fDTime)
        {
            if (m_luaLateUpdate != null)
                m_luaLateUpdate(fTime, fDTime);
        }

        public void Lua_Destroy()
        {
            if (m_luaDestroy != null)
                m_luaDestroy();

            m_luaScript = null;
            m_luaEnv = null;
        }

        public LuaEnv m_luaEnv = new LuaEnv();
        public LuaTable m_luaScript;
        //private Injection[] m_injections;

        private Action m_luaInit;
        private Action<float, float> m_luaUpdate;
        private Action<float, float> m_luaLateUpdate;
        private Action m_luaDestroy;
    }
}

namespace XLua.LuaDLL
{
    using System.Runtime.InteropServices;

    public partial class Lua
    {
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_rapidjson(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaDLL.lua_CSFunction))]
        public static int LoadRapidJson(System.IntPtr L)
        {
            return luaopen_rapidjson(L);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lpeg(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaDLL.lua_CSFunction))]
        public static int LoadLpeg(System.IntPtr L)
        {
            return luaopen_lpeg(L);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_protobuf_c(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaDLL.lua_CSFunction))]
        public static int LoadProtobufC(System.IntPtr L)
        {
            return luaopen_protobuf_c(L);
        }
    }
}