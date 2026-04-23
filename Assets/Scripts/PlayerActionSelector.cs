using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using VContainer;
using R3;

using Ryosanhin.InputServices;
using Ryosanhin.TimeControllers;
using Ryosanhin.TimeControllers.Wrappers;
using StateMachines.StateMachinesAsync;
using StateMachines.StateMachinesAsync.Commands;

public class PlayerActionSelector : MonoBehaviour
{
	IInputService _inputService;
	IReadOnlyTimeController _outGameTime;
	
	[Inject]
	public void Construct(IInputService inputService, OutGameTime outGameTime){
		this._inputService=inputService;
		this._outGameTime=outGameTime.Entity;
		SelectAction(destroyCancellationToken).Forget();
	}
	
	public async UniTask SelectAction(CancellationToken cancellationToken){
		var factory=new StateFactory(_inputService, _outGameTime);
		var stateMachine=new StateMachine<StateType>(factory);
		
		await stateMachine.ExecuteAsync(StateType.Command, cancellationToken);
		
		await UniTask.Yield(cancellationToken:cancellationToken);
	}
}
