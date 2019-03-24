using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

namespace Roma
{
    public class FspNetRunTime : Singleton
    {
        public FspNetRunTime():base(true)
        {

        }

        public void Conn(string ip, int port, Action coonCb)
        {
            Debug.Log("new start Connect");
            Stop();
            m_netState = NetState.Connecting;
            m_recvHeartBeatTime = 0;

            m_ip = ip;
            m_port = port;
            m_connectedCb = coonCb;

            IPAddress ipAddres = null;
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                Debug.Log("Connect wifi");
                IPAddress[] address = null;
                try
                {
                    address = Dns.GetHostAddresses(m_ip);
                }
                catch (Exception e)
                {
                    Debug.Log("Dns解析异常, ip:" + m_ip + " port:" + m_port + " " + e);
                    m_netState = NetState.ConnFail;
                    return;
                }

                if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    Debug.Log("Connect InterNetworkV6");
                    m_socketClient = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                }
                else
                {
                    Debug.Log("Connect InterNetworkV4");
                    m_socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                ipAddres = address[0];
            }
            else
            {
                Debug.Log("Connect 4g, 源 ip:" + m_ip);
                if (m_ip.Contains(".com"))
                {
                    DnsCsv csv = CsvManager.Inst.GetCsv<DnsCsv>((int)eAllCSV.eAC_Dns);
                    DnsCsvData data = csv.GetData(m_ip);
                    m_ip = data.ip;
                    Debug.Log("Connect 4g, 最终 ip：" + m_ip);
                }

                m_socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress.TryParse(m_ip, out ipAddres);
            }

            if (ipAddres == null)
            {
                Debug.Log("ipAddres is null, ip:" + m_ip + " port:" + m_port);
                m_netState = NetState.ConnFail;
                return;
            }

            m_nSend = new NetAsynSend(m_socketClient);
            m_nRecv = new NetAsynRecv(m_socketClient);

            IPEndPoint serverIP = new IPEndPoint(ipAddres, m_port);
            m_socketClient.BeginConnect(serverIP, ConnSucc, null);
        }

        private void ConnSucc(IAsyncResult ar)
        {
            Debug.Log("大厅网络-连接回调: ip: " + m_ip + " port: " + m_port);
            try
            {
                m_socketClient.EndConnect(ar);
                m_netState = NetState.ConnSucc;
            }
            catch (Exception e)
            {
                Debug.Log("大厅网络-客户端本地，连接异常, ip:" + m_ip + " port:" + m_port + " " + e);
                m_netState = NetState.ConnFail;
                return;
            }
            m_nSend.Start();
            m_nRecv.Start();
            Debug.Log("大厅网络-连接成功: ip: " + m_ip + " port: " + m_port);
        }

        public void Stop()
        {
            if (m_nSend != null)
            {
                m_nSend.Stop();
                m_nSend = null;
            }
            if (m_nRecv != null)
            {
                m_nRecv.Stop();
                m_nRecv = null;
            }
            if (m_socketClient != null)
            {
                m_socketClient.Close();
                m_socketClient = null;
            }
        }

        public override void Update(float fTime, float fDTime)
        {
            if (m_netState == NetState.Connecting || m_netState == NetState.ConnSucc)
            {
                _UpdateCheckHeartBeat();
            }
            if (m_netState == NetState.ConnSucc)
            {
                if (m_connectedCb != null)
                {
                    m_connectedCb();
                    m_connectedCb = null;
                }
                _UpdateReceive();
                _UpdateSend();
                _SendHeartBeat();
                _UpdateOffline();
            }
            if(m_netState == NetState.ConnFail)
            {
                //Stop();
                // 连接失败弹出对话框
                // LoginModule login = (LoginModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelLogin);
                // if (!login.AutoConnect())
                // {
                //     login.OpenReConnDialog();
                // }
            }
        }

        public  NetworkReachability m_netType;
		
		public bool IsConnected()
        {
            return m_netState == NetState.ConnSucc;
        }
		
        public void Lua_SendMessage(int luaId, int len, LusuoStream stream)
        {
            // LuaNetMessage_Send msg = LuaNetMessage_Send.Create(luaId, len, stream);
            // SendMessage(msg);
        }

        public void SendMessage(NetMessage msg)
        {
            if (!IsConnected())
            {
                return;
            }
            m_nSend.SendMessage(msg);
        }

        private void _UpdateSend()
        {
            m_nSend.Update();
        }

        private void _UpdateReceive() 
		{
            m_nRecv.Update();
		}

        private void _SendHeartBeat()
        {

        }

        public void RecvHeartBeatTime()
        {
            m_recvHeartBeatTime = 0;
        }

        public void _UpdateCheckHeartBeat()
        {

        }

        private void _UpdateOffline()
        {
            m_offlineCurTime += FSPParam.clientFrameMsTime;
            if (m_offlineCurTime > m_offlineMaxTime)
            {
                m_offlineCurTime = 0;
                if (m_socketClient==null || !m_socketClient.Connected)
                {
                    Debug.Log("大厅网络-本地通过m_socketClient.Connected检测离线，转为ConnFail");
                    m_netState = NetState.ConnFail;
                }
            }
        }


        public NetState GetState()
        {
            return m_netState;
        }

        public void SetState(NetState state)
        {
              m_netState = state;
        }

        public static FspNetRunTime Inst;

        private string m_ip;
        private int m_port;
        private Socket m_socketClient;
        private NetAsynSend m_nSend;
        private NetAsynRecv m_nRecv;
        private Action m_connectedCb;
        public enum NetState
        {
            Disconnected,   // 未连接，只表示游戏开始没启动连接的状态
            Connecting,
            ConnSucc,
            ConnFail,
            Queue,
        }
        private NetState m_netState = NetState.Disconnected;

        private float m_hbCurTime;
        private float m_hbMaxTime = 2000;

        //private CLGS_CorrectTime m_clgsHeartBeatMsg;
        private float m_offlineCurTime;
        private float m_offlineMaxTime = 1000;
        private int m_recvHeartBeatTime = 0;
    }
}
