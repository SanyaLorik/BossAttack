using System;
using RavingBots.CartoonExplosion;
using UnityEngine;

[Serializable]
public class ExplodeSpawn : IAtackVisual, ISoundPlayer {
    [SerializeField] private VisualEffectPlayer _explodeVisual;
    [SerializeField] private bool _shake;
    [field: SerializeField] public SoundType SoundType { get; private set; }
    public void Play(Vector3 origin, IPlayer damagable) {
        if (damagable != null) {
            _explodeVisual.Play();
            if(_shake) GameEvents.ShakeCameraInvoke();
        }
    }
}


