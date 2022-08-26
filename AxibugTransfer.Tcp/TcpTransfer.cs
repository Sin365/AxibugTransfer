using System.Net.Sockets;

namespace AxibugTransfer.Tcp
{
    public struct transferInfo
    {
        public int cfgIndex; 
        public int clientTransferIndex;
        public TcpClient client1;
        public TcpClient client2;
        public TansferDir tansferDir;
        public bool bNeedStatistics;
    }
    public enum TansferDir
    {
        Local2Target,
        Target2Local,
    }

    public class TcpTransfer
    {
        #region 静态管理
        public static Dictionary<int, TcpTransfer> _DictTransInstance { get; private set; } = new Dictionary<int, TcpTransfer>();
        public static Dictionary<int, (transferInfo, transferInfo)> _DictClientTancefer { get; private set; } = new Dictionary<int, (transferInfo, transferInfo)>();
        public static int InstanceIndex = 0;
        public static int ClientTransferIndex = 0;
        public static void AddSendDataLenght(int index, long lenght)
        {
            if (_DictTransInstance.ContainsKey(index)) _DictTransInstance[index].m_SendDataLenght += lenght;
        }

        public static void AddReceiveDataLenght(int index, long lenght)
        {
            if (_DictTransInstance.ContainsKey(index)) _DictTransInstance[index].m_ReceiveDataLenght += lenght;
        }

        public static void AddClientTancefer(int Index, transferInfo totargetTansfer, transferInfo tolocalTansfer)
        {
            _DictClientTancefer[Index] = (tolocalTansfer, totargetTansfer);
        }
        public static void RemoveClientTancefer(int Index)
        {
            if(_DictClientTancefer.ContainsKey(Index))
                _DictClientTancefer.Remove(Index);
        }

        #endregion

        public TcpTransfer(int localPort, string targetIP, int targetPort, bool needStatistics = false)
        {
            m_InstanceIndex = InstanceIndex++;
            m_LocalPort = localPort;
            m_targetIP = targetIP;
            m_targetPort = targetPort;
            _DictTransInstance[m_InstanceIndex] = this;

            Console.WriteLine($"实例{m_InstanceIndex}初始化,localPort {localPort},targetIP {targetIP},targetPort {targetPort},needStatistics {needStatistics}");
        }

        ~TcpTransfer()
        {
            _DictTransInstance.Remove(m_InstanceIndex);
        }

        int m_InstanceIndex = 0;
        long m_SendDataLenght = 0;
        long m_ReceiveDataLenght = 0;
        int m_LocalPort;
        string m_targetIP;
        int m_targetPort;
        bool m_bNeedStatistics = false;

        public void Start()
        {


            System.ComponentModel.BackgroundWorker hunterwork = new System.ComponentModel.BackgroundWorker();
            hunterwork.DoWork += delegate (object sender, System.ComponentModel.DoWorkEventArgs e)
            {
               StartListener();
            };
            hunterwork.RunWorkerAsync();

        }

        private void StartListener()
        {
            TcpListener tl = new TcpListener(m_LocalPort);
            tl.Start();
            while (true)
            {
                try
                {
                    TcpClient tc1 = tl.AcceptTcpClient();
                    TcpClient tc2 = new TcpClient(m_targetIP, m_targetPort);
                    tc1.SendTimeout = 30000;
                    tc1.ReceiveTimeout = 30000;
                    tc2.SendTimeout = 30000;
                    tc2.ReceiveTimeout = 30000;

                    transferInfo obj1 = new transferInfo() { client1 = tc1, client2 = tc2, cfgIndex = m_InstanceIndex, tansferDir = TansferDir.Local2Target, bNeedStatistics = m_bNeedStatistics };
                    transferInfo obj2 = new transferInfo() { client1 = tc2, client2 = tc1, cfgIndex = m_InstanceIndex, tansferDir = TansferDir.Target2Local, bNeedStatistics = m_bNeedStatistics };

                    AddClientTancefer(ClientTransferIndex++, obj1, obj2);

                    ThreadPool.QueueUserWorkItem(new WaitCallback(tcp_transfer), (object)obj1);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(tcp_transfer), (object)obj2);
                    Console.WriteLine($"实例{m_InstanceIndex} 建立新的连接 {ClientTransferIndex},{tc1.Client.AddressFamily.ToString()}");
                }
                catch { }
            }
        }

        public static void tcp_transfer(object obj)
        {
            transferInfo info = (transferInfo)obj;
            NetworkStream ns1 = info.client1.GetStream();
            NetworkStream ns2 = info.client2.GetStream();
            while (true)
            {
                try
                {
                    byte[] bt = new byte[10240];
                    int count = ns1.Read(bt, 0, bt.Length);
                    ns2.Write(bt, 0, count);

                    if (!info.bNeedStatistics) continue;
                    //统计
                    if (info.tansferDir == TansferDir.Local2Target)
                        TcpTransfer.AddSendDataLenght(info.cfgIndex, count);
                    else
                        TcpTransfer.AddReceiveDataLenght(info.cfgIndex, count);
                }
                catch
                {
                    ns1.Dispose();
                    ns2.Dispose();
                    info.client1.Close();
                    info.client2.Close();
                    break;
                }
            }

            TcpTransfer.RemoveClientTancefer(info.clientTransferIndex);
        }
    }
}