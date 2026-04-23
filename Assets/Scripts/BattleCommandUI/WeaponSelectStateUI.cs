using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using R3;

using StateMachines.StateMachinesAsync.Commands;

public class WeaponSelectStateUI : MonoBehaviour, IBattleCommandUI<IWeaponSelectState>
{
	[SerializeField] Button _primary, _secondary;
	[SerializeField] Canvas _rootCanvas;
	
	public bool IsEnabled=>_rootCanvas.enabled;

	/// <summary>
	/// Disable時に購読解除
	/// </summary>
	CompositeDisposable _disposables;

	public void RegisterState(IWeaponSelectState state)
	{
		_disposables=new();
		
		state.CurrentSelect.Subscribe(num =>
		{
			switch (num)
			{
				case 0:
				EventSystem.current.SetSelectedGameObject(_primary.gameObject);
				state.RequestCancelCooldown();
				break;
				
				case 1:
				EventSystem.current.SetSelectedGameObject(_secondary.gameObject);
				state.RequestCancelCooldown();
				break;
			}
		}).AddTo(_disposables);
	}

	public void CloseWindow(){
		_rootCanvas.enabled=false;
		_disposables.Dispose();
	}
}
