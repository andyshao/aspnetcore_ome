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

namespace aaa.Module.Admin.Controllers2 {
	[Route("[controller]")]
	public class AdaController : BaseController {
		public AdaController(ILogger<AdaController> logger) : base(logger) { }

		[HttpGet]
		public APIReturn test() {
			return APIReturn.记录不存在_或者没有权限;
		}

		[HttpGet("test2")]
		public APIReturn test2() {
			return APIReturn.成功.SetData(Mroom.Select.ToList());
		}
	}
}
