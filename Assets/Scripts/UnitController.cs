using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using Ryosanhin.InputServices;

public class UnitController : MonoBehaviour, IUnit
{
	float _currentFront=-1f;
	ActionEnergy _actionEnergy;
	public ActionEnergy ActionEnergy=>_actionEnergy;
	
	public void Init(ActionEnergy actionEnergy){
		this._actionEnergy=actionEnergy;
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmosSelected(){
		Gizmos.color=Color.red;
		Gizmos.DrawWireCube(transform.position+transform.right+0.5f*transform.up, Vector3.one);
	}
	#endif
}
