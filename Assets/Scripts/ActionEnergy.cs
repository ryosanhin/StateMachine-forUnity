using R3;

using Ryosanhin.TimeControllers;

public class ActionEnergy
{
	const float MAX_ENERGY=12f;
	
	float _speed=6f;
	IReadOnlyTimeController _timeController;
	
	ReactiveProperty<float> _energy=new ReactiveProperty<float>();
	public ReadOnlyReactiveProperty<float> Energy=>_energy;
	
	ReactiveProperty<bool> _isFull=new ReactiveProperty<bool>();
	public ReadOnlyReactiveProperty<bool> IsFull=>_isFull;
	
	public ActionEnergy(IReadOnlyTimeController timeController){
		this._timeController=timeController;
	}
	
	public void Update(){
		AddEnergy(_timeController.DeltaTime*_speed);
	}
	
	public void AddEnergy(float energy){
		if(_energy.Value>=MAX_ENERGY){
			_energy.Value=MAX_ENERGY;
			_isFull.Value=true;
		}else{
			_energy.Value+=energy;
		}
	}
	
	public void ComsumeEnergy(float energy){
		_energy.Value-=energy;
		_isFull.Value=false;
	}
}
