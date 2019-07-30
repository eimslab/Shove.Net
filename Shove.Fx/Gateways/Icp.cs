using System;
using System.Collections.Generic;
using System.Text;

namespace Shove.Gateways
{
    /// <summary>
    /// 
    /// </summary>
    public class Icp
    {
        /// <summary>
        /// 工信部实时域名备案状态查询(通过英迈思的备案系统接口)
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string IcpBeianQueryRealTime(string domainName)
        {
            com_shovesoft_icp.Gateway service = null;

            try
            {
                service = new com_shovesoft_icp.Gateway();

                string url = System.Configuration.ConfigurationManager.AppSettings["Icp_GatewayServiceUrl"];

                if (!String.IsNullOrEmpty(url))
                {
                    service.Url = url;
                }
            }
            catch
            {
                service.Dispose();

                return "错误：初始化接口出错。";
            }

            string result = "";

            try
            {
                result = service.IcpBeianQueryRealTime("eimslab", domainName);
            }
            catch
            {
                service.Dispose();

                return "错误：初始化接口出错。";
            }

            service.Dispose();

            return result;
        }
    }
}
