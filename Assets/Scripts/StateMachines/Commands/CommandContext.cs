using UnityEngine;

namespace StateMachines.StateMachinesAsync.Commands{
	public interface ICommandContext{
		/// <summary>
		/// 行動を起こすユニット
		/// </summary>
		IUnit Actor{get;}
	}
	
	public class PositioningContext : ICommandContext
	{
		public IUnit Actor{get;}

		/// <summary>
		/// 移動先の座標
		/// </summary>
		public Vector3 Destination{get;}
		
		public PositioningContext(IUnit actor, Vector3 destination){
			Actor=actor;
			Destination=destination;
		}
	}
	
	public class AttackContext : ICommandContext
	{
		public IUnit Actor{get;}

		/// <summary>
		/// 武器の情報
		/// </summary>
		public WeaponData WeaponData{get;}

		/// <summary>
		/// 攻撃対象
		/// </summary>
		public IUnit[] TargetUnit{get;}

		/// <summary>
		/// 攻撃座標
		/// </summary>
		public Vector3[] TargetPosition{get;}
		
		public AttackContext(IUnit actor, WeaponData weaponData, IUnit[] targetUnit, Vector3[] targetPosition){
			Actor=actor;
			WeaponData=weaponData;
			TargetUnit=targetUnit;
			TargetPosition=targetPosition;
		}
	}
}