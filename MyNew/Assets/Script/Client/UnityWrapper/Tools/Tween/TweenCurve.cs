using UnityEngine;
using Roma;
/// <summary>
/// Ease In 慢到快
/// Ease Out 快到慢
/// Ease In Out 中间快
/// </summary>
public class TweenCurve : UITweener
{
    private Bezier m_curveTween;
    public bool m_bAutoDir = false;
	public Vector3 from;
    public Vector3 front;   // 相对起点和终点 y =-1,就是曲线向下
    public Vector3 back;    // 相对起点和终点
	public Vector3 to;

	Transform mTrans;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Vector3 position { get { return cachedTransform.localPosition; } set { cachedTransform.localPosition = value; } }

    protected override void Start()
    {
        base.Start();
        if (m_curveTween == null)
        {
            m_curveTween = new Bezier(from,
                        front,
                        back,
                        to);
        }
    }

    public override void Play(bool forward)
    {
        if (m_curveTween == null)
        {
            return;
        }
        m_curveTween.p0 = from;
        m_curveTween.p1 = front;
        m_curveTween.p2 = back;
        m_curveTween.p3 = to;
        base.Play(forward);
        Reset();
    }

	override protected void OnUpdate (float factor, bool isFinished)
    {
        if (m_curveTween == null)
        {
            return;
        }
        Vector3 pos =  m_curveTween.GetPointAtTime(factor);

        if (m_bAutoDir)
        {
            if (Vector3.Distance(pos,position) > 1)
            {
                Quaternion qua = Quaternion.LookRotation(pos - position);
                cachedTransform.localRotation = qua;
            }
        }
        cachedTransform.localPosition = pos;
    }

    static public TweenCurve Begin(GameObject go, float duration, Vector3 to, Vector3 front, Vector3 back)
	{
        TweenCurve comp = UITweener.Begin<TweenCurve>(go, duration);
        comp.from = comp.position;
        comp.to = to;
        comp.front = front;
        comp.back = back;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
		return comp;
	}

    static public TweenCurve Get(GameObject go)
    {
        TweenCurve pos = go.GetComponent<TweenCurve>();
        if (pos == null)
            pos = go.AddComponent<TweenCurve>();
        return pos;
    }
}