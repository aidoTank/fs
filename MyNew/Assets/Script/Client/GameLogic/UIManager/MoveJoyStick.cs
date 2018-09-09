using UnityEngine;
using UnityEngine.EventSystems;
using System;

using Roma;

/// <summary>
/// 因为移动摇杆向左下靠齐，eventData.position为UI坐标并且左下为0
/// 所以设置其子对象的锚点为左下，可以更加方便的控制摇杆位置
/// </summary>
public class MoveJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform m_up;
    public RectTransform m_down;
    public RectTransform m_thumb;

    // 最小开始位置（左下为0,0）
    public Vector2 m_maxMinStartPos = new Vector2(200,200);
    public float m_moveRadius = 150;

    public Vector2 m_originalPos;
    public Vector2 m_delta;
    private Vector2 m_startPos;
 
    public Action<eJoyStickEvent, MoveJoyStick> OnJoySticjEvent;

    private void Start()
    {
        m_up = transform.FindChild("up").GetComponent<RectTransform>();
        m_down = transform.FindChild("down").GetComponent<RectTransform>();
        m_thumb = transform.FindChild("thumb").GetComponent<RectTransform>();

        m_up.gameObject.SetActiveNew(true);
        m_down.gameObject.SetActiveNew(false);
        m_thumb.gameObject.SetActiveNew(false);

        m_originalPos = m_down.anchoredPosition;



        m_moveRadius = m_moveRadius / GUIManager.m_uiWidthScale;

        m_maxMinStartPos.x = m_maxMinStartPos.x / GUIManager.m_uiWidthScale;
        m_maxMinStartPos.y = m_maxMinStartPos.y / GUIManager.m_uiHeightScale;
    }

    /// <summary>
    /// eventData.position返回的是以左下为0，0的起点
    /// 所以为了达到王者荣耀的效果，只用判断eventData.position的位置是否过小
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        m_up.gameObject.SetActiveNew(false);
        m_down.gameObject.SetActiveNew(true);
        m_thumb.gameObject.SetActiveNew(false);

        // 设置按下的位置
        m_startPos.x = Math.Max(m_maxMinStartPos.x, eventData.position.x);
        m_startPos.y = Math.Max(m_maxMinStartPos.y, eventData.position.y);

        m_down.position = GUIManager.ms_uiCamera.ScreenToWorldPoint(m_startPos);

        // 按下时，偏移位置 = 按下的位置 - 最新开始位置
        //m_delta = m_startPos - m_maxMinStartPos;

        // 摇杆在中间时，不移动就好
        if (Math.Abs(m_delta.x) < 10 && Math.Abs(m_delta.y) < 10)
        {
            m_delta = Vector2.zero;
            m_thumb.gameObject.SetActiveNew(false);
        }
        else
        {
            m_thumb.gameObject.SetActiveNew(true);
            m_thumb.position = GUIManager.ms_uiCamera.ScreenToWorldPoint(m_startPos + m_delta.normalized * m_moveRadius);
            m_thumb.right = new Vector3(m_delta.x, m_delta.y, 0);
        }

        if (OnJoySticjEvent != null)
            OnJoySticjEvent(eJoyStickEvent.Down, this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_up.gameObject.SetActiveNew(true);
        m_down.gameObject.SetActiveNew(false);
        m_thumb.gameObject.SetActiveNew(false);
        // 抬起时，按钮复位
        m_down.anchoredPosition = m_originalPos;

        m_delta = Vector2.zero;
        if (OnJoySticjEvent != null)
            OnJoySticjEvent(eJoyStickEvent.Up, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 移动时，偏移位置 = 当前位置-开始位置
        m_delta = eventData.position - m_startPos;

        // 摇杆在中间时，不移动就好
        if(Math.Abs(m_delta.x) < 6 && Math.Abs(m_delta.y) < 6)
        {
            m_delta = Vector2.zero;
            m_thumb.gameObject.SetActiveNew(false);
        }
        else
        {
            m_thumb.gameObject.SetActiveNew(true);
            m_thumb.position = GUIManager.ms_uiCamera.ScreenToWorldPoint(m_startPos + m_delta.normalized * m_moveRadius);
            m_thumb.right = new Vector3(m_delta.x, m_delta.y, 0);
        }
        if (OnJoySticjEvent != null)
            OnJoySticjEvent(eJoyStickEvent.Drag, this);
    }
}