using System;
using UnityEngine;

[Serializable]
public class DamageEffect : IEffect, IEffectValue {
    [SerializeField] private int _damage;

    private Func<int> _damageGetter;

    public void SetValueGetter(Func<int> damageGetter) {
        _damageGetter = damageGetter;
    }
    
    public void ApplyEffect(IPlayer player) {
        int damage = _damageGetter == null ?  _damage : _damageGetter();
        player.Damagable.ApplyDamage(damage);
    }
}