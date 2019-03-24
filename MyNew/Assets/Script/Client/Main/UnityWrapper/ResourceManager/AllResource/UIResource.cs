using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{

    public class UIResource : Resource
    {
        public UIResource(ref ResInfo res)
            : base(ref res)
        {

        }

        //public GameObject GetMainUI()
        //{
        //    GameObject uiObj = GameObject.Find("ui_main");
        //    if (uiObj == null)
        //    {
        //        uiObj = (GameObject)GameObject.Instantiate(m_assertBundle.LoadAsset("ui_main"));
        //        uiObj.name = "ui_main";
        //    }
        //    return uiObj;
        //}

        //private void GetUIPanelInfoObj(Transform trans, ref List<UIPanelInfo> list)
        //{
        //    if (trans.name.Contains("panel_"))
        //    {
        //        UIPanelInfo panelInfo = trans.gameObject.AddComponent<UIPanelInfo>();
        //        panelInfo.enabled = false;
        //        list.Add(panelInfo);
        //    }
        //    for (int i = 0; i < trans.childCount; i++)
        //    {
        //        GetUIPanelInfoObj(trans.GetChild(i), ref list);
        //    }
        //}
    }



}
