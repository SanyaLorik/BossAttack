
public class DamageVisualizer : ProgressVisualizer {
    private IDamagable _damagable;
    
    public void SetDamagable(IDamagable damagable) {
        if (_damagable != null) {
            OnDisable();
        }
        _damagable = damagable;
        
        _damagable.HpUpdated += OnHpUpdated;
        _damagable.DamagableDied += OnDamagableDied;
        _damagable.DamagableSpawned += OnDamagableSpawned;
        // Убрать потом
        OnDamagableSpawned();
    }


    private void OnDisable() {
        if(_damagable == null) return;
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