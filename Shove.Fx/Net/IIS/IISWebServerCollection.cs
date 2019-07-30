using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shove._Net.IIS
{
    /// <summary>
    /// IISWebServerCollection 站点列表操作
    /// </summary>
    public class IISWebServerCollection : CollectionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IISWebServer this[int Identify]
        {
            get
            {
                for (int i = 0; i < this.List.Count; i++)
                {
                    IISWebServer server = (IISWebServer)this.List[i];

                    if (server.Identify == Identify)
                    {
                        return server;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IISWebServer this[string ServerComment]
        {
            get
            {
                ServerComment = ServerComment.Trim();

                for (int i = 0; i < this.List.Count; i++)
                {
                    IISWebServer server = (IISWebServer)this.List[i];

                    if (String.Compare(server.ServerComment.Trim(), ServerComment, true) == 0)
                    {
                        return server;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 获取集合中的全部站点的标识符列表
        /// </summary>
        /// <returns></returns>
        public List<int> Identifies()
        {
            List<int> list = new List<int>();

            for (int i = 0; i < this.List.Count; i++)
            {
                IISWebServer server = (IISWebServer)this.List[i];
                list.Add(server.Identify);
            }


            return list;
        }

        /// <summary>
        /// 获取集合中的全部站点的名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> ServerComments()
        {
            List<string> list = new List<string>();

            for (int i = 0; i < this.List.Count; i++)
            {
                IISWebServer server = (IISWebServer)this.List[i];
                list.Add(server.ServerComment);
            }


            return list;
        }

        internal void Add(IISWebServer WebServer)
        {
            this.List.Add(WebServer);
        }

        /// <summary>
        /// 是否包含指定的网站
        /// </summary>
        /// <param name="ServerComment"></param>
        /// <returns></returns>
        public bool Contains(string ServerComment)
        {
            return (this[ServerComment] != null);
        }

        /// <summary>
        /// 是否包含指定的网站
        /// </summary>
        /// <param name="Identify"></param>
        /// <returns></returns>
        public bool Contains(int Identify)
        {
            return (this[Identify] != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WebServers"></param>
        public void AddRange(IISWebServer[] WebServers)
        {
            for (int i = 0; i <= WebServers.GetUpperBound(0); i++)
            {
                Add(WebServers[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WebServer"></param>
        public void Remove(IISWebServer WebServer)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                if ((IISWebServer)this.List[i] == WebServer)
                {
                    this.List.RemoveAt(i);

                    break;
                }
            }
        }
    }
}