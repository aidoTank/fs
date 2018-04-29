using UnityEngine;
using System.Collections;
using Roma;
public class OccEntiy : MonoBehaviour 
{
	private Vector3 hitPoint;
	private OccMgr iocCam;
	private int counter;
	private Renderer[] rs;
	private bool bShow = true;
	private int frameInterval;

	private RaycastHit h;
	private Ray r;
	
	public void Init ()
    {
		iocCam = Camera.main.GetComponent<OccMgr>();
        if (iocCam == null)
        {
            this.enabled = false;
        }
        rs = GetComponentsInChildren<Renderer>(false);
	}

    public void Show(RaycastHit h)
    {
        counter = Time.frameCount;
        hitPoint = transform.worldToLocalMatrix.MultiplyPoint(h.point);
        if (!bShow)
        {
            Show();
        }
    }

	void Update () 
    {
        if (null == iocCam) return;

        frameInterval = Time.frameCount % 4;
        if(frameInterval == 0)
        {
            if (bShow && Time.frameCount - counter > iocCam.hideDelay)
            {
                if (iocCam.preCullCheck && rs[0].isVisible)
                {
                    var p = transform.localToWorldMatrix.MultiplyPoint(hitPoint);
                    r = new Ray(p, iocCam.transform.position - p);
                    if (Physics.Raycast(r, out h, iocCam.viewDistance))
                    {
                        if (h.transform.tag != iocCam.tag)
                        {
                            Hide();
                        }
                        else
                        {
                            counter = Time.frameCount;
                        }
                    }
                }
                else
                {
                    Hide();
                }
            }
		}
	}
	
	public void Show()
	{
        if (rs == null) return;
        bShow = true;
        for (int i = 0; i < rs.Length; i++)
        {
            rs[i].enabled = true;
        }
	}
	public void Hide()
	{
        if (rs == null) return;
        bShow = false;
		for(int i=0;i<rs.Length;i++)
		{
			rs[i].enabled = false;
		}
	}
}
