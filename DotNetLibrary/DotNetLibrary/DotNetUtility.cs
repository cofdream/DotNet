using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Cofdream.DotNetLibrary
{
    public class DotNetUtility
    {
        public static Action<Object> Log;

        public class Info
        {
            public Process Process;
            public string CommandLine;
        }

        public static List<Info> GetInfo()
        {
            List<Info> infos = new List<Info>();

            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (process.ProcessName.Contains("Unity"))
                {
                    var commandLine = DotNetUtility.GetCommandLineArgs(process);

                    Log?.Invoke(commandLine);

                    infos.Add(new Info()
                    {
                        Process = process,
                        CommandLine = commandLine,
                    });

                }
            }
            return infos;
        }


        /// <summary>
        /// 获取一个正在运行的进程的命令行参数。
        /// 与 <see cref="Environment.GetCommandLineArgs"/> 一样，使用此方法获取的参数是包含应用程序路径的。
        /// 关于 <see cref="Environment.GetCommandLineArgs"/> 可参见：
        /// [.NET 命令行参数包含应用程序路径吗？](https://walterlv.com/post/when-will-the-command-line-args-contain-the-executable-path.html)
        /// </summary>
        /// <param name="process">一个正在运行的进程。</param>
        /// <returns>表示应用程序运行命令行参数的字符串。</returns>
        public static string GetCommandLineArgs(Process process)
        {
            if (process is null) throw new ArgumentNullException(nameof(process));

            try
            {
                return GetCommandLineArgsCore(process);
            }
            catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
            {
                // 没有对该进程的安全访问权限。
                Log?.Invoke("没有对该进程的安全访问权限。");
                return string.Empty;
            }
            catch (InvalidOperationException)
            {
                // 进程已退出。
                Log?.Invoke("进程已退出。");
                return string.Empty;
            }
        }

        public static string GetCommandLineArgsCore(Process process)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            {
                if (searcher != null)
                {
                    using (var managementBaseObjects = searcher.Get())
                    {
                        string cmmandLineIndex = "CommandLine";
                        foreach (var managementBaseObject in managementBaseObjects)
                        {
                            if (managementBaseObject != null)
                            {
                                var commandLine = managementBaseObject[cmmandLineIndex] as string;
                                return commandLine;
                            }
                        }

                        //var @object = managementBaseObjects.Cast<ManagementBaseObject>().SingleOrDefault();
                        //return @object?["CommandLine"]?.ToString() ?? "";
                    }
                }
            }

            Log?.Invoke("未获取到命令行信息。");
            return string.Empty;
        }
    }
}