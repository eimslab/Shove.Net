using System;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Collections.Generic;

namespace Shove.Database
{
    /// <summary>
    /// Shove 的数据库访问组件类的基类
    /// </summary>
    public class DatabaseAccess
    {
        /// <summary>
        /// 
        /// </summary>
        protected const string DesKey = "Q56GtyNkop97Ht334Ttyurfg";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static string GetCallCert()
        {
            string Result = System.Configuration.ConfigurationManager.AppSettings["DllCallCert"];

            if (String.IsNullOrEmpty(Result))
            {
                Result = GetCallCert_Default();
            }

            return Result;
        }

        private static string GetCallCert_Default()
        {
            //"ShoveSoft CO.,Ltd -- by Shove 20050709 深圳宝安"
            string Result = "";

            Result = "ShoveSoft";
            Result += " ";
            Result += "CO.,Ltd ";

            string Result2 = _String.Reverse(Result);

            Result = "--";
            Result += " by Shove ";

            Result = _String.Reverse(Result2) + Result;

            Result2 = _String.Reverse(Result);

            Result = "20050709";
            Result += _String.Reverse("圳深 ");
            Result += _String.Reverse("安宝");

            Result = _String.Reverse(Result);

            Result = _String.Reverse(Result2) + _String.Reverse(Result);

            return Result;
        }

        #region ConnectString

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static string GetConnectionStringFromConfig()
        {
            string Result = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            if (Result == null)
            {
                Result = "";
            }

            return Result;
        }

        /// <summary>
        /// 创建一个连接，从 Web.Config 中的连接串。
        /// </summary>
        /// <returns></returns>
        public static T CreateDataConnection<T>() where T : System.Data.Common.DbConnection, new()
        {
            return (T)CreateDataConnection<T>(GetConnectionStringFromConfig());
        }

        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static T CreateDataConnection<T>(string ConnectionString) where T : System.Data.Common.DbConnection, new()
        {
            if (ConnectionString.StartsWith("0x78AD"))
            {
                ConnectionString = _Security.Encrypt.Decrypt3DES(GetCallCert(), ConnectionString.Substring(6), DesKey);
            }

            T conn = new T(); //new T(ConnectionString);
            conn.ConnectionString = ConnectionString;

            try
            {
                conn.Open();
            }
            catch//(Exception e)
            {
                //throw new Exception(e.Message);
                return null;
            }

            return conn;
        }

        #endregion

        /// <summary>
        /// 过滤 Sql 注入，过滤 condition 等 html 编辑器的恶意代码注入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected static string FilteSqlInfusionForCondition(string input)
        {
            if (Shove._Web.Security.InjectionInterceptor.__SYS_SHOVE_FLAG_IsUsed_InjectionInterceptor)
            {
                return input;
            }
            else
            {
                return _Web.Utility.FilteSqlInfusion(input, false);
            }
        }
    }
}