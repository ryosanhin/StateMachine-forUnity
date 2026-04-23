using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachines.StateMachinesAsync.Commands{
    public class CommandBuilder
    {
		/// <summary>
		/// 行動を起こすユニット
		/// </summary>
		public IUnit Actor{get;set;}
		
		/// <summary>
		/// 移動先の座標
		/// </summary>
		public Vector3 Destination{get;set;}
		
		/// <summary>
		/// 武器の情報
		/// </summary>
		public WeaponData WeaponData{get;set;}
		
		/// <summary>
		/// 攻撃対象
		/// </summary>
		public List<IUnit> TargetUnit{get;}

		/// <summary>
		/// 攻撃座標
		/// </summary>
		public List<Vector3> TargetPosition{get;}
		
		public CommandBuilder()
		{
			TargetUnit=new List<IUnit>();
			TargetPosition=new List<Vector3>();
		}

        public ICommandContext Build(StateType type)
		{
			return type switch{
				StateType.Positioning=>new PositioningContext(Actor, Destination),
				StateType.Targeting=>new AttackContext(Actor, WeaponData, TargetUnit.ToArray(), TargetPosition.ToArray()),
				StateType.Charging=>null,
				_=>throw new ArgumentOutOfRangeException($"{type} is not expected value")
			};
		}
    }
}