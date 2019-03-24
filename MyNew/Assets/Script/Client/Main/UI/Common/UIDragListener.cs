using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;


namespace  Roma
{
public enum eDragEvent
{
    Enter,
    Exit,
    Up,
    Down,
    Drag
}

/// <summary>
/// 因为UGUI的List滑动限制，这个用于单独拖动事件注册
/// </summary>
public class UIDragListener : MonoBehaviour,
                                IPointerEnterHandler,
                                IPointerExitHandler,
                                IPointerDownHandler,
                                IPointerUpHandler,
                                IDragHandler
 

{
    public delegate void DragDelegate (GameObject go, Vector2 state);
    public Action<eDragEvent, Vector2> OnDragEvent;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnDragEvent != null)
            OnDragEvent(eDragEvent.Enter, eventData.delta);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnDragEvent != null)
            OnDragEvent(eDragEvent.Exit, eventData.delta);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnDragEvent != null)
            OnDragEvent(eDragEvent.Down, eventData.delta);
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if (OnDragEvent != null)
            OnDragEvent(eDragEvent.Drag, eventData.delta); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnDragEvent != null)
            OnDragEvent(eDragEvent.Up, eventData.delta);
    }



    static public UIDragListener Get(GameObject go)
    {
        UIDragListener listener = go.GetComponent<UIDragListener>();
        if (listener == null) listener = go.AddComponent<UIDragListener>();
        return listener;
    }
}
}

