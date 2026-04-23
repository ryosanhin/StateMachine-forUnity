using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;

using Ryosanhin.TimeControllers;
using Ryosanhin.InputServices;
using UnityEngine.Analytics;

namespace StateMachines.StateMachinesAsync.Commands{
	/// <summary>
	/// 攻撃の種類を選択
	/// </summary>
	public interface IWeaponSelectState : IState<StateType>
	{
		/// <summary>
		/// 現在の入力
		/// </summary>
		Observable<int> CurrentSelect{get;}

		/// <summary>
		/// クールダウン状態を解除
		/// </summary>
		void RequestCancelCooldown();
	}
	
	public sealed class WeaponSelectState : IWeaponSelectState, IState<StateType>, IExecutableAsyncState<StateType>
	{
		public StateType Type=>StateType.WeaponSelect;
		
		int _currentInput=0;
		Subject<int> _currentSelect=new();
		public Observable<int> CurrentSelect=>_currentSelect;

		// ------------------------------------------------------
		// field
		// ------------------------------------------------------
		/// <summary>
		/// 入力を受け取り
		/// </summary>
		IInputService _inputService;

		/// <summary>
		/// クールダウン状態
		/// </summary>
		UniTaskCompletionSource _cooldownTaskSource;

		/// <summary>
		/// ステートの進む方向
		/// </summary
		int _destination=0;
		
		public WeaponSelectState(CommandBuilder commandBuilder, IInputService inputService)
		{
			_inputService=inputService;
		}
		
		public async UniTask<StateResult<StateType>> ExecuteAsync(CancellationToken cancellationToken)
		{
			// IInputService の購読をまとめて破棄
			using var disposables=new CompositeDisposable();

			// キーによるカーソル移動設定
			_inputService.Direction.SubscribeAwait(async (dire, ct)=>{
				if (dire.sqrMagnitude == 0f)
				{
					return;
				}

				if(dire.x<0){
					_currentInput=0;
				}else{
					if(dire.x>0){
						_currentInput=1;
					}
				}

				// イベント発行
				_currentSelect.OnNext(_currentInput);

				using(var linkedCts= CancellationTokenSource.CreateLinkedTokenSource(ct))
				{
					// クールダウン/入力確定用のトークン
					var linkedCt=linkedCts.Token;
					
					// クールダウンの準備
					_cooldownTaskSource=new();
					using var registration=linkedCt.Register(()=>{
						_cooldownTaskSource.TrySetCanceled(linkedCt);
					});

					// 入力確定の準備
					var decision=UniTask.WaitUntil(()=>_destination!=0, cancellationToken:linkedCt);
					
					// 待機
					await UniTask.WhenAny(_cooldownTaskSource.Task, decision);
					
					// WhenAny で完了しなかった方のタスクは走り続けるらしい、キャンセル必須っぽい。
					linkedCts.Cancel();
				}

			}, AwaitOperation.Drop).AddTo(disposables);

			await UniTask.WaitUntil(()=>_destination!=0, cancellationToken:cancellationToken);

			return new StateResult<StateType>(Transition.Back, StateType.Command);
		}
		
		/// <summary>
		/// 外部からクールダウンの解除を要請
		/// </summary>
		public void RequestCancelCooldown()
		{
			_cooldownTaskSource?.TrySetResult();
		}
	}
}