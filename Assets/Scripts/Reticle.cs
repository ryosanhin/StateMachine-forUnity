using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VContainer;
using R3;

using Ryosanhin.InputServices;

public class Reticle : MonoBehaviour
{
	public Vector3 _reticlePosition;
	
	[Inject]
	public void Construct(FieldPointer fieldPointer){
		fieldPointer.Position.Subscribe(pos=>{
			_reticlePosition=pos;
			transform.position=pos;
		}).AddTo(this);
	}
}
