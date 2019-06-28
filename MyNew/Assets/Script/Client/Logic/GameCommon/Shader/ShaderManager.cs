using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 需要在进入游戏时，就加载的shader资源
    /// </summary>
    public enum eShaderRes      // 对应资源表
    {
        eAlphaDiffuse = 21,
        eNihility = 22,     // 虚无
        eRim = 23,


        eAlphaMask = 31,
        eButtonDark = 32,
        eButtonGray = 33,
        eButtonLight = 34,
        eUIMatAnim = 35,

        eMax,
    }

    /// <summary>
    /// Entity使用的动态shader类型
    /// </summary>
    public enum eShaderType
    {
        eAlphaToHalf,     // 透明至半透明
        eAlphaToHide,     // 透明至消失
        eNihility,        // 虚无
        eRim,             // 无敌


        eColorToBlack,    // 只是设置颜色
        eOutLine,         // 高亮描边
        eDissolve,        // 溶解

        eStone,           // 石化
    }

    public delegate void OnAllShaderLoaded(int cur, int max);
    public delegate void ShaderShowEnd();

    public class ShaderManager : Singleton
    {
        public Dictionary<int, Shader> m_dicShaer = new Dictionary<int, Shader>();

        private Action<int, int> m_onShaderLoaded;
        private int m_curNum;
        private int m_maxNum;

        public ShaderManager()
            : base(true)
        {
        }

        public void LoadAllShader(Action<int, int> loaded)
        {
            m_onShaderLoaded = loaded;
            ResourceFactory.Inst.LoadResource((int)eShaderRes.eAlphaDiffuse, OnShaderLoaded);
            ResourceFactory.Inst.LoadResource((int)eShaderRes.eNihility, OnShaderLoaded);
            ResourceFactory.Inst.LoadResource((int)eShaderRes.eRim, OnShaderLoaded);

            ResourceFactory.Inst.LoadResource((int)eShaderRes.eAlphaMask, OnShaderLoaded);
            ResourceFactory.Inst.LoadResource((int)eShaderRes.eButtonDark, OnShaderLoaded);
            ResourceFactory.Inst.LoadResource((int)eShaderRes.eButtonGray, OnShaderLoaded);
            ResourceFactory.Inst.LoadResource((int)eShaderRes.eButtonLight, OnShaderLoaded);
            ResourceFactory.Inst.LoadResource((int)eShaderRes.eUIMatAnim, OnShaderLoaded);

            m_maxNum = 8;
        }

        private void OnShaderLoaded(Resource res)
        {
            Shader shader = (Shader)res.m_assertBundle.LoadAsset(res.GetResInfo().strName);
            if (Application.isEditor)
            {
                shader = Shader.Find(shader.name);
            }
            m_dicShaer[(int)res.GetResInfo().nResID] = shader;

            m_curNum++;
            if (m_onShaderLoaded != null)
                m_onShaderLoaded(m_curNum, m_maxNum);
        }

        public Shader GetShader(eShaderRes res)
        {
            if (!m_dicShaer.ContainsKey((int)res))
                return null;
            Shader shader = m_dicShaer[(int)res];
            if (shader == null)
            {
                Debug.LogWarning("shader is null:" + res);
            }
            return shader;
        }


        public void RemoveShader(int entHid)
        {
            EntityShaderInfo info;
            if (m_entityMap.TryGetValue(entHid, out info))
            {
                //m_entityMap[info.entity.m_hid] = null;
                m_entityMap.Remove(entHid);
            }
        }

        public void AddShader(EntityShaderInfo info)
        {
            RemoveShader(info.entity.m_hid);
            m_entityMap[info.entity.m_hid] = info;
            for (int i = 0; i < info.entity.m_listRenderer.Count; i++)
            {
                Renderer render = info.entity.m_listRenderer[i];
                if (render == null)
                    continue;
                Material mat = render.material;
                switch (info.type)
                {
                    case eShaderType.eAlphaToHalf:
                        mat.shader = GetShader(eShaderRes.eAlphaDiffuse);
                        info.color.a = 1f;
                        mat.SetColor("_TintColor", info.color);
                        break;
                    case eShaderType.eAlphaToHide:
                        mat.shader = GetShader(eShaderRes.eAlphaDiffuse);
                        info.color.a = 1f;
                        mat.SetColor("_TintColor", info.color);
                        break;
                    case eShaderType.eNihility:
                        mat.shader = GetShader(eShaderRes.eNihility);
                        mat.SetColor("_TintColor", info.color);
                        break;
                    case eShaderType.eRim:
                        mat.shader = GetShader(eShaderRes.eRim);
                        mat.SetColor("_RimColor", info.color);
                        break;
                }
            }
        }

        private void UpdatePower(EntityShaderInfo info, float val)
        {
            //if (info == null)
            //    return;
            Entity ent = info.entity;
            if (ent == null)
                return;
            List<Renderer> list = ent.m_listRenderer;
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Renderer render = list[i];
                if (render == null)
                    continue;

                switch (info.type)
                {
                    case eShaderType.eAlphaToHalf:
                        info.color.a = val < 0.5f ? 1 - val : 0.5f;
                        render.material.SetColor("_TintColor", info.color);
                        break;
                    case eShaderType.eAlphaToHide:
                        if (val < 0.5f)
                            val = 0.5f;
                        info.color.a = 1 - val;
                        render.material.SetColor("_TintColor", info.color);
                        break;
                }



                //if (info.type == eShaderType.eColorToBlack)
                //{
                //    float rgb = ((1 - val) * 0.1f + 0.2f);
                //    render.material.SetColor("_Color",
                //    new Color(
                //        info.color.r * rgb,
                //        info.color.g * rgb,
                //        info.color.b * rgb,
                //        1.0f));
                //}
                //else if (info.type == eShaderType.eOutLine)
                //{
                //    render.material.SetFloat("_RimPower", val);
                //}
                //else if (info.type == eShaderType.eDissolve)
                //{
                //    render.material.SetFloat("_Tile", val);
                //    render.material.SetFloat("_Amount", val);
                //    render.material.SetFloat("_DissSize", val * 0.5f);
                //    render.material.SetTexture("_DissolveText", render.material.GetTexture("_MainTex"));
                //}
            }
        }

        public override void Update(float fTime, float fDTime)
        {
            Dictionary<int, EntityShaderInfo>.Enumerator ms = m_entityMap.GetEnumerator();
            while (ms.MoveNext())
            {
                EntityShaderInfo info = ms.Current.Value;
                info.curTime += fDTime;
                if (info.curTime >= info.showTime)
                {
                    if (info.showEnd != null)   // time out,execute callback
                    {
                        info.showEnd();
                        info.showEnd = null;
                    }
                    if (info.bAutoEnd)
                        m_tempDestoryEntityList.Add(info);

                    UpdatePower(info, 1.0f);
                }
                else
                {
                    UpdatePower(info, info.curTime / info.showTime);
                }
            }

            List<EntityShaderInfo>.Enumerator del = m_tempDestoryEntityList.GetEnumerator();
            while (del.MoveNext())
            {
                EntityShaderInfo info = del.Current;
                RemoveShader(info.entity.m_hid);
                //info = null;
            }
            m_tempDestoryEntityList.Clear();
        }

        private Dictionary<int, EntityShaderInfo> m_entityMap = new Dictionary<int, EntityShaderInfo>();
        private List<EntityShaderInfo> m_tempDestoryEntityList = new List<EntityShaderInfo>();

        public static ShaderManager Inst;
    }

    public struct EntityShaderInfo
    {
        public Entity entity;
        public eShaderType type;
        public Color color;
        public bool bAutoEnd; // 是否自动结束，移除
        public float showTime;
        public float curTime;
        public Action showEnd;
    }
}
