using System;
using UnityEngine;

[Serializable]
public class Melee : IAtackVisual {
    [SerializeField] private BotAnimator _animator;
    public void Play(Vector3 origin, IPlayer _) {
        _animator.PlayMeleeAnimation();
    }
}