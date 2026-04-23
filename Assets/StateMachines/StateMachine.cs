using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace StateMachines.StateMachinesAsync{
	/// <summary>
	/// IExecutableState<TEnum>を実行
	/// </summary>
	/// <param name="T">ステートのタイプを表す列挙型</param>
	public class StateMachine<TEnum> : IStateMachine<TEnum> where TEnum : System.Enum
	{
		readonly IStateFactory<TEnum> _factory;
		
		IExecutableAsyncState<TEnum> _currentState;
		public IState<TEnum> CurrentState=>_currentState;
		
		Subject<TransitionEventArgs<TEnum>> _onStateTransition=new();
		public Observable<TransitionEventArgs<TEnum>> OnStateTransition=>_onStateTransition;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="factory">ステートを生成するファクトリー</param>
		public StateMachine(IStateFactory<TEnum> factory){
			_factory=factory;
		}
		
		/// <summary>
		/// ステートマシン起動
		/// </summary>
		/// <param name="initialStateType">最初のステート</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		/// <returns>CommandStateType(await可能)</returns>
		public async UniTask ExecuteAsync(TEnum initialStateType, CancellationToken cancellationToken){
			var stateStack=new Stack<IExecutableAsyncState<TEnum>>();
			
			var initialState=_factory.GetState(initialStateType);
			stateStack.Push(initialState);
			_onStateTransition.OnNext(new TransitionEventArgs<TEnum>(Transition.Proceed, initialState));
			
			var isContinue=true;
			
			while(isContinue){
				//Peekは一番上の取得のみ
				_currentState=stateStack.Peek();
				
				var result=await _currentState.ExecuteAsync(cancellationToken);
				
				switch(result.Transition){
					case Transition.Proceed:
					//次のステートを追加
					var nextState=_factory.GetState(result.NextStateType);
					stateStack.Push(nextState);
					_onStateTransition.OnNext(new TransitionEventArgs<TEnum>(result.Transition, nextState));
					break;
					
					case Transition.Back:
					//一番上のステートを取り出し、変数で受けないのでそのまま消滅
					if(stateStack.Count>1){
						stateStack.Pop();
						_onStateTransition.OnNext(new TransitionEventArgs<TEnum>(result.Transition, stateStack.Peek()));
					}
					break;
					
					case Transition.Finish:
					isContinue=false;
					break;
				}
			}
			
			_onStateTransition.OnCompleted();
		}
	}
}