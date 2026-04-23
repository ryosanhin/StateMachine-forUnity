using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;

using Ryosanhin.TimeControllers;
using Ryosanhin.TimeControllers.Wrappers;
using Ryosanhin.InputServices;

public class FieldPointer : IDisposable
{
	IInputService _inputService;
	IReadOnlyTimeController _timeContoller;

	ReactiveProperty<Vector3> _position=new();
	/// <summary>
	/// ポインターの位置
	/// </summary>
	public ReadOnlyReactiveProperty<Vector3> Position=>_position;
	
	ReactiveProperty<IUnit> _unit=new();
	/// <summary>
	/// ポインター下のユニット
	/// </summary>
	public ReadOnlyReactiveProperty<IUnit> Unit=>_unit;
	
	
	/// <summary>
	/// 有効な入力があったときのイベント通知
	/// </summary>
	Subject<Vector3> _onValidSelect=new();
	/// <summary>
	/// 有効な入力があったときのイベント通知（Rx）
	/// </summary>
	public Observable<Vector3> OnValidSelect=>_onValidSelect;

	/// <summary>
	/// 無効な入力があったときのイベント通知
	/// </summary>
	Subject<Vector3> _onInvalidSelect=new();
	/// <summary>
	/// 無効な入力があったときのイベント通知（Rx）
	/// </summary>
	public Observable<Vector3> OnInvalidSelect=>_onInvalidSelect;

	/// <summary>
	/// キャンセル時のイベント通知
	/// </summary>
	Subject<Unit> _onCancel=new();
	/// <summary>
	/// キャンセル時のイベント通知（Rx）
	/// </summary>
	public Observable<Unit> OnCancel=>_onCancel;

	/// <summary>
	/// IInputService の購読をまとめて破棄
	/// </summary>
	CompositeDisposable _disposables;
	
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="inputService">入力読み取り用</param>
	/// <param name="outGameTime">タイムスケール読み取り用</param>
	public FieldPointer(IInputService inputService, OutGameTime outGameTime)
	{
		_inputService=inputService;
		_timeContoller=outGameTime.Entity;
		_disposables=new();

		// ポインターによるカーソル移動設定
		inputService.PointerPosition.Subscribe(pos=>{
			const int groundLayerMask = 1<<7;
			const int unitLayerMask = 1<<8;
			var mainCamera = Camera.main;
			var ray = mainCamera.ScreenPointToRay(pos);
			
			if(Physics.Raycast(
				ray,
				out RaycastHit groundHit,
				maxDistance:Mathf.Infinity,
				layerMask:groundLayerMask))
			{
				_position.Value = groundHit.point;
			}
			
			if(Physics.Raycast(
				ray,
				out RaycastHit unitHit,
				maxDistance:Mathf.Infinity,
				layerMask:unitLayerMask))
			{
				_unit.Value = unitHit.collider.GetComponent<IUnit>();
			}else{
				_unit.Value = null;
			}
		}).AddTo(_disposables);
		
		// キーによるカーソル移動設定
		inputService.Direction.Subscribe(dire=>{
			_position.Value+=_timeContoller.DeltaTime*new Vector3(dire.x, 0f, dire.y);
		}).AddTo(_disposables);
	}
	
	/// <summary>
	/// ポインターの位置から選択可能な座標を取得
	/// </summary>
	/// <param name="validator">座標が有効か否か判断</param>
	/// <param name="cancellationToken">キャンセルトークン</param>
	/// <returns>(bool, Vector3)（タプル、await可能）</returns>
	public async UniTask<(bool, Vector3)> SelectPositionAsync(Func<Vector3, bool> validator, CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{	
			var confirm = await ConfirmAsync(cancellationToken);
			
			// キャンセルだったら
			if (confirm == 1)
			{
				return (false, default);
			}
			
			var currentPosition = _position.Value;
			
			if (validator(currentPosition))
			{
				_onValidSelect.OnNext(currentPosition);
				return (true, currentPosition);
			}else{
				_onInvalidSelect.OnNext(currentPosition);
			}
		}
		
		return (false, default);
	}
	
	/// <summary>
	/// ポインターの位置から選択可能なユニットを取得
	/// </summary>
	/// <param name="validator">ユニットが有効か否か判断</param>
	/// <param name="cancellationToken">キャンセルトークン</param>
	/// <returns>(bool, IUnit)（タプル、await可能）</returns>
	public async UniTask<(bool, IUnit)> SelectUnitAsync(Func<IUnit, bool> validator, CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			var confirm = await ConfirmAsync(cancellationToken);
			
			// キャンセルだったら
			if (confirm == 1)
			{
				return (false, null);
			}
			
			var currentPosition = _position.Value;
			var currentUnit = _unit.Value;
			
			if (validator(currentUnit))
			{
				_onValidSelect.OnNext(currentPosition);
				return (true, currentUnit);
			}else{
				_onInvalidSelect.OnNext(currentPosition);
			}
		}
		
		return (false, null);
	}
	
	public void Dispose(){
		_disposables?.Dispose();
	}
	
	/// <summary>
	/// プレイヤーの入力の確定を取得
	/// </summary>
	/// <param name="cancellationToken">キャンセルトークン</param>
	/// <returns>0：確定、1：キャンセル（await可能）</returns>
	async UniTask<int> ConfirmAsync(CancellationToken cancellationToken)
	{
		using var linkedCts= CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		var ct=linkedCts.Token;
		var selectTask = _inputService.MainSelectUp.FirstAsync(ct).AsUniTask();
		var cancelTask = _inputService.MainCancelUp.FirstAsync(ct).AsUniTask();
		
		var (index, _) = await UniTask.WhenAny<Unit>(selectTask, cancelTask);
		
		linkedCts.Cancel();
		return index;
	}
}