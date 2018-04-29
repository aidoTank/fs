//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace Roma
//{
//    /// <summary>
//    /// 需要在进入游戏时，就加载的shader资源
//    /// </summary>
//    public enum eShaderRes      // 对应资源表
//    {
//        eButtonDark = 50,
//        eButtonGray = 51,
//        eButtonLight = 52,
//        eOutLine2 = 53,
//        eMobileBloom = 54,
//        eCCLookupFilter = 55,
//        eAlphaDiffuse = 56,
//        eUIGuideMask = 57,
//        eDissolve = 58,
//        eStone = 59,// 石头shader
//        eMax,
//    }

//    /// <summary>
//    /// Entity使用的动态shader类型
//    /// </summary>
//    public enum eShaderType
//    {
//        eColorToBlack,    // 只是设置颜色
//        eOutLine,         // 高亮描边
//        eDissolve,        // 溶解
//        eAlphaDiffuse,    // 透明
//        eStone,           // 石化
//    }

//    public delegate void OnAllShaderLoaded(int cur, int max);
//    public delegate void ShaderShowEnd();

//    public class ShaderManager : Singleton
//	{
//        public Dictionary<int, Shader> m_dicShaer = new Dictionary<int, Shader>();

//        private OnAllShaderLoaded m_onShaderLoaded;
//        private int m_curNum;
//        private int m_maxNum;

//        public ShaderManager()
//            : base(true)
//        {
//        }

//        public void LoadAllShader(OnAllShaderLoaded loaded)
//        {
//            m_onShaderLoaded = loaded;
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eButtonDark, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eButtonGray, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eButtonLight, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eOutLine2, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eMobileBloom, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eCCLookupFilter, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eAlphaDiffuse, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eUIGuideMask, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eDissolve, OnShaderLoaded, null);
//            ResourceFactory.Inst.LoadResource((uint)eShaderRes.eStone, OnShaderLoaded, null);
//            m_maxNum = (int)eShaderRes.eMax - (int)eShaderRes.eButtonDark;
//        }

//        private void OnShaderLoaded(Resource res)
//        {
//            Shader shader = (Shader)res.m_assertBundle.LoadAsset(res.GetResInfo().strName);
//            if (Application.isEditor)
//            {
//                shader = Shader.Find(shader.name);
//            }
//            m_dicShaer[(int)res.GetResInfo().nResID] = shader;

//            m_curNum++;
//            m_onShaderLoaded(m_curNum, m_maxNum);
//        }

//        public Shader GetShader(eShaderRes res)
//        {
//            if (!m_dicShaer.ContainsKey((int)res))
//                return null;
//            Shader shader = m_dicShaer[(int)res];
//            if (shader == null)
//            {
//                Debug.LogWarning("shader is null:"+ res);
//            }
//            return shader;
//        }

//        public void RemoveShader(EntityShaderInfo info)
//        {
//            if (m_entityMap.ContainsKey(info.entity.m_handleID))
//            {
//                m_entityMap[info.entity.m_handleID] = null;
//                m_entityMap.Remove(info.entity.m_handleID);
//            }
//        }

//        public void AddShader(EntityShaderInfo info)
//        {
//            if (m_entityMap.ContainsKey(info.entity.m_handleID))
//            {
//                info.entity.RestoreShader();
//                info.entity.RestoreColor();
//                m_entityMap[info.entity.m_handleID] = null;
//                m_entityMap.Remove(info.entity.m_handleID);
//            }
//            m_entityMap[info.entity.m_handleID] = info;
//            for (int i = 0; i < info.entity.m_listRenderer.Count; i++)
//            {
//                Renderer render = info.entity.m_listRenderer[i];
//                if (render == null)
//                    continue;
//                if (info.type == eShaderType.eOutLine)
//                {
//                    Material mat = render.material;
//                    mat.shader = GetShader(eShaderRes.eOutLine2);
//                    mat.SetColor("_RimColor", info.color);
//                }
//                else if (info.type == eShaderType.eDissolve)
//                {
//                    Material mat = render.material;
//                    mat.shader = GetShader(eShaderRes.eDissolve);
//                    mat.SetColor("_Color", info.color);
//                }
//                else if (info.type == eShaderType.eAlphaDiffuse)
//                {
//                    Material mat = render.material;
//                    mat.shader = GetShader(eShaderRes.eAlphaDiffuse);
//                    info.color.a = 0.2f;
//                    mat.SetColor("_Color", info.color);
//                }
//                else if (info.type == eShaderType.eStone)
//                {
//                    Material mat = render.material;
//                    mat.shader = GetShader(eShaderRes.eStone);  // 石头shader无需设置颜色
//                    //mat.SetColor("_Color", info.color);
//                }
//            }
//        }

//        private void UpdatePower(EntityShaderInfo info, float val)
//        {
//            for (int i = 0; i < info.entity.m_listRenderer.Count; i++)
//            {
//                Renderer render = info.entity.m_listRenderer[i];
//                if (render == null)
//                    continue;
  
//                if (info.type == eShaderType.eColorToBlack)
//                {
//                    float rgb = ((1 - val) * 0.1f + 0.2f);
//                    render.material.SetColor("_Color",
//                    new Color(
//                        info.color.r * rgb,
//                        info.color.g * rgb,
//                        info.color.b * rgb,
//                        1.0f));
//                }
//                else if (info.type == eShaderType.eOutLine)
//                {
//                    render.material.SetFloat("_RimPower", val);
//                }
//                else if(info.type == eShaderType.eDissolve)
//                {
//                    render.material.SetFloat("_Tile", val);
//                    render.material.SetFloat("_Amount", val);
//                    render.material.SetFloat("_DissSize", val * 0.5f);
//                    render.material.SetTexture("_DissolveText", render.material.GetTexture("_MainTex"));
//                }
//            }
//        }

//        public override void Update(float fTime, float fDTime)
//        {
//            Dictionary<uint, EntityShaderInfo>.Enumerator ms = m_entityMap.GetEnumerator();
//            while (ms.MoveNext())
//            {
//                EntityShaderInfo info = ms.Current.Value;
//                info.curTime += fDTime;
//                if (info.curTime >= info.showTime)
//                {
//                    m_tempDestoryEntityList.Add(info);
//                }
//                else
//                {
//                    UpdatePower(info, info.curTime / info.showTime);
//                }
//            }

//            List<EntityShaderInfo>.Enumerator del = m_tempDestoryEntityList.GetEnumerator();
//            while (del.MoveNext())
//            {
//                EntityShaderInfo info = del.Current;
//                info.entity.RestoreShader();
//                info.entity.RestoreColor();
//                m_entityMap.Remove(info.entity.m_handleID);
//                if (info.showEnd != null)
//                    info.showEnd();
//                info.showEnd = null;
//                info = null;
//            }
//            m_tempDestoryEntityList.Clear();
//        }

//        public class EntityShaderInfo
//        {
//            public Entity entity;
//            public eShaderType type;
//            public Color color;
//            public float showTime;
//            public float curTime = 0.0f;
//            public ShaderShowEnd showEnd;
//        }

//        private Dictionary<uint, EntityShaderInfo> m_entityMap = new Dictionary<uint, EntityShaderInfo>();
//        private List<EntityShaderInfo> m_tempDestoryEntityList = new List<EntityShaderInfo>();

//        public new static ShaderManager Inst;
//	}
//}
