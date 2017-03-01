using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Razor;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using WebHost.Extensions;
using aaa.Module;

namespace WebHost {
	public class Startup {
		public Startup(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true);

			if (env.IsProduction())
				builder.AddJsonFile("/var/webos/aaa/appsettings.json", true, true);

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
			services.AddSession(a => {
				a.IdleTimeout = TimeSpan.FromMinutes(30);
				a.CookieName = "Session_aaa";
			});

			services.LoadInstalledModules(Modules, env);
			services.AddCustomizedMvc(Modules);
			services.Configure<RazorViewEngineOptions>(options => { options.ViewLocationExpanders.Add(new ModuleViewLocationExpander()); });

			#region Swagger UI
			if (env.IsDevelopment())
				services.AddSwaggerGen(options => {
					options.SwaggerDoc("v1", new Info {
						Version = "v1",
						Title = "aaa API",
						Description = "aaa 项目webapi接口说明",
						TermsOfService = "None",
						Contact = new Contact { Name = "duoyi", Email = "", Url = "http://duoyi.com" },
						License = new License { Name = "duoyi", Url = "http://duoyi.com" }
					});
					options.IgnoreObsoleteActions();
					//options.IgnoreObsoleteControllers(); // 类、方法标记 [Obsolete]，可以阻止【Swagger文档】生成
					options.DescribeAllEnumsAsStrings();
					//options.IncludeXmlComments(AppContext.BaseDirectory + @"/Admin.xml"); // 使用前需开启项目注释 xmldoc
					options.OperationFilter<FormDataOperationFilter>();
				});
			#endregion
			services.AddSingleton<IConfigurationRoot>(Configuration);
			services.AddSingleton<IHostingEnvironment>(env);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime) {
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Console.OutputEncoding = Encoding.GetEncoding("GB2312");
			Console.InputEncoding = Encoding.GetEncoding("GB2312");

			// 以下写日志会严重影响吞吐量，高并发项目建议改成 redis 订阅发布形式
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddNLog().AddDebug();
			env.ConfigureNLog("nlog.config");

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			aaa.BLL.RedisHelper.InitializeConfiguration(Configuration);
			aaa.DAL.PSqlHelper.Instance.Log = loggerFactory.CreateLogger("aaa_DAL_sqlhelper");

			app.UseSession();
			app.UseCustomizedMvc();
			app.UseCustomizedStaticFiles(Modules);
		}
	}
}
