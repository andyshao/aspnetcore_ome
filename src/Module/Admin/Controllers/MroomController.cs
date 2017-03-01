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
	public class MroomController : BaseController {
		public MroomController(ILogger<MroomController> logger) : base(logger) { }

		[HttpGet]
		public ActionResult List([FromServices]IConfigurationRoot cfg, [FromQuery] string key, [FromQuery] int?[] Mtype_id, [FromQuery] int[] Mroomtag_id, [FromQuery] int limit = 20, [FromQuery] int page = 1) {
			var select = Mroom.Select
				.Where(!string.IsNullOrEmpty(key), "a.name ilike {0} or a.reason ilike {0} or a.username ilike {0}", string.Concat("%", key, "%"));
			if (Mtype_id.Length > 0) select.WhereMtype_id(Mtype_id);
			if (Mroomtag_id.Length > 0) select.WhereMroomtag_id(Mroomtag_id);
			int count;
			var items = select.Count(out count)
				.LeftJoin<Mtype>("b", "b.id = a.mtype_id").Skip((page - 1) * limit).Limit(limit).ToList();
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
			MroomInfo item = Mroom.GetItem(Id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			ViewBag.item = item;
			return View();
		}

		/***************************************** POST *****************************************/
		[HttpPost(@"add")]
		[ValidateAntiForgeryToken]
		public APIReturn _Add([FromForm] int? Mtype_id, [FromForm] string Name, [FromForm] short? Number, [FromForm] string Reason, [FromForm] Et_mroomstateENUM? State, [FromForm] string Username, [FromForm] int[] mn_Mroomtag) {
			MroomInfo item = new MroomInfo();
			item.Mtype_id = Mtype_id;
			item.Name = Name;
			item.Number = Number;
			item.Reason = Reason;
			item.State = State;
			item.Username = Username;
			item = Mroom.Insert(item);
			//关联 Mroomtag
			foreach (int mn_Mroomtag_in in mn_Mroomtag)
				item.FlagMroomtag(mn_Mroomtag_in);
			return APIReturn.成功.SetData("item", item.ToBson());
		}
		[HttpPost(@"edit")]
		[ValidateAntiForgeryToken]
		public APIReturn _Edit([FromQuery] int Id, [FromForm] int? Mtype_id, [FromForm] string Name, [FromForm] short? Number, [FromForm] string Reason, [FromForm] Et_mroomstateENUM? State, [FromForm] string Username, [FromForm] int[] mn_Mroomtag) {
			MroomInfo item = Mroom.GetItem(Id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			item.Mtype_id = Mtype_id;
			item.Name = Name;
			item.Number = Number;
			item.Reason = Reason;
			item.State = State;
			item.Username = Username;
			int affrows = Mroom.Update(item);
			//关联 Mroomtag
			if (mn_Mroomtag.Length == 0) {
				item.UnflagMroomtagALL();
			} else {
				List<int> mn_Mroomtag_list = mn_Mroomtag.ToList();
				foreach (var Obj_mroomtag in item.Obj_mroomtags) {
					int idx = mn_Mroomtag_list.FindIndex(a => a == Obj_mroomtag.Id);
					if (idx == -1) item.UnflagMroomtag(Obj_mroomtag.Id);
					else mn_Mroomtag_list.RemoveAt(idx);
				}
				mn_Mroomtag_list.ForEach(a => item.FlagMroomtag(a));
			}
			if (affrows > 0) return APIReturn.成功.SetMessage($"更新成功，影响行数：{affrows}");
			return APIReturn.失败;
		}

		[HttpPost("del")]
		[ValidateAntiForgeryToken]
		public APIReturn _Del([FromForm] int[] ids) {
			int affrows = 0;
			foreach (int id in ids)
				affrows += Mroom.Delete(id);
			if (affrows > 0) return APIReturn.成功.SetMessage($"删除成功，影响行数：{affrows}");
			return APIReturn.失败;
		}
	}
}
