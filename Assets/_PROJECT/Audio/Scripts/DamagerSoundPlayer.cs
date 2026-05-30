using System;
using UnityEngine;
using Zenject;

public class DamagerSoundPlayer : MonoBehaviour {
    [SerializeField] private Sound3dEmitter _emitter;
    [SerializeField] private SoundType _soundTypeToHit;
    // [SerializeField] private SoundType _soundTypeToDie;
    
    [Inject] BattleManager _battleManager;
    
    private IPlayer _player;

    private IDamagable Damagable {
        get {
            _player ??= GetComponentInParent<IPlayer>();
            return _player.Damagable;
        }
    }

    private void OnDisable() {
        _battleManager.GameReadyToPlay -= Subscribe;
        _battleManager.MainPlayerWin -= Unsubscribe;
    }
    
    
    private void OnEnable() {
        _battleManager.GameReadyToPlay += Subscribe;
        _battleManager.MainPlayerWin += Unsubscribe;
    }

    
    private void Unsubscribe(bool _) {
        Damagable.HpMinus -= OnHpMinus;
    }


    private void Subscribe() {
        Damagable.HpMinus += OnHpMinus;
    }


    // private void OnDamagableDied(IDamagable player) {
    //     _emitter.Play(_soundTypeToDie);
    //     Debug.Log("Play " + _soundTypeToDie);
    // }


    private void OnHpMinus(int hp) {
        _emitter.Play(_soundTypeToHit);
    }
    

}