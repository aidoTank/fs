using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

namespace Roma
{

    public class AllLuaResource : Resource
    {
        public AllLuaResource(ref ResInfo res)
            : base(ref res)
        {

        }

        public override bool OnLoadedLogic()
        {

            return true;
        }

    }
}
