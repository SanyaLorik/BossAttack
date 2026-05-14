using System;
using UnityEngine;

[Serializable]
public class HealEffect : IEffect, IValueGetter {
    [SerializeField] private int _heal;
    
    private Func<float> _healGetter;
    
    public void SetValueGetter(Func<float> damageGetter) {
        _healGetter = damageGetter;
    }
    
    public void ApplyEffect(IPlayer player) {
        if (player.Damagable != null) {
            float heal = _healGetter == null ?  _heal : _healGetter();
            player.Damagable.ApplyHeal((int)heal);
        }
    }
}