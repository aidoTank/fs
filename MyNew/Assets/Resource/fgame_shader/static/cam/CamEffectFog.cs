using UnityEngine;
using Roma;

public class CamEffectFog : MonoBehaviour
{
    public Texture maskTexture;
    public Shader blurShader;
    [Range(-0.35f,1f)]
    public float alphaValue = 0.2f;

    private Material blurMat = null;

    void Start()
    {
        blurMat = new Material(blurShader);
    }

    void Update()
    {
        blurMat.SetFloat("_Value", alphaValue);
    }

    /// 在所有渲染完成后被调用，来渲染图片的后期处理效果  
    /// source理解为进入shader过滤器的纹理  
    /// destination理解为渲染完成的纹理  
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        blurMat.SetFloat("_Value", alphaValue);
        blurMat.SetFloat("_Scale", (float)Screen.width / (float)Screen.height);
        blurMat.SetTexture("_MaskTex", maskTexture);
        // 世界坐标 转 屏幕为中心点的坐标（-1,1）
        // Vector3 pos = ((MtCreature)CPlayerMgr.GetMaster().GetMtBase()).GetPos();
        // Vector3 sPos = Camera.main.WorldToScreenPoint(pos);
        // sPos = new Vector2(
        //             (sPos.x - Screen.width * 0.5f),
        //             (sPos.y - Screen.height * 0.5f));
        // // 转 屏幕为中心点的UV坐标
        // sPos.x = sPos.x / Screen.width;
        // sPos.y = sPos.y / Screen.height;
        // sPos *= 2;
        //Debug.Log("屏幕坐标：" + sPos);
        //blurMat.SetVector("_UvPos", sPos);
        Graphics.Blit(source, destination, blurMat);
    }
}