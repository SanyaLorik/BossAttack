using System;
using UnityEngine;
using Zenject;

public class FallVoidCollider : MonoBehaviour {
    public event Action<IPusher> PlayerFalledInVoid; 
    
    [Inject] BattleManager _battleManager;
    [Inject] SpawnManager _spawn;
    
    
     private void OnTriggerEnter(Collider collider){
        if (collider.TryGetComponent(out IPlayer player)) {
            player.TeleportToPoint(_spawn.SpawnPoint.position);
            if (player.IsPlaying) {
                _battleManager.PlayerFalled(player);
                PlayerFalledInVoid?.Invoke(player.Pusher);
            }
        }
     }
}