namespace StateMachines.StateMachinesAsync{
	/// <summary>
	/// ステートファクトリー用インターフェース
	/// </summary>
	/// <param name="TEnum">ステートのタイプを表す列挙型</param>
	public interface IStateFactory<TEnum> where TEnum : System.Enum
	{
		/// <summary>
		/// ステートファクトリー用インターフェース
		/// </summary>
		/// <param name="stateType">ステートのタイプ</param>
		/// <returns>IBattleState<TEnum></returns>
		IExecutableAsyncState<TEnum> GetState(TEnum stateType);
	}
}