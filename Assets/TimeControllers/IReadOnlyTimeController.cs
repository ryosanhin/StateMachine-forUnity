using System;
using R3;

namespace Ryosanhin.TimeControllers{
	public interface IReadOnlyTimeController{
		/// <summary>
		/// 時間の倍率(ReactiveExtensions)
		/// </summary>
		ReadOnlyReactiveProperty<float> TimeScale{get;}
		
		/// <summary>
		/// 所謂 Time.deltaTime
		/// </summary>
		float DeltaTime{get;}
	}
}