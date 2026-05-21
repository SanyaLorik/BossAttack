using System;
using UnityEngine;

public interface IDamagable {
    public void ApplyDamage(int damage);
    public void ApplyHeal(int hp);
    
    public int CurrentHp { get; }
    public int MaxHp { get; }
    public void SetMaxHpGetter(Func<int> valueGetter);
    public void Respawn(bool silent);
    
    public Transform Transform { get; }
    public event Action<IDamagable> DamagableDied;
    public event Action<IDamagable> DamagableSpawned;
    public event Action<int> HpUpdated;
}

