using System.Text;

namespace AxibugTransfer.Config
{
    public struct CfgInfo
    {
        public int localPort;
        public string targetIP;
        public int targetPort;
    }

    public static class Config
    {
        public static bool LoadConfig(out List<CfgInfo> cfgList)
        {
            cfgList = new List<CfgInfo>();
            try
            {
                StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "//config.cfg", Encoding.Default);
                String line;
                while (!string.IsNullOrEmpty((line = sr.ReadLine())))
                {
                    if (!line.Contains(":"))
                        continue;
                    try
                    {
                        CfgInfo cfg = new CfgInfo()
                        {
                            localPort = Convert.ToInt32(line.Split(':')[0].Trim()),
                            targetIP = line.Split(':')[1].Trim(),
                            targetPort = Convert.ToInt32(line.Split(':')[2].Trim())
                        };
                        cfgList.Add(cfg);
                    }
                    catch
                    {
                        continue;
                    }
                }
                sr.Close();

                if (cfgList.Count > 0)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine("配置文件异常："+ex.ToString());
                return false;
            }
        }

    }
}