using UnityEngine;
using System.Collections;
public class ShakeCamera : MonoBehaviour
{
    public float m_maxFrameTime = 0.03f;

    private float shakeTime = 0.0f;
    private float fps = 20.0f;
    private float frameTime = 0.0f;
    private float shakeDelta = 0.008f;

    public Camera cam;
    public static bool isshakeCamera =false;

    static public ShakeCamera Get(GameObject go)
    {
        ShakeCamera listener = go.GetComponent<ShakeCamera>();
        if (listener == null)
        {
            listener = go.AddComponent<ShakeCamera>();
        }
        if (listener.cam == null)
        {
            listener.cam = go.GetComponent<Camera>();
        }
        return listener;
    }

    void Start ()
    {
        Get(gameObject);
    }
  
    void Update ()
    {
        if (isshakeCamera)
        {
            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                if (shakeTime <= 0)
                {
                    cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                    isshakeCamera = false;
                    frameTime = m_maxFrameTime;
                }
                else
                {
                    frameTime += Time.deltaTime;
                    if (frameTime > 1.0 / fps)
                    {
                        frameTime = 0;
                        cam.rect = new Rect(shakeDelta * (-1.0f + 2.0f * Random.value), shakeDelta * (-1.0f + 2.0f * Random.value), 1.0f, 1.0f);

                    }
                }
            }
        }
    }
  
    public void Play(float time)
    {
        shakeTime = time;
        isshakeCamera =true;
    }
}