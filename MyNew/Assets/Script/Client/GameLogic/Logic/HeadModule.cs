using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public class HeadModule : Widget
    {
        public UIPanelHead m_uiHead;

        public HeadModule()
                : base(LogicModuleIndex.eLM_PanelHead)
        {

        }

        public static Widget CreateLogicModule()
        {
            return new HeadModule();
        }

        public override void Init()
        {
            m_uiHead = this.GetUI<UIPanelHead>();
        }

        public override void UpdateUI(float fTime, float fDTime)
        {
        }

        public Transform Create(string name)
        {
            return m_uiHead.Create();
        }
        
    }
}


