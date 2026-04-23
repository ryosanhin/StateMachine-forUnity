using System;
using UnityEngine;
using R3;

namespace Ryosanhin.TimeControllers{
	public class TimeController : IReadOnlyTimeController, IDisposable
	{
		/// <summary>
		/// 親の TimeController
		/// </summary>
		IReadOnlyTimeController _parent;
		
		/// <summary>
		/// 購読解除用
		/// </summary>
		IDisposable _disposable;
		
		/// <summary>
		/// 自身の TimeScale
		/// </summary>
		float _localTimeScale=1f;
		
		ReactiveProperty<float> _timeScale=new ReactiveProperty<float>(1f);
		public ReadOnlyReactiveProperty<float> TimeScale=>_timeScale;
		
		public float DeltaTime=>_timeScale.Value*Time.unscaledDeltaTime;
		
		public TimeController(IReadOnlyTimeController parent){
			this._parent=parent;
			
			if(parent!=null){
				_disposable=parent.TimeScale.Subscribe(timeScale=>{
					RecalculateTimeScale();
				});
			}
		}
		
		/// <summary>
		/// このインスタンスの時間の倍率を変更
		/// </summary>
		/// <param name="timeScale">倍率</param>
		public void SetLocalTimeScale(float timeScale){
			_localTimeScale=timeScale;
			RecalculateTimeScale();
		}
		
		/// <summary>
		/// 時間の倍率を再計算
		/// </summary>
		void RecalculateTimeScale(){
			float parentTimeScale=_parent?.TimeScale.CurrentValue ?? 1f;
			_timeScale.Value=parentTimeScale*_localTimeScale;
		}
		
		public void Dispose(){
			_disposable.Dispose();
		}
	}
}