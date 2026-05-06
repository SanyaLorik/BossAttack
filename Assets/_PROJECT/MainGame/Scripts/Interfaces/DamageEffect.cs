using System;
using UnityEngine;

[Serializable]
public class DamageEffect : IEffect {
    [SerializeField] private int _damage;

    
    public void ApplyEffect(IDamagable damagable) {
        damagable.ApplyDamage(_damage);
    }
}