using UnityEngine;

public class ActiveBonus {
    public IBonus Bonus { get; private set; }

    private readonly float _duration;
    private float _endTime;
    
    
    public ActiveBonus(IBonus bonus, float duration) {
        Bonus = bonus;
        _duration = duration;

        Reload();
    }

    
    
    public float Progress 
        => 1 - Mathf.Clamp01((_endTime - Time.time) / _duration);
    
    
    public void Reload() {
        _endTime = Time.time + _duration;
    }
    
}