using System;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class Damagable : IDamagable {
    public int CurrentHp { get; private set; }
    public int MaxHp => _maxHpGetter();
    private Func<int> _maxHpGetter;
    
    public void SetMaxHpGetter(Func<int> valueGetter) {
        _maxHpGetter = valueGetter;
        CurrentHp = _maxHpGetter();
    }

    public Transform Transform { get; private set; }
    public event Action DamagableDied;
    public event Action DamagableSpawned;
    public event Action<int> HpUpdated;


    public Damagable(Transform transform) {
        Transform = transform;
    }


    public void ApplyDamage(int damage) {
        if (damage < 0) damage *= -1;
        CurrentHp -= damage;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, _maxHpGetter());
        HpUpdated?.Invoke(CurrentHp);
        CheckDied();
    }
    

    public void ApplyHeal(int hp) {
        if (hp < 0) hp *= -1;
        CurrentHp += hp;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, _maxHpGetter());
        HpUpdated?.Invoke(CurrentHp);
    }
    
    public void SetSpawned() {
        CurrentHp = _maxHpGetter();
        DamagableSpawned?.Invoke();
    }
    
    public void SetDied() {
        if(CurrentHp == 0) return;
        CurrentHp = 0;
        DamagableDied?.Invoke();
        Debug.Log("Died");
    }

    
    private void CheckDied() {
        if(CurrentHp != 0) return;
        SetDied();
    }
}