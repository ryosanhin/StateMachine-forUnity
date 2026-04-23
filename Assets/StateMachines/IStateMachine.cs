using R3;

namespace StateMachines.StateMachinesAsync{
	public interface IStateMachine<TEnum> where TEnum : System.Enum
	{
		/// <summary>
		/// 現在実行中のステート
		/// </summary>
		IState<TEnum> CurrentState{get;}
		
		/// <summary>
		/// ステート遷移時のイベント通知
		/// </summary>
		Observable<TransitionEventArgs<TEnum>> OnStateTransition{get;}
	}
}