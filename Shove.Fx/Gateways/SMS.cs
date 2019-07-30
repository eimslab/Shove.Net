using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Shove.Gateways
{
    /// <summary>
    /// 访问 sms.gateway.i3km.com 封装的短信网关
    /// </summary>
    public class SMS
    {
        /// <summary>
        /// 发送短信。如果 App.Config 或者 Web.Config 的 AppSetting 中没有 Key="I3kmSMS_GatewayServiceUrl" value="" 的设置，将使用默认的网关地址。 
        /// </summary>
        /// <param name="RegCode"></param>
        /// <param name="RegKey"></param>
        /// <param name="Content"></param>
        /// <param name="To"></param>
        /// <returns></returns>
        public static DataSet SendSMS(string RegCode, string RegKey, string Content, string To)
        {
            return SendSMS(RegCode, RegKey, Content, To, DateTime.Now);
        }

        /// <summary>
        /// 发送短信。如果 App.Config 或者 Web.Config 的 AppSetting 中没有 Key="I3kmSMS_GatewayServiceUrl" value="" 的设置，将使用默认的网关地址。 
        /// </summary>
        /// <param name="RegCode"></param>
        /// <param name="RegKey"></param>
        /// <param name="Content"></param>
        /// <param name="To"></param>
        /// <param name="SendTime">指定的发送时间，可以实现按指定的发送时间再进行发送的功能</param>
        /// <returns></returns>
        public static DataSet SendSMS(string RegCode, string RegKey, string Content, string To, DateTime SendTime)
        {
            I3kmSMS_GatewayService.sms_gateway service = null;

            try
            {
                service = new I3kmSMS_GatewayService.sms_gateway();
                string url = System.Configuration.ConfigurationManager.AppSettings["I3kmSMS_GatewayServiceUrl"];

                if (!String.IsNullOrEmpty(url))
                {
                    service.Url = url;
                }
            }
            catch
            {
                service.Dispose();

                return null;
            }

            DateTime TimeStamp = DateTime.Now;

            string Sign = Shove._Security.Encrypt.MD5(RegCode + TimeStamp.ToString() + Content + To + SendTime.ToString("yyyyMMdd HHmmss") + RegKey);
            DataSet ds = null;

            try
            {
                ds = service.SendSMS_2(RegCode, TimeStamp.ToString(), Sign, Content, To, SendTime);
            }
            catch
            {
                service.Dispose();

                return null;
            }

            service.Dispose();

            return ds;
        }

        /// <summary>
        /// 查询短信账户余额。如果 App.Config 或者 Web.Config 的 AppSetting 中没有 Key="I3kmSMS_GatewayServiceUrl" value="" 的设置，将使用默认的网关地址。 
        /// </summary>
        /// <param name="RegCode"></param>
        /// <param name="RegKey"></param>
        /// <returns></returns>
        public static DataSet GetBalance(string RegCode, string RegKey)
        {
            I3kmSMS_GatewayService.sms_gateway service = null;

            try
            {
                service = new I3kmSMS_GatewayService.sms_gateway();
                string url = System.Configuration.ConfigurationManager.AppSettings["I3kmSMS_GatewayServiceUrl"];

                if (!String.IsNullOrEmpty(url))
                {
                    service.Url = url;
                }
            }
            catch
            {
                service.Dispose();

                return null;
            }

            DateTime TimeStamp = DateTime.Now;

            string Sign = Shove._Security.Encrypt.MD5(RegCode + TimeStamp.ToString() + RegKey);
            DataSet ds = null;

            try
            {
                ds = service.QueryBalance(RegCode, TimeStamp.ToString(), Sign);
            }
            catch
            {
                service.Dispose();

                return null;
            }

            service.Dispose();

            return ds;
        }
    }
}
