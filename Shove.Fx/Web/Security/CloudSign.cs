using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Shove._Web.Security
{
    /// <summary>
    /// 云签名安全机制，用户防止数据伪造
    /// </summary>
    public class CloudSign
    {
        private static string pid = _Web.WebConfig.GetAppSettingsString("ShoveCloudSignPid").Trim();
        private static string key = _Web.WebConfig.GetAppSettingsString("ShoveCloudSignKey").Trim();

        private static void validPidAndKey()
        {
            if (String.IsNullOrEmpty(pid) || String.IsNullOrEmpty(key))
            {
                throw new Exception("ShoveCloudSign configuration error.");
            }
        }

        /// <summary>
        /// 提交签名数据
        /// </summary>
        /// <param name="eid">平台上需要云签名的事件类型</param>
        /// <param name="did">需要签名的数据行ID</param>
        /// <param name="d">签名数据</param>
        /// <param name="cs">云签名系统返回的数字签名</param>
        /// <param name="errorDescription"></param>
        /// <returns></returns>
        public static int putData(int eid, int did, string d, ref string cs, ref string errorDescription)
        {
            cs = "";
            errorDescription = "";

            validPidAndKey();
            string t = DateTime.Now.ToString("yyyyMMddHHmmss");
            string signData = pid + eid.ToString() + did.ToString() + d + t + key;
            string result = Shove._Web.Utility.Post("http://ds.eims.com.cn/p.do?pid=" + pid +
                "&eid=" + eid.ToString() + "&did=" + did.ToString() + "&d=" + d +
                "&t=" + t + "&sign=" + Shove._Security.Encrypt.MD5(signData, "utf-8"),
                "utf-8", 30);

            if (String.IsNullOrEmpty(result))
            {
                throw new Exception("access ds.eims.com.cn is error.");
            }

            result = result.Trim();

            Regex regex = new Regex(@"(?<CODE>[-]*[\d]+)[,](?<MESSAGE>[\S\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = regex.Match(result);

            if (!m.Success)
            {
                throw new Exception(result);
            }

            int r = int.Parse(m.Groups["CODE"].Value);

            if (r == 0)
            {
                cs = m.Groups["MESSAGE"].Value;
            }
            else
            {
                errorDescription = m.Groups["MESSAGE"].Value;
            }

            return r;
        }

        /// <summary>
        /// 校验签名数据
        /// </summary>
        /// <param name="eid">平台上需要云签名的事件类型</param>
        /// <param name="did">签名的数据行ID</param>
        /// <param name="d">签名数据</param>
        /// <param name="cs">提供验证的云签名</param>
        /// <param name="errorDescription"></param>
        /// <returns></returns>
        public static int verifyData(int eid, int did, string d, string cs, ref string errorDescription)
        {
            errorDescription = "";

            validPidAndKey();
            string t = DateTime.Now.ToString("yyyyMMddHHmmss");
            string signData = pid + eid.ToString() + did.ToString() + d + cs + t + key;
            string result = Shove._Web.Utility.Post("http://ds.eims.com.cn/v.do?pid=" + pid +
                "&eid=" + eid.ToString() + "&did=" + did.ToString() + "&d=" + d + "&cs=" + cs +
                "&t=" + t + "&sign=" + Shove._Security.Encrypt.MD5(signData, "utf-8"),
                "utf-8", 30);

            if (String.IsNullOrEmpty(result))
            {
                throw new Exception("access ds.eims.com.cn is error.");
            }

            result = result.Trim();

            if (result == "0")
            {
                return 0;
            }

            Regex regex = new Regex(@"(?<CODE>[-]*[\d]+)[,](?<DESCRIPTION>[\S\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match m = regex.Match(result);

            if (!m.Success)
            {
                throw new Exception(result);
            }

            int r = int.Parse(m.Groups["CODE"].Value);
            errorDescription = m.Groups["DESCRIPTION"].Value;

            return r;
        }
    }
}
