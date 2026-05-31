using System;
using UnityEngine;

public class DamagableProvider : MonoBehaviour {
    public IDamagable Damagable { get; private set; }

    public event Action DamageInitialized; 
    
    public void SetDamagable(IDamagable damagable) {
        Debug.Log("AWAKE SetDamagable " +  GetInstanceID());
        Damagable = damagable;
        DamageInitialized?.Invoke();
    }
}