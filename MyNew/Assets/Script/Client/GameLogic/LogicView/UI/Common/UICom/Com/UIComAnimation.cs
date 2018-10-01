using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIComAnimation : MonoBehaviour {
    public enum eAnimationType
    {
        Null = 0,
        Rotate = 1,         //旋转动画
        KeyFrame = 2,       //帧动画
        Scale = 3,              //绽放缩放动画
        Alpha = 4,           //渐隐渐现
        UpAndDown = 5,          //上下运动
    }

    public enum eRotateDirection
    {
        Clockwise = -1,         //顺时针
        Counterclockwise = 1,       //逆时针
    }

    public eAnimationType type = eAnimationType.Null;
    private bool m_isPlay = false;

    [HideInInspector]
    public int derection = 0;
    public float rotateSpeed = 1f;
    public float eulerAngles = 0f;
    public void InitRotate(float speed, eRotateDirection derection = eRotateDirection.Clockwise)
    {
        type = eAnimationType.Rotate;
        this.derection = (int)derection;
        rotateSpeed = speed;
        m_isPlay = true;
    }

    [HideInInspector]
    public List<GameObject> m_frameObjs;        //动画队列
    public float keyTime = 1;       //播放一帧需要的时间
    public float curTime = 0;
    public int curIndex = 0;
    public void InitKeyFrame(float time)
    {
        type = eAnimationType.KeyFrame;
        m_frameObjs = new List<GameObject>();
        for(int i = 0; i < transform.childCount; i++)
        {
            m_frameObjs.Add(transform.FindChild(i.ToString()).gameObject);
        }
        keyTime = time;
    }
    private void ShowKey(int index)
    {
        for (int i = 0; i < m_frameObjs.Count; i++)
        {
            if (i == index)
                m_frameObjs[i].SetActive(true);
            else m_frameObjs[i].SetActive(false);
        }
    }

    [HideInInspector]
    public float maxSize = 1.2f;       //放大的倍数
    public float scaleTime = 1.0f;     //控制缩放的速度
    public void InitScale(float size, float time)
    {
        type = eAnimationType.Scale;
        maxSize = size - 1;
        scaleTime = time;
    }
    [HideInInspector]
    public CanvasGroup canvasGroup;
    public float alphaTime = 0.5f;     //控制渐变的速度（暂时不用）
    public void InitAlpha()
    {
        type = eAnimationType.Alpha;
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    [HideInInspector]
    public float moveTime = 25;
    public float moveHeigh = 5;     //控制渐变的速度（暂时不用）
    public Vector3 basePosition;
    public void InitUpAndDown(float heigh)
    {
        type = eAnimationType.UpAndDown;
        moveHeigh = heigh;
        basePosition = transform.localPosition;
    }

    public void PlayAnimation()
    {
        m_isPlay = true;
    }

    public void StopAnimation()
    {
        if(type == eAnimationType.Rotate)
        {
            transform.Rotate(Vector3.zero);
        }
        else if (type == eAnimationType.KeyFrame)
        {
            ShowKey(0);
        }
        else if(type == eAnimationType.Scale)
        {
            transform.localScale = Vector3.one;
        }
        else if (type == eAnimationType.Alpha)
        {
            canvasGroup.alpha = 1;
        }
        else if (type == eAnimationType.UpAndDown)
        {
            transform.localPosition = basePosition;
        }
        m_isPlay = false;
    }

    void Update()
    {
        if (!m_isPlay)
            return;
        if(type == eAnimationType.Rotate)
        {
            eulerAngles = Time.deltaTime * rotateSpeed * derection;
            transform.Rotate(new Vector3(0, 0, eulerAngles));
        }
        else if (type == eAnimationType.KeyFrame)
        {
            if (m_frameObjs.Count < 2)      //2张以内不需要播放了
                return; 
            if(curTime < keyTime)
            {
                curTime += Time.deltaTime;
                
            }
            else
            {
                curTime = 0;
                curIndex++;
                if(curIndex >= m_frameObjs.Count)
                    curIndex = 0;
                ShowKey(curIndex);
            }
        }
        else if(type == eAnimationType.Scale)
        {
            float scale = 1 + Mathf.PingPong(scaleTime * Time.time, maxSize);
            transform.localScale = new Vector3(scale, scale, 1); 
        }
        else if(type == eAnimationType.Alpha)
        {
            float alpha = 0.5f + Mathf.PingPong(alphaTime * Time.time, 0.5f);
            canvasGroup.alpha = alpha;
        }
        else if (type == eAnimationType.UpAndDown)
        {
            float heigh = basePosition.y + Mathf.PingPong(moveTime * Time.time, moveHeigh);
            transform.localPosition = new Vector3(basePosition.x, heigh, basePosition.z);
        }
    }

}
