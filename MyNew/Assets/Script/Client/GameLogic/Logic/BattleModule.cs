using UnityEngine;

namespace Roma
{
    public partial class BattleModule : Widget
    {
        public BattleModule()
            : base(LogicModuleIndex.eLM_PanelBattle)
        {
        }

        public static Widget CreateLogicModule()
        {
            return new BattleModule();
        }
        public override void Init()
        {
            m_ui = GetUI<UIPanelBattle>();
        }

        public void SetPorp(int index, string text)
        {
            m_ui.SetPorp(index, text);
        }
        
        public UIPanelBattle m_ui;
    }
}

