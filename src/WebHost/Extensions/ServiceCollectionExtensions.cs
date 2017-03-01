using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using aaa.Module;

namespace WebHost.Extensions {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection LoadInstalledModules(this IServiceCollection services, IList<ModuleInfo> modules, IHostingEnvironment env) {
			var moduleRootFolder = new DirectoryInfo(Path.Combine(env.ContentRootPath, "Module"));
			var moduleFolders = moduleRootFolder.GetDirectories();

			foreach (var moduleFolder in moduleFolders) {
				var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, "bin"));
				if (!binFolder.Exists) continue;

				foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories)) {
					Assembly assembly;
					try {
						assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
					} catch (FileLoadException) {
						assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

						if (assembly == null) throw;
					}

					if (assembly.FullName.Contains(moduleFolder.Name))
						modules.Add(new ModuleInfo {
							Name = moduleFolder.Name,
							Assembly = assembly,
							Path = moduleFolder.FullName
						});
				}
			}

			return services;
		}
		public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules) {
			var mvcBuilder = services.AddMvc()
				.AddRazorOptions(o => {
					foreach (var module in modules) {
						var a = MetadataReference.CreateFromFile(module.Assembly.Location);
						o.AdditionalCompilationReferences.Add(a);
					}
				})
				.AddViewLocalization()
				.AddDataAnnotationsLocalization();

			foreach (var module in modules) {
				mvcBuilder.AddApplicationPart(module.Assembly);

				var moduleInitializerType =
					module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
				if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer))) {
					var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
					moduleInitializer.Init(services);
				}
			}

			return services;
		}
	}
}