using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.DirectoryServices;

namespace Shove._Net.IIS
{
    /// <summary>
    /// IISWebServer
    /// </summary>
    public class IISWebServer
    {
        /// <summary>
        /// 
        /// </summary>
        internal int Identify = -1;

        /// <summary>
        /// 网站说明
        /// </summary>
        public string ServerComment = "Way";

        /// <summary>
        /// 脚本支持
        /// </summary>
        public bool AccessScript = true;

        /// <summary>
        /// 读取
        /// </summary>
        public bool AccessRead = true;

        /// <summary>
        /// 物理路径
        /// </summary>
        public string Path = @"c:\interpub\wwwroot";

        /// <summary>
        /// 目录浏览
        /// </summary>
        public bool EnableDirBrowsing = false;

        /// <summary>
        /// 默认文档
        /// </summary>
        public string DefaultDoc = "index.aspx";

        /// <summary>
        /// 使用默认文档
        /// </summary>
        public bool EnableDefaultDoc = true;

        /// <summary>
        /// 应用程序连接池
        /// </summary>
        public string AppPoolId = "DefaultAppPool";

        /// <summary>
        /// 估计与 Log 相关
        /// </summary>
        public bool DontLog = true;

        /// <summary>
        /// 是否允许匿名访问
        /// </summary>
        public bool AuthAnonymous = true;

        /// <summary>
        /// 是否启用系统默认的身份验证
        /// </summary>
        public bool AnonymousPasswordSync = true;

        private string anonymousUserName = string.Empty;
        /// <summary>
        /// 指定匿名用户名
        /// </summary>
        public string AnonymousUserName
        {
            set
            {
                if (!AnonymousPasswordSync)
                {
                    anonymousUserName = value;
                }
                else
                {
                    //anonymousUserName = "AnonymousPasswordSync 不为 false 不能指定匿名用户。";
                    throw new Exception("AnonymousPasswordSync 不为 false 不能指定匿名用户。");
                }
            }
            get
            {
                return anonymousUserName;
            }
        }

        private string anonymousUserPass = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string AnonymousUserPass
        {
            set
            {
                if (!AnonymousPasswordSync)
                {
                    anonymousUserPass = value;
                }
                else
                {
                    //anonymousUserPass = "AnonymousPasswordSync 不为 false 不能指定匿名用户。";
                    throw new Exception("AnonymousPasswordSync 不为 false 不能指定匿名用户。");
                }
            }
            get
            {
                return anonymousUserPass;
            }
        }

        /// <summary>
        /// 主机头信息
        /// </summary>
        public HostHeader hostHeader = null;

        /// <summary>
        /// 最大连接数 默认为100个连接数
        /// </summary>
        public long MaxConnections = -1;

        /// <summary>
        /// 宽带限制 默认1024=1M
        /// </summary>
        public long MaxBandwidth = -1;

        /// <summary>
        /// 301 跳转参数
        /// </summary>
        public string HttpRedirect = "";

        /// <summary>
        /// 应用程序名
        /// </summary>
        public string AppFriendlyName = "";

        /// <summary>
        /// 包含虚拟目录集合
        /// </summary>
        public IISWebVirtualDirCollection IISWebVirtualDirs = null;

        /// <summary>
        /// 
        /// </summary>
        public IISWebServer()
        {
            IISWebVirtualDirs = new IISWebVirtualDirCollection(this);
        }
    }
}
