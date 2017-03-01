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
	public class MroomtagController : BaseController {
		public MroomtagController(ILogger<MroomtagController> logger) : base(logger) { }

		[HttpGet]
		public ActionResult List([FromServices]IConfigurationRoot cfg, [FromQuery] string key, [FromQuery] int[] Mroom_id, [FromQuery] int limit = 20, [FromQuery] int page = 1) {
			var select = Mroomtag.Select
				.Where(!string.IsNullOrEmpty(key), "a.name ilike {0}", string.Concat("%", key, "%"));
			if (Mroom_id.Length > 0) select.WhereMroom_id(Mroom_id);
			int count;
			var items = select.Count(out count).Skip((page - 1) * limit).Limit(limit).ToList();
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
			MroomtagInfo item = Mroomtag.GetItem(Id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			ViewBag.item = item;
			return View();
		}

		/***************************************** POST *****************************************/
		[HttpPost(@"add")]
		[ValidateAntiForgeryToken]
		public APIReturn _Add([FromForm] string Name, [FromForm] int[] mn_Mroom) {
			MroomtagInfo item = new MroomtagInfo();
			item.Name = Name;
			item = Mroomtag.Insert(item);
			//关联 Mroom
			foreach (int mn_Mroom_in in mn_Mroom)
				item.FlagMroom(mn_Mroom_in);
			return APIReturn.成功.SetData("item", item.ToBson());
		}
		[HttpPost(@"edit")]
		[ValidateAntiForgeryToken]
		public APIReturn _Edit([FromQuery] int Id, [FromForm] string Name, [FromForm] int[] mn_Mroom) {
			MroomtagInfo item = Mroomtag.GetItem(Id);
			if (item == null) return APIReturn.记录不存在_或者没有权限;
			item.Name = Name;
			int affrows = Mroomtag.Update(item);
			//关联 Mroom
			if (mn_Mroom.Length == 0) {
				item.UnflagMroomALL();
			} else {
				List<int> mn_Mroom_list = mn_Mroom.ToList();
				foreach (var Obj_mroom in item.Obj_mrooms) {
					int idx = mn_Mroom_list.FindIndex(a => a == Obj_mroom.Id);
					if (idx == -1) item.UnflagMroom(Obj_mroom.Id);
					else mn_Mroom_list.RemoveAt(idx);
				}
				mn_Mroom_list.ForEach(a => item.FlagMroom(a));
			}
			if (affrows > 0) return APIReturn.成功.SetMessage($"更新成功，影响行数：{affrows}");
			return APIReturn.失败;
		}

		[HttpPost("del")]
		[ValidateAntiForgeryToken]
		public APIReturn _Del([FromForm] int[] ids) {
			int affrows = 0;
			foreach (int id in ids)
				affrows += Mroomtag.Delete(id);
			if (affrows > 0) return APIReturn.成功.SetMessage($"删除成功，影响行数：{affrows}");
			return APIReturn.失败;
		}
	}
}
