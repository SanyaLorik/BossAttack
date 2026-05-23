using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public abstract class TickerBehaviour : MonoBehaviour, IValueGetter {
    [SerializeField] private float _intervalToFindTarget;
    [SerializeField] protected Transform _origin;
    
    protected List<IPlayer> _targets = new();

    private CancellationTokenSource _tokenSource;
    private Func<float> _rateOfFire;
    private UniTask _waitForInitGetterTask;
    private UniTask _waitForSecondToInitTask;
    
    
    [Inject] GameData _gameData;

    
    protected abstract void Tick();
    protected abstract void FindNewTargets();
    protected abstract void OnStart();
    protected abstract void OnEnd();

    
    public void SetValueGetter(Func<float> valueGetter) {
        _rateOfFire = valueGetter;
    }
    

    public void StartSystem() {
        Debug.Log("Start ability");
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new  CancellationTokenSource();
        TickLoopAsync(_tokenSource.Token).Forget();
        FindTargetLoopAsync(_tokenSource.Token).Forget();
        OnStart();
    }

    
    public void Stop() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        OnEnd();
    }


    private async UniTask TickLoopAsync(CancellationToken token) {
        InitializeTasks();
        await UniTask.WhenAny(_waitForInitGetterTask, _waitForSecondToInitTask);
        if (_rateOfFire == null) {
            Debug.LogError("_rateOfFire == null");
        }
        float interval = _rateOfFire();
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

    
    private void InitializeTasks() {
        _waitForInitGetterTask = UniTask.WaitWhile(() => _rateOfFire == null);
        _waitForSecondToInitTask = UniTask.WaitForSeconds(2f);
    }
    
   
}