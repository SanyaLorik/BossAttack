using System;
using UnityEngine;

[Serializable]
public class Melee : ITickBehaviour {
    [SerializeField] private BotAnimator _animator;
    public void OnTick(Vector3 origin, IPlayer _) {
        _animator.PlayMeleeAnimation();
    }
}