using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Roma
{ 
    public enum eHUDType
    {
        NONE,
        FIGHT_ADDBLOOD = 1, //加血
        FIGHT_HARM = 2, // 普通伤害
        FIGHT_CRIT = 3, // 暴击
        FIGHT_EXP = 4,
        FIGHT_SELFHARM = 5,
        FIGHT_BUFF=6,
        FIGHT_BUFF_1 =7,
        Max,
    }
    

    public partial class UIPanelHead : UIBase
    {
        public UIHUD[] m_hudList = new UIHUD[(int)eHUDType.Max];

        public void InitBattleHud()
        {
            m_hudList[(int)eHUDType.FIGHT_ADDBLOOD] = m_root.FindChild("panel/dynamic/hud_1").gameObject.AddComponent<UIHUD>();
            m_hudList[(int)eHUDType.FIGHT_HARM] = m_root.FindChild("panel/dynamic/hud_2").gameObject.AddComponent<UIHUD>();
            m_hudList[(int)eHUDType.FIGHT_CRIT] = m_root.FindChild("panel/dynamic/hud_3").gameObject.AddComponent<UIHUD>();
            m_hudList[(int)eHUDType.FIGHT_EXP] = m_root.FindChild("panel/dynamic/hud_4").gameObject.AddComponent<UIHUD>();
            m_hudList[(int)eHUDType.FIGHT_SELFHARM] = m_root.FindChild("panel/dynamic/hud_5").gameObject.AddComponent<UIHUD>();
            m_hudList[(int)eHUDType.FIGHT_BUFF] = m_root.FindChild("panel/dynamic/hud_6").gameObject.AddComponent<UIHUD>();
            m_hudList[(int)eHUDType.FIGHT_BUFF_1] = m_root.FindChild("panel/dynamic/hud_7").gameObject.AddComponent<UIHUD>();
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



        public void SetHUD(Transform head, eHUDType eType, string text)
        {
            Transform item = head.FindChild("hud_" + (int)eType);
            if (null == item)
            {
                item = (GameObject.Instantiate(m_hudList[(int)eType].gameObject)).GetComponent<RectTransform>();
                item.gameObject.SetActiveNew(true);
                item.SetParent(head);
                item.name = "hud_" + (int)eType;
                switch(eType)
                {
                    case eHUDType.FIGHT_ADDBLOOD:
                        item.localPosition = new Vector3(50, -50);
                        break;
                    case eHUDType.FIGHT_CRIT:
                        item.localPosition = new Vector3(0, 0);
                        break;
                    case eHUDType.FIGHT_EXP:
                        item.localPosition = new Vector3(0, -50f);
                        break;
                    case eHUDType.FIGHT_BUFF:
                        item.localPosition = new Vector3(0, 0);
                        break;
                    case eHUDType.FIGHT_BUFF_1:
                        item.localPosition = new Vector3(0, 0);
                        break;
                    default:
                        item.localPosition = new Vector3(0, -100f);
                        break;
                }
                item.localRotation = Quaternion.identity;
                item.localScale = Vector3.one;
            }
            if(eType == eHUDType.FIGHT_EXP)
            {
                int x = UnityEngine.Random.Range(0, 50);
                int y = UnityEngine.Random.Range(0, 50);
                Vector2 randomPos = new Vector2(x, y);
                item.GetComponent<UIHUD>().Add(text, randomPos);
            }
            else
            {
                item.GetComponent<UIHUD>().Add(text, Vector2.zero);
            }
        }

        /// <summary>
        /// 设置连击
        /// </summary>
        //public void SetCaromHit(Transform head, List<string> list, Action end)
        //{
        //    if (list.Count == 0)
        //        return;
        //    Transform item = head.FindChild("hud_12");
        //    if (null == item)
        //    {
        //        item = (GameObject.Instantiate(m_hudList[12].gameObject)).GetComponent<RectTransform>();
        //        item.gameObject.SetActiveNew(true);
        //        item.SetParent(head);
        //        item.name = "hud_12";
        //        item.localPosition = new Vector3(0, 76, 0);
        //        item.localRotation = Quaternion.identity;
        //        item.localScale = Vector3.one;
        //    }
        //    item.GetComponent<UIHUD_Hit>().Add(list, end);
        //}
    }
}