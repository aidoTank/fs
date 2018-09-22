using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
 
    public class KeyMgr : Singleton
    {
        public static KeyMgr Inst;

        public KeyMgr() : base(true) { }

        private bool m_bMove;
        public override void Update(float fTime, float fDTime)
        { 
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector2 moveVec = new Vector2(h, v);
            if (moveVec != Vector2.zero)
            {
                JoyStickModule ctr = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                ctr.OnMove(false, moveVec);
                m_bMove = true;
            }
            else
            {
                if(m_bMove)
                {
                    JoyStickModule ctr = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
                    ctr.OnMove(true, moveVec);
                    m_bMove = false;
                }
            }
        }
      
    }

}