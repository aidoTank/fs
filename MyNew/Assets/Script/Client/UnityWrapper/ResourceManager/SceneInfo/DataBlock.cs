using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Roma
{
    public class DataBlock
    {
        private string m_dataType = "1";

        public DataBlock()
        {
        }

        public virtual bool Read(ref LusuoStream ls)
        {
            return true;
        }

        public virtual bool Write(LusuoStream ls)
        {
            ls.WriteString(m_dataType);
            return true;
        }
    }
}
