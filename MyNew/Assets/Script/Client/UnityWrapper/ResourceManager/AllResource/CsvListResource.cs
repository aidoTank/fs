using System.Text;
using UnityEngine;

namespace Roma
{

    public class CsvListResource : Resource
    {
        public CsvListResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {
            m_byte = (m_assertBundle.LoadAsset<TextAsset>(m_resInfo.strName)).bytes;
            LogicSystem.Inst.InitCsv(ref CsvManager.Inst);

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
                CsvExWrapper curCsv = CsvManager.Inst.GetOrCreateGetCsv(type);
                if (null != curCsv)
                {
                    //读出这一段大小
                    int iLen = 0;
                    lf.ReadInt(ref iLen);
                    byte[] uData = new byte[iLen];
                    lf.Read(ref uData);
                    bool bSucc = curCsv.Load(uData, Encoding.UTF8);
                    if (!bSucc)
                    {
                        Debug.LogError("策划请注意**************************加载csv错误:" + CsvManager.Inst.GetName(type));
                    }
                }
                else
                {
                    Debug.LogError("加载csv失败:" + CsvManager.Inst.GetName(type) + " csv没有加入到LogicSystem中！");
                }
            }
            lf.Close();
            return true;
        }
    }




}
