using System;
using System.Management;
using System.Collections.Generic;

namespace Shove._System
{
    /// <summary>
    /// SystemInformation 的摘要说明。
    /// </summary>
    public class SystemInformation
    {
        /// <summary>
        /// 构造
        /// </summary>
        public SystemInformation()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 读取系统信息
        /// </summary>
        /// <param name="sInfoType"></param>
        /// <param name="sWin32_Database"></param>
        /// <returns></returns>
        public static string GetWMIInfo(string sInfoType, string sWin32_Database)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select " + sInfoType + " From " + sWin32_Database);
                string sResult = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sResult = mo[sInfoType].ToString().Trim();
                }
                return sResult;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取主板序列号
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSSerialNumber()
        {
            return GetWMIInfo("SerialNumber", "Win32_BIOS");
            //return GetWMIInfo("SerialNumber", "Win32_BaseBoard");
        }

        /// <summary>
        /// 获取CPU序列号
        /// </summary>
        /// <returns></returns>
        public static string GetCPUSerialNumber()
        {
            return GetWMIInfo("ProcessorId", "Win32_Processor");
        }

        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        /// <returns></returns>
        public static string GetHardDiskSerialNumber()
        {
            return GetWMIInfo("SerialNumber", "Win32_PhysicalMedia");
            //return GetWMIInfo("SerialNumber", "Win32_LogicalDisk");
        }

        /// <summary>
        /// 获取网卡地址
        /// </summary>
        /// <returns></returns>
        public static string GetNetCardMACAddress()
        {
            return GetWMIInfo("MACAddress", "Win32_NetworkAdapter WHERE ((MACAddress Is Not NULL) AND (Manufacturer <> 'Microsoft'))");
            //return GetWMIInfo("MACAddress", "Win32_NetworkAdapterConfiguration");
        }

        /// <summary>
        /// 获取网卡地址，多个
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetNetCardMACAddresss()
        {
            IList<string> Result = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select MACAddress From Win32_NetworkAdapter WHERE ((MACAddress Is Not NULL) AND (Manufacturer <> 'Microsoft'))");

                foreach (ManagementObject mo in searcher.Get())
                {
                    Result.Add(mo["MACAddress"].ToString().Trim());
                }
            }
            catch
            {
                
            }

            return Result;
        }
    }
}
