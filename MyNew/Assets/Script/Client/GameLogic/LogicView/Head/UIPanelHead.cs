using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Roma
{
    public partial class UIPanelHead : UIBase
    {
        public GameObject m_headObj;
        public GameObject m_nameObj;
        public GameObject m_levelGo;

        public GameObject m_hp;

        public GameObject m_faceGo;
        public GameObject m_buffGo;  //buff 跟节点

        public Transform m_plateGo;

        private List<Canvas> m_listPos = new List<Canvas>();
        private GameObject m_greenHpObj;
        private GameObject m_redHpObj;
        private Transform m_redLight,
            m_greenLight;

        private void SetPath()
        {
            m_headObj = m_root.FindChild("panel/dynamic/head").gameObject;
            m_nameObj = m_root.FindChild("panel/dynamic/name").gameObject;
            m_levelGo = m_root.FindChild("panel/dynamic/level").gameObject;

            m_buffGo = m_root.FindChild("panel/dynamic/buff").gameObject;
            m_hp = m_root.FindChild("panel/dynamic/hp").gameObject;
            m_faceGo = m_root.FindChild("panel/dynamic/face").gameObject;

            m_plateGo = m_root.FindChild("panel/dynamic/plate");

            m_nameObj.transform.FindChild("txt").GetComponent<Text>().text = "";

            m_greenHpObj = m_root.FindChild("panel/dynamic/hp/green").gameObject;
            m_redHpObj = m_root.FindChild("panel/dynamic/hp/red").gameObject;
            m_redLight = m_root.FindChild("panel/dynamic/hp/red/headle");
            m_greenLight = m_root.FindChild("panel/dynamic/hp/green/headle");
        }

        public override void Init()
        {
            base.Init();
            SetPath();
            InitBattleHud();
            m_levelGo.SetActiveNew(false);
            m_nameObj.SetActiveNew(false);
            m_buffGo.SetActiveNew(false);
            m_plateGo.gameObject.SetActiveNew(false);
            m_hp.SetActiveNew(false);
        }

        public override void OpenPanel(bool bOpen)
        {
            base.OpenPanel(bOpen);
            // 隐藏子节点的canvs
            Canvas[] list = m_panel.GetComponentsInChildren<Canvas>();
            for (int i = 0; i < list.Length; i++)
            {
                list[i].enabled = bOpen;
            }
        }

        public Transform Create()
        {
            RectTransform item = ((GameObject)Instantiate(m_headObj)).GetComponent<RectTransform>();
            item.gameObject.SetActiveNew(true);
            item.SetParent(m_panel.transform);
            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;
            item.localScale = Vector3.one;
            //m_dicTitleOffset[item.gameObject.GetInstanceID()] = 0;

            Canvas can = item.gameObject.AddComponent<Canvas>();
            can.overrideSorting = true;
            m_listPos.Add(can);
            return item;
        }

        public void ChangeAlpha(Transform head, float value)
        {
            if (head != null)
            {
                CanvasGroup cg = head.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = head.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = value;
            }
        }

        public void Remove(Transform head)
        {
            if (head != null)
            {
                GameObject.Destroy(head.gameObject);
            }
        }

        public void UpdatePos(Transform head, Vector3 pos)
        {
            if (head == null)
            {
                return;
            }
            head.localPosition = new Vector2((int)pos.x, (int)pos.y);
        }

        /// <summary>
        /// buff 出现隐藏名字 
        /// </summary>
        /// <param name="head"></param>
        /// <param name="val"></param>
        public void SetNameEnable(Transform head, bool val)
        {
            Transform item = head.FindChild("name");
            //UIItem.SetActiveNew(item, "name", val);
        }

        public void SetName(Transform head, string name, Vector3 offsetPos)
        {
            Transform item = head.FindChild("name");
            if (null == item)
            {
                item = (GameObject.Instantiate(m_nameObj)).GetComponent<RectTransform>();
                item.gameObject.SetActiveNew(true);
                item.SetParent(head);
                item.name = "name";
                item.localRotation = Quaternion.identity;
                item.localScale = Vector3.one;
            }
            item.localPosition = offsetPos;
            UIItem.SetText(item, "txt", name);
        }

        public void SetLevel(Transform head, int level, Vector3 offsetPos)
        {
            Transform item = head.FindChild("level");
            if (null == item)
            {
                item = ((GameObject)GameObject.Instantiate(m_levelGo)).GetComponent<RectTransform>();
                item.gameObject.SetActiveNew(true);
                item.SetParent(head);
                item.name = "level";
                item.localPosition = offsetPos;
                item.localRotation = Quaternion.identity;
                item.localScale = Vector3.one;
            }
            item.SetAsFirstSibling();

            //等级字
            for (int i = 0; i < 15; i++)
            {
               // UIItem.SetActiveNew(item, "txt/" + i, false);
                if (i == level - 1)
                {
               //     UIItem.SetActiveNew(item, "txt/" + i, true);
                }

            }

            //等级背景
            //if (level < 6)
            //{
            //    UIItem.SetActiveNew(item, "bg_level/0", true);
            //    UIItem.SetActiveNew(item, "bg_level/1", false);
            //    UIItem.SetActiveNew(item, "bg_level/2", false);
            //}
            //else if (level > 5 && level < 11)
            //{

            //    UIItem.SetActiveNew(item, "bg_level/0", false);
            //    UIItem.SetActiveNew(item, "bg_level/1", true);
            //    UIItem.SetActiveNew(item, "bg_level/2", false);
            //}
            //else
            //{
            //    UIItem.SetActiveNew(item, "bg_level/0", false);
            //    UIItem.SetActiveNew(item, "bg_level/1", false);
            //    UIItem.SetActiveNew(item, "bg_level/2", true);
            //}
        }

        public void SetLevelTip(Transform head, bool show)
        {
            //Transform item = head.FindChild("level/bg");
            //Transform effectPos= head.FindChild("level/bg/effect");
            //if (item != null)
            //{
            //item.SetActiveNew(show);
            //int effectid=  CEffectMgr.CreateUI(31013, effectPos, 20);
            //}
        }

        public void SetExp(Transform head, int cur, int max, Vector3 offsetPos)
        {
            Transform item = head.FindChild("level");
            if (null == item)
            {
                item = (GameObject.Instantiate(m_levelGo)).GetComponent<RectTransform>();
                item.gameObject.SetActiveNew(true);
                item.SetParent(head);
                item.name = "level";
                item.localPosition = offsetPos;
                item.localRotation = Quaternion.identity;
                item.localScale = Vector3.one;
            }
            item.FindChild("level/value").GetComponent<Image>().fillAmount = (float)cur / (float)max;
        }

        float m_perHp = 1;
        float m_curHp = 1;

        /// <summary>
        /// 血量
        /// </summary>
        /// <param name="head"></param>
        /// <param name="cur"></param>
        /// <param name="max"></param>
        /// <param name="offsetPos"></param>
        /// <param name=""></param>
        public void SetHp(Transform head, float cur, float max)
        {
            if (head == null)
                return;

            Transform item = head.FindChild("hp");
            if (item == null)
                SetTeam(head, true);
            //设置血量
            m_curHp = (float)cur / max;
            m_head = head;
            SetHp(m_curHp);

            //SetHpAni(m_curHp);
            m_perHp = m_curHp;
        }

        private void SetHp(float hp)
        {
            m_head.FindChild("hp/red/hp").GetComponent<Image>().fillAmount = hp;
            m_head.FindChild("hp/green/hp").GetComponent<Image>().fillAmount = hp;
        }

        /// <summary>
        /// 血量变化动画
        /// </summary>
        /// <param name="hp"></param>
        private void SetHpAni(float hp)
        {
            m_showAni = true;
        }

        private void UpdateHpAni()
        {
            timer += Time.deltaTime;

            if (m_curHp > m_perHp) //加血
            {
                m_head.FindChild("hp/hp/headle").SetActiveNew(true);
                Transform hp1 = m_head.FindChild("hp/hhp");
                if (hp1 != null)
                {

                    hp1.SetActiveNew(true);
                    hp1.GetComponent<RectTransform>().sizeDelta =
                   Vector2.Lerp(new Vector2(m_perHp, 14), new Vector2(m_curHp, 14), timer * 5);
                }
                if (timer > 0.5f)
                {
                    m_head.FindChild("hp/hp").GetComponent<RectTransform>().sizeDelta =
                  Vector2.Lerp(new Vector2(m_perHp, 14), new Vector2(m_curHp, 14), timer * 5);

                    if (timer > 0.8f)
                    {
                        m_showAni = false;
                        timer = 0;
                        hp1.SetActiveNew(false);
                        m_head.FindChild("hp/hp/headle").SetActiveNew(false);
                    }
                }
            }
            else
            {
                m_head.FindChild("hp/hp/headle").SetActiveNew(true);
                m_head.FindChild("hp/hp").GetComponent<RectTransform>().sizeDelta =
                    Vector2.Lerp(new Vector2(m_perHp, 14), new Vector2(m_curHp, 14), timer * 15);
                if (timer > 0.3f)
                {
                    Transform hp1 = m_head.FindChild("hp/hhp");
                    if (hp1 != null)
                    {
                        hp1.SetActiveNew(true);
                        hp1.GetComponent<RectTransform>().sizeDelta =
                       Vector2.Lerp(new Vector2(m_perHp, 14), new Vector2(m_curHp, 14), timer * 10);
                    }
                    if (timer > 0.5f)
                    {
                        m_showAni = false;
                        timer = 0;
                        hp1.SetActiveNew(false);
                        m_head.FindChild("hp/hp/headle").SetActiveNew(false);
                    }
                }
            }
        }

        Transform m_head;
        bool m_showAni = false;
        float timer = 0;
        public override void Update()
        {
            base.Update();
            if (m_showAni)
                UpdateHpAni();
        }

        /// <summary>
        /// 设置队伍  血条颜色变化
        /// </summary>
        /// <param name="head"></param>
        /// <param name="isTeamMate"></param>
        public void SetTeam(Transform head, bool isTeamMate)
        {
            if (head == null)
                return;

            Transform item = head.FindChild("hp");

            if (null == item)
            {
                GameObject temp = m_hp;
                item = Instantiate(temp).GetComponent<RectTransform>();
                item.SetParent(head);
                item.name = "hp";
                item.localPosition = Vector2.zero;
                item.localRotation = Quaternion.identity;
                item.localScale = Vector3.one;
            }

            item.gameObject.SetActive(true);
            item.FindChild("red").SetActiveNew(!isTeamMate);
            item.FindChild("green").SetActiveNew(isTeamMate);

            Transform namePos = head.FindChild("name");
            if (namePos == null)
                SetName(head, "", Vector2.zero);

            Text name = head.FindChild("name/txt").GetComponent<Text>();
            name.color = isTeamMate ? Color.green : Color.red;
        }

        public void ShowHp(Transform head, bool bShow, bool isTeamMate)
        {
            Transform item = head.FindChild("hp");
            if (item != null)
            {
                item.gameObject.SetActiveNew(bShow);
            }
        }

        public void SetHud(Transform head, string str, eHUDType type, Vector3 offsetPos)
        {
            if (head == null)
                return;

            SetHUD(head, type, str);
        }

        /// <summary>
        /// 显示刻度
        /// </summary>
        public void SetPlateValue(Transform head, float blood)
        {
            Transform item = head.FindChild("plate");
            if (null == item)
            {
                item = ((GameObject)GameObject.Instantiate(m_plateGo.gameObject)).GetComponent<RectTransform>();
                item.gameObject.SetActiveNew(true);
                item.SetParent(head);
                item.name = "plate";
                item.localPosition = m_platePos;
                item.localRotation = Quaternion.identity;
                item.localScale = Vector3.one;
            }
            int count = (int)blood / 200;

            count = count > 8 ? 8 : count;
            float maxLength = item.GetComponent<RectTransform>().sizeDelta.x;
            blood = blood > 1500 ? 1500 : blood;

            if (blood > 0)
            {
                for (int i = 0; i < count + 1; i++)
                {
                    Transform cItem = UIItem.GetChild(item, i.ToString());
                    cItem.gameObject.SetActiveNew(true);
                    float x = (200 * i * maxLength) / blood - maxLength * 0.5f;
                    cItem.localPosition = new Vector2(x, 0);
                }
            }
        }

        /// <summary>
        /// 设置表情
        /// </summary>
        /// <param name="head"></param>
        /// <param name="resid"></param>
        /// <param name="offsetPos"></param>
        public void SetFace(Transform head, int resid, float time)
        {
            Transform item = head.FindChild("face");
            HideSelf self = null;
            if (item != null)
                self = item.gameObject.GetComponent<HideSelf>();
            if (null == item)
            {
                item = ((GameObject)GameObject.Instantiate(m_faceGo)).GetComponent<RectTransform>();
                item.gameObject.SetActiveNew(true);
                item.SetParent(head);
                item.name = "face";
                item.localPosition = Vector2.zero;
                item.localRotation = Quaternion.identity;
                item.localScale = Vector3.one;
                self = item.gameObject.AddComponent<HideSelf>();
            }
            self.m_time = time;
            self.isShow = true;
            item.gameObject.SetActiveNew(true);
            Transform icon = item.FindChild("icon");
            if (item.FindChild("icon").GetComponent<Animation>() != null)
                item.FindChild("icon").GetComponent<Animation>().Play();
            UIItem.SetRawImage(icon, resid, this);
            //UIItem.SetUIMatAnimation(icon, resid, true);
        }

        private Vector2 m_platePos = new Vector2(1, 1.1f);
    }

}