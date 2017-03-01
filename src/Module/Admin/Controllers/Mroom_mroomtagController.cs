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
	public class Mroom_mroomtagController : BaseController {
		public Mroom_mroomtagController(ILogger<Mroom_mroomtagController> logger) : base(logger) { }

		[HttpGet]
		public ActionResult List([FromServices]IConfigurationRoot cfg, [FromQuery] int?[] Mroom_id, [FromQuery] int?[] Mroomtag_id, [FromQuery] int limit = 20, [FromQuery] int page = 1) {
			var select = Mroom_mroomtag.Select;
			if (Mroom_id.Length > 0) select.WhereMroom_id(Mroom_id);
			if (Mroomtag_id.Length > 0) select.WhereMroomtag_id(Mroomtag_id);
			int count;
			var items = select.Count(out count)
				.LeftJoin<Mroom>("b", "b.id = a.mroom_id")
				.LeftJoin<Mroomtag>("c", "c.id = a.mroomtag_id").Skip((page - 1) * limit).Limit(limit).ToList();
			ViewBag.items = items;
			ViewBag.count = count;
			return View();
		}

		[HttpGet(@"add")]
		public ActionResult Edit() {
			return View();
		}
		[HttpGet(@"edit")]
		public ActionResult Edit([FromQuery] int Mroom_id, [FromQuery] int Mroomtag_id) {
			Mroom_mroomtagInfo item = Mroom_mroomtag.GetItem(Mroom_id, Mroomtag_id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			ViewBag.item = item;
			return View();
		}

		/***************************************** POST *****************************************/
		[HttpPost(@"add")]
		[ValidateAntiForgeryToken]
		public APIReturn _Add([FromForm] int? Mroom_id, [FromForm] int? Mroomtag_id) {
			Mroom_mroomtagInfo item = new Mroom_mroomtagInfo();
			item.Mroom_id = Mroom_id;
			item.Mroomtag_id = Mroomtag_id;
			item = Mroom_mroomtag.Insert(item);
			return APIReturn.成功.SetData("item", item.ToBson());
		}
		[HttpPost(@"edit")]
		[ValidateAntiForgeryToken]
		public APIReturn _Edit([FromQuery] int Mroom_id, [FromQuery] int Mroomtag_id) {
			Mroom_mroomtagInfo item = Mroom_mroomtag.GetItem(Mroom_id, Mroomtag_id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			int affrows = Mroom_mroomtag.Update(item);
			if (affrows > 0) return APIReturn.成功.SetMessage($"更新成功，影响行数：{affrows}");
			return APIReturn.失败;
		}

		[HttpPost("del")]
		[ValidateAntiForgeryToken]
		public APIReturn _Del([FromForm] string[] ids) {
			int affrows = 0;
			foreach (string id in ids) {
				string[] vs = id.Split(',');
				affrows += Mroom_mroomtag.Delete(int.Parse(vs[0]), int.Parse(vs[1]));
			}
			if (affrows > 0) return APIReturn.成功.SetMessage($"删除成功，影响行数：{affrows}");
			return APIReturn.失败;
		}
	}
}
