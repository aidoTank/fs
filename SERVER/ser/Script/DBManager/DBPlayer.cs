using Chloe;
using Chloe.Entity;
using Chloe.MySql;
using System;
using System.Data;

public enum Gender
{
    Man = 1,
    Woman
}

[TableAttribute("User")]
public class TableUser
{
    [Column(IsPrimaryKey = true)]
    [AutoIncrement]
    public int Id { get; set; }
    [Column(DbType = DbType.String)]
    public string UserName { get; set; }    // 唯一id=openid
    [Column(DbType = DbType.String)]
    public string PassWord { get; set; }
}

[TableAttribute("Player")]
public class TablePlayer
{
    [Column(IsPrimaryKey = true)]           // 添加数据非得有key
    public string UserName { get; set; }
    public string Name { get; set; }
    public Gender? Gender { get; set; }
    public int Occ { get; set; }
    public int ResId { get; set; }
    public int MapId { get; set; }
    public int X { get; set; } 
    public int Y { get; set; }
    public int Dir { get; set; }
}

class DBPlayer
{
    public static DBPlayer Inst;
    private MySqlContext context;

    public DBPlayer(MySqlContext con)
    {
        context = con;
    }

    private bool CanRegister(string name)
    {
        try
        {
            IQuery<TableUser> q = context.Query<TableUser>();
            // 如果是查询一条数据，用FirstOrDefault，无数据能返回空，而不是异常
            TableUser u = q.Where(user => user.UserName == name).FirstOrDefault();
            if (u == null)
                return true;
            return false;
        }
        catch(Exception e)
        {
            return false;
        }
    }

    public bool Register(string userName, string pw)
    {
        if(!CanRegister(userName))
        {
            Console.WriteLine("[DataMgr]Register !CanRegister");
            return false;
        }

        try
        {
            TableUser user = new TableUser();
            user.UserName = userName;
            user.PassWord = pw;
            context.Insert(user);
            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine("插入失败");
            Console.ReadKey();
            return false;
        }
    }

    public TablePlayer CreatePlayer(string userName, string name,  int occ)
    {
        try
        {
            TablePlayer player = new TablePlayer();
            player.UserName = userName;
            player.Name = name;
            player.Occ = occ;
            context.Insert(player);
            return player;
        }
        catch(Exception e)
        {
            return null;
        }
    }

    public bool CheckPassWord(string userName, string passWord)
    {
        try
        {
            IQuery<TableUser> q = context.Query<TableUser>();
            TableUser u = q.Where(user => user.UserName == userName && user.PassWord == passWord).FirstOrDefault();
            if (u != null)
                return true;
            return false;
        }
        catch(Exception e)
        {
            Console.WriteLine("检测用户名错误" + e.Message);
            return false;
        }
    }

    public TablePlayer GetPlayer(string userName)
    {
        try
        {
            IQuery<TablePlayer> q = context.Query<TablePlayer>();
            return q.Where(player => player.UserName == userName).FirstOrDefault();
        }
        catch (Exception e)
        {
            Console.WriteLine("获取玩家信息失败" + e.Message);
            return null;
        }
    }

    public bool SavePlayer(Player player)
    {
        try
        {
            player.data.X = player.publicData.x;
            player.data.Y = player.publicData.y;
            player.data.Dir = player.publicData.dir;

            context.Update(player.data);
            Console.WriteLine("保存玩家数据成功:" + player.publicData.name);
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }
}

