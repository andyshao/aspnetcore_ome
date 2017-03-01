using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using aaa.BLL;
using aaa.Model;

namespace aaa.Module.Admin.Controllers {
	[Route("[controller]")]
	[Obsolete]
	public class SysController : Controller {
		[HttpGet(@"connection")]
		public object Get_connection() {
			List<Hashtable> ret = new List<Hashtable>();
			foreach (var conn in PSqlHelper.Instance.Pool.AllConnections) {
				ret.Add(new Hashtable() {
						{ "数据库", conn.SqlConnection.Database },
						{ "状态", conn.SqlConnection.State },
						{ "最后活动", conn.LastActive },
						{ "获取次数", conn.UseSum }
					});
			}
			return new {
				FreeConnections = PSqlHelper.Instance.Pool.FreeConnections.Count,
				AllConnections = PSqlHelper.Instance.Pool.AllConnections.Count,
				List = ret
			};
		}
		[HttpGet(@"connection/redis")]
		public object Get_connection_redis() {
			List<Hashtable> ret = new List<Hashtable>();
			foreach (var conn in RedisHelper.Instance.AllConnections) {
				ret.Add(new Hashtable() {
						{ "数据库", conn.Client.ClientGetName() },
						{ "最后活动", conn.LastActive },
						{ "获取次数", conn.UseSum }
					});
			}
			return new {
				FreeConnections = RedisHelper.Instance.FreeConnections.Count,
				AllConnections = RedisHelper.Instance.AllConnections.Count,
				List = ret
			};
		}

		[HttpGet(@"init_sysdir")]
		public APIReturn Get_init_sysdir() {
			/*
			if (Sysdir.SelectByParent_id(null).Count() > 0)
				return new APIReturn(-33, "本系统已经初始化过，页面没经过任何操作退出。");

			SysdirInfo dir1, dir2, dir3;
			dir1 = Sysdir.Insert(null, DateTime.Now, "运营管理", 1, null);

			dir2 = Sysdir.Insert(dir1.Id, DateTime.Now, "mroom", 1, "/mroom/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "列表", 1, "/mroom/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "添加", 2, "/mroom/add");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "编辑", 3, "/mroom/edit");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "删除", 4, "/mroom/del");

			dir2 = Sysdir.Insert(dir1.Id, DateTime.Now, "mroom_mroomtag", 2, "/mroom_mroomtag/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "列表", 1, "/mroom_mroomtag/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "添加", 2, "/mroom_mroomtag/add");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "编辑", 3, "/mroom_mroomtag/edit");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "删除", 4, "/mroom_mroomtag/del");

			dir2 = Sysdir.Insert(dir1.Id, DateTime.Now, "mroomtag", 3, "/mroomtag/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "列表", 1, "/mroomtag/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "添加", 2, "/mroomtag/add");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "编辑", 3, "/mroomtag/edit");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "删除", 4, "/mroomtag/del");

			dir2 = Sysdir.Insert(dir1.Id, DateTime.Now, "mtype", 4, "/mtype/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "列表", 1, "/mtype/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "添加", 2, "/mtype/add");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "编辑", 3, "/mtype/edit");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "删除", 4, "/mtype/del");

			dir2 = Sysdir.Insert(dir1.Id, DateTime.Now, "reserve", 5, "/reserve/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "列表", 1, "/reserve/");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "添加", 2, "/reserve/add");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "编辑", 3, "/reserve/edit");
			dir3 = Sysdir.Insert(dir2.Id, DateTime.Now, "删除", 4, "/reserve/del");
			*/
			return new APIReturn(0, "管理目录已初始化完成。");
		}
	}
}
