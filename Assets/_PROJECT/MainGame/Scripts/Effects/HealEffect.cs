using System;
using UnityEngine;

[Serializable]
public class HealEffect : IEffect {
    [SerializeField] private int _heal;
    
    public void ApplyEffect(IPlayer player) {
        player.Damagable.ApplyHeal(_heal);
    }
}