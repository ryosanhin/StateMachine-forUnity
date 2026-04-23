using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

using Ryosanhin.TimeControllers;
using Ryosanhin.InputServices;

namespace StateMachines.StateMachinesAsync.Commands{
	/// <summary>
	/// 行動選択
	/// </summary>
	public interface ICommandState : IState<StateType>
	{
		/// <summary>
		/// 選択確定（true）/選択中断（false）のイベント通知
		/// </summary>
		Observable<bool> IsProceed{get;}
		
		/// <summary>
		/// 現在の選択のイベント通知、長押し用に Observable をUniTaskループ内で複数回発火
		/// </summary>
		Observable<int> CurrentSelect{get;}
		
		/// <summary>
		/// クールダウン状態を解除
		/// </summary>
		void RequestCancelCooldown();
	}
	
	public sealed class CommandState : ICommandState, IState<StateType>, IExecutableAsyncState<StateType>
	{
		/// ------------------------------------------------------
		/// ICommandState
		/// ------------------------------------------------------
		public StateType Type=>StateType.Command;
		
		Subject<bool> _isProceed=new();
		public Observable<bool> IsProceed=>_isProceed;
		
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
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CommandState(IInputService inputService){
			_inputService=inputService;
		}
		
		public async UniTask<StateResult<StateType>> ExecuteAsync(CancellationToken cancellationToken){
			// IInputService の購読をまとめて破棄
			using var disposables=new CompositeDisposable();
			
			// 選択キーによる選択を確定
			_inputService.MainSelectUp.Subscribe(_=>{
				_destination=1;
			}).AddTo(disposables);
			
			// 取消キーによる選択の中断、前のステートに戻る
			_inputService.MainCancelUp.Subscribe(_=>{
				_destination=-1;
			}).AddTo(disposables);

			// キーによるカーソル移動設定
			_inputService.Direction.SubscribeAwait(async (dire, ct) =>
			{
				// 入力が無ければ即return
				if(dire.sqrMagnitude==0f){
					return;
				}
				
				// 安全のためキャンセル操作が最優先になるようにコードで解決
				if(dire.y>0){
					_currentInput=0;
				}else{
					if(dire.y<0){
						_currentInput=2;
					}
				}
				if(dire.x<0){
					_currentInput=3;
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
			
			var isProceed=_destination>0;
			
			_isProceed.OnNext(isProceed);
			
			// 購読をSubject側から解除
			_isProceed.OnCompleted();
			_currentSelect.OnCompleted();
			
			if(isProceed){
				return _currentInput switch{
					0=>new StateResult<StateType>(Transition.Proceed, StateType.Positioning),
					1=>new StateResult<StateType>(Transition.Proceed, StateType.WeaponSelect),
					2=>new StateResult<StateType>(Transition.Proceed, StateType.Charging),
					3=>new StateResult<StateType>(Transition.Back, StateType.UnitSelect),
					_=>throw new InvalidOperationException($"{_currentInput} is not expected value"),
				};
			}
			
			return new StateResult<StateType>(Transition.Back, StateType.UnitSelect);
		}
		
		public void RequestCancelCooldown(){
			_cooldownTaskSource?.TrySetResult();
		}
	}
}