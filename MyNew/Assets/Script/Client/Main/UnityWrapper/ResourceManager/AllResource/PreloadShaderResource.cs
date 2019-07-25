using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;


namespace Roma
{
    /// <summary>
    /// 预加载shader，shader的使用不仅仅是加载，还需要预编译
    /// 在非运行模式，过一遍游戏场景，通过收集器收集依赖的shader
    /// 依赖的shader会被收集到ShaderVariantCollection文件中
    /// 在游戏首次运行时执行预编译
    /// 
    /// 如果是动态加载的shader，是不会被收集到的，比如战斗中变化的shader
    /// 它由ab进行单独加载并单独编译使用
    /// </summary>
    public class PreloadShaderResource : Resource
    {

        public PreloadShaderResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {
            ShaderVariantCollection shader = m_assertBundle.LoadAsset<ShaderVariantCollection>(m_resInfo.strName);
            m_assertBundle.LoadAllAssets();
            shader.WarmUp();
            return true;
        }
    }
}
