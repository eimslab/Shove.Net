using System;
using System.Collections.Generic;
using System.Text;

namespace Shove._Security
{
    /// <summary>
    /// ShoveSoft License
    /// </summary>
    public class License
    {
        private static Boolean isRefreshed = false;

        private static Boolean domainNameAllow = false;
        private static Boolean webPagesAllow = false;
        private static Boolean adminPagesAllow = false;
        private static Boolean iOSAllow = false;
        private static Boolean andoridAllow = false;
        private static Boolean windowsPhoneAllow = false;

        private readonly static String key = "com.shove.security.License";

        /// <summary>
        /// 设置许可证
        /// </summary>
        /// <param name="license"></param>
        public static void Update(String license)
        {
            _Web.WebConfig.SetConfigKeyValue("AppSetting", key, license);
            Refresh(license);
        }

        private static void Refresh()
        {
            Refresh(_Web.WebConfig.GetAppSettingsString(key));
        }

        /// <summary>
        /// 刷新许可状态
        /// </summary>
        /// <param name="license"></param>
        public static void Refresh(String license)
        {
            isRefreshed = true;

            domainNameAllow = false;
            webPagesAllow = false;
            adminPagesAllow = false;
            iOSAllow = false;
            andoridAllow = false;
            windowsPhoneAllow = false;

            try
            {
                license = Encrypt.DecryptSES(license, "ZAQwsxCdeRFV1234", "utf-8");
            }
            catch //(Exception e)
            {
                return;
            }

            // 格式：2019-12-12 00:00:00;www.baidu.com,baidu.com;1;1;1;1;1
            if (!String.IsNullOrEmpty(license))
            {
                String[] strs = license.Split(';');

                if ((strs == null) || (strs.Length < 7))
                {
                    return;
                }

                DateTime dt = _Convert.StrToDateTime(strs[0], DateTime.Now.AddYears(-10).ToString());

                if (dt > DateTime.Now)
                {
                    String currentDomainName = _Web.Utility.GetUrlWithoutHttp();

                    domainNameAllow = ("," + strs[1] + ",").Contains("," + currentDomainName + ",");
                    webPagesAllow = (strs[2] == "1");
                    adminPagesAllow = (strs[3] == "1");
                    iOSAllow = (strs[4] == "1");
                    andoridAllow = (strs[5] == "1");
                    windowsPhoneAllow = (strs[6] == "1");
                }
            }
        }

        /// <summary>
        /// 获取域名是否在授权范围内
        /// </summary>
        /// <returns></returns>
        public static Boolean getDomainNameAllow()
        {
            if (!isRefreshed)
            {
                Refresh();
            }

            return domainNameAllow;
        }

        /// <summary>
        /// 获取前台页面许可
        /// </summary>
        /// <returns></returns>
        public static Boolean getWebPagesAllow()
        {
            if (!isRefreshed)
            {
                Refresh();
            }

            return webPagesAllow;
        }

        /// <summary>
        /// 获取后台页面许可
        /// </summary>
        /// <returns></returns>
        public static Boolean getAdminPagesAllow()
        {
            if (!isRefreshed)
            {
                Refresh();
            }

            return adminPagesAllow;
        }

        /// <summary>
        /// 获取 iOS 接口提供的许可
        /// </summary>
        /// <returns></returns>
        public static Boolean getiOSAllow()
        {
            if (!isRefreshed)
            {
                Refresh();
            }

            return iOSAllow;
        }

        /// <summary>
        /// 获取 Android 接口提供的许可
        /// </summary>
        /// <returns></returns>
        public static Boolean getAndoridAllow()
        {
            if (!isRefreshed)
            {
                Refresh();
            }

            return andoridAllow;
        }

        /// <summary>
        /// 获取 WindowsPhone 接口提供的许可
        /// </summary>
        /// <returns></returns>
        public static Boolean getWindowsPhoneAllow()
        {
            if (!isRefreshed)
            {
                Refresh();
            }

            return windowsPhoneAllow;
        }
    }
}