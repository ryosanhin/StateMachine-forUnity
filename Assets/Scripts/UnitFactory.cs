using UnityEngine;

using Ryosanhin.TimeControllers;
using Ryosanhin.TimeControllers.Wrappers;

public class UnitFactory : MonoBehaviour
{
	TimeController _timeController;
	
	[SerializeField] TimelineIcon _timelineIcon;
	[SerializeField] RectTransform _clock;
	[SerializeField] UnitController _unitPrefab;

	public void Construct(InGameTime inGameTime){
		this._timeController=inGameTime.Entity;
	}
	
	public IUnit CreateUnit(){
		var unitTime=new TimeController(_timeController);
		var actionEnergy=new ActionEnergy(unitTime);
		var unit=Instantiate(_unitPrefab).GetComponent<UnitController>();
		unit.Init(actionEnergy);
		return unit;
		/*
		var icon=Instantiate(_timelineIcon).transform as RectTransform;
		icon.SetParent(_clock);
		icon.GetComponent<TimelineIcon>().Init(actionEnergy, _clock);
		*/
	}
}
