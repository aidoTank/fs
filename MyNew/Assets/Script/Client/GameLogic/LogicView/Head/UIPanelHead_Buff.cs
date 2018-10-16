using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public partial class UIPanelHead : UIBase
    {
        public void ShowBuff(Transform head, bool bShow)
        {
            Transform buffItem = head.FindChild("buff");
            if (buffItem == null)
            {
                buffItem = ((GameObject)GameObject.Instantiate(m_buffGo.gameObject)).GetComponent<RectTransform>();
                buffItem.gameObject.SetActiveNew(true);
                buffItem.SetParent(head);
                buffItem.name = "buff";
                buffItem.localPosition = Vector2.up * -4;
                buffItem.localRotation = Quaternion.identity;
                buffItem.localScale = Vector3.one;
                head.FindChild("buff").FindChild("slider").gameObject.SetActiveNew(true);
            }
            buffItem.gameObject.SetActiveNew(bShow);
        }

        /// <summary>
        /// 更新BUFF图标列表
        /// </summary>
        public void UpdateBuffList(Transform head, List<UIHeadBuff> list)
        {
            Transform buffItem = head.FindChild("buff");
            Transform listParent = buffItem.FindChild("list");
            UIItem.ClearList(listParent);
            int index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                int iconId = list[i].icon;
                if (iconId == 0)
                    continue;

                Transform icon = UIItem.GetChild(listParent, index.ToString());
                icon.gameObject.SetActiveNew(true);
                UIItem.SetRawImage(icon, iconId, this);
                index++;
            }
            UIItem.SetItemAlign(UIItem.eItemAlignType.Center, listParent);
        }

        public void UpdateBuffPct(Transform head, float pct)
        {
            Transform buffItem = head.FindChild("buff");
            if (buffItem == null)
                return;
            buffItem.FindChild("slider").FindChild("value").GetComponent<Image>().fillAmount = pct;
        }

        public void SetTieYinBuff(Transform head, float value)
        {
            head.transform.FindChild("tieyin").gameObject.SetActiveNew(true);
            if (value == 0)
                head.transform.FindChild("tieyin").gameObject.SetActiveNew(true);
            head.gameObject.SetActiveNew(true);
            head.transform.FindChild("tieyin/value").GetComponent<Image>().fillAmount = value;
        }

        public void SetTieYinBuffEnable(Transform head, bool show)
        {
            head.transform.FindChild("tieyin").gameObject.SetActiveNew(show);
        }
    }
}

