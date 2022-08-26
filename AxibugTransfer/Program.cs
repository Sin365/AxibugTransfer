using AxibugTransfer.Tcp;
using AxibugTransfer.Config;


Console.WriteLine("AxibugTransfer Ver.0.1  By 皓月");

if (!Config.LoadConfig(out List<CfgInfo> cfgList))
{
    Console.WriteLine("请检查配置文件!");
    Console.ReadLine();
    return;
}

foreach (CfgInfo cfg in cfgList)
    new TcpTransfer(cfg.localPort, cfg.targetIP, cfg.targetPort, true).Start();

while (true)
    Console.ReadLine();