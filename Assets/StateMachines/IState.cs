namespace StateMachines.StateMachinesAsync{
	public interface IState<out TEnum> where TEnum : System.Enum
	{
		/// <summary>
		/// ステートの種類
		/// </summary>
		TEnum Type{get;}
	}
}