using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;


public enum AbilityType {
    Shooting,
    Melee,
}


public class AbilitySystem : TickerBehaviour {
    [field: SerializeField] public AbilityType Type { get; private set; }
    [SerializeReference, SubclassSelector] private ITargetProvider _targetProvider;
    [SerializeReference, SubclassSelector] private List<ITargetFilter> _targetFilters;
    [SerializeReference, SubclassSelector] private IEffect _effect;
    [SerializeReference, SubclassSelector] private List<ITickBehaviour> _tickBehaviour;
    [SerializeReference, SubclassSelector] private IAtackCapacity _atackCapacity;
    
    
    public IEffect Effect => _effect;
    public IAtackCapacity AtackCapacity => _atackCapacity;
    
    private IGizmosDrawable _gizmosDrawer;
    private CancellationTokenSource _findTagetSource;
    private IPlayer _target;
    
    public event Action<ISoundPlayer> SoundPlayed;
    public event Action<IPlayer> NewTargetFinded;
    
    

    
    
    [Inject] private DiContainer _diContainer;
    
    
    [Inject]
    private void Init() {
        _diContainer.QueueForInject(_effect);
        _diContainer.QueueForInject(_targetProvider);
        _tickBehaviour.ForEach(t=> _diContainer.QueueForInject(t));
        _diContainer.QueueForInject(_atackCapacity);
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

    protected override void Tick() {
        _target = null;
        if(!_atackCapacity.AllowToUse) return;
        foreach (IPlayer target in _targetProvider.GetTargets(_origin.position)) {
            if(target == null || target.Damagable.CurrentHp == 0) continue;

            
            bool allowed = true;
            foreach (var filter in _targetFilters) {
                if (filter.CanApply(_origin, target) == false) {
                    allowed = false;
                    break;
                }
            }

            if (allowed) {
                NewTargetFinded?.Invoke(target);
                _target = target;
                foreach (var beh in _tickBehaviour) {
                    beh.OnTick(_origin.position, target);
                    if (beh is ISoundPlayer soundPlayer) {
                        SoundPlayed?.Invoke(soundPlayer);
                    }
                }
                _effect.ApplyEffect(target);
                _atackCapacity.SpendOne();
            }
        }
        // нет целей поблизости - бежим по всей карте за ближайшим
        if (_target == null) {
            NewTargetFinded?.Invoke(null);
        }
    }

    protected override void OnStart() {
        _atackCapacity.StartCheckCapacity(true);
    }

    protected override void OnEnd() {
        _atackCapacity.StartCheckCapacity(false);
    }
}