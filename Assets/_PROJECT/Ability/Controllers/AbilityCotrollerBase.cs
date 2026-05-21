using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class AbilityCotrollerBase: MonoBehaviour {
    [field: SerializeField] public List<AbilitySystem> Abilitys { get; private set; }

    protected int _currentAbilityIndex;
    
    [Inject] IPlayer _currentPlayer;

    private void Awake() {
        InitAbility();
        InitStartGetters();
    }

    protected abstract void InitStartGetters();
    
    private void InitAbility() {
        Abilitys.ForEach(a => a.SetSame(_currentPlayer));
    }
    
    public void ReloadAbility() {
        Abilitys[_currentAbilityIndex].ReloadClip();
    }

    public void StopAbility() {
        Abilitys[_currentAbilityIndex].Stop();
    }

    public void StartAbility() {
        Abilitys[_currentAbilityIndex].StartSystem();
    }

}