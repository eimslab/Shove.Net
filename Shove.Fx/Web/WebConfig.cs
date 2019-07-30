using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;

namespace Shove._Web
{
    /// <summary>
    /// Web.config 操作类
    /// 定义为不可继承性
    /// </summary>
    public sealed class WebConfig
    {
        /// <summary>
        /// GetAppSettingsString
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetAppSettingsString(string Key)
        {
            string Result = "";
            try
            {
                Result = WebConfigurationManager.AppSettings.Get(Key).Trim();
            }
            catch { }

            return Result;
        }

        /// <summary>
        /// GetAppSettingsBool
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static bool GetAppSettingsBool(string Key)
        {
            return GetAppSettingsBool(Key, false);
        }

        /// <summary>
        /// GetAppSettingsBool
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static bool GetAppSettingsBool(string Key, bool Default)
        {
            return Shove._Convert.StrToBool(WebConfigurationManager.AppSettings.Get(Key), Default);
        }

        /// <summary>
        /// GetAppSettingsInt
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static int GetAppSettingsInt(string Key)
        {
            return GetAppSettingsInt(Key, 0);
        }

        /// <summary>
        /// GetAppSettingsInt
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int GetAppSettingsInt(string Key, int Default)
        {
            return Shove._Convert.StrToInt(WebConfigurationManager.AppSettings.Get(Key), Default);
        }

        /// <summary>
        /// GetAppSettingsDouble
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static double GetAppSettingsDouble(string Key)
        {
            return GetAppSettingsDouble(Key, 0);
        }

        /// <summary>
        /// GetAppSettingsDouble
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Defalut"></param>
        /// <returns></returns>
        public static double GetAppSettingsDouble(string Key, double Defalut)
        {
            return Shove._Convert.StrToDouble(WebConfigurationManager.AppSettings.Get(Key).Trim(), Defalut);
        }

        //////////////////////////////////////////////////////////////
        //下面是 Web.Config的读写

        /// <summary>
        /// GetConfigString
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetConfigString(string SectionName, string Key)
        {
            if (SectionName == null || SectionName == "")
            {
                NameValueCollection cfgName = WebConfigurationManager.AppSettings;
                if (cfgName[Key] == null || cfgName[Key] == "")
                {
                    throw (new Exception("在 Web.config 文件中未发现配置项: \"" + Key + "\""));
                }
                else
                {
                    return cfgName[Key];
                }
            }
            else
            {
                NameValueCollection cfgName = (NameValueCollection)WebConfigurationManager.GetSection(SectionName);
                if (cfgName[Key] == null || cfgName[Key] == "")
                {
                    throw (new Exception("在 Web.config 文件中未发现配置项: \"" + Key + "\""));
                }
                else
                {
                    return cfgName[Key];
                }
            }
        }

        /// <summary>
        /// 得到配置文件中的配置decimal信息
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static decimal GetConfigDecimal(string SectionName, string Key)
        {
            decimal result = 0;
            string cfgVal = GetConfigString(SectionName, Key);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                result = decimal.Parse(cfgVal);
            }

            return result;
        }

        /// <summary>
        /// 得到配置文件中的配置 int 信息
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static int GetConfigInt(string SectionName, string Key)
        {
            int result = 0;
            string cfgVal = GetConfigString(SectionName, Key);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                result = Int32.Parse(cfgVal);
            }

            return result;
        }

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetConfigKeyValue(string SectionName, string Key, string Value)
        {
            SetConfigKeyValue(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, SectionName, Key, Value);
        }

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="ConfigFileName"></param>
        /// <param name="SectionName"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetConfigKeyValue(string ConfigFileName, string SectionName, string Key, string Value)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(ConfigFileName);
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }

            XmlNode node = doc.SelectSingleNode("//" + SectionName);

            if (node == null)
            {
                throw new InvalidOperationException(SectionName + " section not found in config file.");
            }

            try
            {
                XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", Key));

                if (elem != null)
                {
                    elem.SetAttribute("value", Value);
                }
                else
                {
                    elem = doc.CreateElement("add");
                    elem.SetAttribute("key", Key);
                    elem.SetAttribute("value", Value);
                    node.AppendChild(elem);
                }
                doc.Save(ConfigFileName);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// RemoveSectionKey
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="Key"></param>
        public static void RemoveSectionKey(string SectionName, string Key)
        {
            RemoveSectionKey(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, SectionName, Key);
        }

        /// <summary>
        /// RemoveSectionKey
        /// </summary>
        /// <param name="ConfigFileName"></param>
        /// <param name="SectionName"></param>
        /// <param name="Key"></param>
        public static void RemoveSectionKey(string ConfigFileName, string SectionName, string Key)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(ConfigFileName);
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }

            XmlNode node = doc.SelectSingleNode("//" + SectionName);

            if (node == null)
            {
                throw new InvalidOperationException(SectionName + " section not found in config file.");
            }

            try
            {
                node.RemoveChild(node.SelectSingleNode(string.Format("//add[@key='{0}']", Key)));
                doc.Save(ConfigFileName);
            }
            catch (NullReferenceException e)
            {
                throw new Exception(string.Format("The key {0} does not exist.", Key), e);
            }
        }
    }
}