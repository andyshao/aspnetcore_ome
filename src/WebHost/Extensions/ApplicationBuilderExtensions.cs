﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;
using aaa.Module;

namespace WebHost.Extensions {
	public static class ApplicationBuilderExtensions {

		public static IApplicationBuilder UseCustomizedMvc(this IApplicationBuilder app) {
			app.UseMvc();
			return app;
		}

		public static IApplicationBuilder UseCustomizedStaticFiles(this IApplicationBuilder app, IList<ModuleInfo> modules) {
			app.UseDefaultFiles();
			app.UseStaticFiles(new StaticFileOptions() {
				OnPrepareResponse = (context) => {
					var headers = context.Context.Response.GetTypedHeaders();
					headers.CacheControl = new CacheControlHeaderValue() {
						Public = true,
						MaxAge = TimeSpan.FromDays(60)
					};
				}
			});
			return app;
		}
	}
}
