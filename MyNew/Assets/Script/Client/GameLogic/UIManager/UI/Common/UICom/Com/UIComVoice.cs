using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roma
{
    public class UIComVoice : UI
    {
        public UIComAnimation animation;
        public Text txtRecordTips;

        public override void Init()
        {
            base.Init();
            animation = GetChild("panel/animation").gameObject.AddComponent<UIComAnimation>();
            animation.InitKeyFrame(0.4f);
            animation.PlayAnimation();
            txtRecordTips = GetChild("panel/tips").GetComponent<Text>();
        }
    }
}
