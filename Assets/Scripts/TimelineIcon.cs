using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

using Ryosanhin.TimeControllers;

public class TimelineIcon : MonoBehaviour
{
	RectTransform _rectTransform;
	
	[SerializeField] Transform _iconPin;
	
	public void Init(ActionEnergy actionEnergy, RectTransform clock){
		_rectTransform=transform as RectTransform;
		_rectTransform.anchoredPosition=Vector2.zero;
		
		actionEnergy.Energy.Subscribe(energy=>{
			var radius=90f;
			var coefficient=30f*Mathf.Deg2Rad;
			_rectTransform.anchoredPosition=radius*new Vector2(Mathf.Sin(energy*coefficient), Mathf.Cos(energy*coefficient));
			_iconPin.rotation=LookAt2D(_iconPin.position, clock.position, -Vector3.up);
		}).AddTo(this);
	}
	
	Quaternion LookAt2D(Vector3 ownPosition, Vector3 targetPosition, Vector3 forward){
		Vector3 diff=targetPosition-ownPosition;
		return Quaternion.FromToRotation(forward, diff);
	}
}
