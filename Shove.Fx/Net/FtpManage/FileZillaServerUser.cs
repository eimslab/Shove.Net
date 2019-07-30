using System;
using System.Collections.Generic;
using System.Text;

namespace Shove._Net.FtpManage
{
    /// <summary>
    /// FileZillaServer 用户信息
    /// </summary>
    public class FileZillaServerUser
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name;
        /// <summary>
        /// 密码，复制密码原文后，应立即调用 FileZillaServerUser.HashPassword 的方法进行处理
        /// </summary>
        public string Password;
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// FTP 主目录
        /// </summary>
        public string Directory;

        /// <summary>
        /// FileZillaServer 用户密码加密方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashPassword(string input)
        {
            return Shove . _Security.Encrypt.MD5(input,"gb2312");
        }

        /// <summary>
        /// 校验 FileZillaServer 用户信息是否完整
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool ValidUserInfo(FileZillaServerUser user)
        {
            return (String.IsNullOrEmpty(user.Name) || String.IsNullOrEmpty(user.Password) || String.IsNullOrEmpty(user.Directory));
        }
    }
}