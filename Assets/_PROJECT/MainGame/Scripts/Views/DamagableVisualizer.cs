using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class DamagableVisualizer : ProgressVisualizer {
    [SerializeField] private DamagableProvider _damagableProvider;
    
    private IDamagable _damagable;

    private MainGameStarter _mainGameStarter;
    
    [Inject]
    private void Initialize(MainGameStarter mainGameStarter) {
        _mainGameStarter = mainGameStarter;
        _mainGameStarter.GameStarted += OnGameStarted;
    }
    
    private void Start() {
        InitializeDamagable();
    }
    
    
    private void OnDisable() {
        DamagableUnsubscribe();
        _mainGameStarter.GameStarted -= OnGameStarted;
    }
    

    private void OnEnable() {
        DamabableSubscribe();
    }

    
    private void InitializeDamagable() {
        _damagable = _damagableProvider.Damagable;
        if(_damagable == null) {
            _damagableProvider.DamageInitialized += InitializeDamagable; 
            return;
        }
        _damagableProvider.DamageInitialized -= InitializeDamagable; 
        
        SetProgressPercentage(1, _damagable.CurrentHp);
        DamabableSubscribe();
        DamabableSubscribe();
    }


    private void DamabableSubscribe() {
        if (_damagable == null) return;
        _damagable.HpUpdated += OnHpUpdated;
        _damagable.DamagableDied += OnDamagableDied;
        _damagable.DamagableSpawned += OnDamagableSpawned;
        
    }

    public void DamagableUnsubscribe() {
        if (_damagable == null) return;
        _damagable.HpUpdated -= OnHpUpdated;
        _damagable.DamagableDied -= OnDamagableDied;
        _damagable.DamagableSpawned -= OnDamagableSpawned;
    }

    
    private void OnGameStarted(bool started) {
        if (!started) {
            FastHide();
        }
        else {
            FastShow();
        }
    }
   
    public void HideVisual() {
        _progressContainer.DisactiveSelf();
    }



    private void OnDamagableDied(IDamagable damagable) {
        SetProgressPercentage(0, 0);
        ShowBarAnimation(false);
    } 
    
    private void OnDamagableSpawned(IDamagable damagable) {
        SetProgressPercentage(1, _damagable.CurrentHp);
        ShowBarAnimation(true);
    }
    
    
    private void OnHpUpdated(int hp) {
        float progress = (float)hp / _damagable.MaxHp;
        SetProgressPercentage(progress, hp);
    }
}