using UnityEngine;

namespace Roma
{

    public class ResInfosResource : Resource
    {
        public ResInfosResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {
            TextAsset textAsset = m_assertBundle.LoadAsset<TextAsset>(m_resInfo.strName);    //获取总的Manifest
            m_byte = textAsset.bytes;
            if (m_byte == null || m_byte.Length <= 0)
            {
                return false;
            }
            m_csv.m_mapIDResInfo.Clear();
            m_csv.m_mapNameResInfo.Clear();
            LusuoStream lf = new LusuoStream(m_byte);
            // 先读取文件
            int iCount = 0;
            lf.ReadInt(ref iCount);
            for (int i = 0; i < iCount; i++)
            {
                ResInfo resInfo = new ResInfo();
                resInfo.Load(ref lf);

                if (m_csv.m_mapIDResInfo.ContainsKey(resInfo.nResID))
                {
                    Debug.LogError("重复的资源ID---" + resInfo.nResID.ToString());
                    continue;
                }
                m_csv.m_mapIDResInfo.Add(resInfo.nResID, resInfo);

                if (string.IsNullOrEmpty(resInfo.strName))
                {
                    Debug.LogError("资源名称为空---" + resInfo.nResID.ToString());
                    continue;
                }

                if (m_csv.m_mapNameResInfo.ContainsKey(resInfo.strName))
                {
                    Debug.LogError("重复的资源名称---" + resInfo.nResID.ToString());
                    continue;
                }
                m_csv.m_mapNameResInfo[resInfo.strName] = resInfo;
            }
            lf.Close();
            lf = null;
            m_byte = null;
            return m_csv.m_mapIDResInfo.Count > 0;
        }

        public static ResInfo GetResInfo(int nResID)
        {
            ResInfo res = null;
            if(m_csv.m_mapIDResInfo.TryGetValue(nResID, out res))
            {
                return res;
            }
            else
            {
                Debug.LogError("resinfo无法获取资源:" + nResID);
                return null;
            }
        }

        public static ResInfo GetResInfo(string strResName)
        {
            ResInfo res = null;
            if(m_csv.m_mapNameResInfo.TryGetValue(strResName, out res))
            {
                return res;
            }
            else
            {
                Debug.LogError("无法获取资源:"+ strResName);
                return null;
            }
        }

        private static AllResInfoCsv m_csv = new AllResInfoCsv();
    }

}
