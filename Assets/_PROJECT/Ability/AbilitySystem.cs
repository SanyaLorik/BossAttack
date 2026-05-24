using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;


public enum AbilityType {
    Shooting,
    Melee,
    ParabolicShoot
}

[Serializable]
public enum TargetType {
    Enemy,
    Player,
}


public class AbilitySystem : TickerBehaviour {
    [SerializeField] private TargetType TargetType;
    [field: SerializeField] public AbilityType Type { get; private set; }
    [SerializeReference, SubclassSelector] private ITargetProvider _targetProvider;
    [SerializeReference, SubclassSelector] private List<ITargetFilter> _targetFilters;
    [SerializeReference, SubclassSelector] private List<IAtackVisual> _atackVisuals;
    [SerializeReference, SubclassSelector] private IHitDelivery _hitDelivery;
    [SerializeReference, SubclassSelector] private IEffect _effect;
    [SerializeReference, SubclassSelector] private IAtackCapacity _atackCapacity;
    
    
    public IEffect Effect => _effect;
    public IAtackCapacity AtackCapacity => _atackCapacity;
    
    private IGizmosDrawable _gizmosDrawer;
    private CancellationTokenSource _findTagetSource;
    private IPlayer _target;
    private bool _injected;
    
    public event Action<ISoundPlayer> SoundPlayed;
    public event Action<IPlayer> NewTargetFinded;
    public event Action<IPlayer> NewTargetAttacked;
    
    
    private List<IPlayer> TargetList
        => TargetType == TargetType.Enemy ? 
            _battleInfo.Bosses 
            : 
            _battleInfo.Players;
    
    
    [Inject] private DiContainer _diContainer;
    [Inject] private IBattleInfo _battleInfo;
    
    
    [Inject]
    private void Init() {
        _diContainer.Inject(_effect);
        _diContainer.Inject(_targetProvider);
        _atackVisuals.ForEach(t=> _diContainer.Inject(t));
        _diContainer.Inject(_hitDelivery);
        _diContainer.Inject(_atackCapacity);
        _injected = true;
    }

    
    private void Awake() {
        _gizmosDrawer = _targetProvider as IGizmosDrawable;
    }
    

    private void OnDrawGizmos() {
        // _gizmosDrawer.DrawGizmos(_origin.position);
    }

    
    public void SetSame(IPlayer player) {
        _targetProvider.SetSame(player);
    }

    public void ReloadClip() {
        _atackCapacity.ReloadFull();
    }
    
    

    protected override void Tick() {
        if(!_atackCapacity.AllowToUse || !_injected || !_allowToUse) return;
        foreach (IPlayer target in _targets) {
            if(target == null || target.Damagable.CurrentHp == 0) continue;
            
            bool allowed = true;
            foreach (var filter in _targetFilters) {
                if (filter.CanApply(_origin, target) == false) {
                    allowed = false;
                    break;
                }
            }

            if (allowed) {
                // Визуал
                PlayAtackVisual(target);
                
                // Атака
                DelieveEffect(target);

                _atackCapacity.SpendOne();
            }
        }
    }

    private void DelieveEffect(IPlayer target) {
        _hitDelivery.Deliver(
            _origin.position,
            target,
            TargetList,
            _effect
        );
        if (_hitDelivery is ISoundPlayer hitSoundPlayer) {
            SoundPlayed?.Invoke(hitSoundPlayer);
        }
        NewTargetAttacked?.Invoke(target);
    }

    private void PlayAtackVisual(IPlayer target) {
        foreach (var atackVisual in _atackVisuals) {
            atackVisual.Play(_origin.position, target);
            if (atackVisual is ISoundPlayer soundPlayer) {
                SoundPlayed?.Invoke(soundPlayer);
            }
        }
    }

    protected override void FindNewTargets() {
        _targets = _targetProvider.GetTargets(_origin.position, TargetList);
        
        NewTargetFinded?.Invoke(_targets.Count > 0 ? _targets[0] : null);
    }

    
    private bool _allowToUse;
    protected override void OnStart() {
        Debug.Log("OnStart" + gameObject.name);
        _atackCapacity.StartCheckCapacity(true);
        _allowToUse = true;
        // _atackCapacity.SetVisualState(true);
    }

    protected override void OnEnd() {
        _atackCapacity.StartCheckCapacity(false);
        _allowToUse = false;
        
        // _atackCapacity.SetVisualState(false);
    }
}