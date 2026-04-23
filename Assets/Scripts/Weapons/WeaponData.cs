using UnityEngine;

namespace StateMachines.StateMachinesAsync.Commands{
[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        /// <summary>
		/// 対象に取れる回数
		/// </summary>
        [SerializeField] int _targetCount;
        /// <summary>
		/// 対象に取れる回数
		/// </summary>
        public int TargetCount=>_targetCount;
    }
}