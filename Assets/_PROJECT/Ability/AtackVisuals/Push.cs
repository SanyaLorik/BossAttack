using System;
using UnityEngine;

[Serializable]
public class Push : IAtackVisual {
    public void Play(Vector3 origin, IPlayer player) {
        if(player.Pusher == null) return;
        Vector3 direction = (player.Transform.position - origin).normalized;
        player.Pusher.PushAway(direction);
    }
}