// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using UnityEngine;

// namespace Roma
// {

//     public class TextureResource : Resource
//     {
//         public TextureResource(ref ResInfo res)
//             : base(ref res)
//         {

//         }

//         public override void Destroy()
//         {
//             base.Destroy();
//             m_Tex = null;
//         }

//         public override bool OnLoadedLogic()
//         {
//             object obj = m_assertBundle.mainAsset;
//             m_Tex = (Texture2D)obj;
//             return true;
//         }

//         public Texture2D GetTexture()
//         {
//             return m_Tex;
//         }

//         private Texture2D m_Tex = null;
//     }
// }
