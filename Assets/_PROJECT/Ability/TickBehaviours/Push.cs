using System;
using UnityEngine;

[Serializable]
public class Push : ITickBehaviour {
    public void OnTick(Vector3 origin, IPlayer player) {
        if(player.Pusher == null) return;
        Vector3 direction = (player.Transform.position - origin).normalized;
        player.Pusher.PushAway(direction);
    }
}