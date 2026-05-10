using System;
using UnityEngine;

[Serializable]
public class DamageEffect : IEffect {
    [SerializeField] private int _damage;

    
    public void ApplyEffect(IPlayer player) {
        player.Damagable.ApplyDamage(_damage);
    }
}