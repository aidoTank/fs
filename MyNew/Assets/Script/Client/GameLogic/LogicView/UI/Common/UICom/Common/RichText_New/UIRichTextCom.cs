using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIRichTextCom : MonoBehaviour 
{
    public Text m_text;
    public Image m_image;
    public Transform m_parent;
    public int m_textHeigh;
    public int m_heighOffset;
    public int m_maxWidth;
    public string m_defaultColor;
    private bool m_isCanClick = true;

    public void Init(int maxWidth, int heigh, string dColor)
    {
        Init(maxWidth, heigh, 0, dColor);
    }

    public void Init(int maxWidth, int heigh, int heighOffset, string dColor, bool isCanClick = true)
    {
        m_text = transform.FindChild("text").GetComponent<Text>();
        m_image = transform.FindChild("image").GetComponent<Image>();
        m_parent = transform.FindChild("parent");
        m_text.gameObject.SetActive(false);
        m_image.gameObject.SetActive(false);
        RectTransform rectText = m_text.GetComponent<RectTransform>();
        rectText.anchorMin = Vector2.up;
        rectText.anchorMax = Vector2.up;
        rectText.pivot = Vector2.up;
        RectTransform rectImage = m_image.GetComponent<RectTransform>();
        rectImage.anchorMin = Vector2.up;
        rectImage.anchorMax = Vector2.up;
        rectImage.pivot = Vector2.up;
        if (m_parent == null)
        {
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = Vector3.zero;
            parent.transform.localScale = Vector3.one;
            RectTransform rect = parent.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            rect.pivot = Vector2.up;
            m_parent = rect;
        }
        m_maxWidth = maxWidth;
        m_textHeigh = heigh;
        m_heighOffset = heighOffset;
        m_defaultColor = dColor;
        m_isCanClick = isCanClick;
    }

    UIRichTextItemNew richText = null;
    private UIRichTextItemNew m_richText
    {
        get
        {
            if (richText == null)
                richText = new UIRichTextItemNew(m_text, m_image, m_textHeigh, m_heighOffset, m_maxWidth, m_defaultColor, m_isCanClick);
            return richText;
        }
    }

    public void SetColor(string color)
    {
        m_richText.SetColor(color);
    }

    public int SetText(string text)
    {
        int h = m_richText.SplitText(text);
        m_richText.CreateUI(m_parent);
        return h;
    }

    public int GetWidth()
    {
        return m_richText.GetWidth();
    }

    public void Clear()
    {
        m_richText.ClearUI();
    }
}
