using System;
using System.Diagnostics;
using System.DirectoryServices;

namespace Shove._System
{
    /// <summary>
    /// Processes 的摘要说明。
    /// </summary>
    public class Processes
    {
        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="ProcessesName"></param>
        public static void KillProcesses(string ProcessesName)
        {
            Process[] processes = Process.GetProcessesByName(ProcessesName);
            foreach (Process p in processes)
                p.Kill();
        }

        /// <summary>
        /// 执行控制台命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string ExecuteConsoleCommand(string cmd)
        {
            Process p = new Process();

            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            try
            {
                p.Start();
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.WriteLine("exit");
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();

                return output;
            }
            catch
            {
                return "";
            }
        }

        #region 操作系统服务

        /// <summary>
        /// 获取系统某个服务控制器
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public static System.ServiceProcess.ServiceController GetService(string ServiceName)
        {
            System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();

            foreach (System.ServiceProcess.ServiceController sc in services)
            {
                if (String.Compare(sc.ServiceName, ServiceName, true) == 0)
                {
                    return sc;
                }
            }

            return null;
        }

        /// <summary>
        /// 停止系统服务
        /// </summary>
        /// <param name="ServiceName"></param>
        public static void StopService(string ServiceName)
        {
            System.ServiceProcess.ServiceController sc = GetService(ServiceName);

            if (sc != null)
            {
                StopService(sc);
            }
        }

        /// <summary>
        /// 停止系统服务
        /// </summary>
        /// <param name="sc"></param>
        public static void StopService(System.ServiceProcess.ServiceController sc)
        {
            if (sc.CanStop)
            {
                sc.Stop();
                sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);
            }
        }

        /// <summary>
        /// 启动系统服务
        /// </summary>
        /// <param name="ServiceName"></param>
        public static void StartService(string ServiceName)
        {
            System.ServiceProcess.ServiceController sc = GetService(ServiceName);

            if (sc != null)
            {
                StartService(sc);
            }
        }

        /// <summary>
        /// 启动系统服务
        /// </summary>
        /// <param name="sc"></param>
        public static void StartService(System.ServiceProcess.ServiceController sc)
        {
            if ((sc.Status != System.ServiceProcess.ServiceControllerStatus.Running) && (sc.Status != System.ServiceProcess.ServiceControllerStatus.StartPending))
            {
                sc.Start();
                sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
            }
        }

        #endregion
    }
}
