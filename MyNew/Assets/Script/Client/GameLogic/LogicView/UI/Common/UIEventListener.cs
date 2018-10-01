using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Roma
{
    /// <summary>
    /// 因为UGUI的List滑动限制，这个事件注册不包括拖动事件，已优化内存
    /// </summary>
    public class UIEventListener : MonoBehaviour,
                                    IPointerClickHandler,
                                    IPointerDownHandler,
                                    IPointerEnterHandler,
                                    IPointerExitHandler,
                                    IPointerUpHandler
                                    //ISelectHandler,       // 优化内存，去掉没用到的事件
                                    //IUpdateSelectedHandler,
                                    //IDeselectHandler,
                                    //IDragHandler,      // tips: 在ScrollRect组件下，该事件失效，单独采用UIDragListener处理滑动事件
                                    //IEndDragHandler,  // tips: 在ScrollRect组件下，该事件失效，单独采用UIDragListener处理滑动事件
                                    //IDropHandler,
                                    //IScrollHandler,
                                    //IMoveHandler
    {
        public delegate void VoidDelegate(GameObject go);
        public delegate void BoolDelegate(GameObject go, bool state);
        // 之后根据需求继续扩展

        public VoidDelegate onClick;
        public VoidDelegate onDoubleClick;
        //public VoidDelegate onSelect;           // 优化内存，去掉没用到的事件
        //public VoidDelegate onUpdateSelect;
        //public VoidDelegate onDeSelect;
        //public VoidDelegate onDrag;
        //public VoidDelegate onDragEnd;
        //public VoidDelegate onDrop;
        //public VoidDelegate onScroll;
        //public VoidDelegate onMove;

        public BoolDelegate onHover;
        public BoolDelegate onPress;

        public int index;

        public object parameter;
        public object parameter1;
        public object parameter2;
        public object parameter3;

        public bool bGuideEvent;
        public int nextGuideID;


        private void WaitOnClick(){
            // 被调用后等待0.2s再响应
            //yield return new WaitForSeconds(0.2f);

            onClick(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
            {
                //StartCoroutine("WaitOnClick");
                WaitOnClick();

            }
        }

        /// <summary>
        /// 用于给引导的点击，不保护按钮声音
        /// </summary>
        public void OnGuideClick()
        {
            if (onClick != null)
            {
                //StartCoroutine("WaitOnClick");
                WaitOnClick();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (null != onPress)
            {
                onPress(gameObject, true);
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (null != onPress)
            {
                onPress(gameObject, false);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onHover != null)
                onHover(gameObject, true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (onHover != null)
                onHover(gameObject, false);
        }


        //public void OnSelect(BaseEventData eventData) { if (onSelect != null) onSelect(gameObject); }
        //public void OnUpdateSelected(BaseEventData eventData) { if (onUpdateSelect != null) onUpdateSelect(gameObject); }
        //public void OnDeselect(BaseEventData eventData) { if (onDeSelect != null) onDeSelect(gameObject); }
        //public void OnDrag(PointerEventData eventData) { if (onDrag != null) onDrag(gameObject); }
        //public void OnEndDrag(PointerEventData eventData) { if (onDragEnd != null) onDragEnd(gameObject); }
        //public void OnDrop(PointerEventData eventData) { if (onDrop != null) onDrop(gameObject); }
        //public void OnScroll(PointerEventData eventData) { if (onScroll != null) onScroll(gameObject); }
        //public void OnMove(AxisEventData eventData) { if (onMove != null) onMove(gameObject); }

        public void SetButton(bool bActive)
        {
            GetComponent<UIEventListener>().enabled = bActive;
        }
        
        static public UIEventListener Get(GameObject go,bool canClickScale)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null) listener = go.AddComponent<UIEventListener>();
            //UIButton.Get(go,canClickScale);
            return listener;
        }
        static public UIEventListener Get(GameObject go)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null)
                listener = go.AddComponent<UIEventListener>();
            return listener;
        }

        static public UIEventListener Get(GameObject go, bool canClickScale, bool bConChild,bool bCanClickBig = false, bool bConUIButton = true)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null)
                listener = go.AddComponent<UIEventListener>();

 
            return listener;
        }
    }
}