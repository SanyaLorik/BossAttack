using UnityEngine;
using Zenject;

public class ShieldEnabler : MonoBehaviour {
    // [SerializeField] private ProgressVisualizer progressVisualizer;
    // [SerializeField] private bool _shieldIsEnabled;
    //
    //
    // [Inject] BattleManager _battleManager;
    // [Inject] IPlayer _player;
    //
    //
    // private void Start() {
    //     progressVisualizer.ShieldShowFast(false);
    // }
    //
    // private void OnEnable() {
    //     _battleManager.GameReadyToPlay += HideShield;
    //     progressVisualizer.ShieldShowFast(false);
    //     _player.BonusUser.InvinsibleStatusChanged += OnInvinsibleStatusChanged;
    // }
    //
    //
    // private void OnDisable() {
    //     _battleManager.GameReadyToPlay -= HideShield;
    //     progressVisualizer.ShieldShowFast(false);
    //     _player.BonusUser.InvinsibleStatusChanged -= OnInvinsibleStatusChanged;
    // }
    //
    //
    // private void HideShield() {
    //     progressVisualizer.HideShieldFast();
    // }
    //
    //
    // private void OnInvinsibleStatusChanged(bool enable) {
    //     if (_player.IsPlaying) {
    //         _shieldIsEnabled = false;
    //         progressVisualizer.ShieldEnableAnimate(false);
    //         return;
    //     }
    //     
    //     // Повторно не анимируем
    //     if (enable == _shieldIsEnabled) {
    //         return;
    //     }
    //     
    //     // Если хочет вырубить но 1 включен не трогаем
    //     if (!enable) {
    //         if (_player.BonusUser.IsInvincibleAfterBonus) {
    //             return;
    //         }
    //     }
    //     
    //     // Debug.Log("Shield enable: " + enable);
    //     _shieldIsEnabled =  enable;
    //     progressVisualizer.ShieldEnableAnimate(enable);
    // }
}