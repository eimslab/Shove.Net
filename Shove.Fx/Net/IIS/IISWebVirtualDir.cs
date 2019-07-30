using System;
using System.Collections.Generic;
using System.Text;

namespace Shove._Net.IIS
{
    /// <summary>
    /// IISWebVirtualDir 站点属性
    /// </summary>
    public class IISWebVirtualDir
    {
        private IISWebServer _Parent = null;
        /// <summary>
        /// 
        /// </summary>
        public IISWebServer Parent
        {
            set
            {
                _Parent = value;

                if (_Parent != null)
                {
                    this.ParentServerComment = _Parent.ServerComment;
                    this.ParentIdentify = _Parent.Identify;
                }
            }
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// 所属的网站的网站说明
        /// </summary>
        public string ParentServerComment = "";

        /// <summary>
        /// 所属的网站的标识符
        /// </summary>
        public int ParentIdentify = -1;

        /// <summary>
        /// 虚拟目录名称
        /// </summary>
        public string Name = "Way";

        /// <summary>
        /// 读取
        /// </summary>
        public bool AccessRead = true;

        /// <summary>
        /// 脚本支持
        /// </summary>
        public bool AccessScript = true;

        /// <summary>
        /// 物理路径
        /// </summary>
        public string Path = @"c:\";

        /// <summary>
        /// 默认文档
        /// </summary>
        public string DefaultDoc = "index.aspx";

        /// <summary>
        /// 使用默认文档
        /// </summary>
        public bool EnableDefaultDoc = true;

        /// <summary>
        /// 
        /// </summary>
        public IISWebVirtualDir()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerComment"></param>
        public IISWebVirtualDir(string ServerComment)
        {
            this.ParentServerComment = ServerComment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Identify"></param>
        public IISWebVirtualDir(int Identify)
        {
            this.ParentIdentify = Identify;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ws"></param>
        public IISWebVirtualDir(IISWebServer ws)
        {
            this.Parent = ws;

            if (ws != null)
            {
                this.ParentServerComment = ws.ServerComment;
                this.ParentIdentify = ws.Identify;
            }
        }
    }
}
