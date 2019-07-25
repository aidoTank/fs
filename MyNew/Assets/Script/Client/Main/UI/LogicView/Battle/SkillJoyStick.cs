using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using Roma;

public enum eJoyStickEvent
{
    None,
    Up,
    Down,
    Drag,
}

/// <summary>
/// 按下时，方向焦点在中心
/// </summary>
public class SkillJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject m_btnSkill;
    public RawImage m_icon;
    public GameObject m_mask; // 未升级时的锁
    public GameObject m_x;    // 沉默遮罩
    public Image m_maskCd;
    public Text m_txtCd;

    public Image m_energeImage;
    public Transform m_energePos;

    public Transform m_lvParent;
    public Transform m_levelUp;

    public Transform m_select;
    public RectTransform m_btnFocus;
    public RectTransform m_skillRange;
    private Color SKLL_BLUE = new Color(0f, 0.38f, 0.89f, 0.5f);
    private Color SKLL_RED = new Color(0.79f, 0.16f, 0.21f, 0.5f);

    public int m_moveRadius = 190;
    private int m_cancelRadius = 340;  // 取消技能的半径

    public Vector2 m_delta;
    private Vector2 m_startPos;

    private int m_lv;
    // cd中
    private bool m_cd;
    private float m_cdTime;
    private float m_curCdTime;
    // 被动
    private bool m_pasv;   
    // 沉默
    private bool m_chenmo;

    // 能量回复中
    private bool m_energeCd;
    private float m_curEnergeTime;
    private float m_maxEnergeTime;
    //private int m_curEnergeNum;     // 当前点数量
    private int m_maxEnergeNum;     // 最大点数量

    // 回调(索引，事件，摇杆信息，取消技能）
    public Action<int, eJoyStickEvent, Vector2, bool> OnJoyStickEvent;
    public Action<int> OnLvUpEvent;
    private int m_index;

    //private bool m_bDown = false;
    //private float m_curIntervaTime = 0;
    //private float m_maxIntervalTime = 0.8f;

    public TweenScale tween;
    public void Init()
    {
        int.TryParse(transform.name, out m_index);

        m_btnSkill = transform.FindChild("btn_skill").gameObject;
        tween = m_btnSkill.AddComponent<TweenScale>();
        tween.from = new Vector3(1, 1, 1);
        tween.to = new Vector3(0.9f, 0.9f, 1);
        tween.duration = 0.2f;
        tween.enabled = false;
        m_icon = transform.FindChild("btn_skill/icon").GetComponent<RawImage>();
        m_mask = transform.FindChild("btn_skill/mask").gameObject;
        m_x = transform.FindChild("btn_skill/chenmo").gameObject;
        m_maskCd = transform.FindChild("btn_skill/cd").GetComponent<Image>();
        m_txtCd = transform.FindChild("btn_skill/txt_cd").GetComponent<Text>();

        m_energeImage = transform.FindChild("btn_skill/energe").GetComponent<Image>();
        m_energePos = transform.FindChild("btn_skill/energe_bg");

        m_lvParent = transform.FindChild("btn_skill/lv");

        m_levelUp = transform.FindChild("btn_up").transform;
        m_select = transform.FindChild("select");
        m_btnFocus = transform.FindChild("select/focus").GetComponent<RectTransform>();
        m_skillRange = transform.FindChild("select/range_bule").GetComponent<RectTransform>();

        m_btnFocus.gameObject.SetActiveNew(false);
        m_skillRange.gameObject.SetActiveNew(false);
        m_levelUp.gameObject.SetActiveNew(false);

        UIEventListener.Get(m_levelUp.gameObject).onClick = (go) =>
        {
            if (OnLvUpEvent != null)
                OnLvUpEvent(m_index);
        };

        m_moveRadius = (int)(m_moveRadius / GUIManager.m_uiWidthScale);
        m_cancelRadius = (int)(m_cancelRadius / GUIManager.m_uiWidthScale);

        m_mask.SetActiveNew(true);
        m_x.SetActiveNew(false);
        SetCDMask(false);
        m_txtCd.gameObject.SetActiveNew(false);

        ClearPoint();
        m_energeImage.gameObject.SetActiveNew(false);
    }

    public void SetSelectOrder(int order)
    {
        Canvas can = m_select.GetComponent<Canvas>();
        if (can == null)
            can = m_select.gameObject.AddComponent<Canvas>();

        can.overrideSorting = true;
        can.sortingOrder = order;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (!BattleControlModule.ReplayingInteractable())
        //     return;

        if (m_pasv || m_cd || m_lv == 0 || m_chenmo)
        {
            return;
        }

        SetFocusShow(true);

        // 起点位置固定
        m_startPos = m_skillRange.position;
        m_btnFocus.position = m_skillRange.position;

        // 开始位置转屏幕
        m_startPos = GUIManager.ms_uiCamera.WorldToScreenPoint(m_startPos);


        m_delta = Vector2.zero;
        OnJoyStickEvent(m_index, eJoyStickEvent.Down, m_delta, false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // if (!BattleControlModule.ReplayingInteractable())
        //     return;

        if (m_pasv || m_cd || m_lv == 0 || m_chenmo)
        {
            return;
        }

        SetFocusShow(false);

        Vector2 radius = eventData.position - m_startPos;
        if (radius.magnitude > m_cancelRadius)
        {
            OnJoyStickEvent(m_index, eJoyStickEvent.Up, m_delta, true);
        }
        else
        {
            OnJoyStickEvent(m_index, eJoyStickEvent.Up, m_delta, false);
        }
    }

    public void SetFocusShow(bool bShow)
    {
        m_btnFocus.gameObject.SetActiveNew(bShow);
        m_skillRange.gameObject.SetActiveNew(bShow);
        tween.Play(bShow);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // if (!BattleControlModule.ReplayingInteractable())
        //     return;

        if (m_pasv || m_cd || m_lv == 0 || m_chenmo)
        {
            return;
        }

        // 移动时，偏移位置 = 当前位置-开始位置
        m_delta = eventData.position - m_startPos;
        // 设置最大的偏移位置
        if (m_delta.magnitude >= m_moveRadius)
        {
            m_delta = m_delta.normalized * m_moveRadius;
        }

        Vector2 lastPos = m_startPos + m_delta;
        m_btnFocus.position = GUIManager.ms_uiCamera.ScreenToWorldPoint(lastPos);

        m_delta = m_delta / m_moveRadius;
        // 增加取消技能回调
        Vector2 radius = eventData.position - m_startPos;
        if (radius.magnitude > m_cancelRadius)
        {
            OnJoyStickEvent(m_index, eJoyStickEvent.Drag, m_delta, true);
        }
        else
        {
            OnJoyStickEvent(m_index, eJoyStickEvent.Drag, m_delta, false);
        }
    }


    public void CancelSkillEvent()
    {
        OnJoyStickEvent(m_index, eJoyStickEvent.None, m_delta, false);
    }


    private float m_curUpdateTime;
    private void Update()
    {
        if (m_cd)
        {
            m_curCdTime -= Time.deltaTime;
            m_maskCd.fillAmount = m_curCdTime / m_cdTime;
            if (m_curCdTime <= 0)
            {
                m_cd = false;
                SetCDMask(false);
                m_txtCd.gameObject.SetActiveNew(false);
            }
            // 增加更新间隔，优化GC
            m_curUpdateTime += Time.deltaTime;
            if(m_curUpdateTime >= 1f)
            {
                m_curUpdateTime = 0;
                m_txtCd.text = ((int)m_curCdTime + 1).ToString();
            }
        }
        _UpdateEnerge();
    }

    /// <summary>
    /// 设置冷却时间
    /// </summary>
    public void SetCD(float coolTime)
    {
        m_cdTime = coolTime;
        m_curCdTime = coolTime;
        m_cd = true;
        SetCDMask(true);
        m_txtCd.gameObject.SetActiveNew(true);
        m_txtCd.text = ((int)coolTime + 1).ToString();

        tween.Play(false);

        // 点击超级快时，会出现范围框不消失的问题，需设置CD时隐藏
        m_btnFocus.gameObject.SetActiveNew(false);
        m_skillRange.gameObject.SetActiveNew(false);
        //if(!BattleControlModule.m_bStorage)
        CancelSkillEvent();
    }

    public void SetCDMask(bool bActive)
    {
        m_maskCd.gameObject.SetActiveNew(bActive);
        if (bActive)
        {
            m_maskCd.fillAmount = 1;
        }
    }

    #region 能量设置
    /// <summary>
    /// 设置能量
    /// </summary>
    public void SetPoint(int curPointNum)
    {
        m_energePos.SetActiveNew(true);
        UIItem.SetText(m_energePos, "text", curPointNum.ToString());
    }

    public void ClearPoint()
    {
        m_energePos.SetActiveNew(false);
    }

    /// <summary>
    /// 设置能量遮罩
    /// </summary>
    /// <param name="bActive"></param>
    public void SetEnergeMask(bool bActive)
    {
        m_energeImage.gameObject.SetActiveNew(bActive);
        if (bActive)
        {
            m_energeImage.fillAmount = 1;
        }
    }

    /// <summary>
    /// 设置能量
    /// </summary>
    public void SetEnerge(float maxEnergeCDTime, int curPointNum, int maxPiontNum)
    {
        Debug.Log("设置:energeCDTime" + maxEnergeCDTime + "  curPointNum:" + curPointNum + " maxPiontNum：" + maxPiontNum);
        //m_energeTxt.text = curPointNum.ToString();
        SetPoint(curPointNum);
        if (curPointNum == maxPiontNum)   // 如果满了，关闭CD
        {
            m_energeCd = false;
            SetEnergeMask(false);
        }
        else
        {
            // 第一次，满的时候，开始cd
            if (!m_energeCd)
            {
                m_curEnergeTime = maxEnergeCDTime;
                m_energeCd = true;
                SetEnergeMask(true);
            }
            // 升级之后这里会变化
            m_maxEnergeTime = maxEnergeCDTime;
            m_maxEnergeNum = maxPiontNum;
        }
    }

    private void _UpdateEnerge()
    {
        if (m_energeCd)
        {
            m_curEnergeTime -= Time.deltaTime;
            m_energeImage.fillAmount = m_curEnergeTime / m_maxEnergeTime;
            if (m_curEnergeTime <= 0)
            {
                m_curEnergeTime = m_maxEnergeTime;
            }
        }
    }
    #endregion


    public void SetLv(int lv)
    {
        m_lv = lv;
        for (int i = lv; i < m_lvParent.childCount; i++)
        {
            m_lvParent.GetChild(i).gameObject.SetActiveNew(false);
        }

        if (lv > 0 && lv < 5)
        {
            m_lvParent.FindChild((lv - 1).ToString()).gameObject.SetActiveNew(true);
            // 解除封印
            m_mask.SetActiveNew(false);
        }
        else
        {
            m_mask.SetActiveNew(true);
        }
    }

    /// <summary>
    /// 设置沉默显示
    /// </summary>
    /// <param name="value"></param>
    public void SetChenMoMask(bool value)
    {
        m_x.SetActiveNew(value);
        m_chenmo = value;
        if(value)
        {
            CancelSkillEvent();
        }
    }

    public void SetIcon(int iconid, bool pasv = false)
    {
        UIItem.SetRawImage(m_icon.transform, iconid);
        m_pasv = pasv;
    }

    public void SetColor(Color color)
    {
        m_skillRange.GetComponent<Image>().color = color;
    }

    private int m_skillHintEffectUid;
    public void ShowSkillHintEffect(bool bShow, int order, float scale = 1f)
    {
        if (bShow)
        {
            //if (m_skillHintEffectUid == 0)
                //m_skillHintEffectUid = CEffectMgr.CreateUI(31032, m_btnSkill.transform.FindChild("effect"), order, null, null, scale);
        }
        else
        {
            CEffectMgr.Destroy(m_skillHintEffectUid);
            m_skillHintEffectUid = 0;
        }
    }

    /// <summary>
    /// 点击升级时调用
    /// </summary>
    public void SetLevelUpEffect()
    {
        CEffectMgr.CreateUI(31006, m_btnSkill.transform.FindChild("effect"));
    }

    private int m_lvUpBtnEffectHid;
    public void SetLongEffect(bool bShow)
    {
        m_levelUp.gameObject.SetActiveNew(bShow);
        if (bShow)
        {
            if (m_lvUpBtnEffectHid == 0)
                m_lvUpBtnEffectHid = CEffectMgr.CreateUI(31005, m_levelUp.transform.FindChild("effect"));
        }
        else
        {
            CEffectMgr.Destroy(m_lvUpBtnEffectHid);
            m_lvUpBtnEffectHid = 0;
        }
    }

    public void UnInit()
    {
        SetLongEffect(false);
    }


    public static SkillJoyStick Get(GameObject go)
    {
        SkillJoyStick skill = go.GetComponent<SkillJoyStick>();
        if (skill == null)
        {
            skill = go.AddComponent<SkillJoyStick>();
        }
        return skill;
    }
}