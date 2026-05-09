using UnityEngine;

public class DamageVisualizer : ProgressVisualizer {
    private IDamagable _damagable;
    
    
    private void OnDisable() {
        Unsubscribe();
    }

    private void OnEnable() {
        Subscribe();
    }
    
    public void SetDamagable(IDamagable damagable) {
        Unsubscribe();
        _damagable = damagable;
        Subscribe();
        // Убрать потом
        OnDamagableSpawned();
    }

    private void Subscribe() {
        if (_damagable == null) return;
        _damagable.HpUpdated += OnHpUpdated;
        _damagable.DamagableDied += OnDamagableDied;
        _damagable.DamagableSpawned += OnDamagableSpawned;
    }

    private void Unsubscribe() {
        if (_damagable == null) return;
        _damagable.HpUpdated -= OnHpUpdated;
        _damagable.DamagableDied -= OnDamagableDied;
        _damagable.DamagableSpawned -= OnDamagableSpawned;
    }


    private void OnDamagableDied() {
        SetProgressPercentage(0, 0);
        ShowBarAnimation(false);
    } 
    
    private void OnDamagableSpawned() {
        SetProgressPercentage(1, _damagable.CurrentHp);
        ShowBarAnimation(true);
    }
    
    
    private void OnHpUpdated(int hp) {
        float progress = (float)hp / _damagable.MaxHp;
        SetProgressPercentage(progress, hp);
    }
}