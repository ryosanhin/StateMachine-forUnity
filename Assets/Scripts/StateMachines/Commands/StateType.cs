namespace StateMachines.StateMachinesAsync.Commands{
	/// <summary>
	/// ステートの種類
	/// </summary>
	public enum StateType{
		UnitSelect,
		Command,
		Positioning,
		WeaponSelect,
		Targeting,
		Charging,
		End,
	}
}