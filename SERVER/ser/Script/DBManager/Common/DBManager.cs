using Chloe;
using Chloe.Entity;
using Chloe.MySql;
using System;
using System.Data;

namespace Roma
{

    public class DBManager : Singleton
    {
        public static DBManager Inst;
        public MySqlContext context;
        public DBManager()
            : base(false)
        {

        }

        public override void Init()
        {
            context = new MySqlContext(new MySqlConnectionFactory
                ("Database=game;Data Source=" + GlobalConfig.DB_IP
                + ";User ID=" + GlobalConfig.DB_USERID
                + ";Password=" + GlobalConfig.DB_PASSWORD
                + ";port=" + GlobalConfig.DB_PORT
                + ";CharSet=utf8;SslMode=None"));
            Console.WriteLine("数据库初始化完成");

            InitDBLogic();
        }

        private void InitDBLogic()
        {
            DBPlayer.Inst = new DBPlayer(context);
        }

    }
}

