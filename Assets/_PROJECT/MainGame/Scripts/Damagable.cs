using System;
using UnityEngine;

public class Damagable : IDamagable {
    public int CurrentHp { get; private set; }
    public Transform Transform { get; private set; }
    public event Action DamagableDied;
    public event Action<int> HpUpdated;
    private int _maxHp;

    
    public Damagable(int maxHp, Transform transform) {
        _maxHp = maxHp;
        Transform = transform;
    }


    public void ApplyDamage(int damage) {
        if (damage < 0) damage *= -1;
        CurrentHp -= damage;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, _maxHp);
        HpUpdated?.Invoke(CurrentHp);
        Debug.Log("Снято хп " + damage);
        CheckDied();
    }
    

    public void ApplyHeal(int hp) {
        if (hp < 0) hp *= -1;
        CurrentHp += hp;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, _maxHp);
        Debug.Log("Добавлено хп " + hp);
        HpUpdated?.Invoke(CurrentHp);
    }

    private void CheckDied() {
        if(CurrentHp != 0) return;
        DamagableDied?.Invoke();
    }

}