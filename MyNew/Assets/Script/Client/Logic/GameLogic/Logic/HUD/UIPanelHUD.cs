using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Roma
{
    public class UIPanelHUD : UIBase
    {

        public UIHUD[] m_hudList = new UIHUD[(int)eHUDType.Max];

        public override void Init()
        {
            base.Init();
            InitBattleHud();
        }

        public void InitBattleHud()
        {
            m_hudList[(int)eHUDType.FIGHT_ADDBLOOD] = UIHUD.Get(m_root.FindChild("panel/dynamic/hud_1").gameObject);
            m_hudList[(int)eHUDType.FIGHT_HARM] = UIHUD.Get(m_root.FindChild("panel/dynamic/hud_2").gameObject);
            m_hudList[(int)eHUDType.FIGHT_CRIT] = UIHUD.Get(m_root.FindChild("panel/dynamic/hud_3").gameObject);
            m_hudList[(int)eHUDType.FIGHT_EXP] = UIHUD.Get(m_root.FindChild("panel/dynamic/hud_4").gameObject);
            m_hudList[(int)eHUDType.FIGHT_SELFHARM] = UIHUD.Get(m_root.FindChild("panel/dynamic/hud_5").gameObject);
            m_hudList[(int)eHUDType.FIGHT_BUFF] = UIHUD.Get(m_root.FindChild("panel/dynamic/hud_6").gameObject);
            m_hudList[(int)eHUDType.FIGHT_BUFF_1] = UIHUD.Get(m_root.FindChild("panel/dynamic/hud_7").gameObject);            
            if (Application.isEditor)
            {
                Text text = m_hudList[(int)eHUDType.FIGHT_HARM].transform.FindChild("item/txt").GetComponent<Text>();
                text.material.shader = Shader.Find(text.material.shader.name);

                Text text2 = m_hudList[(int)eHUDType.FIGHT_ADDBLOOD].transform.FindChild("item/txt").GetComponent<Text>();
                text2.material.shader = Shader.Find(text2.material.shader.name);
            }

            m_hudList[(int)eHUDType.FIGHT_CRIT].gameObject.SetActiveNew(true);
            m_hudList[(int)eHUDType.FIGHT_ADDBLOOD].gameObject.SetActiveNew(true);
            m_hudList[(int)eHUDType.FIGHT_HARM].gameObject.SetActiveNew(true);
            m_hudList[(int)eHUDType.FIGHT_EXP].gameObject.SetActiveNew(true);
            m_hudList[(int)eHUDType.FIGHT_SELFHARM].gameObject.SetActiveNew(true);
            m_hudList[(int)eHUDType.FIGHT_BUFF].gameObject.SetActiveNew(true);
            m_hudList[(int)eHUDType.FIGHT_BUFF_1].gameObject.SetActiveNew(true);
            
        }

        public void SetHUD(eHUDType eType, string text, Vector3 pos)
        {
            UIHUD hud = m_hudList[(int)eType];
            pos += Vector3.up * 10;
            hud.Add(text, pos);
        }       
    }

}