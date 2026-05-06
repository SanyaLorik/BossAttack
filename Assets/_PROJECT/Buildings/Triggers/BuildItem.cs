using UnityEngine;
using Zenject;

public class BuildItem : TickerBehaviour {
    [SerializeReference, SubclassSelector] private IEffect _effect;
    [SerializeReference, SubclassSelector] private ITargetProvider _targetProvider;
    
    [Inject] private DiContainer _diContainer;
    
    [Inject]
    private void Init() {
        _diContainer.QueueForInject(_effect);
        _diContainer.QueueForInject(_targetProvider);
    }
    
    protected override void Tick() {
        foreach (var target in _targetProvider.GetTargets(_origin.position)) {
            _effect.ApplyEffect(target);
        }
    }
    
}