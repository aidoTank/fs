using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace Roma
{

    public class CsvListResource : Resource
    {
        private static Dictionary<int, CsvExWrapper> m_mapCSV;

        public static void SetCsvList(Dictionary<int, CsvExWrapper> map)
        {
            m_mapCSV = map;
        }

        public CsvListResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {
            m_byte = (m_assertBundle.LoadAsset<TextAsset>(m_resInfo.strName)).bytes;
            if (m_byte == null || m_byte.Length <= 0)
            {
                return false;
            }
            LusuoStream lf = new LusuoStream(m_byte);

            // 先读取文件
            int iCount = 0;
            lf.ReadInt(ref iCount);
            for (int i = 0; i < iCount; i++)
            {
                // 这里读取id效率更高
                //string csvName;
                //lf.ReadString(out csvName);
                //int type = CsvManager.Inst.GetType(csvName);
                int type = lf.ReadInt();
                CsvExWrapper curCsv = m_mapCSV[type];
                if (null != curCsv)
                {
#if UNITY_EDITOR
                    //CsvEx.editor_formatedCsvName = CsvManager.Inst.GetName(type);
#endif
                    //读出这一段大小
                    int iLen = 0;
                    lf.ReadInt(ref iLen);
                    byte[] uData = new byte[iLen];
                    lf.Read(ref uData);
                    bool succ = curCsv.Load(uData, Encoding.UTF8);
                    if (!succ)
                    {
                        //Debug.LogError("策划请注意**************************加载csv错误:" + CsvManager.Inst.GetName(type));
                    }
                }
                else
                {
                    //Debug.LogError("加载csv失败:" + CsvManager.Inst.GetName(type) + " csv没有加入到LogicSystem中！type:" + type);
                }
            }
            lf.Close();
            return true;
        }
    }
}
