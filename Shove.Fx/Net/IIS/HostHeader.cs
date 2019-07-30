using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;

namespace Shove._Net.IIS
{
    /// <summary>
    /// 
    /// </summary>
    public class HostHeader
    {
        /// <summary>
        /// 
        /// </summary>
        public List<object> Value = new List<object>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="domainName"></param>
        public void Add(string ip, int port, string domainName)
        {
            string headerStr = string.Format("{0}:{1}:{2}", ip, port, domainName);

            if (!Value.Contains(headerStr))
            {
                Value.Add(headerStr);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        public void Add(DirectoryEntry server)
        {
            PropertyValueCollection serverBindings = server.Properties["ServerBindings"];

            Add(serverBindings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverBindings"></param>
        public void Add(PropertyValueCollection serverBindings)
        {
            foreach (object obj in serverBindings)
            {
                if (!Value.Contains(obj))
                {
                    Value.Add(obj);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Value.Clear();
        }
    }
}