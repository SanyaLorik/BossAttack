using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BossAbilityController : MonoBehaviour {
    [SerializeField] private List<AbilitySystem> _abilitys;
    [SerializeField] private int _startAbilityIndex;
    [SerializeField] private PairedValue<float> _diapasoneToChangeAbility;
    
    
    private int _currentAbilityIndex;
    private CancellationTokenSource _tokenSource;

    public event Action<AbilitySystem> NewAbilitySystemEnabled;
    
    
    [Inject] BattleManager _battleManager;
    [Inject] BossStatsCalculator _bossStatsCalculator;

    private void Awake() {
        InitDamage();
    }

    private void InitDamage() {
        foreach (var ability in _abilitys) {
            if (ability.Effect is not IValueGetter abilityEffectValue) {
                continue;
            }
            switch (ability.Type) {
                case (AbilityType.Melee):
                    abilityEffectValue.SetValueGetter(() => _bossStatsCalculator.MeleeDamage);
                    ability.SetValueGetter(() => _bossStatsCalculator.BossIntervalToAtackInMelee);
                    break;
                case (AbilityType.Shooting):
                    abilityEffectValue.SetValueGetter(() => _bossStatsCalculator.ShootDamage);
                    ability.SetValueGetter(() => _bossStatsCalculator.BossIntervalToAtackInShoot);
                    break;
            }
        }
    }

    private void OnEnable() {
        _currentAbilityIndex = _startAbilityIndex;
        StartAbility();
    }

    
    public List<AbilitySystem> GetAbilitys() {
        return _abilitys;
    }

    
    private void StartAbility() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        TimerToChangeAbility(_tokenSource.Token).Forget();
        Debug.Log("Start ability boss");
        _abilitys[_currentAbilityIndex].StartSystem();
        NewAbilitySystemEnabled?.Invoke(_abilitys[_currentAbilityIndex]);
        GameEvents.BossSwitchAbilityInvoke();
    }

    
    private void SetNextAbilityIndex() {
        _abilitys[_currentAbilityIndex].Stop();
        _currentAbilityIndex++;
        if (_currentAbilityIndex == _abilitys.Count) {
            _currentAbilityIndex = 0;
        }
    }

    private async UniTask TimerToChangeAbility(CancellationToken token) {
        float timer = Random.Range(_diapasoneToChangeAbility.From, _diapasoneToChangeAbility.To);
        await UniTask.WaitForSeconds(timer, cancellationToken: token);
        SetNextAbilityIndex();
        StartAbility();
    }
    
  
}