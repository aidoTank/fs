using UnityEngine;
using UnityEngine.UI;
using Roma;
/// <summary>
/// 所有UI界面的基类
/// 打开关闭的记录，拖动等通用行为放在这里
/// 注：UI子类全部以UIPanelXX命名
/// 节点名为小写panel_xx
/// </summary>
public class UI : MonoBehaviour
{
    /// <summary>
    /// 保持开发规范：panel为该对象的子节点
    /// </summary>
    public Transform m_root;
    public GameObject m_panel;
    public GameObject m_btnClose;

    //private UIPanelInfo panelInfo;
    public bool m_isLoaded;
    //public UIPanelInfo.Loaded m_loadedEvent;

    protected Canvas m_canvas;
    protected GraphicRaycaster m_gr;
    protected RectTransform m_rect;

    public virtual void Init()
    {
        m_root = transform;
        Transform tempObj = m_root.FindChild("panel");
        if (tempObj != null)
        {
            m_panel = tempObj.gameObject;
            //CanvasGroup cg = m_panel.GetComponent<CanvasGroup>();
            //if (cg != null)
            //{
            //    cg.alpha = 1;
            //    Destroy(cg);
            //}
            m_canvas = m_panel.GetComponent<Canvas>();
            if (m_canvas == null)
            {
                m_canvas = m_panel.AddComponent<Canvas>();
            }
            m_gr = m_panel.GetComponent<GraphicRaycaster>();
            if (m_gr == null)
            {
                m_gr = m_panel.AddComponent<GraphicRaycaster>();
            }
            m_rect = m_panel.GetComponent<RectTransform>();

            if (!m_panel.activeSelf)
            {
                m_panel.SetActive(true);
            }
            SetActive(false);
        }
        tempObj = m_root.FindChild("panel/btn_close");
        if (tempObj != null)
        {
            m_btnClose = tempObj.gameObject;
        }
    }

    /// <summary>
    /// 无单独Canvas
    /// </summary>
    /// <param name="bChild"></param>
    public virtual void Init(bool bChild)
    {
        if (bChild)
        {
            m_root = transform;
            Transform tempObj = m_root.Find("panel");
            if (tempObj != null)
            {
                m_panel = tempObj.gameObject;
            }
            tempObj = m_root.FindChild("panel/btn_close");
            if (tempObj != null)
            {
                m_btnClose = tempObj.gameObject;
            }
        }
    }

    /// <summary>
    /// 开始下载图集
    /// </summary>
    public void OnLoadUI()
    {
        if (m_isLoaded)
        {
            return;
        }
        //panelInfo = gameObject.GetComponent<UIPanelInfo>();
        //if (null == panelInfo)
        //{
        //    m_isLoaded = true;
        //    return;
        //}
        //panelInfo.enabled = true;
        //if (panelInfo.IsNeedLoad())
        //{
        //    SetActive(false);
        //    panelInfo.Load(OnLoaded);
        //}
        //else
        //{
        //    OnLoaded();
        //}
    }

    public virtual void OnLoaded()
    {
        SetActive(true);
        m_isLoaded = true;
        //if (null != m_loadedEvent)
        //{
        //    m_loadedEvent();
        //    m_loadedEvent = null;
        //}
        //Destroy(panelInfo);
        //panelInfo = null;
    }

    public virtual void OpenPanel(bool bOpen)
    {
        if (null == m_panel)
            return;
        //if (null != panelInfo && panelInfo.IsLoading())
        //    return;
        if (bOpen)
        {
            SetActive(true);
        }
        else
        {
            SetActive(false);
        }
        return;
    }

    public virtual void SetActive(bool open)
    {
        //Debug.Log("=============================="+open);
        if (m_canvas == null)
        {
            m_panel.SetActive(open);
            return;
        }
        if (m_canvas.enabled && open)
        {
            return;
        }
        if (!m_canvas.enabled && !open)
        {
            return;
        }

        m_canvas.enabled = open;
        m_gr.enabled = open;

        if (open)
        {
            m_rect.anchoredPosition3D = Vector3.zero;
            m_rect.sizeDelta = Vector2.zero;
        }
        else
        {
            m_rect.anchoredPosition3D = Vector3.up * 1000;
            m_rect.sizeDelta = Vector2.zero;
        }
    }

    /// <summary>
    /// 阻止射线检测
    /// </summary>
    public void SetBlocksRaycasts(bool bTrue)
    {
        CanvasGroup cg = m_panel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = m_panel.AddComponent<CanvasGroup>();
        }
        cg.blocksRaycasts = bTrue;
        cg.interactable = false;
        cg.ignoreParentGroups = false;
    }

    public virtual bool IsShow()
    {
        if (null == m_panel)
            return false;
        return m_canvas.enabled && m_isLoaded;
    }

    public Transform GetChild(string path)
    {
        return m_root.FindChild(path);
    }

    public void ShowChildCount(Transform parent, int count)
    {
        if (parent.childCount < count)
            return;
        for (int i = 0; i < count; i++)
        {
            parent.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = count; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetOrder(int order)
    {
        if (!m_canvas.overrideSorting)
            m_canvas.overrideSorting = true;
        m_canvas.sortingOrder = order;
    }

    public int GetOrder()
    {
        if (m_canvas == null)
            return 0;
        return m_canvas.sortingOrder;
    }



    //public bool m_isDragPanel;
    //private bool m_isInit = false;

    //private Vector2 m_curPanelPos = Vector2.zero;
    //private bool m_isAnimaState = false;

    //// 这2个作用一样，一个主动，一个被动
    //public bool m_isLoaded = false;
    //public UIPanelInfo.Loaded m_loaded = null;

    //private UIPanelInfo panelInfo;
    //// 界面下载初始化
    //private void Init()
    //{
    //    if (m_isInit) return;
    //    m_isInit = true;

    //    // 是否是拖动界面
    //    if (m_isDragPanel)
    //        UIDragPanel.Add(m_panel);

    //    panelInfo = gameObject.GetComponent<UIPanelInfo>();
    //    if (null == panelInfo)
    //    {
    //        m_isLoaded = true;
    //        return;
    //    }
    //    if (panelInfo.IsNeedLoad())
    //    {
    //        panelInfo.Load(Loaded);
    //        Debug.Log("下载中。。。。。。。。。。。。。。。。。。。。。。");
    //        // 第一次打开先隐藏，下载完成后再激活
    //        m_panel.SetActive(false);
    //    }
    //    else
    //    {
    //        m_isLoaded = true;
    //    }
    //}

    //private void Loaded()
    //{
    //    m_panel.SetActive(true);
    //    m_isLoaded = true;
    //    if (null != m_loaded)
    //    {
    //        m_loaded();
    //    }
    //}



    //public virtual bool OpenAnimaPanel(bool isOpen)
    //{
    //    if (isOpen)
    //    {
    //        OpenPanel(true);
    //    }
    //    return PlayAnima(isOpen);
    //}

    //public virtual bool IsShow()
    //{
    //    if (null == m_panel) return false;
    //    return m_panel.activeSelf && m_isLoaded;
    //}

    //public virtual void SetDragPanel(bool isDrag)
    //{
    //    if (isDrag)
    //        UIDragPanel.Add(m_panel);
    //    else
    //        UIDragPanel.Remove(m_panel);
    //}

    //public GameObject OpenBlackMask(bool bShow)
    //{
        //if (bShow)
        //    UIItem.SetOrder(GUIManager.ms_uiPanelBg.gameObject, GetOrder() - 1);
        //GUIManager.ms_uiPanelBg.gameObject.SetActive(bShow);
        //return GUIManager.ms_uiPanelBg.gameObject;
    //}

    public enum eOpenAnimationType
    {
        Null = 0,
        Scale = 1,
        Alpha = 2,
    }

    public bool PlayAnima(int type)
    {
        //if(!GlobleConfig.m_bInitMainUIOK) return false;
        //if (m_isAnimaState) return false;
        //if (!isOpen)
        //{
        //    m_curPanelPos = m_panel.transform.localPosition;
        //}
        switch (type)
        {
            case (int)eOpenAnimationType.Null:
                break;
            case (int)eOpenAnimationType.Scale:
                TweenScale scale = TweenScale.Get(m_panel);
                scale.method = UITweener.Method.EaseInOut;
                scale.duration = 0.5f;
                scale.from = Vector3.zero;
                scale.to = Vector3.one;
                scale.Reset();
                //scale.onFinished = AnimaEnd;
                scale.Play(true);
                break;
            case (int)eOpenAnimationType.Alpha:
                TweenAlpha alpha = TweenAlpha.Get(m_panel);
                alpha.Reset();
                alpha.delay = 0.5f;      // 显示时间
                alpha.duration = 0.4f;              // 0.8的渐隐
                alpha.from = 0f;
                alpha.to = 1f;
                alpha.Play(true);
                break;
        }





        //TweenPosition pos = TweenPosition.Get(m_panel);
        //pos.method = UITweener.Method.EaseInOut;
        //pos.duration = 0.5f;
        //pos.from = GetPanelBootObjPos(this.name);
        //pos.to = m_curPanelPos;
        //pos.Play(isOpen);
        //pos.onFinished = AnimaEnd;
        //pos.parameter = isOpen;
        //m_isAnimaState = true;
        return true;
    }

    private void AnimaEnd(UITweener ui)
    {
        bool isOpen = (bool)ui.parameter;
        if (!isOpen)
        {
            OpenPanel(false);
        }
        //m_isAnimaState = false;
    }

    // 获取界面启动对象
    //private GameObject GetBootObj(string layoutName)
    //{
    //    GameObject boot = null;
    //    Widget widget = LayoutMgr.Inst.GetLogicModule(layoutName);
    //    switch (widget.GetMouduleIdx())
    //    {
    //        case LogicModuleIndex.eLM_Player:
    //            boot = LayoutMgr.Inst.GetLogicModule<MainMenuModule>(LayoutName.S_MainMenu).GetMenu(eMainMenu.Play);
    //            break;
    //        case LogicModuleIndex.eLM_Backpack:
    //            boot = LayoutMgr.Inst.GetLogicModule<MainMenuModule>(LayoutName.S_MainMenu).GetMenu(eMainMenu.Bag);
    //            break;
    //    }
    //    return boot;
    //}

    //private Vector2 GetPanelBootObjPos(string panelName)
    //{
    //    GameObject obj = GetBootObj(panelName);
    //    if (obj != null)
    //    {
    //        Bounds pos = RectTransformUtility.CalculateRelativeRectTransformBounds(GUIManager.ms_UIRoot.transform, obj.transform);
    //        return pos.center;
    //    }
    //    return Vector2.zero;
    //}

    //public virtual void Update() { }
    //public virtual GameObject GetMenuBtn(int menuIndex) { return null; }
    //public virtual void OpenSubPanel(int nIndex) { }
    //public virtual void SetPhotoTexture(Texture photoTexture) { }

}

