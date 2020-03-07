using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RankChanger
{
    class Program
    {
        static bool pathResolved = false;
        static string clientPath = "";
        static string riotPath = "";
        static ClientApi client;
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (
Object obj, X509Certificate certificate, X509Chain chain,
SslPolicyErrors errors)
            {
                return (true);
            };
            Console.Title = "Wannabe Challenger || https://github.com/nullpntrEx";
            CheckForRiotClient();
            UpdateSystemYaml();
            AwaitClient();
            LockFileParser();
            Console.WriteLine("1 = Challenger || 2 = GrandMaster || 3 = Master || 4 = Diamond || 5 = Platinum");
            var inputKey = Console.ReadKey();
            switch (inputKey.Key)
            {
                case ConsoleKey.D1:
                    client.UpdateRanking("CHALLENGER");
                    break;
                case ConsoleKey.D2:
                    client.UpdateRanking("GRANDMASTER");
                    break;
                case ConsoleKey.D3:
                    client.UpdateRanking("MASTER");
                    break;
                case ConsoleKey.D4:
                    client.UpdateRanking("DIAMOND");
                    break;
                case ConsoleKey.D5:
                    client.UpdateRanking("PLATINUM");
                    break;
                default:
                    client.UpdateRanking("CHALLENGER");
                    break;
            }
            Console.WriteLine("");
            Console.WriteLine("Updated Rank || https://github.com/nullpntrEx");
            Console.ReadKey();

        }
        static void CheckForRiotClient()
        {
            while (!pathResolved)
            {
                Process[] pList = Process.GetProcessesByName("RiotClientUx");
                if (pList.Length > 0)
                {
                    Process p = pList[0];
                    riotPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(p.MainModule.FileName), ".."));
                    clientPath = Path.GetFullPath(Path.Combine(riotPath, "..\\League of Legends"));
                    pathResolved = true;
                }
                else
                {
                    pList = Process.GetProcessesByName("LeagueClientUx");
                    if (pList.Length > 0)
                    {
                        Process p = pList[0];
                        clientPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(p.MainModule.FileName)));
                        riotPath = Path.GetFullPath(Path.Combine(riotPath, "..\\Riot Client"));
                        pathResolved = true;
                    }
                    else
                    {
                        Console.WriteLine("RiotClientUx & LeagueClientUx Not Found || Do you have league of legends open? ");
                        Thread.Sleep(3000);
                    }
                }
            }
        }
        static void UpdateSystemYaml()
        {
            string sysyaml = File.ReadAllText(Path.Combine(clientPath, "system.yaml"));
            if (!sysyaml.Contains("enable_swagger: true"))
            {
                File.AppendAllText(Path.Combine(clientPath, "system.yaml"), "\nenable_swagger: true");
            }
        }
        static void LockFileParser()
        {
            string[] lockFile = new string[] { };
            while (true)
            {
                if (File.Exists(Path.Combine(clientPath, "lockfile")))
                {
                    break;
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            StreamReader sr = new StreamReader(new FileStream(Path.Combine(clientPath, "lockfile"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            lockFile = sr.ReadToEnd().Split(':');
            client = new ClientApi(lockFile[3], int.Parse(lockFile[2]), lockFile[4]);
        }
        static void AwaitClient()
        {
            while (true)
            {
                if (Process.GetProcessesByName("LeagueClient").Length > 0)
                {
                    return;
                }
            }
        }
    }
}
