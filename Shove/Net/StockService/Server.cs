using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using LumiSoft.Net.TCP;
using LumiSoft.Net;
using LumiSoft.Net.IO;

namespace Shove.Net.TCP
{
    /// <summary>
    /// SocketService
    /// </summary>
    public partial class SocketService
    {
        /// <summary>
        /// Server
        /// </summary>
        public class Server
        {
            /// <summary>
            /// 接收到 Client 发来的消息后，通过回调交给应用程序处理消息
            /// </summary>
            /// <param name="ReceiveBuffer"></param>
            /// <returns></returns>
            public delegate byte[] ReceiveHandler(byte[] ReceiveBuffer);

            private string HostName;
            private string IP;
            private int Port;

            private TCP_Server<TCP_ServerSession> tcpServer = null;

            private ReceiveHandler Delegate_ReceiveHandler = null;

            private Utility utility = null;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            /// <param name="desKey"></param>
            /// <param name="delegate_ReceiveHandler"></param>
            public Server(string hostName, string ip, int port, string desKey, ReceiveHandler delegate_ReceiveHandler)
            {
                HostName = hostName;
                IP = ip;
                Port = port;

                utility = new Utility(desKey);

                Delegate_ReceiveHandler = delegate_ReceiveHandler;

                tcpServer = new TCP_Server<TCP_ServerSession>();
                tcpServer.Bindings = new LumiSoft.Net.IPBindInfo[] { new IPBindInfo(HostName, LumiSoft.Net.BindInfoProtocol.TCP, System.Net.IPAddress.Parse(IP), Port) };
                tcpServer.SessionCreated += new EventHandler<TCP_ServerSessionEventArgs<TCP_ServerSession>>(tcpServer_SessionCreated);
            }

            /// <summary>
            /// 开始侦听
            /// </summary>
            /// <param name="ErrorDescription"></param>
            /// <returns></returns>
            public bool Start(ref string ErrorDescription)
            {
                ErrorDescription = "";

                try
                {
                    tcpServer.Start();

                    if (tcpServer.IsRunning)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorDescription = "通讯服务没有成功启动。";

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
            /// 回调
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void tcpServer_SessionCreated(object sender, TCP_ServerSessionEventArgs<TCP_ServerSession> e)
            {
                byte[] ReceiveBuffer = utility.ReceiveData(e.Session.TcpStream);
                byte[] SendBuffer = Delegate_ReceiveHandler(ReceiveBuffer);

                string ErrorDescription = "";
                utility.SendData(e.Session.TcpStream, SendBuffer, ref ErrorDescription);
            }

            /// <summary>
            /// 停止侦听
            /// </summary>
            /// <param name="ErrorDescription"></param>
            /// <returns></returns>
            public bool Stop(ref string ErrorDescription)
            {
                ErrorDescription = "";

                try
                {
                    tcpServer.Stop();

                    if (!tcpServer.IsRunning)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorDescription = "通讯服务没有成功停止。";

                        return false;
                    }
                }
                catch (Exception e)
                {
                    ErrorDescription = e.Message;

                    return false;
                }
            }
        }
    }
}
