//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace Roma
//{

//    public class SceneCfgResource : Resource
//    {
//        public SceneCfgResource(ref ResInfo res)
//            : base(ref res)
//        {

//        }

//        public override bool OnLoadedLogic()
//        {
//            TextAsset textAsset = m_assertBundle.LoadAsset<TextAsset>(m_resInfo.strName);    //获取总的Manifest
//            m_byte = textAsset.bytes;
//            if (null == m_sceneCfg)
//            {
//                if (m_byte == null || m_byte.Length <= 0)
//                {
//                    return false;
//                }
//                LusuoStream lf = new LusuoStream(m_byte);
//                m_sceneCfg = new SceneCfg();
//                m_sceneCfg.Read(ref lf);
//                lf.Close();
//                return true;
//            }
//            return m_sceneCfg != null;
//        }

//        public float GetTerrainHeight(float x, float z, bool smooth)
//        {
//            if (smooth)
//            {
//                return m_sceneCfg.GetTerrainHeightData().SmoothInterpolTerrainHeight(x, z);
//            }
//            return m_sceneCfg.GetTerrainHeightData().FastInterpolTerrainHeight(x, z);
//        }

//        public SceneCfg GetCfg()
//        {
//            return m_sceneCfg;
//        }

//        private SceneCfg m_sceneCfg = null;
//    }

//}
