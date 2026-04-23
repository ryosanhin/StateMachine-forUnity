using UnityEngine;
using R3;

namespace Ryosanhin.InputServices{
	public interface IInputService{
		/// <summary>
		/// 選択キー系の入力終了時のイベント通知
		/// </summary>
		Observable<Unit> MainSelectUp{get;}
		
		/// <summary>
		/// 取消キー系の入力終了時のイベント通知
		/// </summary>
		Observable<Unit> MainCancelUp{get;}
		
		/// <summary>
		/// 方向キー系の変更時のイベント通知
		/// </summary>
		Observable<Vector2> Direction{get;}
		
		/// <summary>
		/// ポインター（マウスとか）の位置の変更時のイベント通知
		/// </summary>
		Observable<Vector2> PointerPosition{get;}
	}
}