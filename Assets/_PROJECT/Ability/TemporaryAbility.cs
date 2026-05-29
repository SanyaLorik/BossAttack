using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

public class TemporaryAbility : MonoBehaviour {
    [SerializeField] private TargetType TypeToAtack;
    [SerializeReference, SubclassSelector] private ITargetProvider _targetProvider;
    [SerializeReference, SubclassSelector] private List<IAtackVisual> _atackVisuals;
    [SerializeReference, SubclassSelector] private IHitDelivery _hitDelivery;
    [SerializeReference, SubclassSelector] private IEffect _effect;
    [SerializeField] private Transform _origin;
    [SerializeField] private int _damage;
    
    private CancellationTokenSource _findTagetSource;
    private IPlayer _target;
    private bool _injected;
    
    public event Action<ISoundPlayer> SoundPlayed;
    
    private List<IPlayer> _targets = new();


    private List<IPlayer> TargetList
        => _playerRegister.PlayUnits;
    
    
    [Inject] private DiContainer _diContainer;
    [Inject] private PlayerRegister _playerRegister;
    
    
    [Inject]
    private void Init() {
        _diContainer.Inject(_effect);
        _diContainer.Inject(_targetProvider);
        _atackVisuals.ForEach(t=> _diContainer.Inject(t));
        _diContainer.Inject(_hitDelivery);
        _injected = true;
        InitDamage();
    }

    
    private void InitDamage() {
        var abilityValue = _effect as IValueGetter;
        if (abilityValue != null) abilityValue.SetValueGetter(() => _damage);
    }

    

    public void Use() {
        if (!_injected) {
            Debug.LogError("Not injected");
            return;
        }

        FindNewTargets();
        foreach (IPlayer target in _targets) {
            if(target == null || target.Damagable.CurrentHp == 0) continue;
            // Визуал
            PlayAtackVisual(target);
            // Атака
            DelieveEffect(target);
        }
    }

    private void DelieveEffect(IPlayer target) {
        _hitDelivery.Deliver(
            _origin.position,
            target,
            TypeToAtack,
            TargetList,
            _effect
        );
        if (_hitDelivery is ISoundPlayer hitSoundPlayer) {
            SoundPlayed?.Invoke(hitSoundPlayer);
        }
    }

    private void PlayAtackVisual(IPlayer target) {
        foreach (var atackVisual in _atackVisuals) {
            atackVisual.Play(_origin.position, target);
            if (atackVisual is ISoundPlayer soundPlayer) {
                SoundPlayed?.Invoke(soundPlayer);
            }
        }
    }

    private void FindNewTargets() {
        _targets = _targetProvider.GetTargets(_origin, TargetList, TypeToAtack);
    }
    
}