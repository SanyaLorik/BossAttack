using System;
using UnityEngine;

public interface IDamagable {
    public void ApplyDamage(int damage);
    public void ApplyHeal(int hp);
    
    public int CurrentHp { get; }
    public int MaxHp { get; }
    
    public Transform Transform { get; }
    public event Action DamagableDied;
    public event Action DamagableSpawned;
    public event Action<int> HpUpdated;
}

