using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Web.SessionState;
using System.IO;

using Shove.Database;

namespace Shove._Web
{
    /// <summary>
    /// Session Read and Write Operate. Note: Add a “SystemPreFix” key in Web.Config file.
    /// </summary>
    public partial class Session
    {
        /// <summary>
        /// 对存放于 SQLServer 中的 Session 进行自定义读写
        /// P_修改了以下存储过程
        /// 1 TempGetAppID: 把 SET @appId = NULL 改为 SET @appId = 12345678 return 0
        /// 2 TempGetStateItem, TmpGetStateItem2, TmpGetStateItem3,
        ///   TempGetStateItemExclusive, TempGetStateItemExclusive2, TempGetStateItemExclusive3,
        ///   TempInsertStatrItemLong, TempInsertStatrItemShort,
        ///   TempInsertUninitializedItem,
        ///   TempReleaseStatItemExclusive,
        ///   TempRemoveStateItem,
        ///   TempResetTimeout,
        ///   TempUpdateStateItemLong, TempUpdateStateItemLongNullShort,
        ///   TempUpdateStateItemShort, TempUpdateStateItemShortNullLong
        ///   以上存储过程 AS 后面加了一句：set @id = substring(@id, 1, 24)
        /// </summary>
        public class SQLServerSession
        {
            string ConnectionString = "";
            string SessionID = "";
            int TimeOut = 720;

            /// <summary>
            /// 构造器
            /// </summary>
            /// <param name="_ConnectionString"></param>
            /// <param name="_SessionID"></param>
            /// <param name="_TimeOut"></param>
            public SQLServerSession(string _ConnectionString, string _SessionID, int _TimeOut)
            {
                ConnectionString = _ConnectionString;
                SessionID = _SessionID.ToLower();
                TimeOut = _TimeOut;
            }

            /// <summary>
            /// 对 Key 的 Session 值进行读写
            /// </summary>
            /// <param name="Key"></param>
            /// <returns></returns>
            public object this[string Key]
            {
                get
                {
                    DataTable dt = MSSQL.Select(ConnectionString, "select top 1 * from ASPStateTempSessions where substring(SessionId, 1, 24) = @SessionId",
                        new MSSQL.Parameter("SessionId", SqlDbType.VarChar, 0, ParameterDirection.Input, SessionID));

                    if ((dt == null) || (dt.Rows.Count < 1))
                    {
                        return null;
                    }

                    MemoryStream ms = new MemoryStream();
                    BinaryReader br = new BinaryReader(ms);

                    //取得序列化之后的session state.
                    byte[] buffer = (byte[])dt.Rows[0]["SessionItemShort"];

                    //创建流，并把流的开始位置定位。
                    ms.SetLength(0L);
                    ms.Write(buffer, 0, buffer.Length);
                    ms.Seek(0L, SeekOrigin.Begin);

                    //去除掉字节流前边的标识字节。
                    br.ReadInt32();
                    bool Checked = br.ReadBoolean();
                    br.ReadBoolean();

                    if (!Checked)
                    {
                        return null;
                    }

                    SessionStateItemCollection ssc = SessionStateItemCollection.Deserialize(br);

                    return ssc[Key];
                }
                set
                {
                    DataTable dt = MSSQL.Select(ConnectionString, "select top 1 * from ASPStateTempSessions where substring(SessionId, 1, 24) = @SessionId",
                        new MSSQL.Parameter("SessionId", SqlDbType.VarChar, 0, ParameterDirection.Input, SessionID));

                    if (dt == null)
                    {
                        throw new Exception("写入 Session 发生错误：读取数据库发生错误！");
                    }

                    byte[] buffer = null;
                    SessionStateItemCollection ssc = null;

                    int int1 = 720;
                    bool bool1 = true;
                    bool bool2 = false;

                    if (dt.Rows.Count < 1)
                    {
                        // 没有 Session 记录，增加一行记录，增加新的 SessionId 记录。
                        ssc = new SessionStateItemCollection();
                        ssc[Key] = value;
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        BinaryReader br = new BinaryReader(ms);

                        //取得序列化之后的session state.
                        buffer = (byte[])dt.Rows[0]["SessionItemShort"];

                        //创建流，并把流的开始位置定位。
                        ms.SetLength(0L);
                        ms.Write(buffer, 0, buffer.Length);
                        ms.Seek(0L, SeekOrigin.Begin);

                        //去除掉字节流前边的标识字节。
                        int1 = br.ReadInt32();
                        bool1 = br.ReadBoolean();
                        bool2 = br.ReadBoolean();

                        ssc = SessionStateItemCollection.Deserialize(br);
                        ssc[Key] = value;
                    }

                    // 序列化
                    MemoryStream ms2 = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(ms2);
                    ms2.SetLength(0L);
                    bw.Write(int1);
                    bw.Write(bool1);
                    bw.Write(bool2);

                    ssc.Serialize(bw);
                    bw.Write((byte)0xFF);

                    // 写入数据库
                    byte[] buffer2 = ms2.ToArray();

                    if (dt.Rows.Count < 1)
                    {
                        MSSQL.ExecuteNonQuery(ConnectionString, "insert into ASPStateTempSessions (SessionId, Created, Expires, LockDate, LockDateLocal, LockCookie, Timeout, Locked, SessionItemShort) values (@SessionId, @Created, @Expires, @LockDate, @LockDateLocal, @LockCookie, @Timeout, @Locked, @SessionItemShort)",
                             new MSSQL.Parameter("SessionId", SqlDbType.VarChar, 0, ParameterDirection.Input, SessionID),
                             new MSSQL.Parameter("Created", SqlDbType.DateTime, 0, ParameterDirection.Input, DateTime.Now),
                             new MSSQL.Parameter("Expires", SqlDbType.DateTime, 0, ParameterDirection.Input, DateTime.Now.AddMinutes(TimeOut)),
                             new MSSQL.Parameter("LockDate", SqlDbType.DateTime, 0, ParameterDirection.Input, DateTime.Now),
                             new MSSQL.Parameter("LockDateLocal", SqlDbType.DateTime, 0, ParameterDirection.Input, DateTime.Now),
                             new MSSQL.Parameter("LockCookie", SqlDbType.Int, 0, ParameterDirection.Input, 0),
                             new MSSQL.Parameter("Timeout", SqlDbType.Int, 0, ParameterDirection.Input, TimeOut),
                             new MSSQL.Parameter("Locked", SqlDbType.Bit, 0, ParameterDirection.Input, false),
                             new MSSQL.Parameter("SessionItemShort", SqlDbType.Image, 0, ParameterDirection.Input, buffer2)
                            );
                    }
                    else
                    {
                        MSSQL.ExecuteNonQuery(ConnectionString, "update ASPStateTempSessions set SessionItemShort = @SessionItemShort where substring(SessionId, 1, 24) = @SessionId",
                            new MSSQL.Parameter("SessionItemShort", SqlDbType.Image, 0, ParameterDirection.Input, buffer2),
                            new MSSQL.Parameter("SessionId", SqlDbType.VarChar, 0, ParameterDirection.Input, SessionID));
                    }
                }
            }
        }
    }
}
