using System.Threading;
using Cysharp.Threading.Tasks;

namespace StateMachines.StateMachinesAsync{
	public interface IExecutableAsyncState<TEnum> : IState<TEnum> where TEnum : System.Enum
	{
		/// <summary>
		/// ステートを実行する
		/// </summary>
		/// <param name="cancellationToken">キャンセルトークン</param>
		/// <returns>StateResult<TEnum>(await可能)</returns>
		UniTask<StateResult<TEnum>> ExecuteAsync(CancellationToken cancellationToken);
	}
}