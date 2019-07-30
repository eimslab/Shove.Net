using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;

namespace Shove._Web
{
    /// <summary>
    /// 
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 获取当前站点的域名
        /// </summary>
        /// <returns></returns>
        public static string GetUrl()
        {
            return GetUrl(HttpContext.Current);
        }

        /// <summary>
        /// 获取当前站点的域名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUrl(HttpContext context)
        {
            string scheme = context.Request.Url.Scheme;
            string url = scheme + "://" + context.Request.Url.Host;
            int port = context.Request.Url.Port;

            if (((scheme == "http") && (port != 80)) || ((scheme == "https") && (port != 443)))
            {
                url += ":" + port.ToString();
            }

            string path = context.Request.ApplicationPath;

            if ((path != "") && (path != "/"))
            {
                url += path;
            }

            return url;
        }

        /// <summary>
        /// 获取当前站点的域名，但不附带前面的 http://
        /// </summary>
        /// <returns></returns>
        public static string GetUrlWithoutHttp()
        {
            return GetUrlWithoutHttp(HttpContext.Current);
        }

        /// <summary>
        /// 获取当前站点的域名，但不附带前面的 http://
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUrlWithoutHttp(HttpContext context)
        {
            string scheme = context.Request.Url.Scheme;
            string url = context.Request.Url.Host;
            int port = context.Request.Url.Port;

            if (((scheme == "http") && (port != 80)) || ((scheme == "https") && (port != 443)))
            {
                url += ":" + port.ToString();
            }

            string path = context.Request.ApplicationPath;
            if ((path != "") && (path != "/"))
            {
                url += path;
            }

            return url;
        }

        /// <summary>
        /// 获取 Url Request 参数，并过滤SQL注入
        /// </summary>
        /// <param name="Key">参数的Key</param>
        /// <returns>经过过滤后的参数值</returns>
        public static string GetRequest(string Key)
        {
            return GetRequest(HttpContext.Current, Key);
        }

        /// <summary>
        /// 获取 Url Request 参数，并过滤SQL注入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Key">参数的Key</param>
        /// <returns>经过过滤后的参数值</returns>
        public static string GetRequest(HttpContext context, string Key)
        {
            string Result = context.Request[Key];

            if (Result == null)
            {
                return "";
            }

            Result = System.Web.HttpUtility.UrlDecode(Result).Trim();

            if (Shove._Web.Security.InjectionInterceptor.__SYS_SHOVE_FLAG_IsUsed_InjectionInterceptor)
            {
                return Result;
            }
            else
            {
                return FilteSqlInfusion(Result);
            }
        }

        private static bool IsCloseMethod_Shove_FilteSqlInfusion = Read_IsCloseMethod_Shove_FilteSqlInfusion();
        private static bool Read_IsCloseMethod_Shove_FilteSqlInfusion()
        {
            bool result = false;
            string str = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("IsCloseMethod_Shove_FilteSqlInfusion");

            if (String.IsNullOrEmpty(str))
            {
                str = "false";
            }

            Boolean.TryParse(str, out result);

            return result;
        }

        /// <summary>
        /// 过滤Sql注入，html 编辑器的恶意代码注入
        /// </summary>
        /// <param name="input">SQL 语句或构成语句的某个部分的字符串</param>
        /// <returns></returns>
        public static string FilteSqlInfusion(string input)
        {
            return FilteSqlInfusion(input, true);
        }

        /// <summary>
        /// 过滤Sql注入，html 编辑器的恶意代码注入
        /// 如果 Web.Config 的 AppSetting 中包含 <add key="IsCloseMethod_Shove_FilteSqlInfusion" value="true" />, 则此方法不工作。
        /// </summary>
        /// <param name="input">SQL 语句或构成语句的某个部分的字符串</param>
        /// <param name="ReplaceSingleQuoteMark">是否替换单引号</param>
        /// <returns></returns>
        public static string FilteSqlInfusion(string input, bool ReplaceSingleQuoteMark)
        {
            if (IsCloseMethod_Shove_FilteSqlInfusion)
            {
                return input;
            }

            if (String.IsNullOrEmpty(input) || String.IsNullOrEmpty(input.Trim(new char[] { ' ', '　', '\t', '\r', '\n', '\v', '\f' })))
            {
                return "";
            }

            double d;
            if (Double.TryParse(input, out d))
            {
                return input;
            }

            if (ReplaceSingleQuoteMark)
            {
                input = input.Replace("'", "’");
            }

            MatchEvaluator evaluator = new MatchEvaluator(MatchEvaluator_FilteSqlInfusion);

            return Regex.Replace(input, @"(update[\t\s\r\n\f\v+　])|(drop[\t\s\r\n\f\v+　])|(delete[\t\s\r\n\f\v+　])|(exec[\t\s\r\n\f\v+　])|(create[\t\s\r\n\f\v+　])|(execute[\t\s\r\n\f\v+　])|(truncate[\t\s\r\n\f\v+　])|(insert[\t\s\r\n\f\v+　])", evaluator, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private static string MatchEvaluator_FilteSqlInfusion(Match match)
        {
            string m = match.Value;
            return _Convert.ToSBC(m.Substring(0, m.Length - 1)) + m[m.Length - 1].ToString();
        }

        /// <summary>
        /// 获取当前页面的实际文件名
        /// </summary>
        /// <returns></returns>
        public static string GetPageFileName()
        {
            return GetPageFileName(HttpContext.Current);
        }

        /// <summary>
        /// 获取当前页面的实际文件名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetPageFileName(HttpContext context)
        {
            return context.Request.ServerVariables["PATH_INFO"].Substring(context.Request.ServerVariables["PATH_INFO"].LastIndexOf("/") + 1);
        }

        #region Request 参数，MD5 Sign

        /// <summary>
        /// 获取冒泡排序法后的 Request Key。
        /// </summary>
        public static string[] GetRequestKeyAndSort()
        {
            HttpContext context = HttpContext.Current;

            if (context.Request.QueryString.Count < 1)
            {
                return null;
            }

            string[] r = new string[context.Request.QueryString.Count];

            HttpContext.Current.Request.QueryString.AllKeys.CopyTo(r, 0);

            return Sort(r, ' ');
        }

        /// <summary>
        /// 构建 REST 模式的网关 Url 的参数，将参数数组进行排序，签名，返回签名后的串
        /// </summary>
        /// <param name="Paramters"></param>
        /// <param name="Key"></param>
        /// <param name="SignatureParamteName"></param>
        /// <returns></returns>
        public static string BuildUrlParamtersAndSignature(string[] Paramters, string Key, string SignatureParamteName)
        {
            string[] SortedParamters = Sort(Paramters, '=');

            string SignatureSource = "";
            string Url = "";

            for (int i = 0; i < SortedParamters.Length; i++)
            {
                if (i > 0)
                {
                    Url += "&";
                }

                SignatureSource += SortedParamters[i].Split('=')[0] + HttpUtility.UrlDecode(SortedParamters[i].Split('=')[1]);
                Url += SortedParamters[i].Split('=')[0] + "=" + SortedParamters[i].Split('=')[1];
            }

            if (Url != "")
            {
                Url += "&";
            }

            Url += SignatureParamteName + "=" + Shove._Security.Encrypt.MD5(Key + SignatureSource);

            return Url;
        }

        /// <summary>
        /// 解析 REST 模式的网关 Url 的参数，检验 Sign，并返回获取到的参数
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="SignatureParamteName"></param>
        /// <param name="ErrorDescription"></param>
        /// <returns></returns>
        public static string[] GetUrlParamtersAndVasidSignature(string Key, string SignatureParamteName, ref string ErrorDescription)
        {
            ErrorDescription = "";

            string[] RequestKey = GetRequestKeyAndSort();

            if (RequestKey == null)
            {
                ErrorDescription = "没有找到参数";

                return null;
            }

            string Sign = Shove._Web.Utility.GetRequest(SignatureParamteName);

            if (String.IsNullOrEmpty(Sign))
            {
                ErrorDescription = "没有提供签名";

                return null;
            }

            string SignSourceString = Key;
            string[] Result = new string[RequestKey.Length - 1];
            int i = 0;

            foreach (string str in RequestKey)
            {
                if (str.Trim().ToLower() == SignatureParamteName.ToLower())
                {
                    continue;
                }

                string Value = Shove._Web.Utility.GetRequest(str);

                SignSourceString += (str + Value);
                Result[i++] = str + "=" + HttpUtility.UrlEncode(Value);
            }

            if (String.Compare(Shove._Security.Encrypt.MD5(SignSourceString), Sign, StringComparison.OrdinalIgnoreCase) != 0)
            {
                ErrorDescription = "签名无效";

                return null;
            }

            return Result;
        }

        /// <summary>
        /// 字符串数组排序
        /// </summary>
        /// <param name="input"></param>
        /// <param name="SplitChar"></param>
        /// <returns></returns>
        private static string[] Sort(string[] input, char SplitChar)
        {
            if ((input == null) || (input.Length < 2))
            {
                return input;
            }

            string[] r = new string[input.Length];
            input.CopyTo(r, 0);

            int i, j; //交换标志 
            string temp;

            bool exchange;

            for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假

                for (j = r.Length - 2; j >= i; j--)
                {
                    if (SplitChar == ' ')
                    {
                        if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)　//交换条件
                        {
                            exchange = true; //发生了交换，故将交换标志置为真 
                        }
                    }
                    else
                    {
                        if (System.String.CompareOrdinal(r[j + 1].Split(SplitChar)[0], r[j].Split(SplitChar)[0]) < 0)　//交换条件
                        {
                            exchange = true; //发生了交换，故将交换标志置为真 
                        }
                    }

                    if (exchange)
                    {
                        temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;
                    }
                }

                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }

            return r;
        }

        #endregion

        /// <summary>
        /// Post 提交
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="encodeing"></param>
        /// <param name="TimeoutSeconds"></param>
        /// <returns></returns>
        public static string Post(string Url, string encodeing, int TimeoutSeconds)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";

                if (TimeoutSeconds > 0)
                {
                    request.Timeout = 1000 * TimeoutSeconds;
                }

                request.AllowAutoRedirect = true;
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string CharSet = response.CharacterSet;

                    if (String.IsNullOrEmpty(encodeing))
                    {
                        if (String.IsNullOrEmpty(CharSet) || (CharSet == "ISO-8859-1"))
                        {
                            string head = response.Headers["Content-Type"];
                            Regex regex = new Regex(@"charset=[^""]?[""](?<G0>([^""]+?))[""]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            Match m = regex.Match(head);

                            if (m.Success)
                            {
                                CharSet = m.Groups["G0"].Value;
                            }
                        }

                        if (CharSet == "ISO-8859-1")
                        {
                            CharSet = "GB2312";
                        }
                        if (String.IsNullOrEmpty(CharSet))
                        {
                            CharSet = "UTF-8";
                        }
                    }

                    Stream s = null;

                    if (response.ContentEncoding.ToLower() == "gzip")
                    {
                        s = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                    }
                    else if (response.ContentEncoding.ToLower() == "deflate")
                    {
                        s = new System.IO.Compression.DeflateStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                    }
                    else
                    {
                        s = response.GetResponseStream();
                    }

                    reader = new StreamReader(s, System.Text.Encoding.GetEncoding(String.IsNullOrEmpty(encodeing) ? CharSet : encodeing));
                    string html = reader.ReadToEnd();

                    return html;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
    }
}