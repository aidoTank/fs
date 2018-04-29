using System.Collections.Generic;

namespace Roma
{
    public enum ResType
    {
        None = -1,
        ResInfosResource = 0,
        ManifestResource = 1,
        CsvListResource = 2,
        AllLuaResource,

        EffectResource,
        PanelResource,
        IconResource,      // 界面动态下载大小图统称为icon

        ModelResource,
        SceneCfgResource,
        LightMapResource,

        BoneResource,   // 蒙皮骨骼
    }


    public class ResInfo
    {
        public void Save(ref LusuoStream f)
        {
            string strRef;
            strRef = Type2String(iType);
            f.WriteString(strRef);

            f.WriteString(strUrl);
            f.WriteString(strName);
 
            f.WriteInt(nResID);
        }

        public void Load(ref LusuoStream f)
        {
            string strOut;
            f.ReadString(out strOut);
            iType = (ResType)String2Type(strOut);

            f.ReadString(out strUrl);
            f.ReadString(out strName);
            f.ReadInt(ref nResID);
        }

        static string GetPrefixString(string str)
        {
            int iPos = str.IndexOf("_", 0) + 1;
            return str.Substring(0, iPos);
        }

        public static ResType PrefixString2Type(string strName)
        {
            if(strName == "allresinfo")
            {
                return ResType.ResInfosResource;
            }
            else if (strName == "allcsvinfo")
            {
                return ResType.CsvListResource;
            }
            else if (strName == "alllua")
            {
                return ResType.AllLuaResource;
            }

            strName = GetPrefixString(strName);
            if (strName == "fx_")
            {
                return ResType.EffectResource;
            }
            else if (strName == "panel_")
            {
                return ResType.PanelResource;
            }
            else if (strName == "icon_")
            {
                return ResType.IconResource;
            }

            else if (strName == "mo_")
            {
                return ResType.ModelResource;
            }
            else if (strName == "cfg_")
            {
                return ResType.SceneCfgResource;
            }
            else if (strName == "lm_")
            {
                return ResType.LightMapResource;
            }
            else if (strName == "bo_")
            {
                return ResType.BoneResource;
            }
            return ResType.None;
        }

        public static ResType String2Type(string strName)
        {
            if (strName == "resinfo")
            {
                return ResType.ResInfosResource;
            }
            else if (strName == "csv")
            {
                return ResType.CsvListResource;
            }
            else if (strName == "lua")
            {
                return ResType.AllLuaResource;
            }

            else if (strName == "特效")
            {
                return ResType.EffectResource;
            }
            else if (strName == "面板")
            {
                return ResType.PanelResource;
            }
            else if (strName == "图标")
            {
                return ResType.IconResource;
            }

            else if (strName == "模型")
            {
                return ResType.ModelResource;
            }
            else if (strName == "地图配置")
            {
                return ResType.SceneCfgResource;
            }
            else if (strName == "光照图")
            {
                return ResType.LightMapResource;
            }
            else if (strName == "骨骼蒙皮")
            {
                return ResType.BoneResource;
            }
            return ResType.None;
        }

        public static string Type2String(ResType type)
        {
            switch (type)
            {
                case ResType.ResInfosResource: return "resinfo";
                case ResType.AllLuaResource: return "lua";
                case ResType.CsvListResource: return "csv";
                case ResType.EffectResource: return "特效";
                case ResType.PanelResource: return "面板";
                case ResType.IconResource: return "图标";

                case ResType.ModelResource: return "模型";
                case ResType.SceneCfgResource: return "地图配置";
                case ResType.LightMapResource: return "光照图";

                case ResType.BoneResource: return "骨骼蒙皮";
                default: { return ""; }
            }
        }

        public int nResID;
        public string strUrl;
        public string strName;
        public ResType iType;
        public bool m_bDepend = true;  // 是否依赖其他资源
        public int m_size;

        private static List<string> MainResPrefix = new List<string> {
            "allresinfo",
            "allcsvinfo",
            "alllua",
            "panel_",
            "icon_",

            "fx_",
            "mo_",
            "cfg_",
            "lm_",

            "bo_",
        };

        public static bool IsMainResFile(string name)
        {
            if (name.Contains(".meta"))
                return false;
            if (name.Contains(".manifest"))
                return false;
            for(int i = 0; i < MainResPrefix.Count; i ++)
            {
                if (name.Contains(MainResPrefix[i]))
                    return true;
            }
            return false;
        }
    }
}