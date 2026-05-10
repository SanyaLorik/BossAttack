using System;
using UnityEngine;

[Serializable]
public class HealEffect : IEffect {
    [SerializeField] private int _heal;
    
    public void ApplyEffect(IPlayer player) {
        if (player.Damagable != null) {
            player.Damagable.ApplyHeal(_heal);
        }
    }
}