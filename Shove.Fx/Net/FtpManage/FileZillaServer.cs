using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess; 

namespace Shove._Net.FtpManage
{
    /// <summary>
    /// FileZillaServer 管理接口
    /// </summary>
    public class FileZillaServer
    {
        private string ExecuteFileName;
        private string ConfigFileName;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ExecuteFileName">FileZilla server.exe 的带完整路径的文件名</param>
        /// <param name="ConfigFileName">FileZilla server.xml 的带完整路径的文件名</param>
        public FileZillaServer(string ExecuteFileName, string ConfigFileName)
        {
            //if (!System.IO.File.Exists(ExecuteFileName) || !System.IO.File.Exists(ConfigFileName))
            //{
            //    throw new Exception("File " + ExecuteFileName + " or " + ConfigFileName + " not exists.");
            //}

            this.ExecuteFileName = ExecuteFileName;
            this.ConfigFileName = ConfigFileName;
        }

        /// <summary>
        /// 获取一个 FTP 用户信息
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="ReturnDescription"></param>
        /// <returns></returns>
        public FileZillaServerUser GetUser(string UserName, ref string ReturnDescription)
        {
            ReturnDescription = "";
            XPathDocument xmldoc = null;

            try
            {
                xmldoc = new XPathDocument(ConfigFileName);
            }
            catch (Exception e)
            {
                ReturnDescription = ConfigFileName + " 加载失败：" + e.Message;

                return null;
            }

            XPathNavigator nav = xmldoc.CreateNavigator();
            XPathNodeIterator nodes = nav.Select("FileZillaServer/Users/User");

            bool isUserFound = false;

            while (nodes.MoveNext())
            {
                if (nodes.Current.GetAttribute("Name", "").Trim().ToLower() == UserName.Trim().ToLower())
                {
                    isUserFound = true;

                    break;
                }
            }

            if (!isUserFound)
            {
                ReturnDescription = "用户不存在";

                return null;
            }

            FileZillaServerUser Result = new FileZillaServerUser();
            Result.Name = nodes.Current.GetAttribute("Name", "").Trim();

            XPathNodeIterator node_option = nodes.Current.Select("Option");
            int option_ok_count = 0;

            while (node_option.MoveNext())
            {
                if (node_option.Current.GetAttribute("Name", "") == "Pass")
                {
                    Result.Password = node_option.Current.Value;
                    option_ok_count++;
                }
                else if (node_option.Current.GetAttribute("Name", "") == "Enabled")
                {
                    Result.Enabled = (node_option.Current.Value == "1");
                    option_ok_count++;
                }

                if (option_ok_count >= 2)
                {
                    break;
                }
            }

            if (option_ok_count < 2)
            {
                ReturnDescription = ConfigFileName + " 不完整。";

                return null;
            }

            XPathNodeIterator node_permission = nodes.Current.Select("Permissions/Permission");

            if (!node_permission.MoveNext())
            {
                ReturnDescription = ConfigFileName + " 不完整。";

                return null;
            }

            Result.Directory = node_permission.Current.GetAttribute("Dir", "");

            return Result;
        }

        /// <summary>
        /// 获取全部 FTP 用户列表
        /// </summary>
        /// <param name="ReturnDescription"></param>
        /// <returns></returns>
        public IList<FileZillaServerUser> GetUsers(ref string ReturnDescription)
        {
            ReturnDescription = "";
            XPathDocument xmldoc = null;

            try
            {
                xmldoc = new XPathDocument(ConfigFileName);
            }
            catch (Exception e)
            {
                ReturnDescription = ConfigFileName + " 加载失败：" + e.Message;

                return null;
            }

            XPathNavigator nav = xmldoc.CreateNavigator();
            XPathNodeIterator nodes = nav.Select("FileZillaServer/Users/User");

            IList<FileZillaServerUser> Result = new List<FileZillaServerUser>();

            while (nodes.MoveNext())
            {
                FileZillaServerUser user = new FileZillaServerUser();
                user.Name = nodes.Current.GetAttribute("Name", "").Trim();

                XPathNodeIterator node_option = nodes.Current.Select("Option");
                int option_ok_count = 0;

                while (node_option.MoveNext())
                {
                    if (node_option.Current.GetAttribute("Name", "") == "Pass")
                    {
                        user.Password = node_option.Current.Value;
                        option_ok_count++;
                    }
                    else if (node_option.Current.GetAttribute("Name", "") == "Enabled")
                    {
                        user.Enabled = (node_option.Current.Value == "1");
                        option_ok_count++;
                    }

                    if (option_ok_count >= 2)
                    {
                        break;
                    }
                }

                if (option_ok_count < 2)
                {
                    //ReturnDescription = ConfigFileName + " 不完整。";

                    //return null;

                    continue;
                }

                XPathNodeIterator node_permission = nodes.Current.Select("Permissions/Permission");

                if (!node_permission.MoveNext())
                {
                    //ReturnDescription = ConfigFileName + " 不完整。";

                    //return null;

                    continue;
                }

                user.Directory = node_permission.Current.GetAttribute("Dir", "");
                Result.Add(user);
            }

            return Result;
        }

        /// <summary>
        /// 创建一个 FTP 用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ReturnDescription"></param>
        /// <returns></returns>
        public bool CreateUser(FileZillaServerUser user, ref string ReturnDescription)
        {
            ReturnDescription = "";

            #region 判断

            if (FileZillaServerUser.ValidUserInfo(user))
            {
                ReturnDescription = "用户信息不完整。";

                return false;
            }

            FileZillaServerUser temp = GetUser(user.Name, ref ReturnDescription);

            if (temp != null)
            {
                ReturnDescription = "用户名已经存在。";

                return false;
            }

            //if (!String.IsNullOrEmpty(ReturnDescription))
            //{
            //    return false;
            //}

            ReturnDescription = "";

            #endregion

            XmlDocument doc = new XmlDocument();

            #region 加载 Xml

            try
            {
                doc.LoadXml(System.IO.File.ReadAllText(ConfigFileName));//, Encoding.UTF8));
            }
            catch (Exception e)
            {
                ReturnDescription = ConfigFileName + " 加载失败：" + e.Message;

                return false;
            }

            XmlNode node = doc.SelectSingleNode("FileZillaServer/Users");

            if (node == null)
            {
                ReturnDescription = ConfigFileName + " 不完整。";

                return false;
            }

            #endregion

            #region 增加节点

            XmlNode node_user = doc.CreateElement("User");
            XmlAttribute att_user_Name = doc.CreateAttribute("Name");
            att_user_Name.InnerText = user.Name.Trim();
            node_user.Attributes.Append(att_user_Name);

            XmlNode node_option_Pass = doc.CreateElement("Option");
            node_user.AppendChild(node_option_Pass);
            XmlAttribute att_option_Pass = doc.CreateAttribute("Name");
            att_option_Pass.InnerText = "Pass";
            node_option_Pass.Attributes.Append(att_option_Pass);
            node_option_Pass.InnerText = user.Password;

            XmlNode node_option_Group = doc.CreateElement("Option");
            node_user.AppendChild(node_option_Group);
            XmlAttribute att_option_Group = doc.CreateAttribute("Name");
            att_option_Group.InnerText = "Group";
            node_option_Group.Attributes.Append(att_option_Group);

            XmlNode node_option_Bypass_server_userlimit = doc.CreateElement("Option");
            node_user.AppendChild(node_option_Bypass_server_userlimit);
            XmlAttribute att_option_Bypass_server_userlimit = doc.CreateAttribute("Name");
            att_option_Bypass_server_userlimit.InnerText = "Bypass server userlimit";
            node_option_Bypass_server_userlimit.Attributes.Append(att_option_Bypass_server_userlimit);
            node_option_Bypass_server_userlimit.InnerText = "0";

            XmlNode node_option_User_Limit = doc.CreateElement("Option");
            node_user.AppendChild(node_option_User_Limit);
            XmlAttribute att_option_User_Limit = doc.CreateAttribute("Name");
            att_option_User_Limit.InnerText = "User Limit";
            node_option_User_Limit.Attributes.Append(att_option_User_Limit);
            node_option_User_Limit.InnerText = "0";

            XmlNode node_option_IP_Limit = doc.CreateElement("Option");
            node_user.AppendChild(node_option_IP_Limit);
            XmlAttribute att_option_IP_Limit = doc.CreateAttribute("Name");
            att_option_IP_Limit.InnerText = "IP Limit";
            node_option_IP_Limit.Attributes.Append(att_option_IP_Limit);
            node_option_IP_Limit.InnerText = "0";

            XmlNode node_option_Enabled = doc.CreateElement("Option");
            node_user.AppendChild(node_option_Enabled);
            XmlAttribute att_option_Enabled = doc.CreateAttribute("Name");
            att_option_Enabled.InnerText = "Enabled";
            node_option_Enabled.Attributes.Append(att_option_Enabled);
            node_option_Enabled.InnerText = user.Enabled ? "1" : "0";

            XmlNode node_option_Comments = doc.CreateElement("Option");
            node_user.AppendChild(node_option_Comments);
            XmlAttribute att_option_Comments = doc.CreateAttribute("Name");
            att_option_Comments.InnerText = "Comments";
            node_option_Comments.Attributes.Append(att_option_Comments);

            XmlNode node_option_ForceSsl = doc.CreateElement("Option");
            node_user.AppendChild(node_option_ForceSsl);
            XmlAttribute att_option_ForceSsl = doc.CreateAttribute("Name");
            att_option_ForceSsl.InnerText = "ForceSsl";
            node_option_ForceSsl.Attributes.Append(att_option_ForceSsl);
            node_option_ForceSsl.InnerText = "0";

            XmlNode node_IpFilter = doc.CreateElement("IpFilter");
            node_user.AppendChild(node_IpFilter);
            XmlNode node_IpFilter_Disallowed = doc.CreateElement("Disallowed");
            node_IpFilter.AppendChild(node_IpFilter_Disallowed);
            XmlNode node_IpFilter_Allowed = doc.CreateElement("Allowed");
            node_IpFilter.AppendChild(node_IpFilter_Allowed);

            XmlNode node_Permissions = doc.CreateElement("Permissions");
            node_user.AppendChild(node_Permissions);
            XmlNode node__Permissions_Permission = doc.CreateElement("Permission");
            node_Permissions.AppendChild(node__Permissions_Permission);
            XmlAttribute att_Permissions_Permission_Dir = doc.CreateAttribute("Dir");
            att_Permissions_Permission_Dir.InnerText = user.Directory;
            node__Permissions_Permission.Attributes.Append(att_Permissions_Permission_Dir);

            XmlNode node_option_Permission_FileRead = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_FileRead);
            XmlAttribute att_option_Permission_FileRead = doc.CreateAttribute("Name");
            att_option_Permission_FileRead.InnerText = "FileRead";
            node_option_Permission_FileRead.Attributes.Append(att_option_Permission_FileRead);
            node_option_Permission_FileRead.InnerText = "1";

            XmlNode node_option_Permission_FileWrite = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_FileWrite);
            XmlAttribute att_option_Permission_FileWrite = doc.CreateAttribute("Name");
            att_option_Permission_FileWrite.InnerText = "FileWrite";
            node_option_Permission_FileWrite.Attributes.Append(att_option_Permission_FileWrite);
            node_option_Permission_FileWrite.InnerText = "1";

            XmlNode node_option_Permission_FileDelete = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_FileDelete);
            XmlAttribute att_option_Permission_FileDelete = doc.CreateAttribute("Name");
            att_option_Permission_FileDelete.InnerText = "FileDelete";
            node_option_Permission_FileDelete.Attributes.Append(att_option_Permission_FileDelete);
            node_option_Permission_FileDelete.InnerText = "1";

            XmlNode node_option_Permission_FileAppend = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_FileAppend);
            XmlAttribute att_option_Permission_FileAppend = doc.CreateAttribute("Name");
            att_option_Permission_FileAppend.InnerText = "FileAppend";
            node_option_Permission_FileAppend.Attributes.Append(att_option_Permission_FileAppend);
            node_option_Permission_FileAppend.InnerText = "1";

            XmlNode node_option_Permission_DirCreate = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_DirCreate);
            XmlAttribute att_option_Permission_DirCreate = doc.CreateAttribute("Name");
            att_option_Permission_DirCreate.InnerText = "DirCreate";
            node_option_Permission_DirCreate.Attributes.Append(att_option_Permission_DirCreate);
            node_option_Permission_DirCreate.InnerText = "1";

            XmlNode node_option_Permission_DirDelete = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_DirDelete);
            XmlAttribute att_option_Permission_DirDelete = doc.CreateAttribute("Name");
            att_option_Permission_DirDelete.InnerText = "DirDelete";
            node_option_Permission_DirDelete.Attributes.Append(att_option_Permission_DirDelete);
            node_option_Permission_DirDelete.InnerText = "1";

            XmlNode node_option_Permission_DirList = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_DirList);
            XmlAttribute att_option_Permission_DirList = doc.CreateAttribute("Name");
            att_option_Permission_DirList.InnerText = "DirList";
            node_option_Permission_DirList.Attributes.Append(att_option_Permission_DirList);
            node_option_Permission_DirList.InnerText = "1";

            XmlNode node_option_Permission_DirSubdirs = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_DirSubdirs);
            XmlAttribute att_option_Permission_DirSubdirs = doc.CreateAttribute("Name");
            att_option_Permission_DirSubdirs.InnerText = "DirSubdirs";
            node_option_Permission_DirSubdirs.Attributes.Append(att_option_Permission_DirSubdirs);
            node_option_Permission_DirSubdirs.InnerText = "1";

            XmlNode node_option_Permission_IsHome = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_IsHome);
            XmlAttribute att_option_Permission_IsHome = doc.CreateAttribute("Name");
            att_option_Permission_IsHome.InnerText = "IsHome";
            node_option_Permission_IsHome.Attributes.Append(att_option_Permission_IsHome);
            node_option_Permission_IsHome.InnerText = "1";

            XmlNode node_option_Permission_AutoCreate = doc.CreateElement("Option");
            node__Permissions_Permission.AppendChild(node_option_Permission_AutoCreate);
            XmlAttribute att_option_Permission_AutoCreate = doc.CreateAttribute("Name");
            att_option_Permission_AutoCreate.InnerText = "AutoCreate";
            node_option_Permission_AutoCreate.Attributes.Append(att_option_Permission_AutoCreate);
            node_option_Permission_AutoCreate.InnerText = "0";

            XmlNode node_SpeedLimits = doc.CreateElement("SpeedLimits");
            node_user.AppendChild(node_SpeedLimits);
            XmlAttribute att_SpeedLimits_DlType = doc.CreateAttribute("DlType");
            att_SpeedLimits_DlType.InnerText = "0";
            node_SpeedLimits.Attributes.Append(att_SpeedLimits_DlType);
            XmlAttribute att_SpeedLimits_DlLimit = doc.CreateAttribute("DlLimit");
            att_SpeedLimits_DlLimit.InnerText = "10";
            node_SpeedLimits.Attributes.Append(att_SpeedLimits_DlLimit);
            XmlAttribute att_SpeedLimits_ServerDlLimitBypass = doc.CreateAttribute("ServerDlLimitBypass");
            att_SpeedLimits_ServerDlLimitBypass.InnerText = "0";
            node_SpeedLimits.Attributes.Append(att_SpeedLimits_ServerDlLimitBypass);
            XmlAttribute att_SpeedLimits_UlType = doc.CreateAttribute("UlType");
            att_SpeedLimits_UlType.InnerText = "0";
            node_SpeedLimits.Attributes.Append(att_SpeedLimits_UlType);
            XmlAttribute att_SpeedLimits_UlLimit = doc.CreateAttribute("UlLimit");
            att_SpeedLimits_UlLimit.InnerText = "10";
            node_SpeedLimits.Attributes.Append(att_SpeedLimits_UlLimit);
            XmlAttribute att_SpeedLimits_ServerUlLimitBypass = doc.CreateAttribute("ServerUlLimitBypass");
            att_SpeedLimits_ServerUlLimitBypass.InnerText = "0";
            node_SpeedLimits.Attributes.Append(att_SpeedLimits_ServerUlLimitBypass);
            XmlNode node_SpeedLimits_Download = doc.CreateElement("Download");
            node_SpeedLimits.AppendChild(node_SpeedLimits_Download);
            XmlNode node_SpeedLimits_Upload = doc.CreateElement("Upload");
            node_SpeedLimits.AppendChild(node_SpeedLimits_Upload);

            node.AppendChild(node_user);

            #endregion

            while (true)
            {
                try
                {
                    //StopService();
                    System.Threading.Thread.Sleep(2000);
                    doc.Save(ConfigFileName);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    //StartService();
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }
            }

            ReloadConfig();

            return true;
        }

        /// <summary>
        /// 删除 FTP 用户
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="ReturnDescription"></param>
        /// <returns></returns>
        public bool RemoveUser(string UserName, ref string ReturnDescription)
        {
            ReturnDescription = "";

            #region 判断

            FileZillaServerUser temp = GetUser(UserName, ref ReturnDescription);

            if (temp == null)
            {
                return false;
            }

            ReturnDescription = "";

            #endregion

            XmlDocument doc = new XmlDocument();

            #region 加载 Xml

            try
            {
                doc.LoadXml(System.IO.File.ReadAllText(ConfigFileName));//, Encoding.UTF8));
            }
            catch (Exception e)
            {
                ReturnDescription = ConfigFileName + " 加载失败：" + e.Message;

                return false;
            }

            XmlNode node = doc.SelectSingleNode("FileZillaServer/Users");

            if (node == null)
            {
                ReturnDescription = ConfigFileName + " 不完整。";

                return false;
            }

            #endregion

            XmlNode node_user = null;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Attributes["Name"].Value.Trim().ToLower() == UserName.Trim().ToLower())
                {
                    node_user = node.ChildNodes[i];
                    break;
                }
            }

            if (node_user == null)
            {
                ReturnDescription = "用户不存在";

                return false;
            }

            node.RemoveChild(node_user);

            while (true)
            {
                try
                {
                    //StopService();
                    System.Threading.Thread.Sleep(2000);
                    doc.Save(ConfigFileName);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    //StartService();
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
            }

            ReloadConfig();

            return true;
        }

        /// <summary>
        /// 编辑 FTP 用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ReturnDescription"></param>
        /// <returns></returns>
        public bool EditUser(FileZillaServerUser user, ref string ReturnDescription)
        {
            ReturnDescription = "";

            #region 判断

            FileZillaServerUser temp = GetUser(user.Name, ref ReturnDescription);


            if (temp == null)
            {
                ReturnDescription = "用户不存在" + user.Name;
                return false;
            }

            ReturnDescription = "";

            #endregion

            XmlDocument doc = new XmlDocument();

            #region 加载 Xml

            try
            {
                doc.LoadXml(System.IO.File.ReadAllText(ConfigFileName));//, Encoding.UTF8));
            }
            catch (Exception e)
            {
                ReturnDescription = ConfigFileName + " 加载失败：" + e.Message;

                return false;
            }

            XmlNode node = doc.SelectSingleNode("FileZillaServer/Users");

            if (node == null)
            {
                ReturnDescription = ConfigFileName + " 不完整。";

                return false;
            }

            #endregion

            XmlNode node_user = null;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Attributes["Name"].Value.Trim().ToLower() == user.Name.Trim().ToLower())
                {
                    node_user = node.ChildNodes[i];
                    break;
                }
            }

            if (node_user == null)
            {
                ReturnDescription = "用户不存在";

                return false;
            }

            XmlNode node_option_Pass = null;
            XmlNode node_option_Enabled = null;
            XmlNode node_option_Permissions = null;

            for (int i = 0; i < node_user.ChildNodes.Count; i++)
            {
                if ((node_user.ChildNodes[i].Name == "Option") && node_user.ChildNodes[i].Attributes["Name"].Value == "Pass")
                {
                    node_option_Pass = node_user.ChildNodes[i];
                }
                else if ((node_user.ChildNodes[i].Name == "Option") && node_user.ChildNodes[i].Attributes["Name"].Value == "Enabled")
                {
                    node_option_Enabled = node_user.ChildNodes[i];
                }
                else if (node_user.ChildNodes[i].Name == "Permissions")
                {
                    node_option_Permissions = node_user.ChildNodes[i];
                    node_option_Permissions = node_option_Permissions.ChildNodes[0];
                }
            }

            if ((node_option_Pass == null) || (node_option_Enabled == null) || (node_option_Permissions == null))
            {
                ReturnDescription = ConfigFileName + " 不完整。";

                return false;
            }

            node_option_Pass.InnerText = user.Password;
            node_option_Enabled.InnerText = user.Enabled ? "1" : "0";
            node_option_Permissions.Attributes["Dir"].Value = user.Directory;

            while (true)
            {
                try
                {
                    //StopService();
                    System.Threading.Thread.Sleep(2000);
                    doc.Save(ConfigFileName);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    //StartService();
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
            }

            ReloadConfig();

            return true;
        }

        /// <summary>
        /// 查询已使用磁盘空间的大小
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ReturnDescription"></param>
        /// <returns></returns>
        public long QueryUsedSpaceSize(FileZillaServerUser user, ref string ReturnDescription)
        {
            ReturnDescription = "";
            DirectoryInfo di = new DirectoryInfo(user.Directory);

            if (!di.Exists)
            {
                ReturnDescription = "文件夹不存在。";

                return -1;
            }

            return Shove._IO.File.GetDirectorySize(user.Directory);
            //return 0;
        }

        private void ReloadConfig()
        {
            System.Threading.Thread thread = new System.Threading.Thread(_ReloadConfig);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 通知服务重新加载配置
        /// </summary>
        private void _ReloadConfig()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = ExecuteFileName;
            info.Arguments = "/reload-config";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

            try
            {
                process.Start();
            }
            catch { }
        }

        #region Service Control

        /// <summary>
        /// 启动 FTP 服务
        /// </summary>
        public void StartService()
        {
            System.Threading.Thread thread = new System.Threading.Thread(_StartService);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 启动 FTP 服务
        /// </summary>
        private void _StartService()
        {
            try
            {
                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController();
                sc.ServiceName = "FileZilla Server";
                if (sc.Status == ServiceControllerStatus.Stopped)
                    sc.Start();
            }
            catch (Exception ex)
            {
                new Shove._IO.Log("log").Write(ex.ToString());
            }
        }

        /// <summary>
        /// 停止 FTP 服务。本命令是异步操作，停止命令后，需等待数秒，再执行 StartService
        /// </summary>
        public void StopService()
        {
            System.Threading.Thread thread = new System.Threading.Thread(_StopService);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 停止 FTP 服务。本命令是异步操作，停止命令后，需等待数秒，再执行 StartService
        /// </summary>
        private void _StopService()
        {
            try
            {
                //System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
                //process.Start();
                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController();
                sc.ServiceName = "FileZilla Server";
                if (sc.Status == ServiceControllerStatus.Running)
                    sc.Stop(); 
            }
            catch(Exception ex) {
                new Shove._IO.Log("log").Write(ex.ToString());
            }
        }

        #endregion
    }
}

/*
<FileZillaServer>
    <Settings>
        <Item name="Service display name" type="string" />
        <SpeedLimits>
            <Download />
            <Upload />
        </SpeedLimits>
    </Settings>
    <Groups />
    <Users>
        <User Name="shove">
            <Option Name="Pass">62693a17328f470672012cdecb01fdc6</Option>
            <Option Name="Group" />
            <Option Name="Bypass server userlimit">0</Option>
            <Option Name="User Limit">0</Option>
            <Option Name="IP Limit">0</Option>
            <Option Name="Enabled">1</Option>
            <Option Name="Comments" />
            <Option Name="ForceSsl">0</Option>
            <IpFilter>
                <Disallowed />
                <Allowed />
            </IpFilter>
            <Permissions>
                <Permission Dir="D:">
                    <Option Name="FileRead">1</Option>
                    <Option Name="FileWrite">1</Option>
                    <Option Name="FileDelete">1</Option>
                    <Option Name="FileAppend">1</Option>
                    <Option Name="DirCreate">1</Option>
                    <Option Name="DirDelete">1</Option>
                    <Option Name="DirList">1</Option>
                    <Option Name="DirSubdirs">1</Option>
                    <Option Name="IsHome">1</Option>
                    <Option Name="AutoCreate">0</Option>
                </Permission>
            </Permissions>
            <SpeedLimits DlType="0" DlLimit="10" ServerDlLimitBypass="0" UlType="0" UlLimit="10" ServerUlLimitBypass="0">
                <Download />
                <Upload />
            </SpeedLimits>
        </User>
        <User Name="gdsz110.com">
            <Option Name="Pass">99a8d1b4f686f69cbfcd3397fce6a303</Option>
            <Option Name="Group" />
            <Option Name="Bypass server userlimit">0</Option>
            <Option Name="User Limit">0</Option>
            <Option Name="IP Limit">0</Option>
            <Option Name="Enabled">1</Option>
            <Option Name="Comments" />
            <Option Name="ForceSsl">0</Option>
            <IpFilter>
                <Disallowed />
                <Allowed />
            </IpFilter>
            <Permissions>
                <Permission Dir="D:\wwwroot\Other WebSite\gdsz110.com">
                    <Option Name="FileRead">1</Option>
                    <Option Name="FileWrite">1</Option>
                    <Option Name="FileDelete">1</Option>
                    <Option Name="FileAppend">0</Option>
                    <Option Name="DirCreate">1</Option>
                    <Option Name="DirDelete">1</Option>
                    <Option Name="DirList">1</Option>
                    <Option Name="DirSubdirs">1</Option>
                    <Option Name="IsHome">1</Option>
                    <Option Name="AutoCreate">0</Option>
                </Permission>
            </Permissions>
            <SpeedLimits DlType="0" DlLimit="10" ServerDlLimitBypass="0" UlType="0" UlLimit="10" ServerUlLimitBypass="0">
                <Download />
                <Upload />
            </SpeedLimits>
        </User>
    </Users>
</FileZillaServer>
*/