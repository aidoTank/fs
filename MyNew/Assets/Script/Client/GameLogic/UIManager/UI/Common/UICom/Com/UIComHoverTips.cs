
//namespace Roma
//{
//    /// <summary>
//    /// 使用如下
//    //UIComHoverTips tips = UICom<UIComHoverTips>.Inst();
//    //tips.Open();
//    //tips.SetBaseInfo("信息1", new GoodsItem(), "");
//    //tips.SetInfo("信息2");
//    //tips.SetInfo("信息3\n为换行");
//    /// </summary>
//    public class UIComHoverTips : UICom<object>
//    {
//        public UIPanelHoverTips m_ui;

//        public UIComHoverTips()
//        {
//            m_ui = GetCom<UIPanelHoverTips>(UIComName.s_hoverTips);
//            SetVisible(false);
//        }

//        public override void SetVisible(bool bShow)
//        {
//            m_ui.OpenPanel(bShow);
//        }

//        public void Open()
//        {
//            SetVisible(true);
//            m_ui.Open();
//        }

//        public void Close()
//        {
//            SetVisible(false);
//            m_ui.Close();
//        }

//        public void SetBaseInfo(string text, GoodsItem goodsInfo, string binding)
//        {
//           m_ui.SetBaseInfo(text, goodsInfo, binding);
//        }

//        public void SetInfo(string text)
//        {
//            m_ui.SetInfo(text);
//        }
//    }
//}
