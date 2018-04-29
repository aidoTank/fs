using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

namespace Roma
{
    public class PanelResource : Resource
    {
        public PanelResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override GameObject InstantiateGameObject()
        {
            GameObject obj = base.InstantiateGameObject();
            //RectTransform ui = obj.transform.GetComponent<RectTransform>();
            //// 应该在GUIMgr中挂载
            //RectTransform rect = GameObject.Find("panel_root").GetComponent<RectTransform>();
            //ui.SetParent(rect.transform);
            //ui.anchorMin = Vector3.zero;
            //ui.anchorMax = Vector3.one;
            //ui.pivot = Vector3.zero;
            //ui.localScale = Vector3.one;
            //ui.anchoredPosition = Vector2.zero;
            //ui.sizeDelta = Vector3.zero;
            return obj;
        }
    }
}
