//using System;
//using System.Management;
//using System.Text;
//using System.Runtime.InteropServices;
//using System.Collections;
//using System.Collections.Generic;

//namespace Shove._IO
//{
//    /// <summary>
//    /// SetDirRight 的摘要说明。
//    /// </summary>
//    public class SetFileSystemAccessRights
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public SetFileSystemAccessRights()
//        {
//            //
//            // TODO: 在此处添加构造函数逻辑
//            //
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="lpSystemName"></param>
//        /// <param name="lpAccountName"></param>
//        /// <param name="sid"></param>
//        /// <param name="cbSid"></param>
//        /// <param name="ReferencedDomainName"></param>
//        /// <param name="cbReferencedDomainName"></param>
//        /// <param name="peUse"></param>
//        /// <returns></returns>
//        [DllImport("advapi32.dll")]
//        public static extern bool LookupAccountName(string lpSystemName, string lpAccountName, byte[] sid, ref int cbSid, StringBuilder ReferencedDomainName, ref int cbReferencedDomainName, ref int peUse);

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="Dir"></param>
//        /// <param name="UserName"></param>
//        /// <returns></returns>
//        public static int Set(string Dir, string UserName)
//        {
//            if (GetFileSystem(Dir.Substring(0, 1)) != "NTFS")
//                return -1;

//            //if (FindTrustee(Dir, UserName))
//            //	return -2;

//            SetDACL(Dir, UserName, FindTrustee(Dir, UserName));
//            return 0;
//        }

//        private static string GetFileSystem(string diskName)
//        {
//            string fileSystem = "";

//            System.Management.ManagementObjectSearcher diskClass = new ManagementObjectSearcher("select filesystem from Win32_LogicalDisk where name='" + diskName + ":'");
//            ManagementObjectCollection disks = diskClass.Get();

//            foreach (ManagementObject disk in disks)
//            {
//                PropertyDataCollection diskProperties = disk.Properties;
//                foreach (PropertyData diskProperty in diskProperties)
//                {
//                    fileSystem = diskProperty.Value.ToString();
//                }
//            }

//            return fileSystem;
//        }

//        private static bool FindTrustee(string filePath, string userName)
//        {
//            ArrayList trusteesName = new ArrayList();
//            ManagementPath path = new ManagementPath();
//            path.Server = ".";
//            path.NamespacePath = @"root\cimv2";
//            path.RelativePath = @"Win32_LogicalFileSecuritySetting.Path='" + filePath + "'";							//定位到文件夹
//            ManagementObject dir = new ManagementObject(path);
//            ManagementBaseObject outParams = dir.InvokeMethod("GetSecurityDescriptor", null, null);						//获取安全描述符

//            if (((uint)(outParams.Properties["ReturnValue"].Value)) != 0)												//OK
//            {
//                throw new Exception("获取文件描述符失败");
//            }

//            ManagementBaseObject Descriptor = ((ManagementBaseObject)(outParams.Properties["Descriptor"].Value));
//            ManagementBaseObject[] DaclObject = ((ManagementBaseObject[])(Descriptor.Properties["Dacl"].Value));		//获取访问控制列表

//            for (int i = 0; i < DaclObject.Length; i++)
//            {
//                trusteesName.Add(((ManagementBaseObject)DaclObject[i].Properties["Trustee"].Value).Properties["Name"].Value);
//            }

//            return trusteesName.Contains(userName);
//        }

//        private static int SetDACL(string filePath, string userName, bool isUserExist)
//        {
//            //获取帐户信息
//            int cbSid = 100;
//            byte[] userSid = new byte[28];
//            StringBuilder domainName = new StringBuilder(255);
//            int domainNameLength = 255;
//            int sidType = 255;

//            if (!LookupAccountName(null, userName, userSid, ref cbSid, domainName, ref domainNameLength, ref sidType))
//            {
//                return -1;
//            }

//            //获取文件描述符
//            ManagementPath path = new ManagementPath();
//            path.Server = ".";
//            path.NamespacePath = @"root\cimv2";
//            path.RelativePath = @"Win32_LogicalFileSecuritySetting.Path='" + filePath + "'";
//            ManagementObject dir = new ManagementObject(path);
//            ManagementBaseObject outParams = dir.InvokeMethod("GetSecurityDescriptor", null, null);

//            if (((uint)(outParams.Properties["ReturnValue"].Value)) != 0)
//            {
//                return -2;	//"获取文件描述符失败";
//            }

//            ManagementBaseObject Descriptor = ((ManagementBaseObject)(outParams.Properties["Descriptor"].Value));
//            ManagementBaseObject[] DaclObject = ((ManagementBaseObject[])(Descriptor.Properties["Dacl"].Value));	//获取访问控制列表

//            //复制一份访问控制列表，最后一位空着。
//            ManagementBaseObject[] newDacl = new ManagementBaseObject[DaclObject.Length + (isUserExist ? 0 : 1)];
//            if (isUserExist)
//            {
//                int j = 0;
//                for (int i = 0; i < DaclObject.Length; i++)
//                {
//                    ManagementBaseObject t_ace = (ManagementBaseObject)DaclObject[i];
//                    ManagementBaseObject t_trustee = (ManagementBaseObject)t_ace.Properties["Trustee"].Value;

//                    if (t_trustee.Properties["Name"].Value.ToString().Trim().ToLower() == userName.Trim().ToLower())	//存在的用户
//                    {
//                        continue;
//                    }
//                    newDacl[j] = DaclObject[i];
//                    j++;
//                }
//            }
//            else
//            {
//                for (int i = 0; i < DaclObject.Length; i++)
//                {
//                    newDacl[i] = DaclObject[i];
//                }
//            }

//            ManagementBaseObject ace = (ManagementBaseObject)DaclObject[0].Clone();									//复制一个访问控制项
//            ManagementBaseObject trustee = (ManagementBaseObject)ace.Properties["Trustee"].Value;					//设置访问控制项属性

//            trustee.Properties["Domain"].Value = domainName.ToString();
//            trustee.Properties["Name"].Value = userName;
//            trustee.Properties["SID"].Value = userSid;
//            trustee.Properties["SidLength"].Value = 28;//trustee.Properties["SIDString"].Value="S-1-5-21-602162358-708899826-854245398-1005";

//            ace.Properties["Trustee"].Value = trustee;
//            ace.Properties["AccessMask"].Value = 1180063;	//2032127 是全部权限
//            ace.Properties["AceFlags"].Value = 3;
//            ace.Properties["AceType"].Value = 0;

//            //将新的用户设置增加到复制的访问控制列表最后一个元素
//            newDacl[DaclObject.Length + (isUserExist ? -1 : 0)] = ace;

//            //textBox2.Text = newDacl[0].Properties["AccessMask"].Value.ToString();
//            //将安全描述符的DACL属性设为新生成的访问控制列表
//            Descriptor.Properties["Dacl"].Value = newDacl;

//            //设置安全描述符
//            dir.Scope.Options.EnablePrivileges = true;
//            ManagementBaseObject inProperties = dir.GetMethodParameters("SetSecurityDescriptor");
//            inProperties["Descriptor"] = Descriptor;
//            outParams = dir.InvokeMethod("SetSecurityDescriptor", inProperties, null);

//            return 0;
//        }
//    }
//}