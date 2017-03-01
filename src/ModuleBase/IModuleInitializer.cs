using Microsoft.Extensions.DependencyInjection;

namespace aaa.Module {
	public interface IModuleInitializer {
		void Init(IServiceCollection serviceCollection);
	}
}
