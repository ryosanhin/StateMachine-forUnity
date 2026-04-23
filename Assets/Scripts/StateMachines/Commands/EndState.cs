using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;

namespace StateMachines.StateMachinesAsync.Commands{
    /// <summary>
    /// チャージ
    /// </summary>
    public interface IEndState : IState<StateType>
    {
        void Accept();
    }

    public sealed class EndState : IEndState, IState<StateType>, IExecutableAsyncState<StateType>
    {
        public StateType Type=>StateType.Charging;
        
        public EndState(CommandBuilder commandBuilder){}
        
        public async UniTask<StateResult<StateType>> ExecuteAsync(CancellationToken cancellationToken){
            await UniTask.Yield(cancellationToken:cancellationToken);
            return new StateResult<StateType>(Transition.Finish, StateType.End);
        }

        public void Accept(){}
    }
}