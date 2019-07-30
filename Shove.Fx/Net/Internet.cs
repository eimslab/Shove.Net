using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Management;

namespace Shove._Net
{
    /// <summary>
    /// 联网状态、ADSL自动拨号，断开等工具。
    /// </summary>
    public class Internet
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Internet()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;
        [System.Runtime.InteropServices.DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

        /// <summary>
        /// 获取 Internet 连接状态
        /// </summary>
        /// <returns></returns>
        public static int GetInternetConnectedState()
        {
            //reutrn 0 -- 未联网
            //		 1 -- ADSL\Modom
            //		 2 -- 网卡
            //		 -1 -- 未知
            System.Int32 dwFlag = new int();
            if (!InternetGetConnectedState(ref dwFlag, 0))
                return 0;
            else if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
                return 1;
            else if ((dwFlag & INTERNET_CONNECTION_LAN) != 0)
                return 2;
            else
                return -1;
        }

        /// <summary>
        /// RasManager 类
        /// </summary>
        public class RasManager
        {
            private const int RAS_MaxEntryName = 256;
            private const int RAS_MaxPhoneNumber = 128;
            private const int UNLEN = 256;
            private const int PWLEN = 256;
            private const int DNLEN = 15;
            private const int MAX_PATH = 260;
            private const int RAS_MaxDeviceType = 16;
            private const int RAS_MaxCallbackNumber = RAS_MaxPhoneNumber;
            private const int INTERNET_RAS_INSTALLED = 0x10;
            private const int RAS_MaxDeviceName = 128;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="unMsg"></param>
            /// <param name="rasconnstate"></param>
            /// <param name="dwError"></param>
            public delegate void Callback(uint unMsg, int rasconnstate, int dwError);

            /// <summary>
            /// 
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
            public struct RASDIALPARAMS
            {
                /// <summary>
                /// 
                /// </summary>
                public int dwSize;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
                public string szEntryName;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxPhoneNumber + 1)]
                public string szPhoneNumber;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxCallbackNumber + 1)]
                public string szCallbackNumber;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UNLEN + 1)]
                public string szUserName;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PWLEN + 1)]
                public string szPassword;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DNLEN + 1)]
                public string szDomain;
                /// <summary>
                /// 
                /// </summary>
                public int dwSubEntry;
                /// <summary>
                /// 
                /// </summary>
                public int dwCallbackId;
            }

            /// <summary>
            /// 
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct RASCONN
            {
                /// <summary>
                /// 
                /// </summary>
                public int dwSize;
                /// <summary>
                /// 
                /// </summary>
                public IntPtr hrasconn;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
                public string szEntryName;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceType + 1)]
                public string szDeviceType;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceName + 1)]
                public string szDeviceName;
                /// <summary>
                /// 
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string szPhonebook;
                /// <summary>
                /// 
                /// </summary>
                public int dwSubEntry;
            }

            [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
            private static extern int RasDial(int lpRasDialExtensions, string lpszPhonebook, ref RASDIALPARAMS lprasdialparams, int dwNotifierType, Callback lpvNotifier, ref int lphRasConn);

            [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
            private static extern int RasEnumConnections([In, Out] RASCONN[] lprasconn, ref int lpcb, ref int lpcConnections);

            [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
            private static extern int RasHangUp(IntPtr hrasconn);

            private RASDIALPARAMS RasDialParams;
            private int Connection;

            /// <summary>
            /// 构造
            /// </summary>
            public RasManager()
            {
                Connection = 0;
                RasDialParams = new RASDIALPARAMS();
                RasDialParams.dwSize = Marshal.SizeOf(RasDialParams);
            }

            #region Properties

            /// <summary>
            /// 拨号用户名
            /// </summary>
            public string UserName
            {
                get
                {
                    return RasDialParams.szUserName;
                }
                set
                {
                    RasDialParams.szUserName = value;
                }
            }

            /// <summary>
            /// 拨号密码
            /// </summary>
            public string Password
            {
                get
                {
                    return RasDialParams.szPassword;
                }
                set
                {
                    RasDialParams.szPassword = value;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string EntryName
            {
                get
                {
                    return RasDialParams.szEntryName;
                }
                set
                {
                    RasDialParams.szEntryName = value;
                }
            }
            #endregion

            /// <summary>
            /// ADSL 拨号
            /// </summary>
            /// <returns></returns>
            public int Connect()
            {
                Callback rasDialFunc = new Callback(RasManager.RasDialFunc);
                RasDialParams.szEntryName += "\0";
                RasDialParams.szUserName += "\0";
                RasDialParams.szPassword += "\0";
                int result = RasDial(0, null, ref RasDialParams, 0, rasDialFunc, ref 
					Connection);
                return result;
            }

            /// <summary>
            /// 断开所有链接
            /// </summary>
            /// <returns></returns>
            public int HangAllConnection()
            {
                int flags = 0;
                InternetGetConnectedState(ref flags, 0);
                if (!((flags & INTERNET_RAS_INSTALLED) == INTERNET_RAS_INSTALLED))
                    throw new NotSupportedException();

                //create array of structures to pass to API
                int ret;
                int conns = 0;
                RASCONN[] rarr = new RASCONN[256];
                rarr.Initialize();
                rarr[0].dwSize = Marshal.SizeOf(typeof(RASCONN));
                int lr = rarr[0].dwSize * rarr.Length;

                //call RasEnumConnections to loop all RAS connections
                ret = RasEnumConnections(rarr, ref lr, ref conns);
                if (ret != 0) throw new Win32Exception(ret);
                //loop through each RASCONN struct
                for (int i = 0; i < conns; i++)
                {
                    //retrieve RASCONN struct
                    RASCONN r = rarr[i];

                    //if connection bad, handle will be 0
                    if (r.hrasconn == IntPtr.Zero) continue;
                    return RasHangUp(r.hrasconn);
                }

                return -1;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="unMsg"></param>
            /// <param name="rasconnstate"></param>
            /// <param name="dwError"></param>
            public static void RasDialFunc(uint unMsg, int rasconnstate, int dwError)
            {
            }
        }

        /// <summary>
        /// 设置活动网络的 DNS
        /// </summary>
        /// <param name="dns1"></param>
        /// <param name="dns2"></param>
        public void SetNetworkAdapterDNS(string dns1, string dns2)
        {
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"])
                {
                    continue;
                }

                inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                inPar["DNSServerSearchOrder"] = new string[] { dns1, dns2 };
                outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);

                break;
            }
        }
    }
}