using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using R3;

using StateMachines.StateMachinesAsync.Commands;

public class CommandStateUI : MonoBehaviour, IBattleCommandUI<ICommandState>
{
	/// <summary>
	/// ボタン
	/// </summary>
	[SerializeField] Button _walk, _attack, _charge, _cancel;

	/// <summary>
	/// 表示をon/offする用
	/// </summary>
	[SerializeField] Canvas _rootCanvas;
	
	public bool IsEnabled=>_rootCanvas.enabled;

	/// <summary>
	/// Disable時に購読解除
	/// </summary>
	CompositeDisposable _disposables;

	public void RegisterState(ICommandState state)
	{
		_disposables=new();

		state.CurrentSelect.Subscribe(num=>
		{
			switch(num)
			{
				case 0:
				EventSystem.current.SetSelectedGameObject(_walk.gameObject);
				state.RequestCancelCooldown();
				break;
				case 1:
				EventSystem.current.SetSelectedGameObject(_attack.gameObject);
				state.RequestCancelCooldown();
				break;
				case 2:
				EventSystem.current.SetSelectedGameObject(_charge.gameObject);
				state.RequestCancelCooldown();
				break;
				case 3:
				EventSystem.current.SetSelectedGameObject(_cancel.gameObject);
				state.RequestCancelCooldown();
				break;
			}
		}).AddTo(_disposables);

		state.IsProceed.Subscribe(b =>
		{
			CloseWindow();
		}).AddTo(_disposables);
	}
	
	public void CloseWindow(){
		_rootCanvas.enabled=false;
		_disposables.Dispose();
	}
}
