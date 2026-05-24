using System;
using UnityEngine;
using Zenject;

public class FallVoidCollider : MonoBehaviour {
    public event Action<IPlayer> PlayerFalledInVoid; 
    
    [Inject] BattleManager _battleManager;
    [Inject] private RespawnManager _respawn;
    
    
     private void OnTriggerEnter(Collider collider){
        if (collider.TryGetComponent(out IPlayer player)) {
            if (player.IsPlaying) {
                _battleManager.PlayerFalled(player);
                PlayerFalledInVoid?.Invoke(player);
            }
            else {
                player.TeleportToPoint(_respawn.SpawnPoint.position);
            }
        }
     }
}