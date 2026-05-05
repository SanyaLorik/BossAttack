using System;
using UnityEngine;
using Zenject;

public class FallVoidCollider : MonoBehaviour {
    public event Action<IPusher> PlayerFalledInVoid; 
    
    [Inject] BattleManager _battleManager;
    [Inject] RespawnManager _respawn;
    
    
     private void OnTriggerEnter(Collider collider){
        if (collider.TryGetComponent(out IPlayer player)) {
            player.TeleportToPoint(_respawn.SpawnPoint.position);
            if (player.IsPlaying) {
                _battleManager.PlayerFalled(player);
                PlayerFalledInVoid?.Invoke(player.Pusher);
            }
        }
     }
}