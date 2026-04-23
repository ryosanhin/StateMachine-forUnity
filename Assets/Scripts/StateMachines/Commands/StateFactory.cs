using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

using Ryosanhin.InputServices;
using Ryosanhin.TimeControllers;

namespace StateMachines.StateMachinesAsync.Commands{
	public class StateFactory : IStateFactory<StateType>
	{
		IInputService _inputService;
		IReadOnlyTimeController _outGameTime;
		CommandBuilder _commandBuilder;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="inputService">入力読み取り用</param>
		public StateFactory(IInputService inputService, IReadOnlyTimeController outGameTime){
			this._inputService=inputService;
			this._outGameTime=outGameTime;
			this._commandBuilder=new();
		}
		
		public IExecutableAsyncState<StateType> GetState(StateType type){
			return type switch{
				StateType.UnitSelect=>new UnitSelectState(_commandBuilder, _inputService),
				StateType.Command=>new CommandState(_inputService),
				StateType.Positioning=>new PositioningState(_commandBuilder, _inputService, _outGameTime),
				StateType.WeaponSelect=>new WeaponSelectState(_commandBuilder, _inputService),
				StateType.Targeting=>new TargetingState(_commandBuilder, _inputService, _outGameTime),
				StateType.Charging=>new ChargeState(_commandBuilder),
				StateType.End=>new EndState(_commandBuilder),
			};
		}
	}
}