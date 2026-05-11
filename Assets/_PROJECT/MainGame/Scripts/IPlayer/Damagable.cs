using System;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class Damagable : IDamagable {
    
    public Transform Transform { get; private set; }
    public event Action DamagableDied;
    public event Action DamagableSpawned;
    public event Action<int> HpUpdated;
    

    private bool IsDied => CurrentHp == 0;
    public int CurrentHp { get; private set; }
    public int MaxHp => _maxHpGetter();
    private Func<int> _maxHpGetter;
    
    public void SetMaxHpGetter(Func<int> valueGetter) {
        _maxHpGetter = valueGetter;
        CurrentHp = _maxHpGetter();
    }




    public Damagable(Transform transform) {
        Transform = transform;
    }


    public void ApplyDamage(int damage) {
        if(IsDied) return;
        if (damage < 0) damage *= -1;
        CurrentHp -= damage;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, _maxHpGetter());
        HpUpdated?.Invoke(CurrentHp);
        CheckDied();
    }
    

    public void ApplyHeal(int hp) {
        if(IsDied) return;
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
        CurrentHp = 0;
        DamagableDied?.Invoke();
        Debug.Log("Died");
    }

    
    private void CheckDied() {
        if(CurrentHp != 0) return;
        SetDied();
    }
}