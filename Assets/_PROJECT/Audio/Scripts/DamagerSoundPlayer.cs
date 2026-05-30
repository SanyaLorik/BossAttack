using UnityEngine;

public class DamagerSoundPlayer : MonoBehaviour {
    [SerializeField] private Sound3dEmitter _emitter;
    [SerializeField] private SoundType _soundTypeToHit;
    // [SerializeField] private SoundType _soundTypeToDie;
    
    
    private IPlayer _player;
    private IDamagable Damagable => _player.Damagable;

    
    private void Awake() {
        _player = GetComponentInParent<IPlayer>();
    }

    private void OnDisable() {
        Damagable.HpMinus -= OnHpMinus;
    }

    
    private void OnEnable() {
        Damagable.HpMinus += OnHpMinus;
    }

    // private void OnDamagableDied(IDamagable player) {
    //     _emitter.Play(_soundTypeToDie);
    //     Debug.Log("Play " + _soundTypeToDie);
    // }


    private void OnHpMinus(int hp) {
        _emitter.Play(_soundTypeToHit);
        Debug.Log("Play " + _soundTypeToHit);
    }
    

}