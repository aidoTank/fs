using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roma
{
    public class LoadProcess
    {
        public void ResetLoadProcess()
        {
            fPercent = 0.0f;
            strCurInfo = string.Empty;
            m_bDone = false;
        }

        public float fPercent = 0.0f;         //当前下载资源的进度
        public string strCurInfo;
        public bool m_bDone = false;
    }
}
