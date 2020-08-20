using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using LumiSoft.Net.TCP;
using LumiSoft.Net;
using LumiSoft.Net.IO;

namespace Shove._Net.TCP
{
    /// <summary>
    /// SocketService
    /// </summary>
    public partial class SocketService
    {
        /// <summary>
        /// Client
        /// </summary>
        public class Client
        {
            private string HostNameOrIP;
            private int Port;

            private TCP_Client tcpClient = new TCP_Client();

            private Utility utility = null;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="hostNameOrIP"></param>
            /// <param name="port"></param>
            /// <param name="desKey"></param>
            public Client(string hostNameOrIP, int port, string desKey)
            {
                HostNameOrIP = hostNameOrIP;
                Port = port;

                utility = new Utility(desKey);
            }

            /// <summary>
            /// 连接服务器
            /// </summary>
            /// <param name="ErrorDescription"></param>
            /// <returns></returns>
            public bool Connect(ref string ErrorDescription)
            {
                ErrorDescription = "";

                try
                {
                    tcpClient.Connect(HostNameOrIP, Port);

                    if (tcpClient.IsConnected)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorDescription = "连接服务器失败。";

                        return false;
                    }
                }
                catch (Exception e)
                {
                    ErrorDescription = e.Message;

                    return false;
                }
            }

            /// <summary>
            /// 断开连接
            /// </summary>
            /// <param name="ErrorDescription"></param>
            /// <returns></returns>
            public bool DisConnect(ref string ErrorDescription)
            {
                ErrorDescription = "";

                if (!tcpClient.IsConnected)
                {
                    return true;
                }

                try
                {
                    tcpClient.Disconnect();

                    return true;
                }
                catch (Exception e)
                {
                    ErrorDescription = e.Message;

                    return false;
                }
            }

            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="SendBuffer"></param>
            /// <param name="ErrorDescription"></param>
            /// <returns></returns>
            public bool SendData(byte[] SendBuffer, ref string ErrorDescription)
            {
                ErrorDescription = "";

                if (!tcpClient.IsConnected)
                {
                    ErrorDescription = "没有连接到远程服务器。";

                    return false;
                }

                return utility.SendData(tcpClient.TcpStream, SendBuffer, ref ErrorDescription);
            }

            /// <summary>
            /// 接收数据
            /// </summary>
            /// <param name="ErrorDescription"></param>
            /// <returns></returns>
            public byte[] ReceiveData(ref string ErrorDescription)
            {
                ErrorDescription = "";

                if (!tcpClient.IsConnected)
                {
                    ErrorDescription = "没有连接到远程服务器。";

                    return null;
                }

                return utility.ReceiveData(tcpClient.TcpStream);
            }
        }
    }
}
