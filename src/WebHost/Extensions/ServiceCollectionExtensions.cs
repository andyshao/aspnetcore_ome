using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions {
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