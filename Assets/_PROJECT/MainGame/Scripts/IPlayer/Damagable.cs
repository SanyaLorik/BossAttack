using System;
using UnityEngine;

public class Damagable : IDamagable {
    
    private IPlayer _player;
    
    private bool IsDied => CurrentHp == 0;
    private Func<int> _maxHpGetter;


    public void Respawn(bool silent) {
        CurrentHp = _maxHpGetter();
        if (!silent) {
            DamagableSpawned?.Invoke(this);
        }
    }

    public Transform Transform { get; private set; }
    public event Action<IDamagable> DamagableDied;
    public event Action<IDamagable> DamagableSpawned;
    public event Action<int> HpUpdated;
    

    public int CurrentHp { get; private set; }
    public int MaxHp => _maxHpGetter();
    
    public void SetMaxHpGetter(Func<int> valueGetter) {
        _maxHpGetter = valueGetter;
        CurrentHp = _maxHpGetter();
    }


    public Damagable(Transform transform, IPlayer player) {
        _player =  player;
        Transform = transform;
    }



    public void ApplyDamage(int damage) {
        if(IsDied) return;
        if(_player.BonusUser is { IsInvincibleAfterBonus: true }) return;
        
        if (damage < 0) damage *= -1;
        CurrentHp -= damage;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, _maxHpGetter());
        HpUpdated?.Invoke(CurrentHp);
        CheckDied();
    }
    

    public void ApplyHeal(int hp) {
        if(IsDied) return;
        if (hp < 0) hp *= -1;
        CurrentHp += hp;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, _maxHpGetter());
        HpUpdated?.Invoke(CurrentHp);
    }
    
    public void SetSpawned() {
        CurrentHp = _maxHpGetter();
        DamagableSpawned?.Invoke(this);
    }
    
    public void SetDied() {
        CurrentHp = 0;
        DamagableDied?.Invoke(this);
        Debug.Log("Died");
    }

    
    private void CheckDied() {
        if(CurrentHp != 0) return;
        SetDied();
    }
}