using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class TickerBehaviour : MonoBehaviour {
    [SerializeField] private float _interval;
    [SerializeField] protected Transform _origin;

    private CancellationTokenSource _tokenSource;

    private void OnEnable() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new  CancellationTokenSource();
        TickLoopAsync(_tokenSource.Token).Forget();
    }

    private void OnDisable() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }

    private async UniTask TickLoopAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            Tick();
            await UniTask.WaitForSeconds(_interval, cancellationToken: token);
        }
    }
    
    protected abstract void Tick();
    
}