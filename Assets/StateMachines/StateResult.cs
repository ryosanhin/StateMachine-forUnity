namespace StateMachines.StateMachinesAsync{
	/// <summary>
	/// ステートの結果(ジェネリック)
	/// </summary>
	/// <param name="TEnum">ステートのタイプを表す列挙型</param>
	public readonly struct StateResult<TEnum> where TEnum : System.Enum
	{
		public readonly Transition Transition{get;}
		public readonly TEnum NextStateType{get;}
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="transition">次の遷移の向き</param>
		/// <param name="nextStateType">次に実行するステート</param>
		public StateResult(Transition transition, TEnum nextStateType){
			this.Transition=transition;
			this.NextStateType=nextStateType;
		}
	}
}