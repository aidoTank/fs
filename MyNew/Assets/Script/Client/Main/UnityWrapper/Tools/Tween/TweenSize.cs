using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TweenSize : UITweener
{
    public Vector2 from;
    public Vector2 to;

    RectTransform mTrans;

    public RectTransform cachedTransform { get { if (mTrans == null) mTrans = transform.GetComponent<RectTransform>(); return mTrans; } }
    public Vector3 sizeDelta { get { return cachedTransform.sizeDelta; } set { cachedTransform.sizeDelta = value; } }

    override protected void OnUpdate(float factor, bool isFinished)
    {
        cachedTransform.sizeDelta = from * (1f - factor) + to * factor;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenSize Begin(GameObject go, float duration, Vector3 size)
    {
        TweenSize comp = UITweener.Begin<TweenSize>(go, duration);
        comp.from = comp.sizeDelta;
        comp.to = size;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    static public TweenSize Get(GameObject go)
    {
        TweenSize size = go.GetComponent<TweenSize>();
        if(size == null)
            size = go.AddComponent<TweenSize>();
        return size;
    }
}
