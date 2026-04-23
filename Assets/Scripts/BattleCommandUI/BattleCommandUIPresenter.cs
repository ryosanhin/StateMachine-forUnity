using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VContainer;
using R3;

using Ryosanhin.InputServices;
using Ryosanhin.TimeControllers;
using Ryosanhin.TimeControllers.Wrappers;
using StateMachines.StateMachinesAsync;
using StateMachines.StateMachinesAsync.Commands;
using System.Collections.Generic;

public class BattleCommandUIPresenter : MonoBehaviour
{
    [SerializeField] CommandStateUI _commandStateUI;

    [SerializeField] WeaponSelectStateUI _weaponSelectStateUI;
    
    public void RegisterStateMachine(IStateMachine<StateType> stateMachine)
    {
        stateMachine.OnStateTransition.Subscribe(args =>
        {
            switch (args.State)
            {
                case ICommandState commandState:
                _commandStateUI.RegisterState(commandState);
                break;

                case IWeaponSelectState weaponSelectState:
                _weaponSelectStateUI.RegisterState(weaponSelectState);
                break;
            }
        }).AddTo(this);
    }
}
