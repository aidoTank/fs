using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Roma;
using UnityEngine.UI;
//using star_def;

/// <summary>
/// 图片：[88]， 里面只能是两位字符串
/// 颜色文本：<color=112233><文字>
/// 超链接：<link=LinkType:value:color><文字>
/// </summary>
public class UIRichTextMgrNew
{
    public enum ImageType
    {
        playerInfo = 0, // 玩家图像
        face,           // 表情图像 
        item,          // 道具图像
    }

    public struct ImageInfo
    {
        public string name;
        public ImageType type;
        public int x;
        public int y;
        public string param;
    }
    //表情信息
    public static Dictionary<int, string> m_faceInfoList = new Dictionary<int, string>()
    {
        {1, "1_1"},
        {2, "2_2"},
        {3, "3_3"},
        {4, "4_4"},
        {5, "5_5"},
        {6, "6_6"},
        {7, "7_7"},
        {8, "8_8"},
        {9, "9_9"},
        {10, "10_10"},
        {11, "11_11"},
        {12, "12_12"},
        {13, "13_13"},
        {14, "14_14"},
        {15, "15_15"},
        {16, "16_16"},
        {17, "17_17"},
        {18, "18_18"},
        {19, "19_19"},
        {20, "20_20"},
        {21, "21_21"},
        {22, "22_22"},
        {23, "23_23"},
        {24, "24_24"},
        {25, "25_25"},
        {26, "26_26"},
        {27, "27_27"},
        {28, "28_28"},
        {29, "29_29"},
        {30, "30_30"},
        {31, "31_31"},
        {32, "32_32"},
    };

    public static Dictionary<string, ImageInfo> s_imageList = new Dictionary<string, ImageInfo>();
    private static bool m_isInit = false;

    public static void Init()
    {
        if (m_isInit)
        {
            return;
        }
        InitCacheText();

        m_isInit = true;
        // 初始化性别
        ImageInfo sex1 = new ImageInfo();
        sex1.name = "s1";
        sex1.type = ImageType.playerInfo;
        sex1.x = 40;
        sex1.y = 40;
        s_imageList.Add(sex1.name, sex1);

        ImageInfo sex2 = new ImageInfo();
        sex2.name = "s2";
        sex2.type = ImageType.playerInfo;
        sex2.x = 40;
        sex2.y = 40;
        s_imageList.Add(sex2.name, sex2);

        // 初始化vip
        for (int i = 0; i < 9; i++)
        {
            ImageInfo vip = new ImageInfo();
            vip.name = "v" + (i + 1);
            vip.type = ImageType.playerInfo;
            vip.x = 40;
            vip.y = 40;
            s_imageList.Add(vip.name, vip);
        }

        // 初始化表情，名称是纯id
        for (int i = 1; i < m_faceInfoList.Count + 1; i++)
        {
            ImageInfo img = new ImageInfo();
            img.name = (i < 10) ? ("0" + i) : i.ToString();
            img.type = ImageType.face;
            img.x = 40;
            img.y = 40;
            // 表情是多张图，所以需要额外的参数去记录
            img.param = m_faceInfoList.ContainsKey(i) ? m_faceInfoList[i] : "0_1";
            s_imageList.Add(img.name, img);
        }

        //// 初始化道具图标
        //IconCsv itemCsv = CsvManager.Inst.GetCsv<IconCsv>((int)eAllCSV.eAC_Icon);
        //foreach(KeyValuePair<int, IconCsvData> item in itemCsv.m_dicData)
        //{
        //    ImageInfo img = new ImageInfo();
        //    img.name = IntToTwoStr(item.Value.iconId);    // 这里应该存图标ID
        //    img.type = ImageType.item;
        //    img.x = 40;
        //    img.y = 40;
        //    s_imageList.Add(img.name, img);
        //}
    }

    public static string IntToTwoStr(int iconId)
    {
        // tips:C#中char占用2个字节
        byte[] bytes = BitConverter.GetBytes(iconId);
        char one = BitConverter.ToChar(bytes, 0);
        char two = BitConverter.ToChar(bytes, 2);
        return one + "" + two;
    }
    public static int TwoStrToInt(string str)
    {
        byte[] byte1 = BitConverter.GetBytes(str[0]);
        byte[] byte2 = BitConverter.GetBytes(str[1]);
        byte[] newByte = new byte[4];
        newByte[0] = byte1[0];
        newByte[1] = byte1[1];
        newByte[2] = byte2[0];
        newByte[3] = byte2[1];
        return BitConverter.ToInt32(newByte, 0);
    }

    // 在这里填写，单击超链接之后的逻辑
    public static void OnClickLink(int type, string value)
    {
        ulong id1, id2;     //id1用于玩家的UID，id2用于指定物品之类的ID，如果不需要则填0
        //Debug.Log("你点击的类型是" + type + " 值是：" + value);
        string[] strList = value.Split('*');
        if (strList.Length == 2 && ulong.TryParse(strList[0], out id1) && ulong.TryParse(strList[1], out id2))
        {
            //switch (type)
            //{
            //    case (int)LinkType.item:
            //        MsgGetOtherRole.GetOtherRole(id1, GET_OTHER_ROLE_TYPE_.GET_OTHER_ROLE_TYPE_ITEM, id2);
            //        break;
            //    case (int)LinkType.pet:
            //        MsgGetOtherRole.GetOtherRole(id1, GET_OTHER_ROLE_TYPE_.GET_OTHER_ROLE_TYPE_PET, id2);
            //        break;
            //    case (int)LinkType.player:
            //        CheckInfoMgr.AskPlayerInfo(id1, star_def.GET_OTHER_ROLE_TYPE_.GET_OTHER_ROLE_TYPE_DETAIL);
            //        break;
            //    case (int)LinkType.team:
            //        NewTeamMgr.OnSendOtherTeamInfo(id1);
            //        break;
            //    case (int)LinkType.addTeam:
            //        NewTeamMgr.OnSendApplyTeam(id1, true);
            //        break;
            //    case (int)LinkType.link:
            //        break;
            //    case (int)LinkType.title:
            //        TitleMgr.OnSendGetTitleInfo(id1, (int)id2);
            //        break;
            //    case (int)LinkType.skill:
            //        ComSkillInfoModule skillInfo = LayoutMgr.Inst.GetLogicModule<ComSkillInfoModule>(LayoutName.S_ComSkillInfo);
            //        skillInfo.OpenEnd = () =>
            //        {
            //            skillInfo.SetSkill(int.Parse(value));
            //        };
            //        skillInfo.SetVisible(true);
            //        break;
            //    case (int)LinkType.achieve:
            //        AchieveMgr.OnSendGetAchieveInfo(id1, (int)id2);
            //        break;
            //    case (int)LinkType.task:
            //        ComQuestInfoModule comTask = LayoutMgr.Inst.GetLogicModule<ComQuestInfoModule>(LayoutName.S_ComQuestInfo);
            //        comTask.OpenEnd = () =>
            //        {
            //            comTask.Open((int)id2);
            //        };
            //        comTask.SetVisible(true);
            //        break;
            //    case (int)LinkType.auction:
            //        AuctionMgr.OnSendGetGooodsInfo(id1);
            //        break;
            //    case (int)LinkType.space:
            //        MySpaceMgr.OnSendOtherInfo(id1);
            //        break;
            //    case (int)LinkType.chatPlayer:
            //        LayoutMgr.Inst.GetLogicModule<MainChatModule>(LayoutName.S_MainChat).OnMainChatPlayer(id1);
            //        break;
            //    case (int)LinkType.playerMenu:
            //        LayoutMgr.Inst.GetLogicModule<FamilyModule>(LayoutName.S_Family).OnMemberEventClick(id1);
            //        break;
            //}
        }
    }

    // 在这里填写，悬停超链接之后的逻辑
    public static void OnHoverLink(bool hover, int type, string value)
    {
        Debug.Log("你悬停的类型是" + type + " 值是：" + value);
        switch (type)
        {
      
        }
    }

    public static LinkedList<Transform> CacheText = new LinkedList<Transform>();
    public static LinkedList<Transform> CacheImage = new LinkedList<Transform>();
    public static GameObject m_cacheTextParent;
    public static GameObject m_cacheImageParent;
    private static int m_cacheTextNum = 0;   // 记录当前创建的个数
    private static int m_cacheImageNum = 0;

    public static void InitCacheText()
    {
        m_cacheTextParent = new GameObject("text_cache");
        for (int i = 0; i < 10; i ++ )
        {
            CacheText.AddLast(CreateCacheText());
        }
        m_cacheImageParent = new GameObject("image_cache");
        for (int i = 0; i < 10; i++)
        {
            CacheImage.AddLast(CreateCacheImage());
        }
    }

    private static Transform CreateCacheText()
    {
        GameObject item = new GameObject((m_cacheTextNum ++).ToString());
        Text text = item.AddComponent<Text>();
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        item.transform.SetParent(m_cacheTextParent.transform);
        RectTransform rectText = item.GetComponent<RectTransform>();
        rectText.anchorMin = Vector2.up;
        rectText.anchorMax = Vector2.up;
        rectText.pivot = Vector2.up;
        return item.transform;
    }

    public static Transform GetCacheText()
    {
        if (UIRichTextMgrNew.CacheText.First == null)
        {
            CacheText.AddLast(CreateCacheText());
        }
        Transform item = UIRichTextMgrNew.CacheText.First.Value;
        UIRichTextMgrNew.CacheText.RemoveFirst();
        return item;
    }

    public static void SetCacheText(Transform text)
    {
        text.transform.SetParent(m_cacheTextParent.transform);
        CacheText.AddLast(text);
    }


    private static Transform CreateCacheImage()
    {
        GameObject item = new GameObject((m_cacheImageNum++).ToString());
        Image image = item.AddComponent<Image>();
        image.raycastTarget = false;
        item.transform.SetParent(m_cacheImageParent.transform);
        RectTransform rectText = item.GetComponent<RectTransform>();
        rectText.anchorMin = Vector2.up;
        rectText.anchorMax = Vector2.up;
        rectText.pivot = Vector2.up;
        return item.transform;
    }

    public static Transform GetCacheImage()
    {
        if (UIRichTextMgrNew.CacheImage.First == null)
        {
            CacheImage.AddLast(CreateCacheImage());
        }
        Transform item = UIRichTextMgrNew.CacheImage.First.Value;
        UIRichTextMgrNew.CacheImage.RemoveFirst();
        return item;
    }

    public static void SetCacheImage(Transform image)
    {
        image.transform.SetParent(m_cacheImageParent.transform);
        CacheImage.AddLast(image);
    }
}

