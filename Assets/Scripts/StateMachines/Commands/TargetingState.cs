using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;

using Ryosanhin.TimeControllers;
using Ryosanhin.InputServices;

namespace StateMachines.StateMachinesAsync.Commands{
	/// <summary>
	/// 攻撃場所を選択
	/// </summary>
	public interface ITargetingState : IState<StateType>
	{
		/// <summary>
		/// 選択確定（true）/選択中断（false）のイベント通知
		/// </summary>
		Observable<bool> IsProceed{get;}
		
		/// <summary>
		/// 照準の中心の座標
		/// </summary>
		ReadOnlyReactiveProperty<Vector3> ReticlePosition{get;}
	}
	
	public sealed class TargetingState : ITargetingState, IState<StateType>, IExecutableAsyncState<StateType>
	{
		/// ------------------------------------------------------
		/// ITargetingState
		/// ------------------------------------------------------
		
		public StateType Type=>StateType.Targeting;
		
		Subject<bool> _isProceed=new();
		public Observable<bool> IsProceed=>_isProceed;
		
		ReactiveProperty<Vector3> _reticlePosition=new();
		public ReadOnlyReactiveProperty<Vector3> ReticlePosition=>_reticlePosition;
		
		/// ------------------------------------------------------
		/// TargetingState
		/// ------------------------------------------------------
		
		IInputService _inputService;
		IReadOnlyTimeController _timeContoller;
		CommandBuilder _commandBuilder;
		/// <summary>
		/// ステートの進む方向
		/// </summary
		int _destination=0;
		
		/// <summary>
		/// IInputService の購読をまとめて破棄
		/// </summary>
		CompositeDisposable _disposables;
		
		public TargetingState(CommandBuilder commandBuilder, IInputService inputService, IReadOnlyTimeController timeContoller){
			_inputService=inputService;
			_timeContoller=timeContoller;
			_commandBuilder=commandBuilder;
			_disposables=new();
			
			// ポインタ移動を読み取り
			inputService.PointerPosition.Subscribe(pos=>{
				CalculatePosition();
			}).AddTo(_disposables);
			
			// 選択キーによる選択を確定
			inputService.MainSelectUp.Subscribe(_=>{
				_destination=1;
			}).AddTo(_disposables);
			
			// 取消キーによる選択の中断、前のステートに戻る
			inputService.MainCancelUp.Subscribe(_=>{
				_destination=-1;
			}).AddTo(_disposables);

			// キーによるカーソル移動設定
			inputService.Direction.Subscribe(dire=>{
				_reticlePosition.Value=_timeContoller.DeltaTime*new Vector3(dire.x, 0f, dire.y);
			}).AddTo(_disposables);
		}
		
		public async UniTask<StateResult<StateType>> ExecuteAsync(CancellationToken cancellationToken){
			await UniTask.WaitUntil(()=>_destination!=0, cancellationToken:cancellationToken);

			var isProceed=_destination>0;
			
			_isProceed.OnNext(isProceed);
			
			// 購読をSubject側から解除
			_isProceed.OnCompleted();
			_reticlePosition.OnCompleted();
			
			// Disposeを忘れずに！！！
			_disposables.Dispose();
			
			return new StateResult<StateType>(Transition.Finish, StateType.WeaponSelect);
		}
		
		void CalculatePosition(){
			int groundLayerMask=1<<7;
			
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, maxDistance:Mathf.Infinity, layerMask:groundLayerMask)){
				_reticlePosition.Value=hit.point;
			}
		}
	}
}