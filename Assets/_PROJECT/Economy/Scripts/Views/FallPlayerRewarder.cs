using UnityEngine;
using Zenject;

public class FallPlayerRewarder : MonoBehaviour {
    // [Inject] private FallVoidCollider _fallVoidCollider;
    // [Inject] private PlayerMovement _mainPlayer;
    // [Inject] private PlayerBank _playerBank;
    // [Inject] private EconomyCalculator _economyCalculator;
    //
    //
    // private void OnEnable() {
    //     _fallVoidCollider.PlayerFalledInVoid += OnPlayerFalledInVoid;
    // }
    //
    //
    // private void OnPlayerFalledInVoid(IPusher pusher) {
    //     if(pusher == _mainPlayer.Pusher) return; // Соболезнуем
    //     
    //     if (pusher.LastPlayerContact == _mainPlayer.Pusher) {
    //         RewardPlayerToKillInnocentBot();
    //     }
    // }
    //
    // private void RewardPlayerToKillInnocentBot() {
    //     _playerBank.AddMoney(_economyCalculator.CalcRewardToFall());
    // }
}