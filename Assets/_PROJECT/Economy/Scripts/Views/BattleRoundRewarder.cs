using UnityEngine;
using Zenject;

public class BattleRewarder : MonoBehaviour {
    [Header("Множитель награды за раунд")]
    [SerializeField] private float _baseReward;
    [SerializeField] private AnimationCurve _rewardCurve;
    [SerializeField] private float _bossProgressCurveMultiplier;
    
    
    [Inject] private PlayerBank _bank;
    [Inject] PlayerRegister _playerRegister;
    [Inject] BattleManager _battleManager;
    [Inject] BossesDiesObserver _bossesDiesObserver;
    [Inject] PlayerLevel _playerLevel;

    private void OnEnable() {
        _bossesDiesObserver.BossDied += RewardPlayerToKillBoss;
    }

    private void RewardPlayerToKillBoss(IPlayer boss) {
        int countDiesBosses = 0;
        int countBosses = 0;
        
        foreach (var player in _playerRegister.PlayUnits) {
            if (player.TargetType != TargetType.Boss) continue;
            countBosses++;
            if (player.Damagable.CurrentHp == 0) countDiesBosses++;
        }

        float bossesProgress = (float)countDiesBosses / countBosses;
        float bossReward = _baseReward * _playerLevel.CurrentLevel + _rewardCurve.Evaluate(bossesProgress) * _bossProgressCurveMultiplier;
        Debug.Log("bossKillReward = " + bossReward);
        _bank.AddMoney((int)bossReward);
    }

    private void OnNewRoundStarted(int number) {
        // Debug.Log("OnNewRoundStarted");
        // if(!_playerRegister.MainPlayerPlay || number == 1) return;
        // float roundProgress = (float) number / _battleManager.AllRoundsCount;
        // float roundReward = _baseReward * number + _rewardCurve.Evaluate(roundProgress) * _bossProgressCurveMultiplier;
        // Debug.Log("roundReward = " + roundReward);
        // _bank.AddMoney((int)roundReward);
    }
}