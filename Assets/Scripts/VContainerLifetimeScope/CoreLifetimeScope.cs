using UnityEngine;
using VContainer;
using VContainer.Unity;

using Ryosanhin.InputServices;
using Ryosanhin.TimeControllers;
using Ryosanhin.TimeControllers.Wrappers;

public class CoreLifetimeScope : LifetimeScope
{
	TimeController _globalTime;
	TimeController _inGameTime;
	TimeController _outGameTime;
	
	[SerializeField] GameObject _inputServices;
	
    protected override void Configure(IContainerBuilder builder){
		_globalTime=new(null);
		_inGameTime=new(_globalTime);
		_outGameTime=new(_globalTime);
		
		builder.Register<GlobalTime>(Lifetime.Singleton).WithParameter<TimeController>(_globalTime);
		builder.Register<InGameTime>(Lifetime.Singleton).WithParameter<TimeController>(_inGameTime);
		builder.Register<OutGameTime>(Lifetime.Singleton).WithParameter<TimeController>(_outGameTime);

		builder.RegisterComponent(_inputServices.GetComponent<IInputService>());
    }
}
