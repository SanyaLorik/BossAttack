using System;
using UnityEngine;

[Serializable]
public class HealEffect : IEffect {
    [SerializeField] private int _heal;
    
    public void ApplyEffect(IDamagable damagable) {
        damagable.ApplyHeal(_heal);
    }
}