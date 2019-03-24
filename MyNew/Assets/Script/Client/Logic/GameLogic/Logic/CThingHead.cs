using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Roma
{
    public class CThingHead
    {
        HeadModule head;
        public static Vector2 s_headHidePos = new Vector2(0, -4000);
        /// <summary>
        /// 是否显示血条等
        /// </summary>
        private bool m_showHp = true;
        private bool m_showHead = true;

        public CThingHead(string name)
        {
            head = (HeadModule) LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelHead);
            m_headObj = head.Create(name);
            UpdatePosToHide();
        }

        public void SetHeadShow(bool bShow)
        {
            if (m_headObj == null)
                return;
            if (bShow)
            {
                m_showHead = true;
            }
            else
            {
                m_headObj.transform.localPosition = s_headHidePos;
                m_showHead = false;
            }
        }

        public void UpdatePos(Vector3 wPos)
        {
            if (!m_showHead)
            {
                return;
            }
            Vector2 headPos = GetHeadNameScreenPos(wPos);
            if (headPos != Vector2.zero)
            {
                head.m_uiHead.UpdatePos(m_headObj, new Vector2(
                    (headPos.x - GUIManager.m_screenHalf.x) * GUIManager.GetWidthScale(),
                    (headPos.y - GUIManager.m_screenHalf.y) * GUIManager.GetHeightScale())
                    );
            }
            _UpdateBuff();
        }


        public Vector2 GetHeadNameScreenPos(Vector3 wPos)
        {
            if (Camera.main != null)
            {
                return Camera.main.WorldToScreenPoint(wPos);
            }
            return Vector2.zero;
        }

        public void UpdatePosToHide()
        {
            head.m_uiHead.UpdatePos(m_headObj, s_headHidePos);
        }

        public void SetName(string name)
        {
            m_headObj.name = name;
            head.m_uiHead.SetName(m_headObj, name, m_namePos);
        }

        public void RemoveHead()
        {
            head.m_uiHead.Remove(m_headObj);
        }

        public void SetHp(float cur, float max)
        {
            if (!m_showHp)
                return;

            head.m_uiHead.SetHp(m_headObj, cur, max);
            head.m_uiHead.SetPlateValue(m_headObj, max);
        }

        public void ShowHp(bool bShow)
        {
            head.m_uiHead.ShowHp(m_headObj, bShow,true);
        }

        public void SetTeam(bool bTeam)
        {
            head.m_uiHead.SetTeam(m_headObj, bTeam);
        }

        public void SetHud(string text,eHUDType type)
        {
            head.m_uiHead.SetHud(m_headObj, text, type,m_hudPos);
        }
        
        public void SetLevel(int level)
        {
            head.m_uiHead.SetLevel(m_headObj, level, m_levelPos);
        }

        public void SetExp(int cur, int max)
        {
            head.m_uiHead.SetExp(m_headObj, cur, max, m_levelPos);
        }

        public void SetLevelExpTip(bool bShow)
        {
            head.m_uiHead.SetLevelTip(m_headObj, bShow);
        }

        public Transform GetHeadObjTran()
        {
            return m_headObj;
        }

        public void SetHeadEnable(bool value)
        {
            m_headObj.gameObject.SetActiveNew(value);
        }

        public void SetFace(int resid)
        {
            head.m_uiHead.SetFace(m_headObj, resid,1);
        }

        public void SetHeadAlpha(float val)
        {
            head.m_uiHead.ChangeAlpha(m_headObj, val);
        }

        public void SetTieYinBuff(bool show, float val)
        {
            head.m_uiHead.SetTieYinBuff(m_headObj, val);
        }

        #region BUFF相关
        public void SetBuff(int buffId, int icon, float maxTime)
        {
            if(maxTime > 0)
            {
                for(int i = 0; i < m_listBuff.Count; i ++)
                {
                    UIHeadBuff buff = m_listBuff[i];
                    if(buff.buffId == buffId)
                    {
                        buff.curTime = maxTime;
                        return;
                    }
                }
                UIHeadBuff b = new UIHeadBuff();
                b.buffId = buffId;
                b.icon = icon;
                b.curTime = maxTime;
                b.maxTime = maxTime;
                m_listBuff.Add(b);

                head.m_uiHead.ShowBuff(m_headObj, true);
            }
            else
            {
                for (int i = 0; i < m_listBuff.Count; i++)
                {
                    UIHeadBuff buff = m_listBuff[i];
                    if (buff.buffId == buffId)
                    {
                        m_listBuff.Remove(buff);
                        break;
                    }
                }
                if(m_listBuff.Count == 0)
                {
                    head.m_uiHead.ShowBuff(m_headObj, false);
                }
            }
            head.m_uiHead.UpdateBuffList(m_headObj, m_listBuff);
        }

        private void _UpdateBuff()
        {
            if (head == null || head.m_uiHead == null)
                return;

            if (m_listBuff.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < m_listBuff.Count; i ++)
            {
                UIHeadBuff buff = m_listBuff[i];
                buff.curTime -= Time.deltaTime;
                buff.pct = buff.curTime / buff.maxTime;
            }
            head.m_uiHead.UpdateBuffPct(m_headObj, m_listBuff[m_listBuff.Count - 1].pct);
        }

        private List<UIHeadBuff> m_listBuff = new List<UIHeadBuff>();
        #endregion

        private Transform m_headObj;
        public Vector2 m_namePos = new Vector2(0, 32);
        private Vector2 m_levelPos = new Vector2(-73, -3);
        public Vector2 m_hudPos = new Vector2(0, 90);

    }
    public class UIHeadBuff
    {
        public int buffId;
        public int icon;
        public float maxTime;
        public float curTime;
        public float pct;
    }
}
