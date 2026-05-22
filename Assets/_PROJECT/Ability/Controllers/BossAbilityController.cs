using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BossAbilityController : AbilityCotrollerBase {
    [SerializeField] private int _startAbilityIndex;
    [SerializeField] private PairedValue<float> _diapasoneToChangeAbility;
    
    
    private CancellationTokenSource _tokenSource;

    public event Action<AbilitySystem> NewAbilitySystemEnabled;
    
    [Inject] BattleManager _battleManager;
    [Inject] BossStatsCalculator _bossStatsCalculator;

    
    protected override void InitStartGetters() {
        InitDamage();
    }
    
    
    
    private void OnEnable() {
        _currentAbilityIndex = _startAbilityIndex;
        // Покачто сразу прям, потом врубать после отсчета
        StartAbility();
    }
    

    private void InitDamage() {
        foreach (var ability in Abilitys) {
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
                case (AbilityType.ParabolicShoot):
                    abilityEffectValue.SetValueGetter(() => _bossStatsCalculator.ShootDamage);
                    ability.SetValueGetter(() => _bossStatsCalculator.BossIntervalToAtackInParabolicShoot);
                    break;
            }
        }
    }

    
    public List<AbilitySystem> GetAbilitys() {
        return Abilitys;
    }

    
    private void StartAbility() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        TimerToChangeAbility(_tokenSource.Token).Forget();
        Debug.Log("Start ability boss");
        Abilitys[_currentAbilityIndex].StartSystem();
        NewAbilitySystemEnabled?.Invoke(Abilitys[_currentAbilityIndex]);
        GameEvents.BossSwitchAbilityInvoke();
    }




    private void SetNextAbilityIndex() {
        Abilitys[_currentAbilityIndex].Stop();
        _currentAbilityIndex++;
        if (_currentAbilityIndex == Abilitys.Count) {
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