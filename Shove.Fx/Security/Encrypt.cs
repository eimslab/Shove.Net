using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using System.Security.Cryptography;
using System.Data;

namespace Shove._Security
{
    /// <summary>
    /// 字符串加密、解密函数
    /// </summary>
    public class Encrypt
    {
        private static string GetCallCert()
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

        #region ReplaceKey

        private class ReplaceKey_01
        {
            public string[,] Key;

            public ReplaceKey_01()
            {
                Key = new string[2, 84];

                Key[0, 0] = "00"; Key[1, 0] = "A";
                Key[0, 1] = "11"; Key[1, 1] = "B";
                Key[0, 2] = "22"; Key[1, 2] = "C";
                Key[0, 3] = "33"; Key[1, 3] = "D";
                Key[0, 4] = "44"; Key[1, 4] = "E";
                Key[0, 5] = "55"; Key[1, 5] = "F";
                Key[0, 6] = "66"; Key[1, 6] = "G";
                Key[0, 7] = "77"; Key[1, 7] = "H";
                Key[0, 8] = "88"; Key[1, 8] = "I";
                Key[0, 9] = "99"; Key[1, 9] = "J";

                Key[0, 10] = "10"; Key[1, 10] = "K";
                Key[0, 11] = "20"; Key[1, 11] = "L";
                Key[0, 12] = "30"; Key[1, 12] = "M";
                Key[0, 13] = "40"; Key[1, 13] = "N";
                Key[0, 14] = "50"; Key[1, 14] = "O";
                Key[0, 15] = "60"; Key[1, 15] = "P";
                Key[0, 16] = "70"; Key[1, 16] = "Q";
                Key[0, 17] = "80"; Key[1, 17] = "R";
                Key[0, 18] = "90"; Key[1, 18] = "S";

                Key[0, 19] = "01"; Key[1, 19] = "T";
                Key[0, 20] = "21"; Key[1, 20] = "U";
                Key[0, 21] = "31"; Key[1, 21] = "V";
                Key[0, 22] = "41"; Key[1, 22] = "W";
                Key[0, 23] = "51"; Key[1, 23] = "X";
                Key[0, 24] = "61"; Key[1, 24] = "Y";
                Key[0, 25] = "71"; Key[1, 25] = "Z";
                Key[0, 26] = "81"; Key[1, 26] = "a";
                Key[0, 27] = "91"; Key[1, 27] = "b";

                Key[0, 28] = "02"; Key[1, 28] = "c";
                Key[0, 29] = "12"; Key[1, 29] = "d";
                Key[0, 30] = "32"; Key[1, 30] = "e";
                Key[0, 31] = "42"; Key[1, 31] = "f";
                Key[0, 32] = "52"; Key[1, 32] = "g";
                Key[0, 33] = "62"; Key[1, 33] = "h";
                Key[0, 34] = "72"; Key[1, 34] = "i";
                Key[0, 35] = "82"; Key[1, 35] = "j";
                Key[0, 36] = "92"; Key[1, 36] = "k";

                Key[0, 37] = "03"; Key[1, 37] = "l";
                Key[0, 38] = "13"; Key[1, 38] = "m";
                Key[0, 39] = "23"; Key[1, 39] = "n";
                Key[0, 40] = "43"; Key[1, 40] = "o";
                Key[0, 41] = "53"; Key[1, 41] = "p";
                Key[0, 42] = "63"; Key[1, 42] = "q";
                Key[0, 43] = "73"; Key[1, 43] = "r";
                Key[0, 44] = "83"; Key[1, 44] = "s";
                Key[0, 45] = "93"; Key[1, 45] = "t";

                Key[0, 46] = "04"; Key[1, 46] = "u";
                Key[0, 47] = "14"; Key[1, 47] = "v";
                Key[0, 48] = "24"; Key[1, 48] = "w";
                Key[0, 49] = "34"; Key[1, 49] = "x";
                Key[0, 50] = "54"; Key[1, 50] = "y";
                Key[0, 51] = "64"; Key[1, 51] = "z";
                Key[0, 52] = "74"; Key[1, 52] = "!";
                Key[0, 53] = "84"; Key[1, 53] = "@";
                Key[0, 54] = "94"; Key[1, 54] = "#";

                Key[0, 55] = "05"; Key[1, 55] = "$";
                Key[0, 56] = "15"; Key[1, 56] = "%";
                Key[0, 57] = "25"; Key[1, 57] = "^";
                Key[0, 58] = "35"; Key[1, 58] = "&";
                Key[0, 59] = "45"; Key[1, 59] = "*";
                Key[0, 60] = "65"; Key[1, 60] = "(";
                Key[0, 61] = "75"; Key[1, 61] = ")";
                Key[0, 62] = "85"; Key[1, 62] = "_";
                Key[0, 63] = "95"; Key[1, 63] = "-";

                Key[0, 64] = "06"; Key[1, 64] = "+";
                Key[0, 65] = "16"; Key[1, 65] = "=";
                Key[0, 66] = "26"; Key[1, 66] = "|";
                Key[0, 67] = "36"; Key[1, 67] = "\\";
                Key[0, 68] = "46"; Key[1, 68] = "<";
                Key[0, 69] = "56"; Key[1, 69] = ">";
                Key[0, 70] = "76"; Key[1, 70] = ",";
                Key[0, 71] = "86"; Key[1, 71] = ".";
                Key[0, 72] = "96"; Key[1, 72] = "?";

                Key[0, 73] = "07"; Key[1, 73] = "/";
                Key[0, 74] = "17"; Key[1, 74] = "[";
                Key[0, 75] = "27"; Key[1, 75] = "]";
                Key[0, 76] = "37"; Key[1, 76] = "{";
                Key[0, 77] = "47"; Key[1, 77] = "}";
                Key[0, 78] = "57"; Key[1, 78] = ":";
                Key[0, 79] = "67"; Key[1, 79] = ";";
                Key[0, 80] = "87"; Key[1, 80] = "\"";
                Key[0, 81] = "97"; Key[1, 81] = "\'";

                Key[0, 82] = "08"; Key[1, 82] = "`";
                Key[0, 83] = "18"; Key[1, 83] = "~";
            }
        }

        private class ReplaceKey_04
        {
            public string[,] Key;

            public ReplaceKey_04()
            {
                Key = new string[2, 84];

                Key[0, 0] = "00"; Key[1, 0] = "S";
                Key[0, 1] = "11"; Key[1, 1] = "B";
                Key[0, 2] = "22"; Key[1, 2] = "H";
                Key[0, 3] = "33"; Key[1, 3] = "e";
                Key[0, 4] = "44"; Key[1, 4] = "F";
                Key[0, 5] = "55"; Key[1, 5] = "E";
                Key[0, 6] = "66"; Key[1, 6] = "G";
                Key[0, 7] = "77"; Key[1, 7] = "z";
                Key[0, 8] = "88"; Key[1, 8] = "I";
                Key[0, 9] = "99"; Key[1, 9] = "b";

                Key[0, 10] = "10"; Key[1, 10] = "K";
                Key[0, 11] = "20"; Key[1, 11] = "L";
                Key[0, 12] = "30"; Key[1, 12] = "g";
                Key[0, 13] = "40"; Key[1, 13] = "N";
                Key[0, 14] = "50"; Key[1, 14] = "l";
                Key[0, 15] = "60"; Key[1, 15] = "n";
                Key[0, 16] = "70"; Key[1, 16] = "Q";
                Key[0, 17] = "80"; Key[1, 17] = "R";
                Key[0, 18] = "90"; Key[1, 18] = "a";

                Key[0, 19] = "01"; Key[1, 19] = "T";
                Key[0, 20] = "21"; Key[1, 20] = "U";
                Key[0, 21] = "31"; Key[1, 21] = "j";
                Key[0, 22] = "41"; Key[1, 22] = "W";
                Key[0, 23] = "51"; Key[1, 23] = "X";
                Key[0, 24] = "61"; Key[1, 24] = "w";
                Key[0, 25] = "71"; Key[1, 25] = "Z";
                Key[0, 26] = "81"; Key[1, 26] = "A";
                Key[0, 27] = "91"; Key[1, 27] = "J";

                Key[0, 28] = "02"; Key[1, 28] = "c";
                Key[0, 29] = "12"; Key[1, 29] = "d";
                Key[0, 30] = "32"; Key[1, 30] = "D";
                Key[0, 31] = "42"; Key[1, 31] = "f";
                Key[0, 32] = "52"; Key[1, 32] = "M";
                Key[0, 33] = "62"; Key[1, 33] = "h";
                Key[0, 34] = "72"; Key[1, 34] = "i";
                Key[0, 35] = "82"; Key[1, 35] = "V";
                Key[0, 36] = "92"; Key[1, 36] = "k";

                Key[0, 37] = "03"; Key[1, 37] = "O";
                Key[0, 38] = "13"; Key[1, 38] = "m";
                Key[0, 39] = "23"; Key[1, 39] = "P";
                Key[0, 40] = "43"; Key[1, 40] = "o";
                Key[0, 41] = "53"; Key[1, 41] = "p";
                Key[0, 42] = "63"; Key[1, 42] = "x";
                Key[0, 43] = "73"; Key[1, 43] = "t";
                Key[0, 44] = "83"; Key[1, 44] = "s";
                Key[0, 45] = "93"; Key[1, 45] = "r";

                Key[0, 46] = "04"; Key[1, 46] = "u";
                Key[0, 47] = "14"; Key[1, 47] = "v";
                Key[0, 48] = "24"; Key[1, 48] = "Y";
                Key[0, 49] = "34"; Key[1, 49] = "q";
                Key[0, 50] = "54"; Key[1, 50] = "y";
                Key[0, 51] = "64"; Key[1, 51] = "C";
                Key[0, 52] = "74"; Key[1, 52] = "!";
                Key[0, 53] = "84"; Key[1, 53] = "@";
                Key[0, 54] = "94"; Key[1, 54] = "#";

                Key[0, 55] = "05"; Key[1, 55] = "$";
                Key[0, 56] = "15"; Key[1, 56] = "%";
                Key[0, 57] = "25"; Key[1, 57] = "^";
                Key[0, 58] = "35"; Key[1, 58] = "&";
                Key[0, 59] = "45"; Key[1, 59] = "*";
                Key[0, 60] = "65"; Key[1, 60] = "(";
                Key[0, 61] = "75"; Key[1, 61] = ")";
                Key[0, 62] = "85"; Key[1, 62] = "_";
                Key[0, 63] = "95"; Key[1, 63] = "-";

                Key[0, 64] = "06"; Key[1, 64] = "+";
                Key[0, 65] = "16"; Key[1, 65] = "=";
                Key[0, 66] = "26"; Key[1, 66] = "|";
                Key[0, 67] = "36"; Key[1, 67] = "\\";
                Key[0, 68] = "46"; Key[1, 68] = "<";
                Key[0, 69] = "56"; Key[1, 69] = ">";
                Key[0, 70] = "76"; Key[1, 70] = ",";
                Key[0, 71] = "86"; Key[1, 71] = ".";
                Key[0, 72] = "96"; Key[1, 72] = "?";

                Key[0, 73] = "07"; Key[1, 73] = "/";
                Key[0, 74] = "17"; Key[1, 74] = "[";
                Key[0, 75] = "27"; Key[1, 75] = "]";
                Key[0, 76] = "37"; Key[1, 76] = "{";
                Key[0, 77] = "47"; Key[1, 77] = "}";
                Key[0, 78] = "57"; Key[1, 78] = ":";
                Key[0, 79] = "67"; Key[1, 79] = ";";
                Key[0, 80] = "87"; Key[1, 80] = "\"";
                Key[0, 81] = "97"; Key[1, 81] = "\'";

                Key[0, 82] = "08"; Key[1, 82] = "`";
                Key[0, 83] = "18"; Key[1, 83] = "~";
            }
        }

        #endregion

        #region Edition01
        private static string EncryptString01(string CallPassword, string s)
        {
            if (CallPassword != GetCallCert())
                return "";

            if (s == "") return "";

            byte[] Byte = Shove._String.Compress(s);
            int len = Byte.Length;

            int Key = (System.DateTime.Now.Millisecond + 200) / 2;
            string d = Key.ToString().PadLeft(3, '0');

            for (int i = 0; i < len; i++)
                d += ((int)Byte[i] + Key).ToString().PadLeft(3, '0');

            ReplaceKey_01 rk = new ReplaceKey_01();
            for (int i = 0; i < 52; i++)
                d = d.Replace(rk.Key[0, i], rk.Key[1, i]);
            return d;
        }

        private static string UnEncryptString01(string CallPassword, string d)
        {
            if (CallPassword != GetCallCert())
                return "";

            if ((d == null) || (d == ""))
                return "";

            int i;
            ReplaceKey_01 rk = new ReplaceKey_01();
            for (i = 51; i >= 0; i--)
                d = d.Replace(rk.Key[1, i], rk.Key[0, i]);

            int len, Key = int.Parse(d.Substring(0, 3));

            d = d.Substring(3, d.Length - 3);
            len = d.Length / 3;

            byte[] Byte = new byte[len];

            for (i = 0; i < len; i++)
                Byte[i] = (byte)(int.Parse(d.Substring(i * 3, 3)) - Key);

            return Shove._String.Decompress(Byte);
        }
        #endregion

        #region Edition02
        private static string EncryptString02(string CallPassword, string s)
        {
            if (CallPassword != GetCallCert())
                return "";

            if (s == "") return "";

            byte[] Byte = System.Text.Encoding.UTF8.GetBytes(s);
            int len = Byte.Length;

            int Key = (System.DateTime.Now.Millisecond + 200) / 2;
            int[] Keys = new int[3];
            Keys[0] = Key / 100;
            Keys[1] = (Key % 100) / 10;
            Keys[2] = (Key % 10);

            string d = Key.ToString().PadLeft(3, '0');

            for (int i = 0; i < len; i++)
            {
                int t = Byte[i];
                t += Keys[i % 3];

                d += t.ToString().PadLeft(3, '0');
            }

            ReplaceKey_01 rk = new ReplaceKey_01();
            for (int i = 0; i < 84; i++)
                d = d.Replace(rk.Key[0, i], rk.Key[1, i]);
            return "02" + d;
        }

        private static string UnEncryptString02(string CallPassword, string d)
        {
            if (CallPassword != GetCallCert())
                return "";

            if ((d == null) || (d == "") || (d.Length < 2) || (!d.StartsWith("02")))
                return "";

            d = d.Substring(2, d.Length - 2);

            int i;
            ReplaceKey_01 rk = new ReplaceKey_01();
            for (i = 83; i >= 0; i--)
                d = d.Replace(rk.Key[1, i], rk.Key[0, i]);

            int len, Key = int.Parse(d.Substring(0, 3));
            int[] Keys = new int[3];
            Keys[0] = Key / 100;
            Keys[1] = (Key % 100) / 10;
            Keys[2] = (Key % 10);

            d = d.Substring(3, d.Length - 3);
            len = d.Length / 3;

            byte[] Byte = new byte[len];

            for (i = 0; i < len; i++)
                Byte[i] = (byte)(int.Parse(d.Substring(i * 3, 3)) - Keys[i % 3]);

            return System.Text.Encoding.UTF8.GetString(Byte);
        }
        #endregion

        #region Edition03
        private static string EncryptString03(string CallPassword, string s)
        {
            if (CallPassword != GetCallCert())
                return "";

            if (s == "") return "";

            byte[] Byte = System.Text.Encoding.UTF8.GetBytes(s);
            int len = Byte.Length;

            int Key = (System.DateTime.Now.Millisecond + 200) / 2;
            int[] Keys = new int[3];
            Keys[0] = Key / 100;
            Keys[1] = (Key % 100) / 10;
            Keys[2] = (Key % 10);

            string d = Key.ToString().PadLeft(3, '0');

            for (int i = 0; i < len; i++)
            {
                int t = Byte[i];
                t += Keys[i % 3];

                d += t.ToString().PadLeft(3, '0');
            }

            ReplaceKey_01 rk = new ReplaceKey_01();
            for (int i = 0; i < 52; i++)
                d = d.Replace(rk.Key[0, i], rk.Key[1, i]);
            return "03" + d;
        }

        private static string UnEncryptString03(string CallPassword, string d)
        {
            if (CallPassword != GetCallCert())
                return "";

            if ((d == null) || (d == "") || (d.Length < 2) || (!d.StartsWith("03")))
                return "";

            d = d.Substring(2, d.Length - 2);

            int i;
            ReplaceKey_01 rk = new ReplaceKey_01();
            for (i = 51; i >= 0; i--)
                d = d.Replace(rk.Key[1, i], rk.Key[0, i]);

            int len, Key = int.Parse(d.Substring(0, 3));
            int[] Keys = new int[3];
            Keys[0] = Key / 100;
            Keys[1] = (Key % 100) / 10;
            Keys[2] = (Key % 10);

            d = d.Substring(3, d.Length - 3);
            len = d.Length / 3;

            byte[] Byte = new byte[len];

            for (i = 0; i < len; i++)
                Byte[i] = (byte)(int.Parse(d.Substring(i * 3, 3)) - Keys[i % 3]);

            return System.Text.Encoding.UTF8.GetString(Byte);
        }
        #endregion

        #region Edition04
        private static string EncryptString04(string CallPassword, string s)
        {
            if (CallPassword != GetCallCert())
                return "";

            if (s == "") return "";

            byte[] Byte = System.Text.Encoding.UTF8.GetBytes(s);
            int len = Byte.Length;

            int Key = (System.DateTime.Now.Millisecond + 200) / 2;
            int[] Keys = new int[3];
            Keys[0] = Key / 100;
            Keys[1] = (Key % 100) / 10;
            Keys[2] = (Key % 10);

            string d = Key.ToString().PadLeft(3, '0');

            for (int i = 0; i < len; i++)
            {
                int t = Byte[i];
                t += Keys[i % 3];

                d += t.ToString().PadLeft(3, '0');
            }

            ReplaceKey_04 rk = new ReplaceKey_04();
            for (int i = 0; i < 52; i++)
                d = d.Replace(rk.Key[0, i], rk.Key[1, i]);
            return "04" + d;
        }

        private static string UnEncryptString04(string CallPassword, string d)
        {
            if (CallPassword != GetCallCert())
                return "";

            if ((d == null) || (d == "") || (d.Length < 2) || (!d.StartsWith("04")))
                return "";

            d = d.Substring(2, d.Length - 2);

            int i;
            ReplaceKey_04 rk = new ReplaceKey_04();
            for (i = 51; i >= 0; i--)
                d = d.Replace(rk.Key[1, i], rk.Key[0, i]);

            int len, Key = int.Parse(d.Substring(0, 3));
            int[] Keys = new int[3];
            Keys[0] = Key / 100;
            Keys[1] = (Key % 100) / 10;
            Keys[2] = (Key % 10);

            d = d.Substring(3, d.Length - 3);
            len = d.Length / 3;

            byte[] Byte = new byte[len];

            for (i = 0; i < len; i++)
                Byte[i] = (byte)(int.Parse(d.Substring(i * 3, 3)) - Keys[i % 3]);

            return System.Text.Encoding.UTF8.GetString(Byte);
        }
        #endregion

        #region Edition05
        private static string EncryptString05(string CallPassword, string input)
        {
            if (CallPassword != GetCallCert())
                return "";

            if (input == "") return "";

            string _3deskey = System.Configuration.ConfigurationManager.AppSettings["DesKey"];

            if (String.IsNullOrEmpty(_3deskey))
            {
                _3deskey = "56GtyNkop97Ht334TtyurfgQ";
            }

            return "05" + Encrypt3DES(CallPassword, input, _3deskey);
        }

        private static string UnEncryptString05(string CallPassword, string input)
        {
            if (CallPassword != GetCallCert())
                return "";

            if ((input == null) || (input == "") || (input.Length < 2) || (!input.StartsWith("05")))
                return "";

            input = input.Substring(2, input.Length - 2);

            string _3deskey = System.Configuration.ConfigurationManager.AppSettings["DesKey"];

            if (String.IsNullOrEmpty(_3deskey))
            {
                _3deskey = "56GtyNkop97Ht334TtyurfgQ";
            }

            return Decrypt3DES(CallPassword, input, _3deskey);
        }
        #endregion

        /// <summary>
        /// 加密字符串，请在 web.config 或者 app.config 中增加一个 appSettings: key="DesKey" value="你的 3DesKey"
        /// </summary>
        /// <param name="CallPassword"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncryptString(string CallPassword, string s)
        {
            return EncryptString05(CallPassword, s);
        }

        /// <summary>
        /// 解密字符串，请在 web.config 或者 app.config 中增加一个 appSettings: key="DesKey" value="你的 3DesKey"
        /// </summary>
        /// <param name="CallPassword"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string UnEncryptString(string CallPassword, string d)
        {
            if ((d == null) || (d == ""))
                return "";

            if (d.Length < 2)
                return UnEncryptString01(CallPassword, d);

            switch (d.Substring(0, 2))
            {
                case "02":
                    return UnEncryptString02(CallPassword, d);
                case "03":
                    return UnEncryptString03(CallPassword, d);
                case "04":
                    return UnEncryptString04(CallPassword, d);
                case "05":
                    return UnEncryptString05(CallPassword, d);
                default:
                    return UnEncryptString01(CallPassword, d);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CallPassword"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NoUnEncryptString(string CallPassword, string s)
        {
            if (CallPassword != GetCallCert())
                return "";

            int i, len, sum;
            sum = 1;
            len = s.Length;
            for (i = 0; i < len; i++)
            {
                sum = sum * (int)s[len - i - 1];
                while (sum > 99999) sum = (int)(sum / 10);
            }
            while (sum < 10000000) sum *= 11;
            while (sum > 99999999) sum = (int)(sum / 10);
            return sum.ToString();
        }

        /// <summary>
        /// 加密注册码的函数，和 Delphi 写的 Extfunc.dll 里面的那个完全一样
        /// 参数：密码，源串，前缀如 CF5-，节数，每节长度，生成的序列号字符类型：0-数字 1大写字母 2小写字母
        /// </summary>
        public static string NoUnEncryptString(string CallPassword, string SourceStr, string Marking_str, int Part_Num, int Part_Len, int Ch_Type)
        {
            if (CallPassword != GetCallCert())
                return "";

            int[,] ch_ord = new int[3, 2];
            ch_ord[0, 0] = _Convert.Asc('0');
            ch_ord[0, 1] = _Convert.Asc('9');
            ch_ord[1, 0] = _Convert.Asc('A');
            ch_ord[1, 1] = _Convert.Asc('Z');
            ch_ord[2, 0] = _Convert.Asc('a');
            ch_ord[2, 1] = _Convert.Asc('z');

            int[] sum = new int[Part_Num];
            string[] sumStr = new string[Part_Num];
            string[] PartStr = new string[Part_Num];

            sum[0] = 371;
            int i;
            for (i = 1; i <= Part_Num - 1; i++)
                sum[i] = sum[i - 1] * 3;

            int len = SourceStr.Length;
            int j;
            string t_s;
            for (i = 0; i < Part_Num; i++)
            {
                for (j = 0; j < len; j++)
                {
                    sum[i] = System.Math.Abs(sum[i] + System.Math.Abs(sum[i] * _Convert.Asc(SourceStr[j])));
                    while (true)
                    {
                        try
                        {
                            t_s = sum[i].ToString();
                            break;
                        }
                        catch
                        {
                            sum[i] /= 10;
                        }
                    }
                    t_s = _String.Reverse(t_s);
                    while (true)
                    {
                        try
                        {
                            sum[i] = int.Parse(t_s);
                            break;
                        }
                        catch
                        {
                            t_s = t_s.Substring(1, t_s.Length - 1);
                        }
                    }
                }
                sumStr[i] = sum[i].ToString();
                while (sumStr[i].Length < Part_Len + 1)
                {
                    sum[i] = System.Math.Abs((sum[i] * 3));
                    sumStr[i] = sumStr[i] + sum[i].ToString();
                }
                PartStr[i] = "";
                for (j = 0; j < Part_Len; j++)
                {
                    t_s = sumStr[i].Substring(j, 2);
                    int ch = int.Parse(t_s);
                    if (ch == 0) ch = 53;
                    while ((ch < ch_ord[Ch_Type, 0]) || (ch > ch_ord[Ch_Type, 1]))
                    {
                        while (ch < ch_ord[Ch_Type, 0]) ch *= 11;
                        while (ch > ch_ord[Ch_Type, 1]) ch /= 2;
                    }
                    PartStr[i] = PartStr[i] + _Convert.Chr(ch).ToString();
                }
            }
            string Result = Marking_str;
            for (i = 0; i < Part_Num; i++)
            {
                if (i > 0) Result += "-";
                Result += PartStr[i];
            }
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CallPassword"></param>
        /// <param name="isUseBiosSerialNumber"></param>
        /// <param name="isUseIdeDiskSerialNumber"></param>
        /// <param name="isUseNetAdapterMacAddress"></param>
        /// <param name="isUseCpuSerialNumber"></param>
        /// <returns></returns>
        public static string GetMachineSerialNumber(string CallPassword, bool isUseBiosSerialNumber, bool isUseIdeDiskSerialNumber, bool isUseNetAdapterMacAddress, bool isUseCpuSerialNumber)
        {
            if (CallPassword != GetCallCert())
                return "";

            string s = "";
            if (isUseBiosSerialNumber)
                s += _System.SystemInformation.GetBIOSSerialNumber();
            if (isUseIdeDiskSerialNumber)
                s += _System.SystemInformation.GetHardDiskSerialNumber();
            if (isUseNetAdapterMacAddress)
                s += _System.SystemInformation.GetNetCardMACAddress();
            if (isUseCpuSerialNumber)
                s += _System.SystemInformation.GetCPUSerialNumber();

            return NoUnEncryptString(CallPassword, s, "MID-", 5, 5, 0);
        }

        /// <summary>
        /// MD5 摘要，使用缺省字符集
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5(string input)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(input, "MD5");

            //return MD5(input, Encoding.Default.WebName);
        }

        /// <summary>
        /// MD5 摘要，使用指定的字符集
        /// </summary>
        /// <param name="input"></param>
        /// <param name="CharsetName"></param>
        /// <returns></returns>
        public static string MD5(string input, string CharsetName)
        {
            return MD5(input, Encoding.GetEncoding(CharsetName));
        }

        /// <summary>
        /// MD5 摘要，使用指定的字符集
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string MD5(string input, Encoding encoding)
        {
            byte[] t = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(input));
            StringBuilder sb = new StringBuilder(32);

            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        #region DES 加密解密

        /// <summary>
        /// 3DES 机密
        /// </summary>
        /// <param name="CallPassword"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt3DES(string CallPassword, string input, string key)
        {
            if (CallPassword != GetCallCert())
                return "";

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.UTF8.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.Zeros;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = ASCIIEncoding.UTF8.GetBytes(input);
            byte[] EncryptResult = DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length);

            string Result = "";
            foreach (byte b in EncryptResult)
            {
                Result += b.ToString("X").PadLeft(2, '0');
            }
            return Result;
        }

        /// <summary>
        /// 3DES 解密
        /// </summary>
        /// <param name="CallPassword"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt3DES(string CallPassword, string input, string key)
        {
            if (CallPassword != GetCallCert())
                return "";

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.UTF8.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.Zeros;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            byte[] Buffer = new byte[input.Length / 2];

            for (int i = 0; i < input.Length / 2; i++)
            {
                try
                {
                    Buffer[i] = (byte)Convert.ToInt16(input.Substring(i * 2, 2), 16);
                }
                catch
                {
                    return "";
                }
            }

            string Result = "";
            try
            {
                Result = ASCIIEncoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch
            {

            }
            return Decrypt3DES_TrimZreo(Result);
        }

        private static string Decrypt3DES_TrimZreo(string input)
        {
            while (input.EndsWith("\0"))
            {
                input = input.Substring(0, input.Length - 1);
            }

            return input;
        }

        #endregion

        #region AES 加密解密

        private static byte[] AES_IV = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };
        // Key = "12345678901234567890123456789012"; Key 示例，要求 32 位

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="CallPassword">调用密码</param>
        /// <param name="input">待加密的字符串</param>
        /// <param name="key">加密密钥,要求为32位</param>
        /// <returns>加密成功返回加密后的字符串，失败 throw</returns>
        public static string EncryptAES(string CallPassword, string input, string key)
        {
            if (CallPassword != GetCallCert())
                return "";

            byte[] inputData = UTF8Encoding.UTF8.GetBytes(input);

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = UTF8Encoding.UTF8.GetBytes(key.Substring(0, 32));
            rijndaelProvider.IV = AES_IV;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="CallPassword">调用密码</param>
        /// <param name="input">待解密的字符串</param>
        /// <param name="key">解密密钥,要求为32位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败 throw</returns>
        public static string DecryptAES(string CallPassword, string input, string key)
        {
            if (CallPassword != GetCallCert())
                return "";

            byte[] inputData = Convert.FromBase64String(input);

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = UTF8Encoding.UTF8.GetBytes(key.Substring(0, 32));
            rijndaelProvider.IV = AES_IV;
            ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

            byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return UTF8Encoding.UTF8.GetString(decryptedData);
        }

        #endregion

        #region SES 加密解密，与 ShoveEIMS3 的 C++ 代码兼容

        /// <summary>
        /// 加密，与 ShoveEIMS3 的 C++ 代码兼容
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="encodingName"></param>
        /// <returns></returns>
        public static string EncryptSES(string input, string key, string encodingName)
        {
            Ses ses = new Ses(key, encodingName);

            byte[] byte_input = Encoding.GetEncoding(encodingName).GetBytes(input);
            int len = ses.GetEncryptResultLength(byte_input);

            byte[] output = new byte[len];
            ses.Encrypt(byte_input, output);

            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// 解密，与 ShoveEIMS3 的 C++ 代码兼容
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="encodingName"></param>
        /// <returns></returns>
        public static string DecryptSES(string input, string key, string encodingName)
        {
            if (String.IsNullOrEmpty(input))
            {
                return "";
            }

            Ses ses = new Ses(key, encodingName);

            byte[] byte_input = Convert.FromBase64String(input);
            byte[] temp_output = new byte[input.Length];

            int output_len = 0;
            ses.Decrypt(byte_input, byte_input.Length, temp_output, ref output_len);

            byte[] ouput = new byte[output_len];
            Array.Copy(temp_output, ouput, output_len);

            return Encoding.GetEncoding(encodingName).GetString(ouput);
        }

        #endregion

        #region 对参数列表进行 MD5 签名

        /// <summary>
        /// 对参数进行签名
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static string ParamterSignature(string Key, params object[] Params)
        {
            string SignSource = "";

            foreach (object Param in Params)
            {
                SignSource += ParamterToString(Param);
            }

            return MD5(SignSource + Key);
        }

        private static string ParamterToString(object Param)
        {
            if (Param is DateTime)
            {
                return ((DateTime)Param).ToString("yyyyMMddHHmmss");
            }
            else if (Param is DataTable)
            {
                return _Convert.DataTableToXML((DataTable)Param);
            }
            else if (Param is DataSet)
            {
                return ((DataSet)Param).GetXml();
            }
            else if (Param is string)
            {
                return (string)Param;
            }

            return Param.ToString();
        }

        #endregion
    }
}