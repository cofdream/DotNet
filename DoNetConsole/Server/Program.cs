using Cofdream.DotNetLibrary;
using Cofdream.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{

    [System.Serializable]
    public class ProcessInfo
    {
        public string ProcessName;
        public int ProcessId;
        public string CommandLine;
    }


    class Program
    {

        static void Main(string[] args)
        {
            var infos = DotNetUtility.GetInfo();


            //var server = new TCPServer();
            //server.Init();
            //Console.WriteLine("Server Init Done.");

            List<ProcessInfo> processInfos = new List<ProcessInfo>(infos.Count);

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in infos)
            {
                //server.Send($"ProcessName; {item.Process.ProcessName} CommandLine:{item.CommandLine}");
                processInfos.Add(new ProcessInfo()
                {
                    ProcessName = item.Process.ProcessName,
                    ProcessId = item.Process.Id,
                    CommandLine = item.CommandLine
                });
            }

            foreach (var item in processInfos)
            {
                stringBuilder.AppendLine(item.ProcessName);
                stringBuilder.AppendLine(item.ProcessId.ToString());
                stringBuilder.AppendLine(item.CommandLine);

            }

            string path = @"C:\Users\chen\Desktop\Info\info.json";

            if (File.Exists(path) == false)
            {
                File.Create(path);
            }

            File.WriteAllText(path, stringBuilder.ToString());

            Console.WriteLine("Done.");
        }
    }
}