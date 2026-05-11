using System;
using UnityEngine;

[Serializable]
public class DamageEffect : IEffect, IEffectValue {
    [SerializeField] private int _damage;

    private Func<float> _damageGetter;

    public void SetValueGetter(Func<float> damageGetter) {
        _damageGetter = damageGetter;
    }
    
    public void ApplyEffect(IPlayer player) {
        float damage = _damageGetter == null ?  _damage : _damageGetter();
        player.Damagable.ApplyDamage((int)damage);
    }
}