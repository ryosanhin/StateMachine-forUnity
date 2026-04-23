using StateMachines.StateMachinesAsync;
using StateMachines.StateMachinesAsync.Commands;

public interface IBattleCommandUI<in T> where T : IState<StateType>
{
	/// <summary>
	/// 表示/非表示
	/// </summary>
	bool IsEnabled{get;}

	/// <summary>
	/// UIの表示、ステートとの連携
	/// </summary>
	/// <param name="state">ステート</param>
	void RegisterState(T state);
	
	/// <summary>
	/// UIを非表示にしたいとき用
	/// </summary>
	void CloseWindow();
}
