using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using LumiSoft.Net.IO;

namespace Shove._Net.TCP
{
    /// <summary>
    /// SocketService
    /// </summary>
    public partial class SocketService
    {
        internal class Utility
        {
            private const string DesKey = "Q56GtyNkop97Ht334Ttyurfg";

            internal byte[] ReceiveData(SmartStream TcpStream)
            {
                IList<byte> bytes = new List<byte>();
                int b;

                do
                {
                    try
                    {
                        b = TcpStream.ReadByte();
                    }
                    catch
                    {
                        return null;
                    }

                    if (b == -1)
                    {
                        if (!IsReceiveEnd(bytes))
                        {
                            return null;
                        }

                        break;
                    }

                    bytes.Add((byte)b);
                } while (!IsReceiveEnd(bytes));

                byte[] Result = Extract(bytes);

                return Decrypt(Result);
            }

            private bool IsReceiveEnd(IList<byte> bytes)
            {
                if ((bytes == null) || (bytes.Count < 10))
                {
                    return false;
                }

                int Position = bytes.Count;
                if (bytes[--Position] == (byte)0 && bytes[--Position] == (byte)0 && bytes[--Position] == (byte)0 && bytes[--Position] == (byte)'D' && bytes[--Position] == (byte)'N' && bytes[--Position] == (byte)'E' && bytes[--Position] == (byte)0 && bytes[--Position] == (byte)0 && bytes[--Position] == (byte)0 && bytes[--Position] == (byte)0)
                {
                    return true;
                }

                return false;
            }

            private byte[] Extract(IList<byte> bytes)
            {
                if ((bytes == null) || (bytes.Count < 10))
                {
                    return null;
                }

                byte[] r = new byte[bytes.Count - 10];
                for (int i = 0; i < bytes.Count - 10; i++)
                {
                    r[i] = bytes[i];
                }

                return r;
            }

            internal bool SendData(SmartStream TcpStream, byte[] SendBuffer, ref string ErrorDescription)
            {
                ErrorDescription = "";

                byte[] r1 = Encrypt(SendBuffer);
                byte[] r2 = new byte[r1.Length + 10];

                r1.CopyTo(r2, 0);
                int Position = r1.Length;
                r2[Position++] = (byte)0;
                r2[Position++] = (byte)0;
                r2[Position++] = (byte)0;
                r2[Position++] = (byte)0;
                r2[Position++] = (byte)'E';
                r2[Position++] = (byte)'N';
                r2[Position++] = (byte)'D';
                r2[Position++] = (byte)0;
                r2[Position++] = (byte)0;
                r2[Position++] = (byte)0;

                try
                {
                    TcpStream.Write(r2, 0, r2.Length);

                    return true;
                }
                catch (Exception e)
                {
                    ErrorDescription = e.Message;

                    return false;
                }
            }

            private byte[] Encrypt(byte[] input)
            {
                TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

                DES.Key = ASCIIEncoding.ASCII.GetBytes(DesKey);
                DES.Mode = CipherMode.ECB;
                DES.Padding = PaddingMode.PKCS7;

                ICryptoTransform DESEncrypt = DES.CreateEncryptor();

                return DESEncrypt.TransformFinalBlock(input, 0, input.Length);
            }

            private byte[] Decrypt(byte[] input)
            {
                TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

                DES.Key = ASCIIEncoding.ASCII.GetBytes(DesKey);
                DES.Mode = CipherMode.ECB;
                DES.Padding = PaddingMode.PKCS7;

                ICryptoTransform DESDecrypt = DES.CreateDecryptor();

                return DESDecrypt.TransformFinalBlock(input, 0, input.Length);
            }
        }
    }
}
