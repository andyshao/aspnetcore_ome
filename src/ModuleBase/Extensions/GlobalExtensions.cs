using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class GlobalExtensions {
	public static object Json(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, object obj) {
		string str = JsonConvert.SerializeObject(obj);
		if (!string.IsNullOrEmpty(str)) str = Regex.Replace(str, @"<(/?script[\s>])", "<\"+\"$1", RegexOptions.IgnoreCase);
		if (html == null) return str;
		return html.Raw(str);
	}
}
