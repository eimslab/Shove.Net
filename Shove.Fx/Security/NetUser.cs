using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.DirectoryServices;
using System.Security.AccessControl;

namespace Shove._Security
{
    /// <summary>
    /// NetUser 操作类
    /// </summary>
    public class NetUser
    {
        #region DllImport

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="level"></param>
        /// <param name="buf"></param>
        /// <param name="parm_err"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public static extern int NetUserAdd([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref USER_INFO_1 buf, int parm_err);

        /// <summary>
        /// 用户信息结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_1
        {
            /// <summary>
            /// 用户名
            /// </summary>
            public string usri1_name;
            /// <summary>
            /// 用户密码
            /// </summary>
            public string usri1_password;
            /// <summary>
            /// 密码有效时长
            /// </summary>
            public int usri1_password_age;
            /// <summary>
            /// 
            /// </summary>
            public int usri1_priv;
            /// <summary>
            /// 
            /// </summary>
            public string usri1_home_dir;
            /// <summary>
            /// 备注
            /// </summary>
            public string comment;
            /// <summary>
            /// 
            /// </summary>
            public int usri1_flags;
            /// <summary>
            /// 登录后执行的脚本
            /// </summary>
            public string usri1_script_path;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public static extern int NetUserDel([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username);
        
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="username"></param>
        /// <param name="level"></param>
        /// <param name="bufptr"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public static extern int NetUserGetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, out IntPtr bufptr);
        
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="username"></param>
        /// <param name="level"></param>
        /// <param name="buf"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public static extern int NetUserSetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, ref USER_INFO_1 buf, int error);
        
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="domainname"></param>
        /// <param name="username"></param>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public static extern int NetUserChangePassword([MarshalAs(UnmanagedType.LPWStr)] string domainname, [MarshalAs(UnmanagedType.LPWStr)] string username, [MarshalAs(UnmanagedType.LPWStr)] string oldpassword, [MarshalAs(UnmanagedType.LPWStr)] string newpassword);
        
        /// <summary>
        /// 获得用户列表的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_0
        {
            /// <summary>
            /// 
            /// </summary>
            public String Username;
        }
        
        /// <summary>
        /// 获得用户列表
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="level"></param>
        /// <param name="filter"></param>
        /// <param name="bufptr"></param>
        /// <param name="prefmaxlen"></param>
        /// <param name="entriesread"></param>
        /// <param name="totalentries"></param>
        /// <param name="resume_handle"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public extern static int NetUserEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, int filter, out IntPtr bufptr, int prefmaxlen, out int entriesread, out int totalentries, out int resume_handle);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public extern static int NetApiBufferFree(IntPtr Buffer);

        /// <summary>
        /// 本地组、用户信息结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOCALGROUP_USERS_INFO_0
        {
            /// <summary>
            /// 
            /// </summary>
            public string groupname;
        }

        /// <summary>
        /// 获取用户组信息
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="username"></param>
        /// <param name="level"></param>
        /// <param name="flags"></param>
        /// <param name="bufptr"></param>
        /// <param name="prefmaxlen"></param>
        /// <param name="entriesread"></param>
        /// <param name="totalentries"></param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public extern static int NetUserGetLocalGroups([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, int flags, out IntPtr bufptr, int prefmaxlen, out int entriesread, out int totalentries);

        #endregion

        /// <summary>
        /// 向用户组添加用户
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="GroupName">用户组名</param>
        /// <param name="ReturnDescription">返回的错误消息</param>
        /// <returns></returns>
        public static bool AddUserToGroup(string UserName, string GroupName, ref string ReturnDescription)
        {
            bool bRet = false;
            ReturnDescription = "";

            DirectoryEntry oLocalMachine = null;
            DirectoryEntry oGroup = null;
            DirectoryEntry oNewUser = null;

            try
            {
                oLocalMachine = new DirectoryEntry("WinNT://" + Environment.MachineName);
                oGroup = oLocalMachine.Children.Find(GroupName, "group");
                oNewUser = oLocalMachine.Children.Find(UserName, "user");
            }
            catch
            { }

            try
            {
                if (oGroup != null)
                {
                    if (oNewUser != null)
                    {
                        if (!IsUserInGroups(oNewUser, GroupName))
                        {
                            oGroup.Invoke("Add", new object[] { oNewUser.Path });
                            oGroup.CommitChanges();
                            bRet = true;
                        }
                        else
                        {
                            oGroup.Invoke("Remove", new object[] { oNewUser.Path });
                            oGroup.CommitChanges();
                            bRet = true;
                        }
                    }
                    else
                    {
                        ReturnDescription = string.Format("没有找到用户[{0}]", UserName);
                    }
                }
                else
                {
                    ReturnDescription = string.Format("没有找到用户组[{0}]", GroupName);
                }
            }
            catch (Exception ex)
            {
                ReturnDescription = string.Format("往用户组[{0}]增加用户[{1}]时发生异常:\r\n{2}", GroupName, UserName, ex.Message);
            }
            finally
            {
                if (oLocalMachine != null)
                {
                    oLocalMachine.Close();
                    oLocalMachine.Dispose();
                    oLocalMachine = null;
                }
                if (oGroup != null)
                {
                    oGroup.Close();
                    oGroup.Dispose();
                    oGroup = null;
                }
                if (oNewUser != null)
                {
                    oNewUser.Close();
                    oNewUser.Dispose();
                    oNewUser = null;
                }
            }

            return bRet;
        }

        /// <summary>
        /// 判断用户是否已在用户组中
        /// </summary>
        /// <param name="userObj"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private static bool IsUserInGroups(DirectoryEntry userObj, string groupName)
        {
            bool bRet = false;
            if (userObj != null)
            {
                object oGroups = userObj.Invoke("groups", null);
                DirectoryEntry oGroup = null;
                try
                {
                    foreach (object group in (System.Collections.IEnumerable)oGroups)
                    {
                        oGroup = new DirectoryEntry(group);
                        if (oGroup.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                        {
                            bRet = true;
                            break;
                        }
                    }
                }
                catch { }
            }
            return bRet;
        }
    }
}
