using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roma;

public class UIRichTextItemNew
{
    private string m_curLineStr;
    private float m_curLinePixel;
    // 行内容列表
    private List<string> m_listLineStr = new List<string>();
    private List<UIRichTextLineInfo> m_listLineInfo = new List<UIRichTextLineInfo>();

    /// <summary>
    /// 预设的文本组件，用于获取信息，以及创建
    /// </summary>
    private Transform m_parent;
    private Text m_text;
    private Image m_image;
    private int m_textHight = 30;
    private int m_heighOffset = 4;
    private string m_defaultColor;
    private bool m_isCanClick = true;
    private int m_lineMaxPixel = 200;
    /// <summary>
    /// 返回的宽
    /// </summary>
    private int m_curLineWidth;

    private List<Transform> m_userCacheText = new List<Transform>();
    private List<Transform> m_userCacheImage = new List<Transform>();


    
    //public UIRichTextItemNew(Text text, Image image, int lineMaxPixel, string defaultColor, bool hideEvent = false)
    //{
    //    m_hideEvent = hideEvent;
    //    m_text = text;
    //    m_image = image;
    //    m_lineMaxPixel = lineMaxPixel;
    //    m_defaultColor = defaultColor;
    //}

    public UIRichTextItemNew(Text text, Image image, int textHight, int heighOffset, int lineMaxPixel, string defaultColor, bool isCanClick = true)
    {
        m_text = text;
        m_image = image;
        m_textHight = textHight;
        m_heighOffset = heighOffset;
        m_lineMaxPixel = lineMaxPixel;
        m_defaultColor = defaultColor;
        m_isCanClick = isCanClick;
    }

    public int GetWidth()
    {
        if (m_listLineStr.Count > 1)
        {
            return m_lineMaxPixel;
        }
        else
        {
            return (int)m_curLineWidth;
        }
    }

    public void SetColor(string color)
    {
        m_defaultColor = color;
    }

    /// <summary>
    /// 返回高度
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int SplitText(string text)
    {
        m_listLineInfo.Clear();
        m_listLineStr.Clear();
        m_curLineStr = "";
        m_curLinePixel = 0;
        int maxLen = text.Length;
        for (int i = 0; i < maxLen; i++)
        {
            switch (text[i])
            {
                #region 图片
                case '[':
                    if (i + 3 < maxLen)
                    {
                        if (text[i + 3] == ']')
                        {
                            string imageId = text.Substring(i + 1, 2);
                            if (UIRichTextMgrNew.s_imageList.ContainsKey(imageId))
                            {
                                int imagePixel = UIRichTextMgrNew.s_imageList[imageId].x;
                                // 累加像素
                                m_curLinePixel += imagePixel;
                                // 解析图片字符
                                string imageStr = text.Substring(i, 4);
                                if (m_curLinePixel == m_lineMaxPixel)
                                {
                                    // 刚好到换行
                                    m_curLineStr += imageStr;
                                    AddLineToList();
                                }
                                else if (m_curLinePixel > m_lineMaxPixel)
                                {
                                    // 如果大于最大像素，先添加到行，再累加字符
                                    AddLineToList();
                                    m_curLineStr += imageStr;
                                    m_curLinePixel = imagePixel;
                                }
                                else
                                {
                                    m_curLineStr += imageStr;
                                }
                                i += 3;
                            }
                            else
                            {
                                // 如果没有这个id,就直接路过
                                AddSingleStr(text[i].ToString());
                            }
                        }
                        else
                        {
                            // 如果没有结束符，一样路过
                            AddSingleStr(text[i].ToString());
                        }
                    }
                    else
                    {
                        // 如果长度不够，路过
                        AddSingleStr(text[i].ToString());
                    }
                    break;
                #endregion
                #region
                case '<':
                    int endIndex = text.IndexOf('>', i + 1);
                    if (endIndex != -1)
                    {
                        string parseStr = text.Substring(i, endIndex - i + 1);
                        string[] parseArray = parseStr.Split('=');
                        if (parseArray.Length == 2)
                        {
                            if(endIndex + 1 > text.Length)
                            {
                                Debug.LogError("解析符格式不对："+ text);
                                return 0;
                            }
                            if (text[endIndex + 1] == '<')      // 具体内容的{
                            {
                                int linkEndIndex = text.IndexOf('>', endIndex + 2); // 具体内容的}
                                if(linkEndIndex != -1)
                                {
                                    // 截取全部，如{超链接}，但是像素长度只计算内容
                                    string linkStr = text.Substring(endIndex + 1, linkEndIndex - endIndex);
                                    float linkStrPixel = GetPixelByStr(linkStr.Substring(1, linkStr.Length - 2));
                                    // 累加像素
                                    m_curLinePixel += linkStrPixel;
                                    if (m_curLinePixel == m_lineMaxPixel)
                                    {
                                        m_curLineStr += parseStr + linkStr;
                                        AddLineToList();
                                    }
                                    else if (m_curLinePixel > m_lineMaxPixel)
                                    {
                                        float tempLinkTxtPixel = 0;    // 临时链接文字像素不包含{}
                                        int cutIndex = 0;           // 它之前的就是当前行，之后的就是下一行
                                        // {超链接}大于时，开始截取,获取截取点
                                        for (int c = 1; c < linkStr.Length - 1;c ++)
                                        {
                                            tempLinkTxtPixel += GetPixelByStr(linkStr[c].ToString());
                                            if (m_curLinePixel - linkStrPixel + tempLinkTxtPixel > m_lineMaxPixel)
                                            {
                                                cutIndex = c;
                                                break;
                                            }
                                        }
                                        if (cutIndex == 1)  // 如果第一个加起来都超过一行，那么就直接转到下行
                                        {
                                            AddLineToList();
                                            m_curLineStr += parseStr + linkStr;
                                            m_curLinePixel = linkStrPixel;
                                        }
                                        else
                                        {
                                            // 组合当前行超链接{1:1} + {超链接}
                                            m_curLineStr += parseStr + linkStr.Substring(0, cutIndex) + ">";
                                            AddLineToList();    // 生成一行
                                            // 开始组合下一行
                                            m_curLineStr += parseStr + "<" + linkStr.Substring(cutIndex, linkStr.Length - cutIndex);
                                            linkStrPixel = GetPixelByStr(linkStr.Substring(cutIndex, linkStr.Length - 1 - cutIndex));
                                            m_curLinePixel = linkStrPixel;
                                        }
                                    }
                                    else
                                    {
                                        m_curLineStr += parseStr + linkStr;
                                    }
                                    i += linkEndIndex - i;
                                }
                                else
                                {
                                    // 内容么有}结束符
                                    AddSingleStr(text[i].ToString());
                                }
                            }
                        }
                        else
                        {
                            // {}中无= ， 超链接：{link=LinkType:value:color}{文字}
                            AddSingleStr(text[i].ToString());
                        }
                    }
                    else
                    {
                        // 没有第一个}结束符
                        AddSingleStr(text[i].ToString());
                    }
                    break;
                #endregion
                case '\n':
                    AddLineToList();
                    break;
                //case '\t':
                //    AddSingleStr("    ");
                //    break;
                default:
                    AddSingleStr(text[i].ToString());
                    break;
            }
            if (i == text.Length - 1)
            {
                AddLineToList();
            }
        }

        return SetLineInfo();
    }

    private void AddSingleStr(string str)
    {
        // 累加当前字符像素
        m_curLinePixel += GetPixelByStr(str);
        if (m_curLinePixel == m_lineMaxPixel)       // 刚好是一行，就累加字符，获取一行
        {
            m_curLineStr += str;
            AddLineToList();
        }
        else if (m_curLinePixel > m_lineMaxPixel)   // 大于一行，就先获取一行，再继续累加像素和字符
        {
            AddLineToList();
            m_curLineStr += str;
            m_curLinePixel += GetPixelByStr(str);
        } 
        else
        {
            m_curLineStr += str;
        }
    }

    private void AddLineToList()
    {
        if (!string.IsNullOrEmpty(m_curLineStr))
        {
            m_listLineStr.Add(m_curLineStr);
            m_curLineWidth = (int)m_curLinePixel;
            m_curLineStr = "";
            m_curLinePixel = 0;
        }
    }

    /// <summary>
    /// 获取字符串的像素长度(动态)
    /// </summary>
    private float GetPixelByStr(string str)
    {
        float pixel = 0;
        for (int index = 0; index < str.Length; index++)
        {
            CharacterInfo info;
            m_text.font.RequestCharactersInTexture(str[index].ToString(), m_text.fontSize);     // 消耗比下面的少10KGC
            m_text.font.GetCharacterInfo(str[index], out info, m_text.fontSize);
            pixel += info.advance;
        }
        return pixel;

        //float tempPiexl = m_text.cachedTextGenerator.GetPreferredWidth(
        //        str,
        //        m_text.GetGenerationSettings(Vector2.zero));
        //return tempPiexl * GUIManager.GetWidthScale();
    }

    private int SetLineInfo()
    {
        int itemHight = 0;
        for (int i = 0; i < m_listLineStr.Count; i++)
        {
            UIRichTextLineNew line = new UIRichTextLineNew(m_text, m_image, m_textHight, m_heighOffset);
            UIRichTextLineInfo lineInfo = line.CreateLine(m_listLineStr[i]);
            m_listLineInfo.Add(lineInfo);
            itemHight += lineInfo.hight;
        }
        return itemHight;
    }

    public void ClearUI()
    {
        if (m_parent != null)
        {
            if (m_userCacheText.Count != 0)
            {
                for (int i = 0; i < m_userCacheText.Count; i++)
                {
                    UIRichTextMgrNew.SetCacheText(m_userCacheText[i].transform);
                }
                m_userCacheText.Clear();
            }

            for (int i = 0; i < m_userCacheImage.Count; i++)
            {
                UIRichTextMgrNew.SetCacheImage(m_userCacheImage[i].transform);
            }
            m_userCacheImage.Clear();
            //else
            //{
            //    for (int i = 0; i < m_parent.childCount; i++)
            //    {
            //        UIRichTextMgrNew.SetCacheText(m_parent.GetChild(i));
            //    }
            //}
        }
    }

    public void CreateUI(Transform parent)
    {
        m_parent = parent;
        ClearUI();
        int curLineHight = 0;
        for (int i = 0; i < m_listLineInfo.Count; i++)
        {
            UIRichTextLineInfo lineInfo = m_listLineInfo[i];
            CreateLine(lineInfo, curLineHight);
            curLineHight -= lineInfo.hight;
        }
    }

    private void CreateLine(UIRichTextLineInfo lineInfo, int curLineHight)
    {
        for (int index = 0; index < lineInfo.comList.Count; index++)
        {
            CreateLineCom(curLineHight, lineInfo.comList[index]);
        }
    }

    private void CreateLineCom(int curLineHight, UIRichTextComInfo comInfo)
    {
        if (comInfo.type == eUIRichRTextComType.Text)
        {
            float lastHight = curLineHight - (comInfo.pos.y - m_textHight); // 如果是文本，当前的高度-(图片与文本的差)
            CrateText(new Vector2(comInfo.pos.x, lastHight), comInfo.text);
        }
        if (comInfo.type == eUIRichRTextComType.Link)
        {
            float lastHight = curLineHight - (comInfo.pos.y - m_textHight);
            CreateLink(new Vector2(comInfo.pos.x, lastHight), comInfo.info, comInfo.text);
        }
        if (comInfo.type == eUIRichRTextComType.Image)
        {
            float lastHight = curLineHight;
            CreateImage(new Vector2(comInfo.pos.x, lastHight), comInfo.info);
        }
    }


    private void CrateText(Vector2 pos, string txt)
    {
        RectTransform item = UIRichTextMgrNew.GetCacheText().GetComponent<RectTransform>();
        m_userCacheText.Add(item);
        item.SetParent(m_parent);
        item.localPosition = pos;
        item.localScale = Vector3.one;
        item.sizeDelta = Vector2.zero;
        Text label = item.GetComponent<Text>();
        label.font = m_text.font;
        label.fontSize = m_text.fontSize;
        label.text = "<color='#" + m_defaultColor + "'>" + txt + "</color>";

        if(label.raycastTarget)   // 普通文本不需要点击
        {
            label.raycastTarget = false;
        }
    }

    private void CreateLink(Vector2 pos, string info, string text)
    {
        Transform item = UIRichTextMgrNew.GetCacheText();
        m_userCacheText.Add(item);
        item.SetParent(m_parent);
        item.localPosition = pos;
        item.localScale = Vector3.one;
        Text label = item.GetComponent<Text>();
        label.font = m_text.font;
        label.fontSize = m_text.fontSize;

        // 根据不同的类型，创建不同组件
        string[] infoArray = info.Split('=');
        string type = infoArray[0];
        string data = infoArray[1];
        if (type.Equals("color"))
        {
            if (label.raycastTarget)   // 普通文本不需要点击
                label.raycastTarget = false;
            label.text = "<color='#" + data + "'>" + text + "</color>";
        }
        else if (type.Equals("link"))
        {
            if (!label.raycastTarget && m_isCanClick)  
                label.raycastTarget = true;
            string[] linkArray = data.Split(':');
            int tpye = int.Parse(linkArray[0]);
            string color = linkArray[2];
            label.text = "<color='#" + color + "'>" + text + "</color>";
            UIEventListener lis = UIEventListener.Get(item.gameObject);
            lis.onClick = OnClickLink;
            lis.parameter = tpye + "=" + linkArray[1];
            lis.onHover = OnHoverLink;
        }
        label.rectTransform.sizeDelta = new Vector2(label.preferredWidth, label.preferredHeight);
    }

    // 创建图片
    private void CreateImage(Vector2 pos, string imageId)
    {
        Transform item = UIRichTextMgrNew.GetCacheImage();
        m_userCacheImage.Add(item);
        UIRichTextMgrNew.ImageInfo iInfo = UIRichTextMgrNew.s_imageList[imageId];
        if (iInfo.type == UIRichTextMgrNew.ImageType.playerInfo)
        {
            
        }
        else if (iInfo.type == UIRichTextMgrNew.ImageType.face)
        {
            string[] animaInfo = UIRichTextMgrNew.s_imageList[imageId].param.Split('_');
            //UIImageAnima anima = UIImageAnima.Get(item.gameObject);
            //anima.enabled = true;
            //anima.Play((uint)UIAtlasID.icon_expression, int.Parse(animaInfo[0]), int.Parse(animaInfo[1]));
            //anima.m_fps = anima.m_endIndex - anima.m_startIndex + 1;
        }
        else if (iInfo.type == UIRichTextMgrNew.ImageType.item)
        {
            //UILoadImage comIcon = UILoadImage.Get(item.gameObject);
            //comIcon.Load(UIRichTextMgrNew.TwoStrToInt(imageId));
            //// 因为用到缓存image,这里禁用动画
            //UIImageAnima anima = UIImageAnima.Get(item.gameObject);
            //if(anima != null)
            //    anima.enabled = false;
        }
        item.gameObject.SetActive(true);
        item.SetParent(m_parent);
        item.localPosition = pos;
        item.localScale = Vector3.one;
        // 图片比较大，所以在这里更新当前行的高度
        item.GetComponent<RectTransform>().sizeDelta = new Vector2(iInfo.x, iInfo.y);
    }


    private void OnClickLink(GameObject go)
    {
        if (UIEventListener.Get(go).parameter == null)
            return;
        string[] parenArray = UIEventListener.Get(go).parameter.ToString().Split('=');
        if(parenArray.Length == 2)
        {
            int type = int.Parse(parenArray[0]);
            UIRichTextMgrNew.OnClickLink(type, parenArray[1]);
        }
    }

    private void OnHoverLink(GameObject go, bool isHover)
    {
        if (UIEventListener.Get(go).parameter == null)
            return;
        string[] parenArray = UIEventListener.Get(go).parameter.ToString().Split('=');
        int type = int.Parse(parenArray[0]);
        string value = parenArray[1];
        UIRichTextMgrNew.OnHoverLink(isHover, type, value);
    }
}

