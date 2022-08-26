using AxibugTransfer.Tcp;
using AxibugTransfer.Config;

if (!Config.LoadConfig(out List<CfgInfo> cfgList))
{
    Console.WriteLine("请检查配置文件!");
    return;
}

foreach (CfgInfo cfg in cfgList)
    new TcpTransfer(cfg.localPort, cfg.targetIP, cfg.targetPort, true).Start();

while (true)
    Console.ReadLine();