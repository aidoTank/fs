using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roma;

public enum eUIRichRTextComType
{
    Text,
    Image,
    Link,
}

public class UIRichTextComInfo
{
    public eUIRichRTextComType type;
    public Vector2 pos;
    public string text;      // 普通文本
    public string info;     // 超链接信息，图片信息
}

public struct UIRichTextLineInfo
{
    public int hight;
    public List<UIRichTextComInfo> comList;
}

public class UIRichTextLineNew
{
    private List<UIRichTextComInfo> m_comList = new List<UIRichTextComInfo>();
    private float m_curLinePixel = 0f;
    private string m_curLineStr = "";
    private int m_curLineHight = 30;
    private int m_heighOffset = 4;

    /// <summary>
    /// 预设的文本组件，用于获取信息，以及创建
    /// </summary>
    private Text m_text;
    private Image m_iamge;

    public UIRichTextLineNew(Text text, Image image, int textHight, int heighOffset)
    {
        m_text = text;
        m_iamge = image;
        m_curLineHight = textHight;
        m_heighOffset = heighOffset;
    }

    public UIRichTextLineInfo CreateLine(string lineStr)
    {
        m_curLineStr = "";
        m_curLinePixel = 0;
        for (int i = 0; i < lineStr.Length; i++)
        {
            switch (lineStr[i])
            {
                #region 图片
                case '[':
                    // 查找结束符的位置，如果后面第三个是*，那么这个就是图片
                    if (i + 3 < lineStr.Length)
                    {
                        if (lineStr[i + 3] == ']')
                        {
                            string imageId = lineStr.Substring(i + 1, 2);
                            // 遍历表情库
                            if (UIRichTextMgrNew.s_imageList.ContainsKey(imageId))
                            {
                                // 在图片创建前，先创建文字label
                                CrateText(m_curLinePixel - GetPixelByStr(m_curLineStr), m_curLineStr);
                                m_curLineStr = "";

                                // 创建图片
                                CreateImage(m_curLinePixel, imageId);
                                // 然后累加像素和字符
                                m_curLinePixel += UIRichTextMgrNew.s_imageList[imageId].x;
                                i += 3;
                            }
                            else
                            {
                                // 如果图片库不存在,继续累加字符
                                AddSingleStr(lineStr[i].ToString());
                            }
                        }
                        else
                        {
                            // 如果没有结束符
                            AddSingleStr(lineStr[i].ToString());
                        }
                    }
                    else
                    {
                        // 如果长度溢出，就累加当前的
                        AddSingleStr(lineStr[i].ToString());
                    }
                    break;
                #endregion
                #region 超链接等
                case '<':
                    int endIndex = lineStr.IndexOf('>', i + 1);
                    if (endIndex != -1)
                    {
                        // 获取待解析字符串
                        string parseStr = lineStr.Substring(i, endIndex - i + 1);
                        string[] parseArray = parseStr.Split('=');
                        // 如果符合解析规则，那么继续
                        if (parseArray.Length == 2)
                        {
                            if (lineStr[endIndex + 1] == '<')
                            {
                                // 获取结束符位置,如果有结束符
                                int linkEndIndex = lineStr.IndexOf('>', endIndex + 2);
                                if (linkEndIndex != -1)
                                {
                                    // {超链接}此时不需要{}
                                    string linkStr = lineStr.Substring(endIndex + 1 + 1, linkEndIndex - endIndex - 2);
                                    float linkStrPixel = GetPixelByStr(linkStr);

                                    CrateText(m_curLinePixel - GetPixelByStr(m_curLineStr), m_curLineStr);
                                    m_curLineStr = "";

                                    // 创建链接对象
                                    CreateCom(m_curLinePixel,
                                        parseArray[0].Remove(0, 1) + "=" + parseArray[1].Remove(parseArray[1].Length -1, 1),
                                        linkStr);
                                    m_curLinePixel += linkStrPixel;
                                    i += linkEndIndex - i;
                                }
                            }
                        }
                        else
                        {
                            // {}中无= ， 超链接：{link=LinkType:value:color}{文字}
                            AddSingleStr(lineStr[i].ToString());
                        }
                    }
                    else
                    {
                        // 没有第一个}结束符
                        AddSingleStr(lineStr[i].ToString());
                    }
                    break;
                #endregion
                default:
                    AddSingleStr(lineStr[i].ToString());
                    break;
            }
        }

        CrateText(m_curLinePixel - GetPixelByStr(m_curLineStr), m_curLineStr);

        for (int i = 0; i < m_comList.Count;i ++ )
        {
            m_comList[i].pos.y = m_curLineHight;
        }

        UIRichTextLineInfo lineInfo;
        lineInfo.hight = m_curLineHight + m_heighOffset;
        lineInfo.comList = m_comList;
        return lineInfo;
    }

    private void AddSingleStr(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return;
        }
        m_curLinePixel += GetPixelByStr(str);
        m_curLineStr += str;
    }

    private void CrateText(float posX, string txt)
    {
        if (string.IsNullOrEmpty(txt))
        {
            return;
        }
        UIRichTextComInfo com = new UIRichTextComInfo();
        com.type = eUIRichRTextComType.Text;
        com.pos = new Vector2(posX, 0);
        com.text = txt;
        m_comList.Add(com);
    }

    /// <summary>
    /// 颜色和超链接
    /// </summary>
    private void CreateCom(float posX, string info, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        UIRichTextComInfo com = new UIRichTextComInfo();
        com.type = eUIRichRTextComType.Link;
        com.pos = new Vector2(posX, 0);
        com.text = text;
        com.info = info;
        m_comList.Add(com);
    }

    /// <summary>
    /// 创建图片,一般情况，图片会改变当前行的高度，所以需通过imageid返回图片的高度
    /// </summary>
    private void CreateImage(float posX, string imageId)
    {
        UIRichTextComInfo com = new UIRichTextComInfo();
        com.type = eUIRichRTextComType.Image;
        com.pos = new Vector2(posX, 0);
        com.info = imageId;
        m_comList.Add(com);

        int curHeight = UIRichTextMgrNew.s_imageList[imageId].y;
        if(curHeight > m_curLineHight + m_heighOffset)
        {
            m_curLineHight = curHeight;
        }
    }

    /// <summary>
    /// 于2017年2月25日优化局部变量，tips：这种优化基本没有效果，以后不要向这样去抠变量的优化
    /// </summary>
    private int m_charPiexl;
    private int m_charIndex;
    private CharacterInfo m_charInfo;
    /// <summary>
    /// 获取字符串的像素长度(动态)
    /// </summary>
    public float GetPixelByStr(string str)
    {
        m_charPiexl = 0;
        for (m_charIndex = 0; m_charIndex < str.Length; m_charIndex++)
        {
            m_text.font.RequestCharactersInTexture(str[m_charIndex].ToString(), m_text.fontSize);  // 消耗比下面的少10KGC
            m_text.font.GetCharacterInfo(str[m_charIndex], out m_charInfo, m_text.fontSize);
            m_charPiexl += m_charInfo.advance;
        }
        return m_charPiexl;

        //float tempPiexl = m_text.cachedTextGenerator.GetPreferredWidth(
        //                str,
        //                m_text.GetGenerationSettings(Vector2.zero));
        //return tempPiexl * GUIManager.GetWidthScale();
    }
}
