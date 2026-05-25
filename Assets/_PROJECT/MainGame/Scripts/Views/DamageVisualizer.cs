
using System;
using SanyaBeerExtension;

public class DamageVisualizer : ProgressVisualizer {
    private IDamagable _damagable;


    public void SetDamagable(IDamagable damagable) {
        Unsubscribe();
        _damagable = damagable;
        Subscribe();
        // Убрать потом
        OnDamagableSpawned(_damagable);
    }
    
    
    private void OnDisable() {
        Unsubscribe();
    }

    private void OnEnable() {
        Subscribe();
    }

    public void HideVisual() {
        ShowVisual();
    }

    public void ShowVisual() {
        _progressContainer.ActiveSelf();
    }
    


    private void Subscribe() {
        if (_damagable == null) return;
        _damagable.HpUpdated += OnHpUpdated;
        _damagable.DamagableDied += OnDamagableDied;
        _damagable.DamagableSpawned += OnDamagableSpawned;
    }

    public void Unsubscribe() {
        if (_damagable == null) return;
        _damagable.HpUpdated -= OnHpUpdated;
        _damagable.DamagableDied -= OnDamagableDied;
        _damagable.DamagableSpawned -= OnDamagableSpawned;
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