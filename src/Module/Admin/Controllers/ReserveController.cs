using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NpgsqlTypes;
using aaa.BLL;
using aaa.Model;

namespace aaa.Module.Admin.Controllers {
	[Route("[controller]")]
	public class ReserveController : BaseController {
		public ReserveController(ILogger<ReserveController> logger) : base(logger) { }

		[HttpGet]
		public ActionResult List([FromServices]IConfigurationRoot cfg, [FromQuery] string key, [FromQuery] int?[] Mroom_id, [FromQuery] int limit = 20, [FromQuery] int page = 1) {
			var select = Reserve.Select
				.Where(!string.IsNullOrEmpty(key), "a.username ilike {0}", string.Concat("%", key, "%"));
			if (Mroom_id.Length > 0) select.WhereMroom_id(Mroom_id);
			int count;
			var items = select.Count(out count)
				.LeftJoin<Mroom>("b", "b.id = a.mroom_id").Skip((page - 1) * limit).Limit(limit).ToList();
			ViewBag.items = items;
			ViewBag.count = count;
			return View();
		}

		[HttpGet(@"add")]
		public ActionResult Edit() {
			return View();
		}
		[HttpGet(@"edit")]
		public ActionResult Edit([FromQuery] int Id) {
			ReserveInfo item = Reserve.GetItem(Id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			ViewBag.item = item;
			return View();
		}

		/***************************************** POST *****************************************/
		[HttpPost(@"add")]
		[ValidateAntiForgeryToken]
		public APIReturn _Add([FromForm] int? Id, [FromForm] int? Mroom_id, [FromForm] DateTime? Datetime1, [FromForm] short? Use_minute, [FromForm] string Username) {
			ReserveInfo item = new ReserveInfo();
			item.Id = Id;
			item.Mroom_id = Mroom_id;
			item.Create_time = DateTime.Now;
			item.Datetime1 = Datetime1;
			item.Use_minute = Use_minute;
			item.Username = Username;
			item = Reserve.Insert(item);
			return APIReturn.成功.SetData("item", item.ToBson());
		}
		[HttpPost(@"edit")]
		[ValidateAntiForgeryToken]
		public APIReturn _Edit([FromQuery] int Id, [FromForm] int? Mroom_id, [FromForm] DateTime? Datetime1, [FromForm] short? Use_minute, [FromForm] string Username) {
			ReserveInfo item = Reserve.GetItem(Id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			item.Mroom_id = Mroom_id;
			item.Create_time = DateTime.Now;
			item.Datetime1 = Datetime1;
			item.Use_minute = Use_minute;
			item.Username = Username;
			int affrows = Reserve.Update(item);
			if (affrows > 0) return APIReturn.成功.SetMessage($"更新成功，影响行数：{affrows}");
			return APIReturn.失败;
		}

		[HttpPost("del")]
		[ValidateAntiForgeryToken]
		public APIReturn _Del([FromForm] int[] ids) {
			int affrows = 0;
			foreach (int id in ids)
				affrows += Reserve.Delete(id);
			if (affrows > 0) return APIReturn.成功.SetMessage($"删除成功，影响行数：{affrows}");
			return APIReturn.失败;
		}
	}
}
