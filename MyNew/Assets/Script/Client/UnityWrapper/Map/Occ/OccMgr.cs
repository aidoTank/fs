using UnityEngine;
using System.Collections;
using Roma;
/// <summary>
/// 从屏幕取出几千条随机点
/// 随机分布点的位置[0,1]转换为射线进行获取场景中的对象
/// samples决定一帧发射多少条射线
/// </summary>
public class OccMgr : MonoBehaviour 
{
	public LayerMask layerMsk;

	public int samples;     // 一帧发出射线的个数，这个数量多就会影响CPU的消耗
    public bool preCullCheck;

	public float raysFov;
	public float viewDistance;
	public int hideDelay;

	private RaycastHit hit;
	private Ray r;

	private OccEntiy iocLod;

	private int haltonIndex;
	private float[] hx;
	private float[] hy;
	private int pixels;

    private Vector3[] m_listPoint;

	private Camera cam;
	private Camera rayCaster;
	
    public void SetActive(bool bShow)
    {
        this.enabled = bShow;
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject g in gos)
        {
            OccEntiy occEnt = g.GetComponent<OccEntiy>();
            if (occEnt != null)
            {
                switch (bShow)
                {
                    case true:
                        occEnt.enabled = true;
                        occEnt.Show();
                        break;
                    case false:
                        occEnt.Show();
                        occEnt.enabled = false;
                        break;
                }
            }
        }
    }

	void Start () 
    {
        cam = GetComponent<Camera>();
        hit = new RaycastHit();
        if (viewDistance == 0) viewDistance = 100;
        cam.farClipPlane = viewDistance;
        haltonIndex = 0;
        if (this.GetComponent<SphereCollider>() == null)
        {
            var coll = gameObject.AddComponent<SphereCollider>();
            coll.radius = 1f;
            coll.isTrigger = true;
        }

		pixels = Mathf.FloorToInt(Screen.width * Screen.height / 200f);
		hx = new float[pixels];
		hy = new float[pixels];
        m_listPoint = new Vector3[pixels];

		for(int i= 0; i < pixels; i++)
		{
			hx[i] = HaltonSequence(i, 2);
			hy[i] = HaltonSequence(i, 3);
            m_listPoint[i] = new Vector3(hx[i], hy[i], 0f);
		}

		GameObject goRayCaster = new GameObject("RayCaster");
		goRayCaster.transform.Translate(transform.position);
		goRayCaster.transform.rotation = transform.rotation;
		rayCaster = goRayCaster.AddComponent<Camera>();
		rayCaster.enabled = false;
		rayCaster.clearFlags = CameraClearFlags.Nothing;
		rayCaster.cullingMask = 0;
		rayCaster.aspect = cam.aspect;
		rayCaster.nearClipPlane = cam.nearClipPlane;
		rayCaster.farClipPlane = cam.farClipPlane;
		rayCaster.fieldOfView = raysFov;
		goRayCaster.transform.parent = transform;
	}
	
	void Update ()
    {
		for(int k=0; k <= samples; k++)
		{
			r = rayCaster.ViewportPointToRay(m_listPoint[haltonIndex]);
			haltonIndex++;
			if(haltonIndex >= pixels) haltonIndex = 0;
			if(Physics.Raycast(r, out hit, viewDistance))
			{
                if (iocLod = hit.transform.GetComponent<OccEntiy>())
				{
					iocLod.Show(hit);
				}
			}
		}
	}
	
    // 随机序列
	private float HaltonSequence(int index, int b)
	{
		float res = 0f;
		float f = 1f / b;
		int i = index;
		while(i > 0)
		{
			res = res + f * (i % b);
			i = Mathf.FloorToInt(i/b);
			f = f / b;
		}
		return res;
	}
}