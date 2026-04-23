using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;

using Ryosanhin.TimeControllers;
using Ryosanhin.InputServices;

namespace StateMachines.StateMachinesAsync.Commands{
	/// <summary>
	/// まず行動可能キャラを全取得
	/// キャラ選択
	/// 敵ユニットとプレイヤーユニットの行動順番もここで決定
	/// </summary>
	public interface IUnitSelectState : IState<StateType>{}
	
	public sealed class UnitSelectState : IUnitSelectState, IState<StateType>, IExecutableAsyncState<StateType>
	{
		public StateType Type=>StateType.UnitSelect;
		
		public UnitSelectState(CommandBuilder commandBuilder, IInputService inputService){}
		
		public async UniTask<StateResult<StateType>> ExecuteAsync(CancellationToken cancellationToken){
			await UniTask.Yield(cancellationToken:cancellationToken);
			
			return new StateResult<StateType>(Transition.Proceed, StateType.Command);
		}
	}
}