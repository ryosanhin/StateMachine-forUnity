using VContainer;
using VContainer.Unity;

public class MainGameLifetimeScope : LifetimeScope
{
	protected override void Configure(IContainerBuilder builder){
		builder.Register<FieldPointer>(Lifetime.Singleton);
	}
}
