using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using VContainer;
using R3;

using Ryosanhin.InputServices;
using Ryosanhin.TimeControllers;
using Ryosanhin.TimeControllers.Wrappers;
using StateMachines.StateMachinesAsync;
using StateMachines.StateMachinesAsync.Commands;

public class TestGameSystem : MonoBehaviour
{
	List<IUnit> _units=new();
	TimeController _inGameTimeController, _outGameTimeContoller;
	IInputService _inputService;
	[SerializeField] UnitFactory _unitFactory;

	[SerializeField] BattleCommandUIPresenter _battleCommandUIPresenter;
	
	[Inject]
	public void Construct(IInputService inputService, InGameTime inGameTime, OutGameTime outGameTime){
		_inputService=inputService;
		_inGameTimeController=inGameTime.Entity;
		_outGameTimeContoller=outGameTime.Entity;
		
		AddUnit(_unitFactory.CreateUnit());
		MainLoop(destroyCancellationToken).Forget();
	}
	
	public async UniTask MainLoop(CancellationToken cancellationToken){
		while(true){
			foreach(IUnit unit in _units){
				unit.ActionEnergy.Update();
			}
			
			var readyUnits=_units.Where(unit => unit.ActionEnergy.IsFull.CurrentValue);
			foreach(IUnit unit in readyUnits){
				await InputPlayerCommand(cancellationToken);
			}
			await UniTask.Yield(cancellationToken:cancellationToken);
		}
	}

	/// <summary>
	/// プレイヤーの入力
	/// </summary>
	/// <returns>UniTask：await可能</returns>
	async UniTask InputPlayerCommand(CancellationToken cancellationToken)
	{
		var factory=new StateFactory(_inputService, _outGameTimeContoller);
		var stateMachine=new StateMachine<StateType>(factory);
		
		_battleCommandUIPresenter.RegisterStateMachine(stateMachine);

		await stateMachine.ExecuteAsync(StateType.Command, cancellationToken);
	}

	public void AddUnit(IUnit unit)
	{
		_units.Add(unit);
	}
	
	public void RemoveUnit(IUnit unit){

		_units.Remove(unit);
	}
}
