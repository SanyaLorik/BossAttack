using System;
using RavingBots.CartoonExplosion;
using UnityEngine;

[Serializable]
public class ExplodeSpawn : ITickBehaviour, ISoundPlayer {
    [SerializeField] private CartoonExplosionFX _explodePs;
    [field: SerializeField] public SoundType SoundType { get; private set; }
    public void OnTick(Vector3 origin, IPlayer damagable) {
        if (damagable != null) {
            _explodePs.Play();
            GameEvents.ShakeCameraInvoke();
        }
    }
}