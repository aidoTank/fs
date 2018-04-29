using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TweenFloat : UITweener
{
    public float from;
    public float to;

    public delegate void FloatUpdate(float val);
    public FloatUpdate FloatUpdateEvent;

    override protected void OnUpdate(float factor, bool isFinished)
    {
        float val = from * (1f - factor) + to * factor;
        if(FloatUpdateEvent != null)
            FloatUpdateEvent(val);
    }

    static public TweenFloat Get(GameObject go)
    {
        TweenFloat tf = go.GetComponent<TweenFloat>();
        if(tf == null)
            tf = go.AddComponent<TweenFloat>();
        return tf;
    }
}
