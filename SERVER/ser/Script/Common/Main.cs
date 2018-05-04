using Chloe;
using Chloe.Entity;
using Chloe.MySql;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ser
{
    class ServerMain
    {

        static void Main(string[] args)
        {
            GameMain gm = new GameMain();
            gm.Init();


            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        gm.UnInit();
                        return;
                    case "print":
                        break;
                }
            }
        }
    }
}
