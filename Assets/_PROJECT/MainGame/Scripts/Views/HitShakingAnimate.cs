using DG.Tweening;
using UnityEngine;

public class HitShakingAnimate : MonoBehaviour {
    [SerializeField] private DamagableProvider _damagableProvider;
    [SerializeField] private float _scaleToShake = 1.2f;
    [SerializeField] private Transform _source;
    [SerializeField] private Ease _easeToShake = Ease.Linear;
    [SerializeField] private Ease _easeToUnshake = Ease.Linear;
    [SerializeField] private float _timeToShake = 0.1f;
    
    
    private IDamagable _damagable;
    private Sequence _sequence;
    private float _startScale;
    
    
    private void Start() {
        InitializeDamagable();
        _startScale = _source.localScale.x;
    }

    private void InitializeDamagable() {
        _damagable = _damagableProvider.Damagable;
        if(_damagable == null) {
            _damagableProvider.DamageInitialized += InitializeDamagable; 
            return;
        }
        _damagableProvider.DamageInitialized -= InitializeDamagable; 
        _damagable.HpMinus += Shake;
    }

    private void Shake(int hp) {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        
        _sequence.Append(
            _source.DOScale(_scaleToShake, _timeToShake).SetEase(_easeToShake)
        );
        _sequence.Append(
            _source.DOScale(_startScale, _timeToShake).SetEase(_easeToUnshake)
        );
    }


    private void OnDestroy() {
        _damagable.HpMinus -= Shake;
        _sequence.Kill();
    }
}