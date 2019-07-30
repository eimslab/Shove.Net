using System;
using System.DirectoryServices;
using System.Collections;
using System.Text.RegularExpressions;

namespace Shove._Web
{
    /// <summary>
    /// 对 IIS 进行操作
    /// </summary>
    public class IIS
    {
        #region UserName,Password,HostName的定义

        private string hostName = "localhost";
        private string userName;
        private string password;

        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName
        {
            get
            {
                return hostName;
            }

            set
            {
                hostName = value;
            }
        }

        /// <summary>
        /// 远程操作必须提供的用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        /// <summary>
        /// 远程操作必须提供的用户密码
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                if (UserName.Length <= 1)
                {
                    throw new ArgumentException("还没有指定好用户名。请先指定用户名");
                }
                password = value;
            }
        }

        /// <summary>
        /// 远程操作的用户配置
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void RemoteConfig(string hostName, string userName, string password)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }

        #endregion

        #region 根据路径构造Entry的方法

        /// <summary>
        ///  根据是否有用户名来判断是否是远程服务器。
        ///  然后再构造出不同的DirectoryEntry出来
        /// </summary>
        /// <param name="EntryPath">DirectoryEntry的路径</param>
        /// <returns>返回的是DirectoryEntry实例</returns>
        public DirectoryEntry GetDirectoryEntry(string EntryPath)
        {
            DirectoryEntry Entry;

            if (String.IsNullOrEmpty(UserName))
            {
                Entry = new DirectoryEntry(EntryPath);
            }
            else
            {
                Entry = new DirectoryEntry(EntryPath, UserName, Password, AuthenticationTypes.Secure);
            }

            return Entry;
        }

        #endregion

        #region 添加，删除网站的方法

        /// <summary>
        ///  创建一个新的网站。根据传过来的信息进行配置
        /// </summary>
        /// <param name="siteInfo">存储的是新网站的信息</param>
        public void CreateWebSite(SiteInfo siteInfo)
        {
            if (!EnsureNewSiteEnavaible(siteInfo.BindString))
            {
                throw new Exception("站点已经存在。" + Environment.NewLine + siteInfo.BindString);
            }

            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(entPath);

            if (rootEntry == null)
            {
                throw new Exception("IIS 目录打开失败。");
            }

            long newSiteNum = GetNewWebSiteID();

            DirectoryEntry newSiteEntry = rootEntry.Children.Add(newSiteNum.ToString(), "IIsWebServer");

            newSiteEntry.CommitChanges();
            newSiteEntry.Properties["ServerBindings"].Value = siteInfo.BindString;
            newSiteEntry.Properties["ServerComment"].Value = siteInfo.CommentOfWebSite;
            newSiteEntry.CommitChanges();

            DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");

            vdEntry.CommitChanges();
            vdEntry.Properties["Path"].Value = siteInfo.WebPath;
            vdEntry.CommitChanges();
        }

        /// <summary>
        ///  删除一个网站。根据网站名称删除。
        /// </summary>
        /// <param name="SiteName">网站名称</param>
        public void DeleteWebSiteByName(string SiteName)
        {
            long siteNum = GetWebSiteNum(SiteName);

            if (siteNum < 0)
            {
                throw new Exception("IIS 站点“" + SiteName + "”打开失败。");
            }

            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);

            if (siteEntry == null)
            {
                throw new Exception("IIS 站点“" + SiteName + "”打开失败。");
            }

            string rootPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(rootPath);

            if (rootEntry == null)
            {
                throw new Exception("IIS 目录打开失败。");
            }

            rootEntry.Children.Remove(siteEntry);
            rootEntry.CommitChanges();
        }

        #endregion

        #region Start和Stop网站的方法

        /// <summary>
        /// 启动一个网站
        /// </summary>
        /// <param name="SiteName"></param>
        public void StartWebSite(string SiteName)
        {
            long siteNum = GetWebSiteNum(SiteName);

            if (siteNum < 0)
            {
                throw new Exception("IIS 站点“" + SiteName + "”打开失败。");
            }

            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);

            if (siteEntry == null)
            {
                throw new Exception("IIS 站点“" + SiteName + "”打开失败。");
            }

            siteEntry.Invoke("Start", new object[] { });
        }

        /// <summary>
        /// 停止一个网站
        /// </summary>
        /// <param name="SiteName"></param>
        public void StopWebSite(string SiteName)
        {
            long siteNum = GetWebSiteNum(SiteName);

            if (siteNum < 0)
            {
                throw new Exception("IIS 站点“" + SiteName + "”打开失败。");
            }

            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);

            if (siteEntry == null)
            {
                throw new Exception("IIS 站点“" + SiteName + "”打开失败。");
            }

            siteEntry.Invoke("Stop", new object[] { });
        }

        #endregion

        #region 确认网站是否相同

        /// <summary>
        ///  确定一个新的网站与现有的网站没有相同的。这样防止将非法的数据存放到IIS里面去
        /// </summary>
        /// <param name="bindStr">网站邦定信息</param>
        /// <returns>真为可以创建，假为不可以创建</returns>
        public bool EnsureNewSiteEnavaible(string bindStr)
        {
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);

            if (ent == null)
            {
                throw new Exception("IIS 目录打开失败。");
            }

            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        if (child.Properties["ServerBindings"].Value.ToString() == bindStr)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region 获取一个网站编号的方法

        /// <summary>
        ///  获取一个网站的编号。根据网站的ServerBindings或者ServerComment来确定网站编号
        /// </summary>
        /// <param name="SiteName"></param>
        /// <returns>返回网站的编号</returns>
        public long GetWebSiteNum(string SiteName)
        {
            Regex regex = new Regex(SiteName);
            string tmpStr;

            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);

            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        tmpStr = child.Properties["ServerBindings"].Value.ToString();

                        if (regex.Match(tmpStr).Success)
                        {
                            return _Convert.StrToLong(child.Name, -1);
                        }
                    }

                    if (child.Properties["ServerComment"].Value != null)
                    {
                        tmpStr = child.Properties["ServerComment"].Value.ToString();

                        if (regex.Match(tmpStr).Success)
                        {
                            return _Convert.StrToLong(child.Name, -2);
                        }
                    }
                }
            }

            // 站点不存在
            return -3;
        }

        #endregion

        #region 获取新网站id的方法

        /// <summary>
        ///  获取网站系统里面可以使用的最小的ID。
        ///  这是因为每个网站都需要有一个唯一的编号，而且这个编号越小越好。
        ///  这里面的算法经过了测试是没有问题的。
        /// </summary>
        /// <returns>最小的id</returns>
        public long GetNewWebSiteID()
        {
            ArrayList list = new ArrayList();
            string tmpStr;

            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);

            if (ent == null)
            {
                throw new Exception("IIS 目录打开失败。");
            }

            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name.ToString();
                    list.Add(Convert.ToInt32(tmpStr));
                }
            }

            list.Sort();

            long i = 1;
            foreach (long j in list)
            {
                if (i == j)
                {
                    i++;
                }
            }

            return i;
        }

        #endregion

        #region 新网站信息结构体

        /// <summary>
        /// 新网站信息结构体
        /// </summary>
        public struct SiteInfo
        {
            private string hostIP;   // The Hosts IP Address
            private string portNum;   // The New Web Sites Port.generally is "80"
            private string descOfWebSite; // 网站表示。一般为网站的网站名。例如"www.dns.com.cn"
            private string commentOfWebSite;// 网站注释。一般也为网站的网站名。
            private string webPath;   // 网站的主目录。例如"e:\tmp"

            /// <summary>
            /// 
            /// </summary>
            /// <param name="hostIP"></param>
            /// <param name="portNum"></param>
            /// <param name="descOfWebSite"></param>
            /// <param name="commentOfWebSite"></param>
            /// <param name="webPath"></param>
            public SiteInfo(string hostIP, string portNum, string descOfWebSite, string commentOfWebSite, string webPath)
            {
                this.hostIP = hostIP;
                this.portNum = portNum;
                this.descOfWebSite = descOfWebSite;
                this.commentOfWebSite = commentOfWebSite;
                this.webPath = webPath;
            }

            /// <summary>
            /// 
            /// </summary>
            public string BindString
            {
                get
                {
                    return String.Format("{0}:{1}:{2}", hostIP, portNum, descOfWebSite);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string CommentOfWebSite
            {
                get
                {
                    return commentOfWebSite;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string WebPath
            {
                get
                {
                    return webPath;
                }
            }
        }

        #endregion

        #region 主机头操作

        /// <summary>
        /// 增加站点主机头(根据站点名称)
        /// </summary>
        /// <param name="SiteName"></param>
        /// <param name="IPAddress"></param>
        /// <param name="Port"></param>
        /// <param name="Url"></param>
        public void AddHostHeader(string SiteName, string IPAddress, string Port, string Url)
        {
            AddHostHeader(GetWebSiteNum(SiteName), IPAddress, Port, Url);
        }

        /// <summary>
        /// 增加站点主机头(根据站点号，也就是站点标识)
        /// </summary>
        /// <param name="SiteNum"></param>
        /// <param name="IPAddress"></param>
        /// <param name="Port"></param>
        /// <param name="Url"></param>
        public void AddHostHeader(long SiteNum, string IPAddress, string Port, string Url)
        {
            if (SiteNum < 0)
            {
                throw new Exception("IIS 站点号“" + SiteNum.ToString() + "”打开失败。");
            }

            string entry = String.Format("IIS://{0}/w3svc/{1}", HostName, SiteNum);
            DirectoryEntry site = GetDirectoryEntry(entry);

            if (site == null)
            {
                throw new Exception("IIS 站点号“" + SiteNum.ToString() + "”打开失败。");
            }

            PropertyValueCollection serverBindings = site.Properties["ServerBindings"];

            string headerStr = string.Format("{0}:{1}:{2}", IPAddress, Port, Url);

            if (!serverBindings.Contains(headerStr))
            {
                serverBindings.Add(headerStr);
                site.CommitChanges();
            }
        }

        /// <summary>
        /// 删除站点主机头(根据站点名称)
        /// </summary>
        /// <param name="SiteName"></param>
        /// <param name="IPAddress"></param>
        /// <param name="Port"></param>
        /// <param name="Url"></param>
        public void DeleteHostHeader(string SiteName, string IPAddress, string Port, string Url)
        {
            DeleteHostHeader(GetWebSiteNum(SiteName), IPAddress, Port, Url);
        }

        /// <summary>
        /// 删除站点主机头(根据站点号，也就是站点标识)
        /// </summary>
        /// <param name="SiteNum"></param>
        /// <param name="IPAddress"></param>
        /// <param name="Port"></param>
        /// <param name="Url"></param>
        public void DeleteHostHeader(long SiteNum, string IPAddress, string Port, string Url)
        {
            if (SiteNum < 0)
            {
                throw new Exception("IIS 站点号“" + SiteNum.ToString() + "”打开失败。");
            }

            string entry = String.Format("IIS://{0}/w3svc/{1}", HostName, SiteNum);
            DirectoryEntry site = GetDirectoryEntry(entry);

            if (site == null)
            {
                throw new Exception("IIS 站点号“" + SiteNum.ToString() + "”打开失败。");
            }

            PropertyValueCollection serverBindings = site.Properties["ServerBindings"];

            string headerStr = string.Format("{0}:{1}:{2}", IPAddress, Port, Url);

            if (serverBindings.Contains(headerStr))
            {
                serverBindings.Remove(headerStr);
                site.CommitChanges();
            }
        }

        #endregion

        /// <summary>
        /// 进程池的操作项
        /// </summary>
        public enum ApplicationPoolOperateType
        {
            /// <summary>
            /// 启动进程池
            /// </summary>
            Start,
            /// <summary>
            /// 停止进程池
            /// </summary>
            Stop,
            /// <summary>
            /// 回收进程池
            /// </summary>
            Recycle
        }

        /// <summary>
        /// 复位 IIS 应用程序池, Operator = "Stop"、"Recycle"等。//"Stop" 为停止应用程序池,"Recycle"为复位IIS程序池
        /// </summary>
        public void RestartApplicationPool(string applicationPoolName, ApplicationPoolOperateType OperateType)
        {
            DirectoryEntry applicationPool = new DirectoryEntry("IIS://localhost/W3SVC/AppPools/" + applicationPoolName);
            applicationPool.Invoke(OperateType.ToString());
        }

        /// <summary>
        /// 获取所有的进程池列表
        /// </summary>
        /// <returns></returns>
        public string[] GetApplicationPools()
        {
            DirectoryEntry ApplicationPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            DirectoryEntries Pools = ApplicationPools.Children;

            ArrayList al = new ArrayList();

            foreach (DirectoryEntry pool in Pools)
            {
                al.Add(pool.Name);
            }

            if (al.Count < 1)
            {
                return null;
            }

            string[] Result = new string[al.Count];
            for (int i = 0; i < al.Count; i++)
            {
                Result[i] = al[i].ToString();
            }

            return Result;
        }

        /// <summary>
        /// 创建一个新的进程池
        /// </summary>
        /// <param name="PoolName">进程池名称</param>
        /// <param name="PeriodicRestartTime">定时回收资源分钟数</param>
        public void CreateApplicationPool(string PoolName, int PeriodicRestartTime)
        {
            DirectoryEntry ApplicationPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            System.DirectoryServices.DirectoryEntry NewPool = ApplicationPools.Children.Add(PoolName, "IISApplicationPool");

            NewPool.Properties["PeriodicRestartTime"][0] = PeriodicRestartTime;
            NewPool.CommitChanges();
        }
    }
}
