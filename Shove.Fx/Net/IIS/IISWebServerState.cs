
using System;
using System.DirectoryServices;
using System.Collections;

namespace Shove._Net.IIS
{
    /// <summary>
    /// IISWebServer的状态
    /// </summary>
    public enum IISWebServerState
    {
        /// <summary>
        /// 
        /// </summary>
        Starting = 1,

        /// <summary>
        /// 
        /// </summary>
        Started = 2,

        /// <summary>
        /// 
        /// </summary>
        Stopping = 3,

        /// <summary>
        /// 
        /// </summary>
        Stopped = 4,

        /// <summary>
        /// 
        /// </summary>
        Pausing = 5,

        /// <summary>
        /// 
        /// </summary>
        Paused = 6,

        /// <summary>
        /// 
        /// </summary>
        Continuing = 7
    }
}