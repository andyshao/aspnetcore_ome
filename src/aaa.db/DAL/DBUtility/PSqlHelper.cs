using System;
using System.Collections;
using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace aaa.BLL {
	/// <summary>
	/// 数据库操作代理类，全部支持走事务
	/// </summary>
	public abstract partial class PSqlHelper : aaa.DAL.PSqlHelper {
	}
}
namespace aaa.DAL {
	public abstract partial class PSqlHelper {
		private static string _connectionString;
		public static string ConnectionString {
			get {
				if (string.IsNullOrEmpty(_connectionString)) _connectionString = BLL.RedisHelper.Configuration["ConnectionStrings:npgsql"];
				return _connectionString;
			}
			set {
				_connectionString = value;
				Instance.Pool.ConnectionString = value;
			}
		}
		public static Executer Instance { get; } = new Executer(new LoggerFactory().CreateLogger("aaa_DAL_sqlhelper"), ConnectionString);
		static PSqlHelper() {
			var nameTranslator = new NpgsqlMapNameTranslator();
			NpgsqlConnection.MapEnumGlobally<Model.Et_mroomstateENUM>("public.et_mroomstate", nameTranslator);
		}
		public class NpgsqlMapNameTranslator : INpgsqlNameTranslator {
			public string TranslateMemberName(string clrName) => clrName;
			public string TranslateTypeName(string clrName) => clrName;
		}

		public static string Addslashes(string filter, params object[] parms) { return Executer.Addslashes(filter, parms); }
		public static void ExecuteReader(Action<NpgsqlDataReader> readerHander, string cmdText, params NpgsqlParameter[] cmdParms) {
			Instance.ExecuteReader(readerHander, CommandType.Text, cmdText, cmdParms);
		}
		public static object[][] ExeucteArray(string cmdText, params NpgsqlParameter[] cmdParms) {
			return Instance.ExeucteArray(CommandType.Text, cmdText, cmdParms);
		}
		public static int ExecuteNonQuery(string cmdText, params NpgsqlParameter[] cmdParms) {
			return Instance.ExecuteNonQuery(CommandType.Text, cmdText, cmdParms);
		}
		public static object ExecuteScalar(string cmdText, params NpgsqlParameter[] cmdParms) {
			return Instance.ExecuteScalar(CommandType.Text, cmdText, cmdParms);
		}
		/// <summary>
		/// 开启事务（不支持异步），10秒未执行完将超时
		/// </summary>
		/// <param name="handler">事务体 () => {}</param>
		public static void Transaction(AnonymousHandler handler) {
			Transaction(handler, TimeSpan.FromSeconds(10));
		}
		/// <summary>
		/// 开启事务（不支持异步）
		/// </summary>
		/// <param name="handler">事务体 () => {}</param>
		/// <param name="timeout">超时</param>
		public static void Transaction(AnonymousHandler handler, TimeSpan timeout) {
			try {
				Instance.BeginTransaction(timeout);
				handler();
				Instance.CommitTransaction();
			} catch (Exception ex) {
				Instance.RollbackTransaction();
				throw ex;
			}
		}
		public static NpgsqlRange<T> ParseNpgsqlRange<T>(string s) { return Executer.ParseNpgsqlRange<T>(s); }
		public static BitArray Parse1010(string _1010) { return Executer.Parse1010(_1010); }
	}
}