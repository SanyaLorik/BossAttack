using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public abstract class TickerBehaviour : MonoBehaviour, IValueGetter {

    [SerializeField] private float _interval;
    [SerializeField] protected Transform _origin;

    private CancellationTokenSource _tokenSource;
    private Func<float> _intervalGetter;
    
    [Inject] GameData _gameData;
    
    
    public void Stop() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
       OnEnd();
    }

    public void Start() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new  CancellationTokenSource();
        TickLoopAsync(_tokenSource.Token).Forget();
        OnStart();
    }


    private void OnDisable() {
        Stop();
    }

    private async UniTask TickLoopAsync(CancellationToken token) {
        float interval = _intervalGetter == null ?  _interval : _intervalGetter();
        interval = MathF.Max(interval, _gameData.PlayerRateOfFireMinimum);
        while (!token.IsCancellationRequested) {
            Tick();
            await UniTask.WaitForSeconds(interval, cancellationToken: token);
        }
    }
    
    protected abstract void Tick();
    protected abstract void OnStart();
    protected abstract void OnEnd();

    public void SetValueGetter(Func<float> valueGetter) {
        _intervalGetter = valueGetter;
    }
}