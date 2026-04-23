using UnityEngine;
using UnityEngine.InputSystem;
using R3;

namespace Ryosanhin.InputServices{
	public class InputService : MonoBehaviour, IInputService
	{
		[SerializeField] PlayerInput _playerInput;
		
		
		Subject<Unit> _mainSelectUp=new();
		public Observable<Unit> MainSelectUp=>_mainSelectUp;
		
		Subject<Unit> _mainCancelUp=new();
		public Observable<Unit> MainCancelUp=>_mainCancelUp;
		
		Subject<Vector2> _direction=new();
		public Observable<Vector2> Direction=>_direction;
		
		Subject<Vector2> _pointerPosition=new();
		public Observable<Vector2> PointerPosition=>_pointerPosition;
		
		InputAction _mainSelectAction;
		InputAction _mainCancelAction;
		InputAction _directionAction;
		InputAction _pointerPositionAction;
		
		void Awake(){
			_mainSelectAction=_playerInput.actions["MainSelect"];
			_mainCancelAction=_playerInput.actions["MainCancel"];
			_directionAction=_playerInput.actions["Direction"];
			_pointerPositionAction=_playerInput.actions["PointerPosition"];
		}
		
		void OnEnable(){
			_mainSelectAction.canceled+=OnMainSelectCanceled;
			_mainCancelAction.canceled+=OnMainCancelCanceled;
			_pointerPositionAction.performed+=OnPointerPositionPerformed;
		}
		
		void OnDisable(){
			_mainSelectAction.canceled-=OnMainSelectCanceled;
			_mainCancelAction.canceled-=OnMainCancelCanceled;
			_pointerPositionAction.performed-=OnPointerPositionPerformed;
		}
		
		/// <summary>
		/// Direction は押しっぱなしの可能性があるので Update 内で処理
		/// </summary>
		void Update(){
			_direction.OnNext(_directionAction.ReadValue<Vector2>());
		}
		
		/// <summary>
		/// MainSelect がキャンセルされた = ボタンが押下から解除されたときのイベント
		/// </summary>
		void OnMainSelectCanceled(InputAction.CallbackContext context){
			_mainSelectUp.OnNext(Unit.Default);
		}
		
		/// <summary>
		/// MainCancel がキャンセルされた = ボタンが押下から解除されたときのイベント
		/// </summary>
		void OnMainCancelCanceled(InputAction.CallbackContext context){
			_mainCancelUp.OnNext(Unit.Default);
		}
		
		/// <summary>
		/// PointerPosition が動作した = 値が変化したときのイベント
		/// </summary>
		void OnPointerPositionPerformed(InputAction.CallbackContext context){
			_pointerPosition.OnNext(context.ReadValue<Vector2>());
		}
	}
}