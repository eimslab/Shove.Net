using System;
using System.Collections.Generic;
using System.Text;

namespace Shove._Net.IIS
{
    /// <summary>
    /// 对进程池的操作事件
    /// </summary>
    public enum AppPoolMethodType
    {
        /// <summary>
        /// 启动
        /// </summary>
        Start = 1,
        
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 2,
        
        /// <summary>
        /// 回收
        /// </summary>
        Recycle = 3
    }
}
