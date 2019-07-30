using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;

namespace Shove._Web
{
    /// <summary>
    /// Cache Read Write Operate. Note: Add ¡°SystemPreFix¡±¡°CacheSeconds¡± keys in Web.Config file.
    /// </summary>
    public class Cache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetCache(string Key, object Value)
        {
            SetCache(HttpContext.Current, Key, Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetCache(HttpContext context, string Key, object Value)
        {
            int CacheSeconds = WebConfig.GetAppSettingsInt("CacheSeconds", 0);

            SetCache(context, Key, Value, CacheSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="CacheSeconds"></param>
        public static void SetCache(string Key, object Value, int CacheSeconds)
        {
            SetCache(HttpContext.Current, Key, Value, CacheSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="CacheSeconds"></param>
        public static void SetCache(HttpContext context, string Key, object Value, int CacheSeconds)
        {
            Key = WebConfig.GetAppSettingsString("SystemPreFix") + Key;

            context.Cache.Remove(Key);

            if (CacheSeconds <= 0)
            {
                return;
            }

            context.Cache.Insert(Key, Value, null, DateTime.Now.AddSeconds(CacheSeconds), System.Web.Caching.Cache.NoSlidingExpiration); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static object GetCache(string Key)
        {
            return GetCache(HttpContext.Current, Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static object GetCache(HttpContext context, string Key)
        {
            Key = WebConfig.GetAppSettingsString("SystemPreFix") + Key;

            int CacheSeconds = WebConfig.GetAppSettingsInt("CacheSeconds", 0);

            if (CacheSeconds <= 0)
            {
                context.Cache.Remove(Key);
                return null;
            }

            return context.Cache[Key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int GetCacheAsInt(string Key, int Default)
        {
            return GetCacheAsInt(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int GetCacheAsInt(HttpContext context, string Key, int Default)
        {
            object Value = GetCache(context, Key);

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
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static long GetCacheAsLong(string Key, long Default)
        {
            return GetCacheAsLong(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static long GetCacheAsLong(HttpContext context, string Key, long Default)
        {
            object Value = GetCache(context, Key);

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
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string GetCacheAsString(string Key, string Default)
        {
            return GetCacheAsString(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string GetCacheAsString(HttpContext context, string Key, string Default)
        {
            object Value = GetCache(context, Key);

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
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static bool GetCacheAsBoolean(string Key, bool Default)
        {
            return GetCacheAsBoolean(HttpContext.Current, Key, Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static bool GetCacheAsBoolean(HttpContext context, string Key, bool Default)
        {
            object Value = GetCache(context, Key);

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
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataSet GetCacheAsDataSet(string Key)
        {
            return GetCacheAsDataSet(HttpContext.Current, Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataSet GetCacheAsDataSet(HttpContext context, string Key)
        {
            object Value = GetCache(context, Key);

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
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataTable GetCacheAsDataTable(string Key)
        {
            return GetCacheAsDataTable(HttpContext.Current, Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static DataTable GetCacheAsDataTable(HttpContext context, string Key)
        {
            object Value = GetCache(context, Key);

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
        /// 
        /// </summary>
        /// <param name="Key"></param>
        public static void ClearCache(string Key)
        {
            ClearCache(HttpContext.Current, Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key"></param>
        public static void ClearCache(HttpContext context, string Key)
        {
            Key = WebConfig.GetAppSettingsString("SystemPreFix") + Key;

            context.Cache.Remove(Key);
        }
    }
}
