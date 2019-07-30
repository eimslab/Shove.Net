using System;
using System.DirectoryServices;
using System.Reflection;
using System.Collections.Generic;

namespace Shove._Net.IIS
{
    /// <summary>
    /// 应用程序池操作类
    /// </summary>
    public enum AppPoolType
    {
        /// <summary>
        /// 集成
        /// </summary>
        Integration = 0,

        /// <summary>
        /// 经典
        /// </summary>
        Classics = 1
    }
}