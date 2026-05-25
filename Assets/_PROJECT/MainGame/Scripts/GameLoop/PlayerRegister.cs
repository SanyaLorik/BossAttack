using System.Collections.Generic;
using UnityEngine;
using Zenject;



public class PlayerRegister : MonoBehaviour {
    public List<IPlayer> PlayUnits { get; private set; } = new();
    
    public bool MainPlayerPlay { get; private set; }

    
    public void RegisterUnit(IPlayer player) {
        PlayUnits.Add(player);
    }
    
    public List<IPlayer> GetBosses() {
        return PlayUnits.FindAll(p => p.TargetType == TargetType.Boss);
    }
    
    public List<IPlayer> GetPlayers() {
        return PlayUnits.FindAll(p => p.TargetType == TargetType.Player);
    }
    
    

    public void UnregisterAllUnits() {
        GetBosses().ForEach(b => Destroy(b.Transform.gameObject));
        PlayUnits.Clear();
    }
    

    public bool AllBossesDied() {
        foreach (var player in PlayUnits) {
            if (player.TargetType == TargetType.Boss && player.Damagable.CurrentHp != 0) {
                return false;
            }
        }

        return true;
    }

}