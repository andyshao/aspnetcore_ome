using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace aaa.WebHost {
	public class Startup {
		public Startup(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder()
				.LoadInstalledModules(Modules, env)
				.AddCustomizedJsonFile(Modules, env, "/var/webos/aaa/");

			this.Configuration = builder.AddEnvironmentVariables().Build();
			this.env = env;

			Newtonsoft.Json.JsonConvert.DefaultSettings = () => {
				var st = new Newtonsoft.Json.JsonSerializerSettings();
				st.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
				return st;
			};
		}

		public static IList<ModuleInfo> Modules = new List<ModuleInfo>();
		public IConfigurationRoot Configuration { get; }
		public IHostingEnvironment env { get; }

		public void ConfigureServices(IServiceCollection services) {
			services.AddSingleton<IDistributedCache>(new RedisCache());
			services.AddSingleton<IConfigurationRoot>(Configuration);
			services.AddSingleton<IHostingEnvironment>(env);

			services.AddSession(a => {
				a.IdleTimeout = TimeSpan.FromMinutes(30);
				a.CookieName = "Session_aaa";
			});
			services.AddCustomizedMvc(Modules);
			services.Configure<RazorViewEngineOptions>(options => { options.ViewLocationExpanders.Add(new ModuleViewLocationExpander()); });

			if (env.IsDevelopment())
				services.AddCustomizedSwaggerGen();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime) {
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Console.OutputEncoding = Encoding.GetEncoding("GB2312");
			Console.InputEncoding = Encoding.GetEncoding("GB2312");

			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddNLog().AddDebug();
			env.ConfigureNLog("nlog.config");

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			aaa.BLL.RedisHelper.InitializeConfiguration(Configuration);
			aaa.DAL.PSqlHelper.Instance.Log = loggerFactory.CreateLogger("aaa_DAL_sqlhelper");

			app.UseSession();
			app.UseMvc();
			app.UseCustomizedStaticFiles(Modules);
		}
	}
}
