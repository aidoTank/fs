using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelJoyStick : UIBase
    {
        //private Camera m_effectCam;

        public MoveJoyStick m_move;          //摇杆
        public SkillJoyStick[] m_SkillBtn;   //技能按钮
        public GameObject m_cancelBtn;
       
        public override void Init()
        {
            base.Init();
      
            GameObject move = m_root.FindChild("panel/dynamic/move").gameObject;
            m_move = MoveJoyStick.Get(move);
            m_SkillBtn = new SkillJoyStick[5];
            Transform skillParent = m_root.FindChild("panel/dynamic/skill");
            for (int i = 0; i < skillParent.childCount; i ++)
            {
                GameObject go = skillParent.FindChild(i.ToString()).gameObject;
                m_SkillBtn[i] = SkillJoyStick.Get(go);
                m_SkillBtn[i].Init();
            }
            m_cancelBtn = m_root.FindChild("panel/dynamic/cancel").gameObject;
            
            m_cancelBtn.SetActiveNew(false);
        }

        public override void UnInitData()
        {
            if (m_SkillBtn == null)
                return;
            for(int i = 0;i < m_SkillBtn.Length; i ++)
            {
                m_SkillBtn[i].UnInit();
            }
        }

       

        /// <summary>
        /// 移动区域显示隐藏
        /// </summary>
        /// <param name="bShow"></param>
        public void SetActiveMoveArea(bool bShow)
        {
            m_move.gameObject.SetActiveNew(bShow);
        }



        /// <summary>
        /// 放技能中被沉默时
        /// </summary>
        public void CloseSkillFocus()
        {
            for(int i = 0; i < m_SkillBtn.Length;i++)
            {
                m_SkillBtn[i].SetFocusShow(false);
            }
        }

        public void SetIcon(int index, int icon, bool pasv = false)
        {
            m_SkillBtn[index].SetIcon(icon, pasv);
        }

        public void ShowSkillLvUp(int index, bool bShow)
        {
            m_SkillBtn[index].SetLongEffect(bShow);
        }

        /// <summary>
        /// 技能点
        /// </summary>
        public void SetPoint(int index, int curPointNum)
        {
            m_SkillBtn[index].SetPoint(curPointNum);
        }

        public void SetCD(int index, float cd)
        {
            m_SkillBtn[index].SetCD(cd);
        }


        public void SetColor(int index, Color color)
        {
            m_SkillBtn[index].SetColor(color);
        }

        public void SetSilence(int index, bool bSilence)
        {
            m_SkillBtn[index].SetChenMoMask(bSilence);
        }

        //public override void Update()
        //{
        //    if(Application.isEditor)
        //        m_effectCam.orthographicSize = Screen.height * 0.5f;
        //}

    }
}
