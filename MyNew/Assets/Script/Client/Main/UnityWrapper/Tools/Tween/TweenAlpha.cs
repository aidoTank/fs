//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Tween the object's alpha.
/// </summary>

[AddComponentMenu("NGUI/Tween/Alpha")]
public class TweenAlpha : UITweener
{
    public float from = 1f;
    public float to = 1f;
    private CanvasGroup cRender;

    /// <summary>
    /// Current alpha.
    /// </summary>

    public float alpha
    {
        get
        {
            if (cRender != null) return cRender.alpha;
            return 0f;
        }
        set
        {
            if (cRender != null) cRender.alpha = value;
        }
    }

    /// <summary>
    /// Find all needed components.
    /// </summary>

    void Init()
    {
        cRender = gameObject.GetComponent<CanvasGroup>();
        if (cRender == null)
        {
            cRender = gameObject.AddComponent<CanvasGroup>();
        }
        cRender.interactable = false;
        cRender.blocksRaycasts = true;
        cRender.ignoreParentGroups = false;
    }

    /// <summary>
    /// Interpolate and update the alpha.
    /// </summary>

    override protected void OnUpdate(float factor, bool isFinished) { alpha = Mathf.Lerp(from, to, factor); }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenAlpha Begin(GameObject go, float duration, float alpha)
    {
        TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration);
        comp.from = comp.alpha;
        comp.to = alpha;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    static public TweenAlpha Get(GameObject go)
    {
        TweenAlpha alpha = go.GetComponent<TweenAlpha>();
        if (alpha == null)
            alpha = go.AddComponent<TweenAlpha>();
        alpha.Init();
        return alpha;
    }
}
