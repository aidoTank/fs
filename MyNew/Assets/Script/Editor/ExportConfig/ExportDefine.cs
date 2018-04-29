using UnityEditor;

    public class ExportTarget
    {
#if UNITY_IOS

    public const BuildTarget m_buildTarget = BuildTarget.iOS;
#elif UNITY_ANDROID
        public const BuildTarget m_buildTarget = BuildTarget.Android;
#elif UNITY_WEBPLAYER
    public const BuildTarget m_buildTarget = BuildTarget.WebPlayer;
#elif UNITY_WEBGL
    public const BuildTarget m_buildTarget = BuildTarget.WebGL;
#else
    public const BuildTarget m_buildTarget = BuildTarget.StandaloneWindows;
#endif

}

