using System;
using UnityEngine;

[Serializable]
public class SpeedBonus : IBonus {
    public BonusType Type => BonusType.Speed;

    
    public void Use(IBonusUser player) {
        player.SetBonusSpeed();
        // Debug.Log("Включена суперскорость");
    }

    public void StopWork(IBonusUser player) {
        player.SetDefaultSpeed();
        // Debug.Log("Суперскорость выключена");
    }
}