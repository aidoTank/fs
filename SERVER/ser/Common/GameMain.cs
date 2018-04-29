using System;
using System.Timers;

public class GameMain
{
    // 主定时器
    private Timer timer = new Timer(33);

    public GameMain()
    {
        ResManager();
    }

    public void ResManager()
    {
        CsvManager.Inst = new CsvManager();
        SingletonManager.Inst.AddSingleton(SingleName.m_csvMgr, CsvManager.Inst);
        NetRunTime.Inst = new NetRunTime();
        SingletonManager.Inst.AddSingleton(SingleName.m_netRun, NetRunTime.Inst);
        NetManager.Inst = new NetManager();
        SingletonManager.Inst.AddSingleton(SingleName.m_netMgr, NetManager.Inst);
        DBManager.Inst = new DBManager();
        SingletonManager.Inst.AddSingleton(SingleName.m_dbMgr, DBManager.Inst);

        MapManager.Inst = new MapManager();
        SingletonManager.Inst.AddSingleton(SingleName.m_mapMgr, MapManager.Inst);
    }

    public void Init()
    {
        SingletonManager.Inst.Init();
        // 定时器
        timer.Elapsed += new ElapsedEventHandler((sender, e) => {
            Update(Sys.GetTimeStamp());
        });
        timer.AutoReset = true;    // 一直自动执行
        timer.Enabled = true;
        Console.WriteLine("==============服务器初始化完毕==============");
    }

    public void Update(long time)
    {
        SingletonManager.Inst.Update(time);
    }

    public void UnInit()
    {
        SingletonManager.Inst.Destroy();

        SingletonManager.Inst.RemoveSingleton(SingleName.m_csvMgr);
        CsvManager.Inst = null;
        SingletonManager.Inst.RemoveSingleton(SingleName.m_netRun);
        NetRunTime.Inst = null;
        SingletonManager.Inst.RemoveSingleton(SingleName.m_netMgr);
        NetManager.Inst = null;
        SingletonManager.Inst.RemoveSingleton(SingleName.m_dbMgr);
        DBManager.Inst = null;

        SingletonManager.Inst.RemoveSingleton(SingleName.m_mapMgr);
        MapManager.Inst = null;
    }
}
