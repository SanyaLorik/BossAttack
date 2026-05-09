using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AbilitySystem : TickerBehaviour {
    [SerializeReference, SubclassSelector] private List<ITargetFilter> _targetFilters;
    [SerializeReference, SubclassSelector] private IEffect _effect;
    [SerializeReference, SubclassSelector] private ITargetProvider _targetProvider;
    [SerializeReference, SubclassSelector] private List<ITickBehaviour> _tickBehaviour;
    
    
    private IGizmosDrawable _gizmosDrawer;
    
    [Inject] private DiContainer _diContainer;
    
    [Inject]
    private void Init() {
        _diContainer.QueueForInject(_effect);
        _diContainer.QueueForInject(_targetProvider);
        _tickBehaviour.ForEach(t=> _diContainer.QueueForInject(t));
    }

    private void Awake() {
        _gizmosDrawer = _targetProvider as IGizmosDrawable;
    }

    private void OnDrawGizmos() {
        _gizmosDrawer.DrawGizmos(_origin.position);
    }

    protected override void Tick() {
        foreach (IDamagable target in _targetProvider.GetTargets(_origin.position)) {
            if(target == null) continue;

            
            bool allowed = true;
            foreach (var filter in _targetFilters) {
                if (filter.CanApply(_origin, target) == false) {
                    allowed = false;
                    break;
                }
            }

            if (allowed) {
                _tickBehaviour.ForEach(beh => beh.OnTick(_origin.position, target));
                _effect.ApplyEffect(target);
            }
        }
    }
    
}