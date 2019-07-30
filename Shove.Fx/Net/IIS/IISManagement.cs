using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.Threading;
using System.Diagnostics;
using IISOle;

namespace Shove._Net.IIS
{
    /// <summary>
    /// IISManagement 的摘要说明。
    /// </summary>
    public class IISManagement
    {
        readonly static object obj_sync = new object();

        #region 获取 IIS 版本

        /// <summary>
        /// 获取 IIS 版本
        /// </summary>
        /// <returns></returns>
        public static int GetIISServerVersion()
        {
            return GetIISServerVersion("localhost");
        }

        /// <summary>
        /// 获取 IIS 版本
        /// </summary>
        /// <param name="MachineName"></param>
        /// <returns></returns>
        public static int GetIISServerVersion(string MachineName)
        {
            DirectoryEntry de = null;

            try
            {
                de = new DirectoryEntry("IIS://" + MachineName + "/W3SVC/INFO");
            }
            catch
            {
                return 0;
            }

            int version = 6;

            try
            {
                version = (int)de.Properties["MajorIISVersionNumber"].Value;
            }
            catch
            {
                version = 5;
            }

            return version;
        }

        #endregion

        #region 获取 IIS 下全部站点、虚拟目录集合

        /// <summary>
        /// 获取本机 IIS 下全部站点、虚拟目录集合
        /// </summary>
        /// <returns></returns>
        public static IISWebServerCollection GetIISWebServers()
        {
            return GetIISWebServers("localhost");
        }

        /// <summary>
        /// 获取指定服务器 IIS 下全部站点、虚拟目录集合，如获取本机，请传入 “localhost”
        /// </summary>
        /// <param name="MachineName"></param>
        /// <returns></returns>
        public static IISWebServerCollection GetIISWebServers(string MachineName)
        {
            IISWebServerCollection result = new IISWebServerCollection();
            DirectoryEntry Service = new DirectoryEntry("IIS://" + MachineName + "/W3SVC");
            IEnumerator ie = Service.Children.GetEnumerator();

            while (ie.MoveNext())
            {
                DirectoryEntry Server = (DirectoryEntry)ie.Current;

                if (Server.SchemaClassName != "IIsWebServer")
                {
                    continue;
                }

                IISWebServer item = new IISWebServer();

                item.Identify = int.Parse(Server.Name);
                item.ServerComment = (string)Server.Properties["ServerComment"][0];
                item.AccessScript = (bool)Server.Properties["AccessScript"][0];
                item.AccessRead = (bool)Server.Properties["AccessRead"][0];
                item.EnableDirBrowsing = (bool)Server.Properties["EnableDirBrowsing"][0];
                item.hostHeader = new HostHeader();
                item.hostHeader.Add(Server);
                item.MaxConnections = (int)Server.Properties["MaxConnections"][0];
                item.MaxBandwidth = (int)Server.Properties["MaxBandwidth"][0] / 1024;

                IEnumerator ieRoot = Server.Children.GetEnumerator();
                DirectoryEntry root = null;
                bool found = false;

                while (ieRoot.MoveNext())
                {
                    root = (DirectoryEntry)ieRoot.Current;

                    if (root.SchemaClassName == "IIsWebVirtualDir")
                    {
                        found = true;

                        break;
                    }
                }

                if (found)
                {
                    item.Path = root.Properties["path"][0].ToString();
                    item.DefaultDoc = (string)root.Properties["DefaultDoc"][0];
                    item.EnableDefaultDoc = (bool)root.Properties["EnableDefaultDoc"][0];
                    item.AppPoolId = (string)root.Properties["AppPoolId"][0];
                    item.DontLog = (bool)root.Properties["DontLog"][0];
                    item.AuthAnonymous = (bool)root.Properties["AuthAnonymous"][0];
                    item.AnonymousPasswordSync = false;
                    item.AnonymousUserName = (string)root.Properties["AnonymousUserName"][0];
                    item.AnonymousUserPass = (string)root.Properties["AnonymousUserPass"][0];
                    item.AnonymousPasswordSync = (bool)root.Properties["AnonymousPasswordSync"][0];
                    item.HttpRedirect = (string)root.Properties["HttpRedirect"][0];
                    item.AppFriendlyName = (string)root.Properties["AppFriendlyName"][0];
                }

                result.Add(item);

                ieRoot = root.Children.GetEnumerator();

                while (ieRoot.MoveNext())
                {
                    DirectoryEntry vd = (DirectoryEntry)ieRoot.Current;

                    if (vd.SchemaClassName != "IIsWebVirtualDir" && vd.SchemaClassName != "IIsWebDirectory")
                    {
                        continue;
                    }

                    IISWebVirtualDir item_vd = new IISWebVirtualDir(item);

                    item_vd.Name = vd.Name;
                    item_vd.AccessRead = (bool)vd.Properties["AccessRead"][0];
                    item_vd.AccessScript = (bool)vd.Properties["AccessScript"][0];
                    item_vd.DefaultDoc = (string)vd.Properties["DefaultDoc"][0];
                    item_vd.EnableDefaultDoc = (bool)vd.Properties["EnableDefaultDoc"][0];

                    if (vd.SchemaClassName == "IIsWebVirtualDir")
                    {
                        item_vd.Path = (string)vd.Properties["Path"][0];
                    }
                    else if (vd.SchemaClassName == "IIsWebDirectory")
                    {
                        item_vd.Path = root.Properties["Path"][0] + @"\" + vd.Name;
                    }

                    item.IISWebVirtualDirs.Add(item_vd);
                }
            }

            return result;
        }

        #endregion

        #region 获取指定的站点

        /// <summary>
        /// 获取本机 IIS 下指定的 IISWebServer
        /// </summary>
        /// <param name="Identify"></param>
        /// <returns></returns>
        public static DirectoryEntry GetIISWebServer(int Identify)
        {
            return GetIISWebServer("localhost", Identify);
        }

        /// <summary>
        /// 获取指定的 IISWebServer
        /// </summary>
        /// <param name="MachineName">查询本机请传入 localhost</param>
        /// <param name="Identify"></param>
        /// <returns></returns>
        public static DirectoryEntry GetIISWebServer(string MachineName, int Identify)
        {
            try
            {
                DirectoryEntry Server = new DirectoryEntry("IIS://" + MachineName + "/W3SVC/" + Identify);

                return Server;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取本机 IIS 下指定的 IISWebServer
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <returns></returns>
        public static DirectoryEntry GetIISWebServer(string ServerComment)
        {
            return GetIISWebServer("localhost", ServerComment);
        }

        /// <summary>
        /// 获取指定的 IISWebServer
        /// </summary>
        /// <param name="MachineName">查询本机请传入 localhost</param>
        /// <param name="ServerComment"></param>
        /// <returns></returns>
        public static DirectoryEntry GetIISWebServer(string MachineName, string ServerComment)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://" + MachineName + "/W3SVC");
            IEnumerator ie = Service.Children.GetEnumerator();

            while (ie.MoveNext())
            {
                DirectoryEntry Server = (DirectoryEntry)ie.Current;

                if (Server.SchemaClassName != "IIsWebServer")
                {
                    continue;
                }

                if (String.Compare(Server.Properties["ServerComment"][0].ToString().ToLower(), ServerComment.Trim(), true) == 0)
                {
                    return Server;
                }
            }

            return null;
        }

        #endregion

        #region 判断指定的 IIS 站点是否存在

        /// <summary>
        /// 判断本机 IIS 站点是否存在
        /// </summary>
        /// <param name="Identify"></param>
        /// <returns></returns>
        public static bool GetIISWebServerExists(int Identify)
        {
            return GetIISWebServerExists("localhost", Identify);
        }

        /// <summary>
        /// 判断指定服务器 IIS 站点是否存在
        /// </summary>
        /// <param name="MachineName">查询本机请传入 localhost</param>
        /// <param name="Identify"></param>
        /// <returns></returns>
        public static bool GetIISWebServerExists(string MachineName, int Identify)
        {
            return (GetIISWebServer(MachineName, Identify) != null);
        }

        /// <summary>
        /// 判断本机 IIS 站点是否存在
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <returns></returns>
        public static bool GetIISWebServerExists(string ServerComment)
        {
            return GetIISWebServerExists("localhost", ServerComment);
        }

        /// <summary>
        /// 判断指定服务器 IIS 站点是否存在
        /// </summary>
        /// <param name="MachineName">查询本机请传入 localhost</param>
        /// <param name="ServerComment"></param>
        /// <returns></returns>
        public static bool GetIISWebServerExists(string MachineName, string ServerComment)
        {
            return (GetIISWebServer(MachineName, ServerComment) != null);
        }

        #endregion

        #region 创建 IIS 站点

        /// <summary>
        /// 创建 IIS 站点
        /// </summary>
        /// <param name="iisws"></param>
        public static void CreateIISWebServer(IISWebServer iisws)
        {
            CreateIISWebServer("localhost", iisws);
        }

        /// <summary>
        /// 创建 IIS 站点
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="iisws"></param>
        public static void CreateIISWebServer(string MachineName, IISWebServer iisws)
        {
            if (String.IsNullOrEmpty(iisws.ServerComment))
            {
                throw new Exception("IISWebServer 的 ServerComment 不能为空。");
            }

            if ((iisws.hostHeader == null) || (iisws.hostHeader.Value.Count < 1))
            {
                throw new Exception("IISWebServer 最少必须有一个主机头信息。");
            }

            DirectoryEntry Service = new DirectoryEntry("IIS://" + MachineName + "/W3SVC");
            DirectoryEntry Server;

            try
            {
                Monitor.Enter(obj_sync);

                IEnumerator ie = Service.Children.GetEnumerator();
                int Identify = 1;

                #region 获取最大的标识号

                while (ie.MoveNext())
                {
                    Server = (DirectoryEntry)ie.Current;

                    if (Server.SchemaClassName != "IIsWebServer")
                    {
                        continue;
                    }

                    int t_Identify = int.Parse(Server.Name);

                    if (t_Identify >= Identify)
                    {
                        Identify = t_Identify + 1;
                    }
                }

                #endregion

                Server = Service.Children.Add(Identify.ToString(), "IIsWebServer");

                Server.Properties["ServerComment"][0] = iisws.ServerComment;
                Server.Properties["Serverbindings"].Add(iisws.hostHeader.Value[0]);
                Server.Properties["AccessScript"][0] = iisws.AccessScript;
                Server.Properties["AccessRead"][0] = iisws.AccessRead;
                Server.Properties["EnableDirBrowsing"][0] = iisws.EnableDirBrowsing;
                Server.Properties["AppPoolId"][0] = iisws.AppPoolId;
                if (iisws.MaxConnections > 0)
                {
                    Server.Properties["MaxConnections"][0] = iisws.MaxConnections.ToString();
                }
                if (iisws.MaxBandwidth > 0)
                {
                    Server.Properties["MaxBandwidth"][0] = (iisws.MaxBandwidth * 1024).ToString();
                }

                Server.CommitChanges();

                DirectoryEntry root = Server.Children.Add("Root", "IIsWebVirtualDir");
                root.Properties["path"][0] = iisws.Path;
                root.Properties["DefaultDoc"][0] = iisws.DefaultDoc;
                root.Properties["EnableDefaultDoc"][0] = iisws.EnableDefaultDoc;

                root.Properties["DontLog"][0] = iisws.DontLog;
                root.Properties["AuthAnonymous"][0] = iisws.AuthAnonymous;
                root.Properties["AnonymousPasswordSync"][0] = iisws.AnonymousPasswordSync;

                if (!iisws.AnonymousPasswordSync)
                {
                    root.Properties["AnonymousUserName"][0] = iisws.AnonymousUserName;
                    root.Properties["AnonymousUserPass"][0] = iisws.AnonymousUserPass;
                }

                if (!string.IsNullOrEmpty(iisws.HttpRedirect))
                {
                    root.Properties["HttpRedirect"][0] = iisws.HttpRedirect;
                }

                root.Properties["AppFriendlyName"][0] = string.IsNullOrEmpty(iisws.AppFriendlyName) ? iisws.ServerComment : iisws.AppFriendlyName; //应用程序名   
                root.Properties["ScriptMaps"].Value = DefaultScriptMaps().ToArray();

                root.CommitChanges();

                System.Threading.Thread.Sleep(500);
                root.Invoke("AppCreate2", new object[1] { 2 });
                //Server.Invoke("start",new object[0]);

                AddHostHeader(Server, iisws.hostHeader);

                iisws.Identify = Identify;
            }
            finally
            {
                Monitor.Exit(obj_sync);
            }
        }

        #region 默认ISAPI筛选
        /// <summary>
        /// 默认ISAPI筛选
        /// </summary>
        /// <returns></returns>
        private static ArrayList DefaultScriptMaps()
        {
            ArrayList list = new ArrayList(55);
            list.Add(@".ad,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".adprototype,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".asa,C:\WINDOWS\system32\inetsrv\asp.dll,5,GET,HEAD,POST,TRACE");
            list.Add(@".asax,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".ascx,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".ashx,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".asmx,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".asp,C:\WINDOWS\system32\inetsrv\asp.dll,5,GET,HEAD,POST,TRACE");
            list.Add(@".aspx,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".axd,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".browser,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".cd,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".cdx,C:\WINDOWS\system32\inetsrv\asp.dll,5,GET,HEAD,POST,TRACE");
            list.Add(@".cer,C:\WINDOWS\system32\inetsrv\asp.dll,5,GET,HEAD,POST,TRACE");
            list.Add(@".compiled,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".config,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".cs,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".csproj,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".css,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".dd,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".exclude,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".idc,C:\WINDOWS\system32\inetsrv\httpodbc.dll,5,GET,POST");
            list.Add(@".java,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".js,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".jsl,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".ldb,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".ldd,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".lddprototype,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".ldf,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".licx,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".master,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".mdb,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".mdf,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".msgx,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".refresh,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".rem,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".resources,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".resx,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".rules,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".sd,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".sdm,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".sdmDocument,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".shtm,C:\WINDOWS\system32\inetsrv\ssinc.dll,5,GET,POST");
            list.Add(@".shtml,C:\WINDOWS\system32\inetsrv\ssinc.dll,5,GET,POST");
            list.Add(@".sitemap,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".skin,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".soap,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".stm,C:\WINDOWS\system32\inetsrv\ssinc.dll,5,GET,POST");
            list.Add(@".svc,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".vb,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".vbproj,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".vjsproj,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".vsdisco,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");
            list.Add(@".webinfo,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,5,GET,HEAD,POST,DEBUG");
            list.Add(@".xoml,C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG");

            return list;
        }
        #endregion

        #endregion

        #region 设置主机头

        /// <summary>
        /// 设置主机头
        /// </summary>
        /// <param name="Identify">站点标识符</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void SetHostHeader(int Identify, HostHeader hostHeader)
        {
            SetHostHeader("localhost", Identify, hostHeader);
        }

        /// <summary>
        /// 设置主机头
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify">站点标识符</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void SetHostHeader(string MachineName, int Identify, HostHeader hostHeader)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            SetHostHeader(Server, hostHeader);
        }

        /// <summary>
        /// 设置主机头
        /// </summary>
        /// <param name="ServerComment">站点名称</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void SetHostHeader(string ServerComment, HostHeader hostHeader)
        {
            SetHostHeader("localhost", ServerComment, hostHeader);
        }

        /// <summary>
        /// 设置主机头
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment">站点名称</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void SetHostHeader(string MachineName, string ServerComment, HostHeader hostHeader)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            SetHostHeader(Server, hostHeader);
        }

        /// <summary>
        /// 设置主机头
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="hostHeader">主机头信息</param>
        public static void SetHostHeader(DirectoryEntry Server, HostHeader hostHeader)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            PropertyValueCollection serverBindings = Server.Properties["ServerBindings"];
            serverBindings.Clear();

            foreach (string header in hostHeader.Value)
            {
                if (!serverBindings.Contains(header))
                {
                    serverBindings.Add(header);
                }
            }

            Server.CommitChanges();
        }

        #endregion

        #region 增加主机头

        /// <summary>
        /// 增加主机头
        /// </summary>
        /// <param name="Identify">站点标识符</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void AddHostHeader(int Identify, HostHeader hostHeader)
        {
            AddHostHeader("localhost", Identify, hostHeader);
        }

        /// <summary>
        /// 增加主机头
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify">站点标识符</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void AddHostHeader(string MachineName, int Identify, HostHeader hostHeader)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            AddHostHeader(Server, hostHeader);
        }

        /// <summary>
        /// 增加主机头
        /// </summary>
        /// <param name="ServerComment">站点名称</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void AddHostHeader(string ServerComment, HostHeader hostHeader)
        {
            AddHostHeader("localhost", ServerComment, hostHeader);
        }

        /// <summary>
        /// 增加主机头
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment">站点名称</param>
        /// <param name="hostHeader">主机头信息</param>
        public static void AddHostHeader(string MachineName, string ServerComment, HostHeader hostHeader)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            AddHostHeader(Server, hostHeader);
        }

        /// <summary>
        /// 增加主机头
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="hostHeader">主机头信息</param>
        public static void AddHostHeader(DirectoryEntry Server, HostHeader hostHeader)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            PropertyValueCollection serverBindings = Server.Properties["ServerBindings"];

            foreach (string header in hostHeader.Value)
            {
                if (!serverBindings.Contains(header))
                {
                    serverBindings.Add(header);
                }
            }

            Server.CommitChanges();
        }

        #endregion

        #region 删除 IIS 站点

        /// <summary>
        /// 删除 IIS 站点
        /// </summary>
        /// <param name="Identify"></param>
        public static void RemoveIISWebServer(int Identify)
        {
            RemoveIISWebServer("localhost", Identify);
        }

        /// <summary>
        /// 删除 IIS 站点
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        public static void RemoveIISWebServer(string MachineName, int Identify)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://" + MachineName + "/W3SVC");
            DirectoryEntry Server = new DirectoryEntry("IIS://" + MachineName + "/W3SVC/" + Identify);

            if (Server == null)
            {
                throw new Exception("IISWebServer “" + Identify + "”不存在。");
            }

            try
            {
                Monitor.Enter(obj_sync);

                Service.Children.Remove(Server);
                Service.CommitChanges();
            }
            finally
            {
                Monitor.Exit(obj_sync);
            }
        }

        /// <summary>
        /// 删除 IIS 站点
        /// </summary>
        /// <param name="ServerComment"></param>
        public static void RemoveIISWebServer(string ServerComment)
        {
            RemoveIISWebServer("localhost", ServerComment);
        }

        /// <summary>
        /// 删除 IIS 站点
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        public static void RemoveIISWebServer(string MachineName, string ServerComment)
        {
            ServerComment = ServerComment.Trim();

            DirectoryEntry Service = new DirectoryEntry("IIS://" + MachineName + "/W3SVC");
            IEnumerator ie = Service.Children.GetEnumerator();
            DirectoryEntry Server = null;

            while (ie.MoveNext())
            {
                DirectoryEntry t_Server = (DirectoryEntry)ie.Current;

                if (t_Server.SchemaClassName != "IIsWebServer")
                {
                    continue;
                }

                if (String.Compare(t_Server.Properties["ServerComment"][0].ToString().Trim(), ServerComment, true) == 0)
                {
                    Server = t_Server;

                    break;
                }
            }

            if (Server == null)
            {
                throw new Exception("站点“" + ServerComment + "”不存在。");
            }

            try
            {
                Monitor.Enter(obj_sync);

                Service.Children.Remove(Server);
                Service.CommitChanges();
            }
            finally
            {
                Monitor.Exit(obj_sync);
            }
        }

        #endregion

        #region 修改 IIS 站点

        /// <summary>
        /// 编辑 IIS 站点信息
        /// </summary>
        /// <param name="iisws"></param>
        public static void EditIISWebServer(IISWebServer iisws)
        {
            EditIISWebServer("localhost", iisws);
        }

        /// <summary>
        /// 编辑 IIS 站点信息
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="iisws"></param>
        public static void EditIISWebServer(string MachineName, IISWebServer iisws)
        {
            if (iisws.hostHeader.Value.Count < 1)
            {
                throw new Exception("IISWebServer 最少必须有一个主机头信息。");
            }

            DirectoryEntry Server = null;

            if (iisws.Identify >= 1)
            {
                Server = GetIISWebServer(MachineName, iisws.Identify);
            }
            else if (String.IsNullOrEmpty(iisws.ServerComment.Trim()))
            {
                Server = GetIISWebServer(MachineName, iisws.ServerComment);
            }

            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            DirectoryEntry Service = Server.Parent;// new DirectoryEntry("IIS://" + MachineName + "/W3SVC");
            IEnumerator ie = Service.Children.GetEnumerator();

            try
            {
                Server.Invoke("stop", new object[0]);

                Monitor.Enter(obj_sync);

                Server.Properties["ServerComment"][0] = iisws.ServerComment;
                Server.Properties["Serverbindings"][0] = iisws.hostHeader.Value[0];
                Server.Properties["AccessScript"][0] = iisws.AccessScript;
                Server.Properties["AccessRead"][0] = iisws.AccessRead;
                Server.Properties["EnableDirBrowsing"][0] = iisws.EnableDirBrowsing;
                Server.Properties["AppPoolId"][0] = iisws.AppPoolId;
                Server.Properties["MaxConnections"][0] = iisws.MaxConnections.ToString();
                Server.Properties["MaxBandwidth"][0] = (iisws.MaxBandwidth * 1024).ToString();

                Server.CommitChanges();

                DirectoryEntry root = getRoot(Server);

                if (root != null)
                {
                    root.Properties["path"][0] = iisws.Path;
                    root.Properties["DefaultDoc"][0] = iisws.DefaultDoc;
                    root.Properties["EnableDefaultDoc"][0] = iisws.EnableDefaultDoc;
                    root.Properties["DontLog"][0] = iisws.DontLog;
                    root.Properties["AuthAnonymous"][0] = iisws.AuthAnonymous;
                    root.Properties["AnonymousPasswordSync"][0] = iisws.AnonymousPasswordSync;

                    if (!iisws.AnonymousPasswordSync)
                    {
                        root.Properties["AnonymousUserName"][0] = iisws.AnonymousUserName;
                        root.Properties["AnonymousUserPass"][0] = iisws.AnonymousUserPass;
                    }

                    if (!string.IsNullOrEmpty(iisws.HttpRedirect))
                    {
                        root.Properties["HttpRedirect"][0] = iisws.HttpRedirect;
                    }

                    root.Properties["AppFriendlyName"][0] = String.IsNullOrEmpty(iisws.AppFriendlyName) ? iisws.ServerComment : iisws.AppFriendlyName; //应用程序名

                    root.CommitChanges();
                }

                AddHostHeader(Server, iisws.hostHeader);

                Thread.Sleep(500);
                Server.Invoke("start", new object[0]);
            }
            finally
            {
                Monitor.Exit(obj_sync);
            }
        }

        #endregion

        #region 编辑 IIS 站点的缺省首页文档

        /// <summary>
        /// 编辑 IIS 站点的缺省首页文档
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="DefaultPageList">由 , 分割，如：index.aspx,default.aspx</param>
        public static void EditIISWebServerDefaultPage(int Identify, string DefaultPageList)
        {
            EditIISWebServerDefaultPage("localhost", Identify, DefaultPageList);
        }

        /// <summary>
        /// 编辑 IIS 站点的缺省首页文档
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <param name="DefaultPageList">由 , 分割，如：index.aspx,default.aspx</param>
        public static void EditIISWebServerDefaultPage(string MachineName, int Identify, string DefaultPageList)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            EditIISWebServerDefaultPage(Server, DefaultPageList);
        }

        /// <summary>
        /// 编辑 IIS 站点的缺省首页文档
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <param name="DefaultPageList">由 , 分割，如：index.aspx,default.aspx</param>
        public static void EditIISWebServerDefaultPage(string ServerComment, string DefaultPageList)
        {
            EditIISWebServerDefaultPage("localhost", ServerComment, DefaultPageList);
        }

        /// <summary>
        /// 编辑 IIS 站点的缺省首页文档
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        /// <param name="DefaultPageList">由 , 分割，如：index.aspx,default.aspx</param>
        public static void EditIISWebServerDefaultPage(string MachineName, string ServerComment, string DefaultPageList)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            EditIISWebServerDefaultPage(Server, DefaultPageList);
        }

        /// <summary>
        /// 编辑 IIS 站点的缺省首页文档
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="DefaultPageList">由 , 分割，如：index.aspx,default.aspx</param>
        public static void EditIISWebServerDefaultPage(DirectoryEntry Server, string DefaultPageList)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            if (String.IsNullOrEmpty(DefaultPageList))
            {
                throw new Exception(DefaultPageList + " 的值不能为空。");
            }

            Server.Properties["DefaultDoc"][0] = DefaultPageList;
            Server.CommitChanges();

            DirectoryEntry root = getRoot(Server);

            root.Properties["DefaultDoc"][0] = DefaultPageList;
            root.CommitChanges();
        }

        #endregion

        #region 编辑 IIS 站点的 Mime 头信息

        /// <summary>
        /// 编辑 IIS 站点的 Mime 头信息
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="MimeMapList">Mime 头列表，用","分隔</param>
        public static void EditIISWebServerMime(int Identify, string MimeMapList)
        {
            EditIISWebServerMime("localhost", Identify, MimeMapList);
        }

        /// <summary>
        /// 编辑 IIS 站点的 Mime 头信息
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <param name="MimeMapList">Mime 头列表，用","分隔</param>
        public static void EditIISWebServerMime(string MachineName, int Identify, string MimeMapList)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            EditIISWebServerMime(Server, MimeMapList);
        }

        /// <summary>
        /// 编辑 IIS 站点的 Mime 头信息
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <param name="MimeMapList">Mime 头列表，用","分隔</param>
        public static void EditIISWebServerMime(string ServerComment, string MimeMapList)
        {
            EditIISWebServerMime("localhost", ServerComment, MimeMapList);
        }

        /// <summary>
        /// 编辑 IIS 站点的 Mime 头信息
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        /// <param name="MimeMapList">Mime 头列表，用","分隔</param>
        public static void EditIISWebServerMime(string MachineName, string ServerComment, string MimeMapList)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            EditIISWebServerMime(Server, MimeMapList);
        }

        /// <summary>
        /// 编辑 IIS 站点的 Mime 头信息
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="MimeMapList">Mime 头列表，用","分隔</param>
        public static void EditIISWebServerMime(DirectoryEntry Server, string MimeMapList)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            if (String.IsNullOrEmpty(MimeMapList))
            {
                MimeMapList = "";
            }

            MimeMapList = MimeMapList.Trim();

            DirectoryEntry root = getRoot(Server);

            // 先执行清除
            root.Properties["MimeMap"].Clear();

            // 如果传入了 mime，则设置为新的 mime 信息
            if (!String.IsNullOrEmpty(MimeMapList))
            {
                string[] mimes = MimeMapList.Split(' ');

                foreach (string mime in mimes)
                {
                    string[] str = mime.Split(',');

                    MimeMapClass mimeInfo = new MimeMapClass();
                    mimeInfo.Extension = str[0];
                    mimeInfo.MimeType = str[1];
                    root.Properties["MimeMap"].Add(mimeInfo);
                }
            }

            root.CommitChanges();
        }

        #endregion

        #region 编辑 IIS 站点的 .NET 框架版本

        /// <summary>
        /// 编辑 IIS 站点的 .NET 框架版本
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="NetExecuteFileNameOfVersion">如：C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe、C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe</param>
        public static void EditIISWebServerNetFrameVersion(int Identify, string NetExecuteFileNameOfVersion)
        {
            EditIISWebServerNetFrameVersion("localhost", Identify, NetExecuteFileNameOfVersion);
        }

        /// <summary>
        /// 编辑 IIS 站点的 .NET 框架版本
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <param name="NetExecuteFileNameOfVersion">如：C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe、C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe</param>
        public static void EditIISWebServerNetFrameVersion(string MachineName, int Identify, string NetExecuteFileNameOfVersion)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            EditIISWebServerNetFrameVersion(Server, NetExecuteFileNameOfVersion);
        }

        /// <summary>
        /// 编辑 IIS 站点的 .NET 框架版本
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <param name="NetExecuteFileNameOfVersion">如：C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe、C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe</param>
        public static void EditIISWebServerNetFrameVersion(string ServerComment, string NetExecuteFileNameOfVersion)
        {
            EditIISWebServerNetFrameVersion("localhost", ServerComment, NetExecuteFileNameOfVersion);
        }

        /// <summary>
        /// 编辑 IIS 站点的 .NET 框架版本
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        /// <param name="NetExecuteFileNameOfVersion">如：C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe、C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe</param>
        public static void EditIISWebServerNetFrameVersion(string MachineName, string ServerComment, string NetExecuteFileNameOfVersion)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            EditIISWebServerNetFrameVersion(Server, NetExecuteFileNameOfVersion);
        }

        /// <summary>
        /// 编辑 IIS 站点的 .NET 框架版本
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="NetExecuteFileNameOfVersion">如：C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe、C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe</param>
        public static void EditIISWebServerNetFrameVersion(DirectoryEntry Server, string NetExecuteFileNameOfVersion)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            if (!System.IO.File.Exists(NetExecuteFileNameOfVersion))
            {
                throw new Exception(".NET 框架执行文件不存在。");
            }

            ProcessStartInfo pi = new ProcessStartInfo(NetExecuteFileNameOfVersion);

            pi.Arguments = " -norestart -s W3SVC/" + Server.Name;
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;

            Process process = new Process();
            process.StartInfo = pi;
            process.Start();
            process.WaitForExit();
        }

        #endregion

        #region 编辑 IIS 站点的 404 页面

        /// <summary>
        /// 编辑 IIS 站点的 404 页面
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="Page"></param>
        public static void EditIISWebServer404Page(int Identify, string Page)
        {
            EditIISWebServer404Page("localhost", Identify, Page);
        }

        /// <summary>
        /// 编辑 IIS 站点的 404 页面
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <param name="Page"></param>
        public static void EditIISWebServer404Page(string MachineName, int Identify, string Page)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            EditIISWebServer404Page(Server, Page);
        }

        /// <summary>
        /// 编辑 IIS 站点的 404 页面
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <param name="Page"></param>
        public static void EditIISWebServer404Page(string ServerComment, string Page)
        {
            EditIISWebServer404Page("localhost", ServerComment, Page);
        }

        /// <summary>
        /// 编辑 IIS 站点的 404 页面
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        /// <param name="Page"></param>
        public static void EditIISWebServer404Page(string MachineName, string ServerComment, string Page)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            EditIISWebServer404Page(Server, Page);
        }

        /// <summary>
        /// 编辑 IIS 站点的 404 页面
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Page"></param>
        public static void EditIISWebServer404Page(DirectoryEntry Server, string Page)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            if (String.IsNullOrEmpty(Page))
            {
                throw new Exception(Page + " 的值不能为空。");
            }

            ArrayList pages = new ArrayList();

            #region 填充 pages

            object[] oldPages = (object[])Server.Properties["HttpErrors"].Value;

            foreach (string str in oldPages)
            {
                if (!str.StartsWith("404,*"))
                {
                    pages.Add(str);
                }
                else
                {
                    pages.Add("404,*,FILE," + Page);
                }
            }

            #endregion

            Server.Properties["HttpErrors"].Clear();
            Server.Properties["HttpErrors"].Value = pages.ToArray();
            Server.CommitChanges();

            DirectoryEntry root = getRoot(Server);

            root.Properties["HttpErrors"].Clear();
            root.Properties["HttpErrors"].Value = pages.ToArray();
            root.CommitChanges();
        }

        #endregion

        #region 编辑 IIS 站点的文件路径

        /// <summary>
        /// 编辑 IIS 站点的文件路径
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="path"></param>
        public static void EditIISWebServerPath(int Identify, string path)
        {
            EditIISWebServerPath("localhost", Identify, path);
        }

        /// <summary>
        /// 编辑 IIS 站点的文件路径
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <param name="path"></param>
        public static void EditIISWebServerPath(string MachineName, int Identify, string path)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            EditIISWebServerPath(Server, path);
        }

        /// <summary>
        /// 编辑 IIS 站点的文件路径
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <param name="path"></param>
        public static void EditIISWebServerPath(string ServerComment, string path)
        {
            EditIISWebServerPath("localhost", ServerComment, path);
        }

        /// <summary>
        /// 编辑 IIS 站点的文件路径
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        /// <param name="path"></param>
        public static void EditIISWebServerPath(string MachineName, string ServerComment, string path)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            EditIISWebServerPath(Server, path);
        }

        /// <summary>
        /// 编辑 IIS 站点的文件路径
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="path"></param>
        public static void EditIISWebServerPath(DirectoryEntry Server, string path)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            if (String.IsNullOrEmpty(path))
            {
                throw new Exception(path + " 的值不能为空。");
            }

            DirectoryEntry root = getRoot(Server);

            root.Properties["Path"][0] = path;
            root.CommitChanges();
        }

        #endregion


        #region 创建虚拟目录

        /// <summary>
        /// 创建虚拟目录
        /// </summary>
        /// <param name="iisvd"></param>
        /// <param name="deleteExists"></param>
        public static void CreateIISWebVirtualDir(IISWebVirtualDir iisvd, bool deleteExists)
        {
            CreateIISWebVirtualDir("localhost", iisvd, deleteExists);
        }

        /// <summary>
        /// 创建虚拟目录
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="iisvd"></param>
        /// <param name="deleteExists"></param>
        public static void CreateIISWebVirtualDir(string MachineName, IISWebVirtualDir iisvd, bool deleteExists)
        {
            if ((iisvd.Parent == null) && (iisvd.ParentIdentify < 1) && (String.IsNullOrEmpty(iisvd.ParentServerComment)))
            {
                throw new Exception("IISWebVirtualDir 没有所属的 IISWebServer，或者其 ParentIdentify 或 ParentServerComment 属性的值。");
            }

            DirectoryEntry Service = new DirectoryEntry("IIS://" + MachineName + "/W3SVC");
            DirectoryEntry Server = null;

            if (iisvd.Parent != null)
            {
                Server = GetIISWebServer(MachineName, iisvd.Parent.Identify);
            }
            else if (iisvd.ParentIdentify > 0)
            {
                Server = GetIISWebServer(MachineName, iisvd.ParentIdentify);
            }
            else if (!String.IsNullOrEmpty(iisvd.ParentServerComment))
            {
                Server = GetIISWebServer(MachineName, iisvd.ParentServerComment);
            }

            if (Server == null)
            {
                throw new Exception("父站点不存在。");
            }

            DirectoryEntry root = getRoot(Server);

            try
            {
                Monitor.Enter(obj_sync);

                if (deleteExists)
                {
                    foreach (DirectoryEntry vd in root.Children)
                    {
                        if (String.Compare(vd.Name.Trim(), iisvd.Name.Trim(), true) == 0)
                        {
                            root.Children.Remove(vd);
                            root.CommitChanges();

                            break;
                        }
                    }
                }

                DirectoryEntry de = root.Children.Add(iisvd.Name, "IIsWebVirtualDir");
                de.Properties["Path"][0] = iisvd.Path;
                de.Properties["DefaultDoc"][0] = iisvd.DefaultDoc;
                de.Properties["EnableDefaultDoc"][0] = iisvd.EnableDefaultDoc;
                de.Properties["AccessScript"][0] = iisvd.AccessScript;
                de.Properties["AccessRead"][0] = iisvd.AccessRead;
                de.Invoke("AppCreate2", new object[1] { 2 });

                root.CommitChanges();
                de.CommitChanges();
            }
            finally
            {
                Monitor.Exit(obj_sync);
            }
        }

        #endregion

        #region 删除虚拟目录

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="VirtualDir"></param>
        public static void RemoveIISWebVirtualDir(int Identify, string VirtualDir)
        {
            RemoveIISWebVirtualDir("localhost", Identify, VirtualDir);
        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <param name="VirtualDir"></param>
        public static void RemoveIISWebVirtualDir(string MachineName, int Identify, string VirtualDir)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            RemoveIISWebVirtualDir(Server, VirtualDir);
        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <param name="VirtualDir"></param>
        public static void RemoveIISWebVirtualDir(string ServerComment, string VirtualDir)
        {
            RemoveIISWebVirtualDir("localhost", ServerComment, VirtualDir);
        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment">站点说明</param>
        /// <param name="VirtualDir">虚拟目录名称</param>
        public static void RemoveIISWebVirtualDir(string MachineName, string ServerComment, string VirtualDir)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            RemoveIISWebVirtualDir(Server, VirtualDir);
        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="iisvd"></param>
        public static void RemoveIISWebVirtualDir(IISWebVirtualDir iisvd)
        {
            RemoveIISWebVirtualDir("localhost", iisvd);
        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="iisvd"></param>
        public static void RemoveIISWebVirtualDir(string MachineName, IISWebVirtualDir iisvd)
        {
            DirectoryEntry Server = null;

            if (iisvd.Parent != null)
            {
                Server = GetIISWebServer(MachineName, iisvd.Parent.Identify);
            }
            else if (iisvd.ParentIdentify > 0)
            {
                Server = GetIISWebServer(MachineName, iisvd.ParentIdentify);
            }
            else if (!String.IsNullOrEmpty(iisvd.ParentServerComment))
            {
                Server = GetIISWebServer(MachineName, iisvd.ParentServerComment);
            }

            RemoveIISWebVirtualDir(Server, iisvd.Name);
        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="VirtualDir">虚拟目录名称</param>
        public static void RemoveIISWebVirtualDir(DirectoryEntry Server, string VirtualDir)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            if (String.IsNullOrEmpty(VirtualDir))
            {
                throw new Exception(VirtualDir + " 的值不能为空。");
            }

            VirtualDir = VirtualDir.ToLower();

            DirectoryEntry root = getRoot(Server);
            bool found = false;

            try
            {
                Monitor.Enter(obj_sync);

                foreach (DirectoryEntry vd in root.Children)
                {
                    if (vd.Name.ToLower().Trim() == VirtualDir)
                    {
                        root.Children.Remove(vd);
                        root.CommitChanges();
                        found = true;

                        break;
                    }
                }
            }
            finally
            {
                Monitor.Exit(obj_sync);
            }

            if (!found)
            {
                throw new Exception("站点“" + Server.Name + "”下不存在“" + VirtualDir + "”这个虚拟目录。");
            }
        }

        #endregion


        #region 获取 IIS 站点的状态

        /// <summary>
        /// 获取 IIS 站点的状态
        /// </summary>
        /// <param name="Identify"></param>
        /// <returns></returns>
        public static IISWebServerState GetIISWebServerState(int Identify)
        {
            return GetIISWebServerState("localhost", Identify);
        }

        /// <summary>
        /// 获取 IIS 站点的状态
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <returns></returns>
        public static IISWebServerState GetIISWebServerState(string MachineName, int Identify)
        {
            DirectoryEntry server = GetIISWebServer(MachineName, Identify);

            return GetIISWebServerState(server);
        }

        /// <summary>
        /// 获取 IIS 站点的状态
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <returns></returns>
        public static IISWebServerState GetIISWebServerState(string ServerComment)
        {
            return GetIISWebServerState("localhost", ServerComment);
        }

        /// <summary>
        /// 获取 IIS 站点的状态
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        /// <returns></returns>
        public static IISWebServerState GetIISWebServerState(string MachineName, string ServerComment)
        {
            DirectoryEntry server = GetIISWebServer(MachineName, ServerComment);

            return GetIISWebServerState(server);
        }

        /// <summary>
        /// 获取 IIS 站点的状态
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static IISWebServerState GetIISWebServerState(DirectoryEntry server)
        {
            if (server == null)
            {
                throw new Exception("IISWebServer 不存在。");
            }

            int state = Shove._Convert.StrToInt(server.Properties["ServerState"][0].ToString(), -1);

            if ((state > 0) && (state <= 7))
            {
                return (IISWebServerState)Enum.Parse(typeof(IISWebServerState), Enum.GetName(typeof(IISWebServerState), state));
            }

            return IISWebServerState.Stopped;
        }

        #endregion

        #region 停止 IIS 站点

        /// <summary>
        /// 停止 IISWebServer
        /// </summary>
        /// <param name="Identify"></param>
        public static void Stop(int Identify)
        {
            Stop("localhost", Identify);
        }

        /// <summary>
        /// 停止 IISWebServer
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        public static void Stop(string MachineName, int Identify)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            Stop(Server);
        }

        /// <summary>
        /// 停止 IISWebServer
        /// </summary>
        /// <param name="ServerComment"></param>
        public static void Stop(string ServerComment)
        {
            Stop("localhost", ServerComment);
        }

        /// <summary>
        /// 停止 IISWebServer
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        public static void Stop(string MachineName, string ServerComment)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            Stop(Server);
        }

        /// <summary>
        /// 停止 IISWebServer
        /// </summary>
        /// <param name="Server"></param>
        public static void Stop(DirectoryEntry Server)
        {
            if (Server == null)
            {
                throw new Exception("IISWebServer 不存在。");
            }

            Server.Invoke("stop", new object[0]);
        }

        #endregion

        #region 启动 IIS 站点

        /// <summary>
        /// 启动 IISWebServer
        /// </summary>
        /// <param name="Identify"></param>
        public static void Start(int Identify)
        {
            Start("localhost", Identify);
        }

        /// <summary>
        /// 启动 IISWebServer
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        public static void Start(string MachineName, int Identify)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            Start(Server);
        }

        /// <summary>
        /// 启动 IISWebServer
        /// </summary>
        /// <param name="ServerComment"></param>
        public static void Start(string ServerComment)
        {
            Start("localhost", ServerComment);
        }

        /// <summary>
        /// 启动 IISWebServer
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        public static void Start(string MachineName, string ServerComment)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            Start(Server);
        }

        /// <summary>
        /// 启动 IISWebServer
        /// </summary>
        /// <param name="Server"></param>
        public static void Start(DirectoryEntry Server)
        {
            if (Server == null)
            {
                throw new Exception("IISWebServer 不存在。");
            }

            Server.Invoke("stop", new object[0]);
            Server.Invoke("start", new object[0]);
        }

        #endregion


        #region 创建应用程序池

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="AppPoolName"></param>
        /// <param name="type">经典、集成</param>
        /// <param name="isStart"></param>
        public static void CreateAppPool(string AppPoolName, AppPoolType type, bool isStart)
        {
            CreateAppPool("localhost", AppPoolName, type, isStart);
        }

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="AppPoolName"></param>
        /// <param name="type">经典、集成</param>
        /// <param name="isStart"></param>
        public static void CreateAppPool(string MachineName, string AppPoolName, AppPoolType type, bool isStart)
        {
            DirectoryEntry pools = new DirectoryEntry("IIS://" + MachineName + "/W3SVC/AppPools");
            DirectoryEntry pool;

            try
            {
                Monitor.Enter(obj_sync);

                if (IISManagement.GetIISServerVersion() < 7)
                {
                    pool = pools.Children.Add(AppPoolName, "IIsApplicationPool");
                    pool.Properties["AppPoolIdentityType"][0] = "2";
                    pool.Properties["PeriodicRestartTime"][0] = "30"; //30分钟自动回收
                }
                else
                {
                    pool = pools.Children.Add(AppPoolName, "IIsApplicationPool");
                    pool.Properties["ManagedPipelineMode"][0] = type;
                }

                pool.CommitChanges();
                pool.Close();

                //修改状态
                pool.Invoke(isStart ? "Start" : "Stop", null);
                pool.CommitChanges();
                pool.Close();
            }
            finally
            {
                Monitor.Exit(obj_sync);
            }
        }

        #endregion

        #region 管理应用程序池

        /// <summary>
        /// 管理应用程序池， method 是管理应用程序池的方法，有三种Start、Stop、Recycle，AppPoolName 是应用程序池名称
        /// </summary>
        /// <param name="method"></param>
        /// <param name="AppPoolName"></param>
        public static void ConfigAppPool(AppPoolMethodType method, string AppPoolName)
        {
            ConfigAppPool("localhost", method, AppPoolName);
        }

        /// <summary>
        /// 管理应用程序池， method 是管理应用程序池的方法，有三种Start、Stop、Recycle，AppPoolName 是应用程序池名称
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="method"></param>
        /// <param name="AppPoolName"></param>
        public static void ConfigAppPool(string MachineName, AppPoolMethodType method, string AppPoolName)
        {
            ConfigAppPool(MachineName, Enum.GetName(typeof(AppPoolMethodType), method), AppPoolName);
        }

        /// <summary>
        /// 管理应用程序池， method 是管理应用程序池的方法，有三种Start、Stop、Recycle，AppPoolName 是应用程序池名称
        /// </summary>
        /// <param name="method"></param>
        /// <param name="AppPoolName"></param>
        public static void ConfigAppPool(string method, string AppPoolName)
        {
            ConfigAppPool("localhost", method, AppPoolName);
        }

        /// <summary>
        /// 管理应用程序池， method 是管理应用程序池的方法，有三种Start、Stop、Recycle，AppPoolName 是应用程序池名称
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="method"></param>
        /// <param name="AppPoolName"></param>
        public static void ConfigAppPool(string MachineName, string method, string AppPoolName)
        {
            DirectoryEntry pools = new DirectoryEntry("IIS://" + MachineName + "/W3SVC/AppPools");
            DirectoryEntry pool = pools.Children.Find(AppPoolName, "IIsApplicationPool");

            pool.Invoke(method, null);
            pools.CommitChanges();
            pools.Close();
        }

        #endregion

        #region 删除应用程序池

        /// <summary>
        /// 删除应用程序池
        /// </summary>
        /// <param name="AppPoolName"></param>
        public static void RemoveAppPool(string AppPoolName)
        {
            RemoveAppPool("localhost", AppPoolName);
        }

        /// <summary>
        /// 删除应用程序池
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="AppPoolName"></param>
        public static void RemoveAppPool(string MachineName, string AppPoolName)
        {
            DirectoryEntry pools = new DirectoryEntry("IIS://" + MachineName + "/W3SVC/AppPools");
            DirectoryEntry pool = pools.Children.Find(AppPoolName, "IIsApplicationPool");

            pools.Children.Remove(pool);
            pools.CommitChanges();
            pools.Close();
        }

        #endregion

        #region 获取应用程序池的列表

        /// <summary>
        /// 获取应用程序池的列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAppPoolList()
        {
            return GetAppPoolList("localhost");
        }

        /// <summary>
        /// 获取应用程序池的列表
        /// </summary>
        /// <param name="MachineName"></param>
        /// <returns></returns>
        public static List<string> GetAppPoolList(string MachineName)
        {
            List<string> list = new List<string>();
            DirectoryEntry pools = new DirectoryEntry("IIS://" + MachineName + "/W3SVC/AppPools");

            foreach (DirectoryEntry de in pools.Children)
            {
                list.Add(de.Name);
            }

            return list;
        }

        #endregion

        #region 给站点设置应用程序池

        /// <summary>
        /// 给站点设置应用程序池
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="AppPoolName"></param>
        public static void AssignAppPool(int Identify, string AppPoolName)
        {
            AssignAppPool("localhost", Identify, AppPoolName);
        }

        /// <summary>
        /// 给站点设置应用程序池
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="Identify"></param>
        /// <param name="AppPoolName"></param>
        public static void AssignAppPool(string MachineName, int Identify, string AppPoolName)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, Identify);

            AssignAppPool(Server, AppPoolName);
        }

        /// <summary>
        /// 给站点设置应用程序池
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <param name="AppPoolName"></param>
        public static void AssignAppPool(string ServerComment, string AppPoolName)
        {
            AssignAppPool("localhost", ServerComment, AppPoolName);
        }

        /// <summary>
        /// 给站点设置应用程序池
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="ServerComment"></param>
        /// <param name="AppPoolName"></param>
        public static void AssignAppPool(string MachineName, string ServerComment, string AppPoolName)
        {
            DirectoryEntry Server = GetIISWebServer(MachineName, ServerComment);

            AssignAppPool(Server, AppPoolName);
        }

        /// <summary>
        /// 给站点或目录设置应用程序池
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="AppPoolName"></param>
        public static void AssignAppPool(DirectoryEntry Server, string AppPoolName)
        {
            if (Server == null)
            {
                throw new Exception("IIS 站点不存在，或者没有指定正确的 IIS 站点的标识符或描述名称。");
            }

            if (String.IsNullOrEmpty(AppPoolName))
            {
                throw new Exception(AppPoolName + " 的值不能为空。");
            }

            DirectoryEntry root = getRoot(Server);
            object[] param = { 0, AppPoolName, true };
            root.Invoke("Appcreate3", param);
        }

        #endregion

        #region 获取指定的应用程序池中包含的网站的标识符列表

        /// <summary>
        /// 获取指定的应用程序池中包含的网站的标识符列表
        /// </summary>
        /// <param name="AppPoolName"></param>
        /// <returns></returns>
        public static List<int> GetIISWebServerFromAppPool(string AppPoolName)
        {
            return GetIISWebServerFromAppPool("localhost", AppPoolName);
        }

        /// <summary>
        /// 获取指定的应用程序池中包含的网站的标识符列表
        /// </summary>
        /// <param name="MachineName"></param>
        /// <param name="AppPoolName"></param>
        /// <returns></returns>
        public static List<int> GetIISWebServerFromAppPool(string MachineName, string AppPoolName)
        {
            List<int> list = new List<int>();
            IISWebServerCollection iiswss = GetIISWebServers(MachineName);

            foreach (IISWebServer ws in iiswss)
            {
                if (String.Compare(ws.AppPoolId, AppPoolName, true) == 0)
                {
                    list.Add(ws.Identify);
                }
            }

            return list;
        }

        #endregion


        private static DirectoryEntry getRoot(DirectoryEntry Server)
        {
            foreach (DirectoryEntry child in Server.Children)
            {
                if ((String.Compare(child.Name, "Root", true) == 0) && (String.Compare(child.SchemaClassName, "IIsWebVirtualDir", true) == 0))
                {
                    return child;
                }
            }

            return null;
        }
    }
}
