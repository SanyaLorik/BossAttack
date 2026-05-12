using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class ClipCapacity : IAtackCapacity, IValueGetter {
    [SerializeField] private CapacityVisualizer _capacityVisualizer;
    
    private CancellationTokenSource _tokenSource;
    private Func<float> _maxCountGetter;
    public bool AllowToUse => CurrentCount > 0;
    public int MaxCount => (int)_maxCountGetter();
    public int CurrentCount { get; private set; }
    


    [Inject] GameData _gameData;
    
    public void SetValueGetter(Func<float> valueGetter) {
        _maxCountGetter = valueGetter;
    }
    
    
    public void StartCheckCapacity(bool start) {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        if (start) {
            SetFull();
            _tokenSource = new CancellationTokenSource();
            CheckCapacityAsync(_tokenSource.Token).Forget();
        }

    }
    
    public void SpendOne() {
        if (CurrentCount != 0) {
            CurrentCount--;
            _capacityVisualizer.SetCapacityValue(CurrentCount, MaxCount);
        }
    }
    
    private void AddOne() {
        if (CurrentCount != MaxCount) {
            CurrentCount++;
            _capacityVisualizer.SetCapacityValue(CurrentCount, MaxCount);
        }
    }
    
    public void SetFull() {
        CurrentCount = MaxCount;
        _capacityVisualizer.SetCapacityValue(CurrentCount, MaxCount);
    }

    private async UniTask CheckCapacityAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            await UniTask.WaitWhile(() => CurrentCount == MaxCount, cancellationToken: token);
            await UniTask.WaitForSeconds(_gameData.ClipReloadOneBulletDuration, cancellationToken: token);
            AddOne();
        }
    }


}