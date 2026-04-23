using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;

namespace StateMachines.StateMachinesAsync.Commands{
    /// <summary>
    /// チャージ
    /// </summary>
    public interface IChargeState : IState<StateType>{}

    public sealed class ChargeState : IChargeState, IState<StateType>, IExecutableAsyncState<StateType>
    {
        public StateType Type=>StateType.Charging;
        
        public ChargeState(CommandBuilder commandBuilder){}
        
        public async UniTask<StateResult<StateType>> ExecuteAsync(CancellationToken cancellationToken){
            await UniTask.Yield(cancellationToken:cancellationToken);
            return new StateResult<StateType>(Transition.Finish, StateType.End);
        }
    }
}