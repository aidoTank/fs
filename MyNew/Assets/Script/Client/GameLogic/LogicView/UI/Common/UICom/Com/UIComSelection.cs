//using UnityEngine;
//using System.Collections;
//using System;
//using UnityEngine.UI;
//using Roma;

//public class UIComSelection : UI
//{
//    //单列
//    public Transform selectionTrans_1;
//    public Transform selectionMenu_1;
//    public Image selectionBg_1;
//    public GameObject baseInfo_1;
//    public Transform infoTrans_1;
//    public UIList selectionList_1;
//    //双列
//    public Transform selectionTrans_2;
//    public Transform infoTrans_2;
//    public UISuperList selectionList_2;


//    public bool isShowBase = false;
//    public int selectionCount;
//    //选项按钮
//    public const int width = 200;
//    public const int heigh = 70;
//    public const int offset = 5;        //高度间距
//    public const int distance = 20;     //距离边缘值
//    public const int infoHeigh = 125;
//    public const int sizeWidth = 260;

//    //选项按钮
//    public const int sizeWidth_2 = 350;
//    public const int sizeHeigh_2 = 450;

//    OccTypeCsv occTypeCsv;

//    public override void Init()
//    {
//        base.Init();
//        selectionTrans_1 = GetChild("panel/selection_one");
//        selectionBg_1 = GetChild("panel/selection_one/parent/bg").gameObject.GetComponent<Image>();
//        baseInfo_1 = GetChild("panel/selection_one/parent/base_info").gameObject;
//        infoTrans_1 = GetChild("panel/selection_one/parent/base_info");
//        selectionMenu_1 = GetChild("panel/selection_one/parent/selection_list");
//        selectionList_1 = GetChild("panel/selection_one/parent/selection_list").gameObject.AddComponent<UIList>();
//        selectionList_1.Init();

//        selectionTrans_2 = GetChild("panel/selection_two");
//        infoTrans_2 = GetChild("panel/selection_two/parent/base_info");
//        selectionList_2 = GetChild("panel/selection_two/parent/selection_list").gameObject.AddComponent<UISuperList>();
//        selectionList_2.Init(160, 70, 2, 4);
//        selectionList_2.offsetX = 5;
//        selectionList_2.offsetY= 2;

//        occTypeCsv = CsvManager.Inst.GetCsv<OccTypeCsv>((int)eAllCSV.eAC_OccType);
//    }


//    public void SetBaseInfo(string icon, string name, string family, int level,int occ, bool isPlayer)
//    {
//        UIItem.SetImage(infoTrans_1, "icon", icon);
//        UIItem.SetText(infoTrans_1, "name", Roma.UIColor.GetNameByLevel(name, level));
//        UIItem.SetText(infoTrans_1, "level", Roma.LevelConvert.GetStringLevel(level));
//        if (isPlayer)
//        {
//            if (string.IsNullOrEmpty(family))
//                UIItem.SetText(infoTrans_1, "family", "暂无家族");
//            else UIItem.SetText(infoTrans_1, "family", "家族：" + family);
//        }
//        else UIItem.SetText(infoTrans_1, "family", family);
//        int occIcon = occTypeCsv.GetOccNameIconByOcc(occ);
//        UIItem.SetImage(infoTrans_1, "occ", occIcon);
//    }

//    public void SetDetailInfo(string icon, string name, string family, int level, int occIcon)
//    {
//        UIItem.SetImage(infoTrans_2, "icon", icon);
//        UIItem.SetText(infoTrans_2, "name", Roma.UIColor.GetNameByLevel(name, level));
//        if (string.IsNullOrEmpty(family))
//            UIItem.SetText(infoTrans_2, "family", "暂无家族");
//        else UIItem.SetText(infoTrans_2, "family", "家族：" + family);
//        UIItem.SetText(infoTrans_2, "level", Roma.LevelConvert.GetStringLevel(level));
//        UIItem.SetImage(infoTrans_2, "occ", occIcon);
//    }

//    public GameObject CreateSelection(int index)
//    {
//        Transform item = selectionList_1.CreateItem(index);
//        return item.gameObject;
//    }

//    public Vector2 InitSelectionSize(int count, bool isShowBase)
//    {
//        int itemHeigh = heigh * selectionCount + offset * (count - 1);
//        if (isShowBase)
//            itemHeigh += infoHeigh;
//        selectionBg_1.rectTransform.sizeDelta = new Vector2(sizeWidth, itemHeigh + distance);
//        return new Vector2(sizeWidth, itemHeigh + distance);
//    }

//    public Vector2 GetSelectionTwoSize()
//    {
//        return new Vector2(sizeWidth_2, sizeHeigh_2);
//    }

//}
