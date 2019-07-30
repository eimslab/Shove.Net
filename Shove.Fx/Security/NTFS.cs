using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.IO;
using System.DirectoryServices;

namespace Shove._Security
{
    /// <summary>
    /// FTNS 分区，文件、目录的权限控制类
    /// </summary>
    public class NTFS
    {
        #region Directory

        /// <summary>
        /// 添加 指定目录 指定用户 指定的 权限
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <param name="Account"></param>
        /// <param name="UserRights"></param>
        public static void AddDirectorySecurity(string DirectoryName, string Account, string UserRights)
        {
            if (!Directory.Exists(DirectoryName) || string.IsNullOrEmpty(Account))
            {
                return;
            }

            FileSystemRights Rights = new FileSystemRights();

            if (UserRights.IndexOf("R") >= 0)
            {
                Rights = Rights | FileSystemRights.Read;
            }
            if (UserRights.IndexOf("C") >= 0)
            {
                Rights = Rights | FileSystemRights.ChangePermissions;
            }
            if (UserRights.IndexOf("F") >= 0)
            {
                Rights = Rights | FileSystemRights.FullControl;
            }
            if (UserRights.IndexOf("W") >= 0)
            {
                Rights = Rights | FileSystemRights.Write;
            }
            if (UserRights.IndexOf("D") >= 0)
            {
                Rights = Rights | FileSystemRights.Delete;
            }

            bool ok;
            DirectoryInfo dInfo = new DirectoryInfo(DirectoryName);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            InheritanceFlags iFlags = new InheritanceFlags();
            iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            FileSystemAccessRule AccessRule2 = new FileSystemAccessRule(Account, Rights, iFlags, PropagationFlags.None, AccessControlType.Allow);
            dSecurity.ModifyAccessRule(AccessControlModification.Add, AccessRule2, out ok);
            dInfo.SetAccessControl(dSecurity);
        }

        /// <summary>
        /// 获取 指定目录 除Administrators和SYSTEM之外的 权限列表
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <returns></returns>
        public static List<StatussAuthorities> GetDirectoryAccountSecurity(string DirectoryName)
        {
            List<StatussAuthorities> dAccount = new List<StatussAuthorities>();
            DirectoryInfo dInfo = new DirectoryInfo(DirectoryName);

            if (dInfo.Exists)
            {
                DirectorySecurity sec = Directory.GetAccessControl(DirectoryName, AccessControlSections.All);

                foreach (FileSystemAccessRule rule in sec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                {
                    if (rule.IdentityReference.Value != @"NT AUTHORITY\SYSTEM" && rule.IdentityReference.Value != @"BUILTIN\Administrators")
                    {
                        dAccount.Add(new StatussAuthorities() { status = rule.IdentityReference.Value, authorities = rule.FileSystemRights.ToString() });
                    }
                }
            }

            return dAccount;
        }

        /// <summary>
        /// 移除 指定目录 指定用户的 权限
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <param name="Account"></param>
        public static void RemoveDirectoryAccountSecurity(string DirectoryName, string Account)
        {
            DirectoryInfo dInfo = new DirectoryInfo(DirectoryName);

            if (dInfo.Exists)
            {
                System.Security.Principal.NTAccount myAccount = new System.Security.Principal.NTAccount(System.Environment.MachineName, Account);

                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                FileSystemAccessRule AccessRule = new FileSystemAccessRule(Account, FileSystemRights.FullControl, AccessControlType.Allow);
                FileSystemAccessRule AccessRule2 = new FileSystemAccessRule(Account, FileSystemRights.FullControl, AccessControlType.Deny);

                InheritanceFlags iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
                PropagationFlags pFlags = PropagationFlags.InheritOnly | PropagationFlags.NoPropagateInherit;

                dSecurity.AccessRuleFactory(myAccount, 983551, false, iFlags, pFlags, AccessControlType.Allow);

                dSecurity.RemoveAccessRuleAll(AccessRule);
                dSecurity.RemoveAccessRuleAll(AccessRule2);

                dInfo.SetAccessControl(dSecurity);
            }
        }

        #endregion

        #region File

        /// <summary>
        /// 添加 指定文件 指定用户 指定的 权限
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Account"></param>
        /// <param name="UserRights"></param>
        public static void AddFileSecurity(string FileName, string Account, string UserRights)
        {
            if (!File.Exists(FileName) || string.IsNullOrEmpty(Account))
            {
                return;
            }

            FileSystemRights Rights = new FileSystemRights();

            if (UserRights.IndexOf("R") >= 0)
            {
                Rights = Rights | FileSystemRights.Read;
            }
            if (UserRights.IndexOf("C") >= 0)
            {
                Rights = Rights | FileSystemRights.ChangePermissions;
            }
            if (UserRights.IndexOf("F") >= 0)
            {
                Rights = Rights | FileSystemRights.FullControl;
            }
            if (UserRights.IndexOf("W") >= 0)
            {
                Rights = Rights | FileSystemRights.Write;
            }
            if (UserRights.IndexOf("D") >= 0)
            {
                Rights = Rights | FileSystemRights.Delete;
            }

            bool ok;
            FileInfo fInfo = new FileInfo(FileName);
            FileSecurity fSecurity = fInfo.GetAccessControl();
            InheritanceFlags iFlags = new InheritanceFlags();
            iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            FileSystemAccessRule AccessRule2 = new FileSystemAccessRule(Account, Rights, iFlags, PropagationFlags.None, AccessControlType.Allow);
            fSecurity.ModifyAccessRule(AccessControlModification.Add, AccessRule2, out ok);
            fInfo.SetAccessControl(fSecurity);
        }

        /// <summary>
        /// 获取 指定文件 除Administrators和SYSTEM之外的 权限列表
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static List<string> GetFileAccountSecurity(string FileName)
        {
            List<string> fAccount = new List<string>();
            FileInfo fInfo = new FileInfo(FileName);

            if (fInfo.Exists)
            {
                FileSecurity fec = File.GetAccessControl(FileName, AccessControlSections.All);

                foreach (FileSystemAccessRule rule in fec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                {
                    if (rule.IdentityReference.Value != @"NT AUTHORITY\SYSTEM" && rule.IdentityReference.Value != @"BUILTIN\Administrators")
                        fAccount.Add(rule.IdentityReference.Value);
                }
            }

            return fAccount;
        }

        /// <summary>
        /// 移除 指定文件 指定用户的 权限
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Account"></param>
        public static void RemoveFileAccountSecurity(string FileName, string Account)
        {
            FileInfo fInfo = new FileInfo(FileName);

            if (fInfo.Exists)
            {
                FileSecurity fSecurity = fInfo.GetAccessControl();
                FileSystemAccessRule AccessRule = new FileSystemAccessRule(Account, FileSystemRights.FullControl, AccessControlType.Allow);
                FileSystemAccessRule AccessRule2 = new FileSystemAccessRule(Account, FileSystemRights.FullControl, AccessControlType.Deny);
                fSecurity.RemoveAccessRuleAll(AccessRule);
                fSecurity.RemoveAccessRuleAll(AccessRule2);
                fInfo.SetAccessControl(fSecurity);
            }
        }

        #endregion
    }

    /// <summary>
    /// NTFS 类使用
    /// </summary>
    public class StatussAuthorities
    {
        internal string status { get; set; }
        internal string authorities { get; set; }
    }
}
