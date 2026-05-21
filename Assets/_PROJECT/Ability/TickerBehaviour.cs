using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public abstract class TickerBehaviour : MonoBehaviour, IValueGetter {

    [SerializeField] private float _intervalToAtack;
    [SerializeField] private float _intervalToFindTarget;
    [SerializeField] protected Transform _origin;
    
    protected List<IPlayer> _targets = new();

    private CancellationTokenSource _tokenSource;
    private Func<float> _intervalToAtackGetter;
    
    [Inject] GameData _gameData;
    
    
    public void Start() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new  CancellationTokenSource();
        TickLoopAsync(_tokenSource.Token).Forget();
        FindTargetLoopAsync(_tokenSource.Token).Forget();
        OnStart();
    }


    private void OnDisable() {
        Stop();
    }
    
    
    public void Stop() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        OnEnd();
    }


    private async UniTask TickLoopAsync(CancellationToken token) {
        float interval = _intervalToAtackGetter == null ?  _intervalToAtack : _intervalToAtackGetter();
        interval = MathF.Max(interval, _gameData.PlayerRateOfFireMinimum);
        while (!token.IsCancellationRequested) {
            Tick();
            await UniTask.WaitForSeconds(interval, cancellationToken: token);
        }
    }
    
    private async UniTask FindTargetLoopAsync(CancellationToken token) {
        float interval = _intervalToFindTarget;
        interval = MathF.Max(interval, _gameData.MinimumTimeToFindNewTarget);
        while (!token.IsCancellationRequested) {
            FindNewTargets();
            await UniTask.WaitForSeconds(interval, cancellationToken: token);
        }
    }
    
    protected abstract void Tick();
    protected abstract void FindNewTargets();
    protected abstract void OnStart();
    protected abstract void OnEnd();

    public void SetValueGetter(Func<float> valueGetter) {
        _intervalToAtackGetter = valueGetter;
    }
}