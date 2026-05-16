using System;
using UnityEngine;

[Serializable]
public class InvincibleBonus : IBonus {
    public BonusType Type => BonusType.Invincible;

    public void Use(IBonusUser player) {
        player.SetInvincible(true);
        // Debug.Log("Включена невидимость");
    }

    public void StopWork(IBonusUser player) {
        player.SetInvincible(false);
        // Debug.Log("Невидимость выключена");
    }
}