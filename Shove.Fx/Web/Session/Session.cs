using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;

namespace Shove._Web
{
    /// <summary>
    /// Session Read and Write Operate. Note: Add a ¡°SystemPreFix¡± key in Web.Config file.
    /// </summary>
    public partial class Session
    {
        /// <summary>
        /// SetSession
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetSession(string Key, object Value)
        {
            SetSession(HttpContext.Current, Key, Value);
        }

        /// <summary>
        /// SetSession
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetSession(HttpContext context, string Key, object Value)
        {
            Key = WebConfig.GetAppSettingsString("SystemPreFix") + Key;

            context.Session.Remove(Key);
            context.Session.Add(Key, Value);
        }

        /// <summary>
        /// GetSession
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static object GetSession(string Key)
        {
            return GetSession(HttpContext.Current, Key);
        }

        /// <summary>
        /// GetSession
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static object GetSession(HttpContext context, string Key)
        {
            Key = WebConfig.GetAppSettingsString("SystemPreFix") + Key;

            return context.Session[Key];
        }

        /// <summary>
        /// GetSessionAsInt
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int GetSessionAsInt(string Key, int Default)
        {
            return GetSessionAsInt(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// GetSessionAsInt
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int GetSessionAsInt(HttpContext context, string Key, int Default)
        {
            object Value = GetSession(context, Key);

            try
            {
                return Convert.ToInt32(Value);
            }
            catch
            {
                return Default;
            }
        }

        /// <summary>
        /// GetSessionAsLong
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static long GetSessionAsLong(string Key, long Default)
        {
            return GetSessionAsLong(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// GetSessionAsLong
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static long GetSessionAsLong(HttpContext context, string Key, long Default)
        {
            object Value = GetSession(context, Key);

            try
            {
                return Convert.ToInt64(Value);
            }
            catch
            {
                return Default;
            }
        }

        /// <summary>
        /// GetSessionAsString
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string GetSessionAsString(string Key, string Default)
        {
            return GetSessionAsString(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// GetSessionAsString
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string GetSessionAsString(HttpContext context, string Key, string Default)
        {
            object Value = GetSession(context, Key);

            try
            {
                return Value.ToString();
            }
            catch
            {
                return Default;
            }
        }

        /// <summary>
        /// GetSessionAsBoolean
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static bool GetSessionAsBoolean(string Key, bool Default)
        {
            return GetSessionAsBoolean(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// GetSessionAsBoolean
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static bool GetSessionAsBoolean(HttpContext context, string Key, bool Default)
        {
            object Value = GetSession(context, Key);

            try
            {
                return Convert.ToBoolean(Value);
            }
            catch
            {
                return Default;
            }
        }

        /// <summary>
        /// GetSessionAsDataSet
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataSet GetSessionAsDataSet(string Key)
        {
            return GetSessionAsDataSet(HttpContext.Current, Key);
        }

        /// <summary>
        /// GetSessionAsDataSet
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataSet GetSessionAsDataSet(HttpContext context, string Key)
        {
            object Value = GetSession(context, Key);

            try
            {
                return (DataSet)Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// GetSessionAsDataTable
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataTable GetSessionAsDataTable(string Key)
        {
            return GetSessionAsDataTable(HttpContext.Current, Key);
        }

        /// <summary>
        /// GetSessionAsDataTable
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataTable GetSessionAsDataTable(HttpContext context, string Key)
        {
            object Value = GetSession(context, Key);

            try
            {
                return (DataTable)Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ClearSession
        /// </summary>
        /// <param name="Key"></param>
        public static void ClearSession(string Key)
        {
            ClearSession(HttpContext.Current, Key);
        }

        /// <summary>
        /// ClearSession
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        public static void ClearSession(HttpContext context, string Key)
        {
            Key = WebConfig.GetAppSettingsString("SystemPreFix") + Key;

            context.Session.Remove(Key);
        }
    }
}
